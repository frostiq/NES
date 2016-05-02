using NeuroEngine.ActivationFunctions;

namespace NeuroEngine.Neurons
{
    public class NeuronWithInput : INeuron
    {
        protected double Input;
        protected string Tag;

        public NeuronWithInput(string tag = "")
        {
            Tag = tag;
        }

        public virtual double Signal => Input;

        public virtual INeuron AddToInput(double value)
        {
            Input += value;
            return this;
        }

        public virtual void Reset()
        {
            Input = 0d;
        }

        public virtual IActivationFunction ActivationFunction { get; } = new IdentityFunction();
    }
}