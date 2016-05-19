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
            var reqSerializer = (ISerializer<Request>) new BinarySerializer();
            var respSerializer = (ISerializer<Response>) new BinarySerializer();
            var log = LogManager.GetLogger("Server");
            var timespan = TimeSpan.FromSeconds(15);

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
                    var networks = Enumerable.Repeat(BuildNetwork(), 3);
                    trainingSupervisor.Train(networks);
                    tester.Dispose();
                });
                Console.Read();
            }
        }

        public static NeuralNetwork BuildNetwork()
        {
            const int inputSize = 9;
            var inputs = MoreEnumerable.GenerateByIndex(i => new NeuronWithInput()).Take(inputSize).ToArray();
            var output = new [] {new BasicNeuron(new IdentityFunction()), new NeuronWithInput()  };
            var graph = new AdjacencyGraph<INeuron, Connection>();
            graph.AddVertex(output[1]);

            for (int i = 0; i < inputSize; i++)
            {
                var delta = 2d * i/(inputSize - 1);
                graph.AddVerticesAndEdge(new Connection(inputs[i], output[0], -1d + delta));
            }
            //output[1].AddToInput(.1);

            return new NeuralNetwork(graph, inputs, output);
        }
    }
}
