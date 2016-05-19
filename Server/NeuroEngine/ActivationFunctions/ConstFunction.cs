namespace NeuroEngine.ActivationFunctions
{
    public class ConstFunction : IActivationFunction
    {
        private readonly double _output;

        public ConstFunction(double output)
        {
            _output = output;
        }

        public double Apply(double input) => _output;
    }
}