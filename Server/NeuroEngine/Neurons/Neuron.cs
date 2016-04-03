using NeuroEngine.ActivationFunctions;

namespace NeuroEngine.Neurons
{
    public class Neuron : AbstractNeuron
    {
        private double? _signal;

        private readonly IActivationFunction _activationFunction;

        public Neuron(IActivationFunction activationFunction)
        {
            _activationFunction = activationFunction;
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

        public override void Reset()
        {
            base.Reset();
            _signal = null;
        }
    }
}
