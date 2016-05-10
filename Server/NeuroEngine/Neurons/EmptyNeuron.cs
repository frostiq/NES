using NeuroEngine.ActivationFunctions;

namespace NeuroEngine.Neurons
{
    public class EmptyNeuron : INeuron
    {
        public double Signal => 0d;

        public INeuron AddToInput(double value)
        {
            return this;
        }

        public void Reset()
        {
        }

        public IActivationFunction ActivationFunction { get; } = new IdentityFunction();

        public string Tag => GetType().Name;
    }
}
