using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Contracts;
using Common.Utility;
using NeuroEngine;
using NeuroServer.Udp;
using NLog;

namespace NeuroServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var reqSerializer = (ISerializer<Request>)new BinarySerializer();
            var respSerializer = (ISerializer<Response>)new BinarySerializer();
            var log = LogManager.GetLogger("Main");
            var testingTime = TimeSpan.FromSeconds(5);
            var inputSize = 9;
            var outputSize = 2;
            var initSetCount = 5;
            var initSet = new InitNeuroSet(inputSize, outputSize);

            using (var remoteTestingController = new RemoteTestingController(testingTime, new ImageManager(), log))
            using (var server = new TcpServer(52200))
            {
                var trainingSupervisor = new TrainingSupervisor(new Interbreeder(3, outputSize), null, remoteTestingController);

                server.OnProcess += bytes =>
                {
                    var request = reqSerializer.Deserialize(bytes);
                    var response = remoteTestingController.Compute(request);
                    return respSerializer.Serialize(response);
                };
                server.Start();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var networks = initSet.BuildNetworks().Take(initSetCount).ToList().AsReadOnly();
                        trainingSupervisor.Train(networks);
                        remoteTestingController.Dispose();
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                        throw;
                    }
                    
                });
                Console.Read();
            }
        }
    }
}
