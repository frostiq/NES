namespace NeuroEngine.Neurons
{
    public abstract class NeuronWithInput : INeuron
    {
        protected double Input;
        protected string Tag;

        protected NeuronWithInput(string tag = "")
        {
            Tag = tag;
        }

        public abstract double Signal { get; }

        public virtual NeuronWithInput AddToInput(double value)
        {
            Input += value;
            return this;
        }

        public virtual void Reset()
        {
            Input = 0d;
        }
    }
}