using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NeuroEngine.ActivationFunctions;
using NeuroEngine.Neurons;
using QuickGraph;

namespace NeuroEngine
{
    public class InitNeuroSet
    {
        private readonly int _inputSize;
        private readonly int _outputSize;
        private readonly Random _random = new Random();

        public InitNeuroSet(int inputSize, int outputSize)
        {
            _inputSize = inputSize;
            _outputSize = outputSize;
        }

        public IEnumerable<NeuralNetwork> BuildNetworks()
        {
            while (true)
            {
                var graph = new AdjacencyGraph<INeuron, Connection>();
                var inputs = MoreEnumerable.GenerateByIndex(_ => new NeuronWithInput()).Take(_inputSize).ToArray();
                var mid =
                    MoreEnumerable.GenerateByIndex(_ => new BasicNeuron(new ThFunction()))
                        .Take(_random.Next(_outputSize, _inputSize))
                        .ToArray();
                var outputs =
                    MoreEnumerable.GenerateByIndex(_ => new BasicNeuron(new ThFunction())).Take(_outputSize).ToArray();

                foreach (var i in inputs)
                    foreach (var m in mid)
                    {
                        var weight = _random.NextDouble();
                        graph.AddVerticesAndEdge(new Connection(i, m, weight));
                    }

                foreach (var m in mid)
                    foreach (var o in outputs)
                    {
                        var weight = _random.NextDouble();
                        graph.AddVerticesAndEdge(new Connection(m, o, weight));
                    }

                yield return new NeuralNetwork(graph, inputs, outputs);
            }
        }
    }
}