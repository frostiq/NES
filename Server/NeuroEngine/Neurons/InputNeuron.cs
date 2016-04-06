namespace NeuroEngine.Neurons
{
    public class InputNeuron : NeuronWithInput
    {
        public InputNeuron(string tag = "") : base(tag)
        {
        }

        public override double Signal => Input;
    }
}
