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
        private readonly ICollection<INeuron> _inputNeurons;
        private readonly ICollection<INeuron> _outputNeurons;
        private readonly BreadthFirstSearchAlgorithm<INeuron, Connection> _algorithm;

        public NeuralNetwork
            (AdjacencyGraph<INeuron, Connection> network, 
            ICollection<INeuron> inputNeurons, 
            ICollection<INeuron> outputNeurons)
        {
            _network = network;
            _outputNeurons = outputNeurons;
            _inputNeurons = inputNeurons;

            var root = new EmptyNeuron();
            _network.AddVertex(root);
            foreach (var inputNeuron in _inputNeurons)
            {
                _network.AddEdge(new Connection(root, inputNeuron, 0d));
            }

            _algorithm = new BreadthFirstSearchAlgorithm<INeuron, Connection>(_network);
            _algorithm.ExamineEdge += AddSignal;
            _algorithm.SetRootVertex(root);
        }

        public double[] Compute(double[] input)
        {
            if (input.Length != _inputNeurons.Count)
            {
                throw new ArgumentException("input.Count != inputNeurons.Count");
            }

            Enumerable.Zip(input, _inputNeurons, (d, neuron) => neuron.AddToInput(d)).Consume();

            _algorithm.Compute();

            var result = _outputNeurons.Select(x => x.Signal).ToArray();
            Reset();

            return result;
        }

        public void Reset()
        {
            foreach (var neuron in _network.Vertices) neuron.Reset();
        }

        public NeuralNetwork InterbreedWith(NeuralNetwork other)
        {
            return this;
        }

        private static void AddSignal(Connection edge)
        {
            edge.Target.AddToInput(edge.Source.Signal * edge.Tag);
        }
    }
}
