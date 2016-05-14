using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MoreLinq;
using NeuroEngine.Neurons;
using QuickGraph;
using QuickGraph.Algorithms.Search;
using QuickGraph.Serialization;

namespace NeuroEngine
{
    public class NeuralNetwork
    {
        private readonly AdjacencyGraph<INeuron, Connection> _network;
        private readonly ICollection<INeuron> _inputNeurons;
        private readonly ICollection<INeuron> _outputNeurons;
        private readonly BreadthFirstSearchAlgorithm<INeuron, Connection> _algorithm;

        public INeuron AlphaVertex { get; }
        public INeuron OmegaVertex { get; }
        public Guid Guid { get; } = Guid.NewGuid();

        public NeuralNetwork
            (AdjacencyGraph<INeuron, Connection> network, 
            ICollection<INeuron> inputNeurons, 
            ICollection<INeuron> outputNeurons)
        {
            _network = network;
            _outputNeurons = outputNeurons;
            _inputNeurons = inputNeurons;

            AlphaVertex = new EmptyNeuron();
            _network.AddVertex(AlphaVertex);
            foreach (var inputNeuron in _inputNeurons)
            {
                _network.AddEdge(new Connection(AlphaVertex, inputNeuron, 0d));
            }

            OmegaVertex = new EmptyNeuron();
            _network.AddVertex(OmegaVertex);
            foreach (var outputNeuron in _outputNeurons)
            {
                _network.AddEdge(new Connection(outputNeuron, OmegaVertex, 0d));
            }

            _algorithm = new BreadthFirstSearchAlgorithm<INeuron, Connection>(_network);
            _algorithm.ExamineEdge += AddSignal;
            _algorithm.SetRootVertex(AlphaVertex);
        }

        public NeuralNetwork(AdjacencyGraph<INeuron, Connection> network, INeuron alphaVertex, INeuron omegaVertex)
        {
            _network = network;
            AlphaVertex = alphaVertex;
            OmegaVertex = omegaVertex;

            _inputNeurons = _network.OutEdges(AlphaVertex).Select(e => e.Target).ToList();
            _outputNeurons = _network.Edges.Where(e => e.Target == OmegaVertex).Select(e => e.Source).ToList();

            _algorithm = new BreadthFirstSearchAlgorithm<INeuron, Connection>(_network);
            _algorithm.ExamineEdge += AddSignal;
            _algorithm.SetRootVertex(AlphaVertex);
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

        public IEnumerable<Connection> GetConnections(INeuron neuron)
        {
            return _network.OutEdges(neuron);
        }

        public void Serialize(string filePath)
        {
            using (var writer = XmlWriter.Create(filePath))
            {
                _network.SerializeToGraphML<INeuron, Connection, AdjacencyGraph<INeuron, Connection>>(writer);
            }
        }

        private static void AddSignal(Connection edge)
        {
            edge.Target.AddToInput(edge.Source.Signal * edge.Weight);
        }
    }
}
