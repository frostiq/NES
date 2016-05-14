using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NeuroEngine;

namespace DAL
{
    public class InMemoryNeuroRepository : INeuroRepository
    {
        private readonly IDictionary<Guid, NeuralNetwork> _networks = new ConcurrentDictionary<Guid, NeuralNetwork>(); 

        public void Create(NeuralNetwork neuralNetwork)
        {
            _networks.Add(neuralNetwork.Guid, neuralNetwork);
        }

        public NeuralNetwork Get(Guid guid)
        {
            return _networks[guid];
        }
    }
}
