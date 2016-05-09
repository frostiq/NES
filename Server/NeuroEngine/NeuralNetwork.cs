using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NeuroEngine.Neurons;
using QuickGraph;
using QuickGraph.Algorithms.Search;

namespace NeuroEngine
{
    public class NeuralNetwork
    {
        private readonly AdjacencyGraph<INeuron, Connection> _network;
        private readonly BreadthFirstSearchAlgorithm<INeuron, Connection> _algorithm;

        public INeuron RootVertex { get; }
        public ICollection<INeuron> InputNeurons { get; }
        public ICollection<INeuron> OutputNeurons { get; }

        public NeuralNetwork
            (AdjacencyGraph<INeuron, Connection> network)
        {
            _network = network;
            OutputNeurons = network.Vertices.Where(network.IsOutEdgesEmpty).ToList();
            InputNeurons = network.Vertices.Where(n => network.Edges.All(e => e.Target != n)).ToList();

            RootVertex = new EmptyNeuron();
            _network.AddVertex(RootVertex);
            foreach (var inputNeuron in InputNeurons)
            {
                _network.AddEdge(new Connection(RootVertex, inputNeuron, 0d));
            }

            _algorithm = new BreadthFirstSearchAlgorithm<INeuron, Connection>(_network);
            _algorithm.ExamineEdge += AddSignal;
            _algorithm.SetRootVertex(RootVertex);
        }

        public double[] Compute(double[] input)
        {
            if (input.Length != InputNeurons.Count)
            {
                throw new ArgumentException("input.Count != inputNeurons.Count");
            }

            Enumerable.Zip(input, InputNeurons, (d, neuron) => neuron.AddToInput(d)).Consume();

            _algorithm.Compute();

            var result = OutputNeurons.Select(x => x.Signal).ToArray();
            Reset();

            return result;
        }

        public void Reset()
        {
            foreach (var neuron in _network.Vertices) neuron.Reset();
        }

        public IEnumerable<Connection> GetConnections(INeuron neuron)
        {
            return _network.OutEdges(neuron);
        }

        private static void AddSignal(Connection edge)
        {
            edge.Target.AddToInput(edge.Source.Signal * edge.Weight);
        }
    }
}
