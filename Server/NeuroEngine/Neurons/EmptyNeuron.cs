namespace NeuroEngine.Neurons
{
    public class EmptyNeuron : INeuron
    {
        public double Signal => 0d;

        public NeuronWithInput AddToInput(double value)
        {
            throw new System.NotSupportedException();
        }

        public void Reset()
        {
        }
    }
}
