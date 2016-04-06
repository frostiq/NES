using NeuroEngine.ActivationFunctions;

namespace NeuroEngine.Neurons
{
    public class BasicNeuron : NeuronWithInput
    {
        private double? _signal;

        private readonly IActivationFunction _activationFunction;

        public BasicNeuron(IActivationFunction activationFunction, string tag = "")
        {
            _activationFunction = activationFunction;
            Tag = tag;
        }

        public override double Signal
        {
            get
            {
                if (!_signal.HasValue)
                {
                    _signal = _activationFunction.Apply(Input);
                }
                return _signal.Value;
            }
        }

        public override NeuronWithInput AddToInput(double value)
        {
            _signal = null;
            return base.AddToInput(value);
        }

        public override void Reset()
        {
            base.Reset();
            _signal = null;
        }
    }
}
