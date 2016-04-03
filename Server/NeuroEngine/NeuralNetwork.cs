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
        private readonly AdjacencyGraph<AbstractNeuron, TaggedEdge<AbstractNeuron, double>> _network;
        private readonly ICollection<InputNeuron> _inputNeurons;
        private readonly ICollection<Neuron> _outputNeurons;

        public NeuralNetwork
            (AdjacencyGraph<AbstractNeuron, TaggedEdge<AbstractNeuron, double>> network, 
            ICollection<InputNeuron> inputNeurons, 
            ICollection<Neuron> outputNeurons)
        {
            _network = network;
            _outputNeurons = outputNeurons;
            _inputNeurons = inputNeurons;
        }

        public ICollection<double> Compute(ICollection<double> input)
        {
            if (input.Count != _inputNeurons.Count)
            {
                throw new ArgumentException("input.Count != inputNeurons.Count");
            }

            Enumerable.Zip(input, _inputNeurons, (d, neuron) => neuron.AddToInput(d)).Consume();

            var alg = new BreadthFirstSearchAlgorithm<AbstractNeuron, TaggedEdge<AbstractNeuron, double>>(_network);
            alg.ExamineEdge += AddSignal;
            alg.Compute();

            return _outputNeurons.Select(x => x.Signal).ToList();
        }

        public void Reset()
        {
            foreach (var neuron in _network.Vertices) neuron.Reset();
        }

        private static void AddSignal(TaggedEdge<AbstractNeuron, double> edge)
        {
            edge.Target.AddToInput(edge.Source.Signal * edge.Tag);
        }
    }
}
