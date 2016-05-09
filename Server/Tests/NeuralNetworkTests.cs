using System.Linq;
using FluentAssertions;
using NeuroEngine;
using NeuroEngine.ActivationFunctions;
using NeuroEngine.Neurons;
using NUnit.Framework;
using Ploeh.AutoFixture;
using QuickGraph;

namespace Tests
{
    [TestFixture]
    public class NeuralNetworkTests
    {

        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void TestCompute_LinkWeight()
        {
            var linkWeight = _fixture.Create<double>();

            var input = new NeuronWithInput();
            var output = new BasicNeuron(new IdentityFunction());
            var graph = new AdjacencyGraph<INeuron, Connection>();
            graph.AddVerticesAndEdge(new Connection(input, output, linkWeight));
            var neuralNetwork = new NeuralNetwork(graph, new []{input}, new []{output});

            var res = neuralNetwork.Compute(new[] {1d});

            res.Should().HaveCount(1);
            res.First().ShouldBeEquivalentTo(linkWeight);
        }

        [Test]
        public void TestCompute_Input()
        {
            var inputSignal = _fixture.Create<double>();

            var input = new NeuronWithInput();
            var output = new BasicNeuron(new IdentityFunction());
            var graph = new AdjacencyGraph<INeuron, Connection>();
            graph.AddVerticesAndEdge(new Connection(input, output, 1d));
            var neuralNetwork = new NeuralNetwork(graph, new[] { input }, new[] { output });

            var res = neuralNetwork.Compute(new[] { inputSignal });

            res.Should().HaveCount(1);
            res.First().ShouldBeEquivalentTo(inputSignal);
        }

        [Test]
        public void TestCompute_ActivationFunc()
        {
            
        }

        [Test]
        public void TestCompute_MultiNeurons()
        {
            var iToMSignal = 1d;
            var mToOSignal = 2d;
            var inputSignals = new[] {_fixture.Create<double>(), _fixture.Create<double>()};
            var inputs = new[] {new NeuronWithInput(), new NeuronWithInput()};
            var mids = new[] {new BasicNeuron(new IdentityFunction(), "m1"), new BasicNeuron(new IdentityFunction(), "m2")};
            var output = new[] {new BasicNeuron(new IdentityFunction(), "o1"), new BasicNeuron(new IdentityFunction(), "o2")};
            var graph = new AdjacencyGraph<INeuron, Connection>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Connection(inputs[0], mids[0], iToMSignal),
                new Connection(inputs[0], mids[1], iToMSignal),
                new Connection(inputs[1], mids[1], iToMSignal),
                new Connection(inputs[1], mids[0], iToMSignal),
                new Connection(mids[0], output[0], mToOSignal),
                new Connection(mids[0], output[1], mToOSignal),
                new Connection(mids[1], output[1], mToOSignal),
                new Connection(mids[1], output[0], mToOSignal),
            });
            var neuralNetwork = new NeuralNetwork(graph, inputs, output);

            var res = neuralNetwork.Compute(inputSignals);

            res.Should().HaveSameCount(output);
            res.Should().HaveElementAt(0, 4 * inputSignals.Sum());
            res.Should().HaveElementAt(1, 4 * inputSignals.Sum());
        }
    }
}
