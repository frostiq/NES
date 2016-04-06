using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NeuroEngine.Neurons;
using QuickGraph;
using QuickGraph.Algorithms.Search;
using Connection = QuickGraph.TaggedEdge<NeuroEngine.Neurons.INeuron, double>;

namespace NeuroEngine
{
    public class NeuralNetwork
    {
        private readonly AdjacencyGraph<INeuron, Connection> _network;
        private readonly ICollection<InputNeuron> _inputNeurons;
        private readonly ICollection<BasicNeuron> _outputNeurons;
        private readonly BreadthFirstSearchAlgorithm<INeuron, Connection> _algorithm;

        public NeuralNetwork
            (AdjacencyGraph<INeuron, Connection> network, 
            ICollection<InputNeuron> inputNeurons, 
            ICollection<BasicNeuron> outputNeurons)
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

        public ICollection<double> Compute(ICollection<double> input)
        {
            if (input.Count != _inputNeurons.Count)
            {
                throw new ArgumentException("input.Count != inputNeurons.Count");
            }

            Enumerable.Zip(input, _inputNeurons, (d, neuron) => neuron.AddToInput(d)).Consume();

            _algorithm.Compute();

            return _outputNeurons.Select(x => x.Signal).ToList();
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
