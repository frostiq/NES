using System;

namespace Common.Contracts
{
    [Serializable]
    public struct Control
    {
        public readonly float DeltaAngle;
        public readonly float DeltaVelocity;
        public readonly bool Restart;

        public Control(float deltaAngle, float deltaVelocity, bool restart)
        {
            DeltaAngle = deltaAngle;
            DeltaVelocity = deltaVelocity;
            Restart = restart;
        }
    }
}