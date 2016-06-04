using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using NeuroEngine;
using NLog;

namespace NeuroServer
{
    public class TrainingSupervisor
    {
        private readonly INeuroRepository _repository;
        private readonly IRemoteTestingController _remoteTestingController;
        private readonly IInterbreeder _interbreeder;
        private int n = 1;

        public TrainingSupervisor(IInterbreeder interbreeder, INeuroRepository repository, IRemoteTestingController remoteTestingController)
        {
            _interbreeder = interbreeder;
            _repository = repository;
            _remoteTestingController = remoteTestingController;
        }

        public void Train(IEnumerable<NeuralNetwork> initSet)
        {
            while (true)
            {
                var testedSet = initSet.Select(nn => new TestResult { Score = _remoteTestingController.Test(nn), NeuralNetwork = nn }).ToList();

                var logger = LogManager.GetLogger("res");
                logger.Info(string.Join(", " ,testedSet.Select(x => x.Score)));

                if (IsSatisfying(testedSet))
                    break;

                var selectedSet = Select(testedSet);

                initSet = selectedSet.Select(tuple => _interbreeder.Interbreed(tuple.Item1, tuple.Item2)).ToList();
            }

        }

        private IEnumerable<Tuple<NeuralNetwork, NeuralNetwork>> Select(ICollection<TestResult> testedSet)
        {
            var res = testedSet.OrderByDescending(x => x.Score).Take(2).ToArray();
            return new []{Tuple.Create(res[0].NeuralNetwork, res[1].NeuralNetwork)};
        }

        private bool IsSatisfying(ICollection<TestResult> testedSet)
        {
            return --n == 0;
        }

        private class TestResult
        {
            public float Score { get; set; }
            public NeuralNetwork NeuralNetwork { get; set; }
        }
    }
}
