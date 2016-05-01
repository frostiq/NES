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
    }
}
