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

            while (_redQueue.Count > 0 && _blueQueue.Count > 0)
            {
                var za = _redQueue.Dequeue();
                var yb = _blueQueue.Dequeue();
                var a = za.Target;
                var b = yb.Target;

                if (IsNotMarked(a) && IsNotMarked(b))
                {
                    var axb = Interbreed(a, b);
                    result.AddVertex(axb);
                    Mark(a, b);
                }

                var newConnections = Translate(za).Concat(Translate(yb));
                result.AddEdgeRange(newConnections);

                GetRandomSubset(redNetwork.GetConnections(a)).ForEach(_redQueue.Enqueue);
                GetRandomSubset(blueNetwork.GetConnections(b)).ForEach(_blueQueue.Enqueue);
            }

            return new NeuralNetwork(result, null, null);
        }


        private INeuron Interbreed(INeuron a, INeuron b)
        {
            return null;
        }

        private ICollection<Connection> Translate(Connection connection)
        {
            return null;
        }

        private IEnumerable<Connection> GetRandomSubset(IEnumerable<Connection> connections)
        {
            return connections.Where(x => _random.Next()%3 > 0);
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
