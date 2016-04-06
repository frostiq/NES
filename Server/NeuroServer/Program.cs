using System;
using Common.Contracts;
using Common.Utility;
using NeuroEngine;
using NeuroEngine.Neurons;
using NeuroServer.Udp;
using QuickGraph;

namespace NeuroServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var network = BuildNetwork();
            var serializer = new BinarySerializer();
            using (var server = new UdpServer(52200))
            {
                server.Start();
                server.OnProcess += bytes =>
                {
                    var output = network.Compute(new[] {0.1d, 0.1d});
                    var deltas = new Deltas((float)output[0], (float)output[1]);
                    return serializer.Serialize(deltas);
                };
                Console.Read();
            }
        }

        public static NeuralNetwork BuildNetwork()
        {
            var angle = new InputNeuron();
            var velocity = new InputNeuron();
            var graph = new AdjacencyGraph<INeuron, Connection>();
            var vertices = new[] {angle, velocity};
            graph.AddVertexRange(vertices);
            return new NeuralNetwork(graph, vertices, vertices);
        }
    }
}
