using System;
using Common.Contracts;

namespace Common.Utility
{
    public class BinarySerializer : ISerializer<Control>
    {
        public byte[] Serialize(Control entity)
        {
            var res = new byte[9];
            Array.Copy(BitConverter.GetBytes(entity.DeltaAngle), res, 4);
            Array.Copy(BitConverter.GetBytes(entity.DeltaVelocity), 0, res, 4, 4);
            Array.Copy(BitConverter.GetBytes(entity.Restart), 0, res, 8, 1);

            return res;
        }

        public Control Deserialize(byte[] bytes)
        {
            return new Control(
                BitConverter.ToSingle(bytes, 0),
                BitConverter.ToSingle(bytes, 4),
                BitConverter.ToBoolean(bytes, 8)
                );
        }
    }
}
