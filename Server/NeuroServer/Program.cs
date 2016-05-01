using System;
using System.Linq;
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
        private const int InputSize = 9;

        static void Main(string[] args)
        {
            var network = BuildNetwork();
            var serializer = new BinarySerializer();
            var imageManager = new ImageManager();
            var _log = LogManager.GetLogger("Server");
            using (var server = new UdpServer(52200))
            {
                server.Start();
                server.OnProcess += bytes =>
                {
                    var input = imageManager.ConvertFromPngToInput(bytes, InputSize);
                    input = imageManager.Normalize(input);
                    //_log.Info(string.Join(";", input.Select(x => x.ToString("F"))));
                    var output = network.Compute(input);
                    //_log.Info(output[0].ToString("F"));
                    var deltas = new Deltas((float)output[0], (float)output[1]);
                    _log.Info(deltas.DeltaAngle);
                    return serializer.Serialize(deltas);
                };
                Console.Read();
            }
        }

        public static NeuralNetwork BuildNetwork()
        {
            var inputs = MoreEnumerable.GenerateByIndex(i => new NeuronWithInput()).Take(InputSize).ToArray();
            var output = new [] {new BasicNeuron(new IdentityFunction()), new NeuronWithInput()  };
            var graph = new AdjacencyGraph<INeuron, Connection>();

            for (int i = 0; i < InputSize; i++)
            {
                var delta = 2d * i/(InputSize - 1);
                graph.AddVerticesAndEdge(new Connection(inputs[i], output[0], -1d + delta));
            }
            //output[1].AddToInput(.1);

            return new NeuralNetwork(graph, inputs, output);
        }
    }
}
