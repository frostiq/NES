using System;
using System.Security.Cryptography.X509Certificates;
using NeuroEngine.Neurons;

namespace NeuroEngine
{
    public class Connection : QuickGraph.TaggedEdge<INeuron, double>, IEquatable<Connection>
    {
        public Connection(INeuron source, INeuron target, double weight) : base(source, target, weight)
        {}

        public double Weight => Tag;

        public bool Equals(Connection other)
        {
            return Source.Equals(other.Source) && Target.Equals(other.Target);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Connection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Source?.GetHashCode() ?? 0)*397) ^ (Target?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(Connection left, Connection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Connection left, Connection right)
        {
            return !Equals(left, right);
        }
    }
}
