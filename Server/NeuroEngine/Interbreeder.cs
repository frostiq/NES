using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NeuroEngine.Neurons;
using QuickGraph;

namespace NeuroEngine
{
    public interface IInterbreeder
    {
        NeuralNetwork Interbreed(NeuralNetwork a, NeuralNetwork b);
    }

    public class Interbreeder : IInterbreeder
    {
        private readonly Queue<Connection> blueQueue = new Queue<Connection>();
        private readonly Queue<Connection> redQueue = new Queue<Connection>();
        private readonly ISet<INeuron> markers = new HashSet<INeuron>();
        private readonly ISet<Tuple<INeuron, INeuron, INeuron>> newNeurons = new HashSet<Tuple<INeuron, INeuron, INeuron>>();
        private readonly Random random = new Random();
        private EmptyNeuron alpha;
        private EmptyNeuron omega;
        private AdjacencyGraph<INeuron, Connection> resultNetwork;

        public NeuralNetwork Interbreed(NeuralNetwork redNetwork, NeuralNetwork blueNetwork)
        {
            redNetwork.GetConnections(redNetwork.AlphaVertex)
                .OrderBy(x => random.Next())
                .ForEach(redQueue.Enqueue);
            blueNetwork.GetConnections(blueNetwork.AlphaVertex)
                .OrderBy(x => random.Next())
                .ForEach(blueQueue.Enqueue);

            alpha = new EmptyNeuron();
            omega = new EmptyNeuron();
            resultNetwork = new AdjacencyGraph<INeuron, Connection>();
            resultNetwork.AddVertex(alpha);
            resultNetwork.AddVertex(omega);

            while (redQueue.Count > 0 && blueQueue.Count > 0)
            {
                var za = redQueue.Dequeue();
                var yb = blueQueue.Dequeue();
                var a = za.Target;
                var b = yb.Target;

                if (IsNotMarked(a) && IsNotMarked(b))
                {
                    var product = Interbreed(a, b);
                    newNeurons.Add(Tuple.Create(a, b, product));
                    Mark(a, b);
                }

                var newConnections = Translate(za).Concat(Translate(yb)).ToList();
                AddNewConnections(newConnections);

                AddRandomSubsetToQueue(redNetwork.GetConnections(a), redQueue);
                AddRandomSubsetToQueue(blueNetwork.GetConnections(b), blueQueue);
            }

            return new NeuralNetwork(resultNetwork, alpha, omega);
        }


        private INeuron Interbreed(INeuron a, INeuron b)
        {
            if (a is EmptyNeuron)
                return a;
            if (b is EmptyNeuron)
                return b;

            return new BasicNeuron(random.Next() % 2 == 0 ? a.ActivationFunction : b.ActivationFunction, a.Tag + b.Tag);
        }

        private IEnumerable<Connection> Translate(Connection connection)
        {
            var sources = FindOffsprings(connection.Source);
            var targets = FindOffsprings(connection.Target);

            sources = sources.Any() ? sources : new[] { alpha };
            targets = targets.Any() ? targets : new[] { omega };

            return
            from s in sources
            from t in targets
            select new Connection(s, t, connection.Weight);
        }

        private ICollection<INeuron> FindOffsprings(INeuron ancestor)
        {
            return newNeurons.Where(t => t.Item1 == ancestor || t.Item2 == ancestor)
                             .Select(t => t.Item3)
                             .ToList();
        }

        private void AddRandomSubsetToQueue(IEnumerable<Connection> connections, Queue<Connection> queue)
        {
            var selected = connections.Where(x => random.Next() % 3 > 0);

            selected.ForEach(queue.Enqueue);
        }

        private bool IsNotMarked(INeuron neuron)
        {
            return !markers.Contains(neuron);
        }

        private void Mark(params INeuron[] neurons)
        {
            markers.UnionWith(neurons);
        }

        private void AddNewConnections(IEnumerable<Connection> connections)
        {
            foreach (var c in connections)
            {
                if (!resultNetwork.ContainsEdge(c))
                {
                    resultNetwork.AddVerticesAndEdge(c);
                }
            }
        }
    }
}
