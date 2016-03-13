using System;
using Common.Contracts;

namespace Common.Utility
{
    public class BinarySerializer : ISerializer<Deltas>
    {
        public byte[] Serialize(Deltas entity)
        {
            var res = new byte[16];
            Array.Copy(BitConverter.GetBytes(entity.DeltaAngle), res, 8);
            Array.Copy(BitConverter.GetBytes(entity.DeltaVelocity), 0, res, 8, 8);

            return res;
        }

        public Deltas Deserialize(byte[] bytes)
        {
            return new Deltas(
                BitConverter.ToSingle(bytes, 0),
                BitConverter.ToSingle(bytes, 8)
                );
        }
    }
}
