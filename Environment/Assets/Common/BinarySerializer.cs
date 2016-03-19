using System;
using Common.Contracts;

namespace Common.Utility
{
    public class BinarySerializer : ISerializer<Deltas>
    {
        public byte[] Serialize(Deltas entity)
        {
            var res = new byte[8];
            Array.Copy(BitConverter.GetBytes(entity.DeltaAngle), res, 4);
            Array.Copy(BitConverter.GetBytes(entity.DeltaVelocity), 0, res, 4, 4);

            return res;
        }

        public Deltas Deserialize(byte[] bytes)
        {
            return new Deltas(
                BitConverter.ToSingle(bytes, 0),
                BitConverter.ToSingle(bytes, 4)
                );
        }
    }
}
