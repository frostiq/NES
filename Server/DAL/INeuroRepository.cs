using System;
using NeuroEngine;

namespace DAL
{
    public interface INeuroRepository
    {
        void Create(NeuralNetwork neuralNetwork);

        NeuralNetwork Get(Guid guid);

    }
}
