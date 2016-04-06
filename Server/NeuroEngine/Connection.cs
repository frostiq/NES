using NeuroEngine.Neurons;

namespace NeuroEngine
{
    public class Connection : QuickGraph.TaggedEdge<INeuron, double>
    {
        public Connection(INeuron source, INeuron target, double tag) : base(source, target, tag)
        {
        }
    }
}
