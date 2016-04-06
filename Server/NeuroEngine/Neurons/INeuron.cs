namespace NeuroEngine.Neurons
{
    public interface INeuron
    {
        double Signal { get; }
        NeuronWithInput AddToInput(double value);
        void Reset();
    }
}
