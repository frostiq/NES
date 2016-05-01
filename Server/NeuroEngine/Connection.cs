using System.Security.Cryptography.X509Certificates;
using NeuroEngine.Neurons;

namespace NeuroEngine
{
    public class Connection : QuickGraph.TaggedEdge<INeuron, double>
    {
        public Connection(INeuron source, INeuron target, double weight) : base(source, target, weight)
        {
            
        }

        public double Weight => Tag;
    }
}
