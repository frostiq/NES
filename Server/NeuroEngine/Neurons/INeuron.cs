using NeuroEngine.ActivationFunctions;

namespace NeuroEngine.Neurons
{
    public interface INeuron
    {
        double Signal { get; }

        INeuron AddToInput(double value);

        void Reset();

        IActivationFunction ActivationFunction { get; }

        string Tag { get; }
    }
}
