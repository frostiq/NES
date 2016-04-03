namespace NeuroEngine.Neurons
{
    public abstract class AbstractNeuron
    {
        protected double Input;

        public AbstractNeuron AddToInput(double value)
        {
            Input += value;
            return this;
        }

        public abstract double Signal { get; }

        public virtual void Reset()
        {
            Input = 0d;
        }
    }
}