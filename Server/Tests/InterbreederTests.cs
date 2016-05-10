﻿using System.Xml;
using NeuroEngine;
using NeuroEngine.ActivationFunctions;
using NeuroEngine.Neurons;
using NUnit.Framework;
using Ploeh.AutoFixture;
using QuickGraph;
using QuickGraph.Serialization;

namespace Tests
{
    [TestFixture]
    public class InterbreederTests
    {
        private Fixture _fixture;
        private Interbreeder _interbreeder;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _interbreeder = new Interbreeder();
        }

        [Test]
        public void Test_Interbreed()
        {
            var a = BuildRedNeuralNetwork();
            var b = BuildBlueNeuralNetwork();

            var res = _interbreeder.Interbreed(a, b);

            res.Serialize(@"C:\Temp\graph.graphml");
        }

        public NeuralNetwork BuildRedNeuralNetwork()
        {
            var iToMSignal = 1d;
            var mToOSignal = 2d;
            var inputs = new[] { new NeuronWithInput("red:1"), new NeuronWithInput("red:2"), new NeuronWithInput("red:3") };
            var mids = new[] { new BasicNeuron(new IdentityFunction(), "red:4"), new BasicNeuron(new IdentityFunction(), "red:5") };
            var output = new[] { new BasicNeuron(new IdentityFunction(), "red:6") };
            var graph = new AdjacencyGraph<INeuron, Connection>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Connection(inputs[0], mids[0], iToMSignal),
                new Connection(inputs[1], mids[0], iToMSignal),
                new Connection(inputs[1], mids[1], iToMSignal),
                new Connection(inputs[2], mids[1], iToMSignal),
                new Connection(mids[0], output[0], mToOSignal),
                new Connection(mids[1], output[0], mToOSignal),
                new Connection(mids[1], inputs[1], mToOSignal),
                new Connection(mids[1], mids[0], mToOSignal),
            });
            return new NeuralNetwork(graph, inputs, output);
        }

        public NeuralNetwork BuildBlueNeuralNetwork()
        {
            var iToMSignal = 1d;
            var mToOSignal = 2d;
            var inputs = new[] { new NeuronWithInput("blue:1"), new NeuronWithInput("blue:2") };
            var mids = new[]
            {
                new BasicNeuron(new IdentityFunction(), "blue:3"),
                new BasicNeuron(new IdentityFunction(), "blue:4"),
                new BasicNeuron(new IdentityFunction(), "blue:5")
            };
            var output = new[] { new BasicNeuron(new IdentityFunction(), "blue:6") };
            var graph = new AdjacencyGraph<INeuron, Connection>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Connection(inputs[0], mids[0], iToMSignal),
                new Connection(inputs[1], mids[1], iToMSignal),
                new Connection(inputs[1], mids[2], iToMSignal),
                new Connection(mids[0], mids[1], mToOSignal),
                new Connection(mids[1], output[0], mToOSignal),
                new Connection(mids[2], output[0], mToOSignal),
            });
            return new NeuralNetwork(graph, inputs, output);
        }


    }
}
