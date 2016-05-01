namespace NeuroEngine.Neurons
{
    public interface INeuron
    {
        double Signal { get; }

        INeuron AddToInput(double value);

        void Reset();
    }
}
