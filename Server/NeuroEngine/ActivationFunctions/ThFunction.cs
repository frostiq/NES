using System;

namespace NeuroEngine.ActivationFunctions
{
    public class ThFunction : IActivationFunction
    {
        private readonly double _alpha;

        public ThFunction(double alpha = 1d)
        {
            _alpha = alpha;
        }

        public double Apply(double input) => Math.Tanh(input/_alpha);
    }
}