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
        private readonly Queue<Connection> _blueQueue = new Queue<Connection>();
        private readonly Queue<Connection> _redQueue = new Queue<Connection>();
        private readonly ISet<INeuron> _markers = new HashSet<INeuron>();
        private readonly ISet<Tuple<INeuron, INeuron>> _newNeurons = new HashSet<Tuple<INeuron, INeuron>>();
        private readonly Random _random = new Random();

        public NeuralNetwork Interbreed(NeuralNetwork redNetwork, NeuralNetwork blueNetwork)
        {
            redNetwork.GetConnections(redNetwork.RootVertex)
                .OrderBy(x => _random.Next())
                .ForEach(_redQueue.Enqueue);
            blueNetwork.GetConnections(blueNetwork.RootVertex)
                .OrderBy(x => _random.Next())
                .ForEach(_blueQueue.Enqueue);

            var result = new AdjacencyGraph<INeuron, Connection>();
            var inputs = new HashSet<INeuron>();
            var outputs = new HashSet<INeuron>();

            while (_redQueue.Count > 0 && _blueQueue.Count > 0)
            {
                var za = _redQueue.Dequeue();
                var yb = _blueQueue.Dequeue();
                var a = za.Target;
                var b = yb.Target;

                if (IsNotMarked(a) && IsNotMarked(b))
                {
                    _newNeurons.Add(Tuple.Create(a,b));
                    Mark(a, b);
                }

                var newConnections = Translate(za).Concat(Translate(yb));
                result.AddEdgeRange(newConnections);

                AddRandomSubsetToQueue(redNetwork.GetConnections(a), _redQueue);
                AddRandomSubsetToQueue(blueNetwork.GetConnections(b), _blueQueue);
            }

            return new NeuralNetwork(result, inputs, outputs);
        }


        private INeuron Interbreed(INeuron a, INeuron b)
        {
            if (a == null || b == null)
                return null;

            return new BasicNeuron(_random.Next() % 2 == 0 ? a.ActivationFunction : b.ActivationFunction);
        }

        private IEnumerable<Connection> Translate(Connection connection)
        {
            var sources = FindOffspring(connection.Source);
            var targets = FindOffspring(connection.Target);

            return
            from s in sources
            from t in targets
            select new Connection(s, t, connection.Weight);
        }

        private ICollection<INeuron> FindOffspring(INeuron ancestor)
        {
            return _newNeurons.Where(t => t.Item1 == ancestor || t.Item2 == ancestor)
                              .Select(t => Interbreed(t.Item1, t.Item2))
                              .ToList();
        } 
        
        private void AddRandomSubsetToQueue(IEnumerable<Connection> connections, Queue<Connection> queue)
        {
            var selected = connections.Where(x => _random.Next() % 3 > 0);

            selected.ForEach(queue.Enqueue);
        }

        private bool IsNotMarked(INeuron neuron)
        {
            return !_markers.Contains(neuron);
        }

        private void Mark(params INeuron[] neurons)
        {
            _markers.UnionWith(neurons);
        }
    }
}
