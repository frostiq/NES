﻿using NeuroEngine.ActivationFunctions;

namespace NeuroEngine.Neurons
{
    public class BasicNeuron : NeuronWithInput
    {
        private double? _signal;

        private readonly IActivationFunction _activationFunction;

        public BasicNeuron(IActivationFunction activationFunction, string tag = null)
            : base(tag)
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

        public override INeuron AddToInput(double value)
        {
            _signal = null;
            return base.AddToInput(value);
        }

        public override void Reset()
        {
            base.Reset();
            _signal = null;
        }

        public override IActivationFunction ActivationFunction => _activationFunction;
    }
}
