using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Contracts;
using Common.Utility;
using MoreLinq;
using NeuroEngine;
using NeuroEngine.ActivationFunctions;
using NeuroEngine.Neurons;
using NeuroServer.Udp;
using NLog;
using QuickGraph;

namespace NeuroServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var reqSerializer = (ISerializer<Request>)new BinarySerializer();
            var respSerializer = (ISerializer<Response>)new BinarySerializer();
            var log = LogManager.GetLogger("Server");
            var timespan = TimeSpan.FromSeconds(3);

            using (var tester = new RemoteTestingController(timespan, new ImageManager(), log))
            using (var server = new TcpServer(52200))
            {
                var trainingSupervisor = new TrainingSupervisor(null, null, tester);

                server.OnProcess += bytes =>
                {
                    var request = reqSerializer.Deserialize(bytes);
                    var response = tester.Compute(request);
                    return respSerializer.Serialize(response);
                };
                server.Start();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var networks = MoreEnumerable.GenerateByIndex(BuildNetwork).Take(N).ToList();
                        trainingSupervisor.Train(networks);
                        tester.Dispose();
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

        public static NeuralNetwork BuildNetwork(int bias)
        {
            const int inputSize = 9;
            var inputs = MoreEnumerable.GenerateByIndex(_ => new NeuronWithInput()).Take(inputSize).ToArray();
            var outputs = new[] { new BasicNeuron(new IdentityFunction()), new NeuronWithInput() };
            var graph = new AdjacencyGraph<INeuron, Connection>();
            var rand = new Random();

            for (int i = 0; i < inputSize; i++)
            {
                var delta = 2d * i / (inputSize - 1);
                graph.AddVerticesAndEdge(new Connection(inputs[i], outputs[0], -1d + delta + (bias - N + 1) *0.4* rand.NextDouble()));
            }

            outputs[1] = new BasicNeuron(new ConstFunction(bias * 0.05));
            graph.AddVertex(outputs[1]);

            return new NeuralNetwork(graph, inputs, outputs);
        }

        private const int N = 5;
    }
}
