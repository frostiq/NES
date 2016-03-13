using System;

namespace Common.Contracts
{
    [Serializable]
    public struct Deltas
    {
        public readonly float DeltaAngle;
        public readonly float DeltaVelocity;

        public Deltas(float deltaAngle, float deltaVelocity)
        {
            DeltaAngle = deltaAngle;
            DeltaVelocity = deltaVelocity;
        }
    }
}