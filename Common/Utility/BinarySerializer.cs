using System;
using System.Linq;
using Common.Contracts;

namespace Common.Utility
{
    public class BinarySerializer : ISerializer<Response>, ISerializer<Request>
    {
        public byte[] Serialize(Response entity)
        {
            var res = new byte[9];
            res[0] = (byte)entity.Type;
            Array.Copy(BitConverter.GetBytes(entity.DeltaAngle), 0, res, 1, 4);
            Array.Copy(BitConverter.GetBytes(entity.DeltaVelocity), 0, res, 5, 4);

            return res;
        }

        Response ISerializer<Response>.Deserialize(byte[] bytes)
        {
            return new Response(
                (Response.MessageType)bytes[0],
                BitConverter.ToSingle(bytes, 1),
                BitConverter.ToSingle(bytes, 5)
                );
        }

        public byte[] Serialize(Request entity)
        {
            byte[] res;
            switch (entity.Type)
            {
                case Request.MessageType.Image:
                {
                    var imageRequest = (ImageRequest) entity;
                    res = new byte[imageRequest.Image.Length + 1];
                    res[0] = (byte) entity.Type;
                    Array.Copy(imageRequest.Image, 0, res, 1, imageRequest.Image.Length);
                    break;
                }
                case Request.MessageType.Score:
                {
                    var scoreRequest = (ScoreRequest) entity;
                    res = new byte[5];
                    res[0] = (byte) entity.Type;
                    Array.Copy(BitConverter.GetBytes(scoreRequest.Score), 0, res, 1, 4);
                    break;
                }
                case Request.MessageType.Empty:
                    res = new byte[1];
                    res[0] = (byte)entity.Type;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return res;
        }

        Request ISerializer<Request>.Deserialize(byte[] bytes)
        {
            var type = (Request.MessageType)bytes[0];
            switch (type)
            {
                case Request.MessageType.Image:
                    return new ImageRequest(bytes.Skip(1).ToArray());
                case Request.MessageType.Score:
                    return new ScoreRequest(BitConverter.ToSingle(bytes, 1));
                case Request.MessageType.Empty:
                    return new EmptyRequest();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
