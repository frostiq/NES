using System;

namespace Common.Contracts
{
    [Serializable]
    public struct Response
    {
        public readonly MessageType Type;
        public readonly float DeltaAngle;
        public readonly float DeltaVelocity;

        public enum MessageType : byte
        {
            Pause = 0,
            Start = 1,
            Control = 2,
            Finish = 3,
            End = 4
        }

        public Response(MessageType type, float deltaAngle = 0f, float deltaVelocity = 0f)
        {
            Type = type;
            DeltaAngle = deltaAngle;
            DeltaVelocity = deltaVelocity;
        }

        public override string ToString()
        {
            return $"{Type}: {DeltaAngle} {DeltaVelocity}";
        }
    }

    public abstract class Request
    {
        public readonly MessageType Type;
        
        public enum MessageType : byte
        {
            Score,
            Image,
            Empty
        }

        protected Request(MessageType type)
        {
            Type = type;
        }
    }

    public class ScoreRequest : Request
    {
        public readonly float Score;

        public ScoreRequest(float score) : base(MessageType.Score)
        {
            Score = score;
        }

        public override string ToString()
        {
            return $"{Type}: {Score}";
        }
    }

    public class ImageRequest : Request
    {
        public readonly byte[] Image;

        public ImageRequest(byte[] image) : base(MessageType.Image)
        {
            Image = image;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    public class EmptyRequest : Request
    {
        public EmptyRequest() : base(MessageType.Empty)
        {
        }
    }
}