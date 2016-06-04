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
        private readonly Logger _logger = LogManager.GetLogger("TrainingSupervisor");

        public TrainingSupervisor(IInterbreeder interbreeder, INeuroRepository repository, IRemoteTestingController remoteTestingController)
        {
            _interbreeder = interbreeder;
            _repository = repository;
            _remoteTestingController = remoteTestingController;
        }

        public void Train(IReadOnlyCollection<NeuralNetwork> initSet)
        {
            while (true)
            {
                _logger.Info($"Testing set (N={initSet.Count})");
                var testedSet = initSet.Select(nn => new TestResult { Score = _remoteTestingController.Test(nn), NeuralNetwork = nn }).ToList();

                _logger.Info(string.Join(", " ,testedSet.Select(x => x.Score)));

                if (IsSatisfying(testedSet))
                    break;

                var selectedSet = Select(testedSet);
                _logger.Info($"{selectedSet.Count()} pairs of neural networks selected for interbreeding");

                initSet = selectedSet.Select(tuple => _interbreeder.Interbreed(tuple.Item1, tuple.Item2)).ToList();
            }

        }

        private IEnumerable<Tuple<NeuralNetwork, NeuralNetwork>> Select(IReadOnlyCollection<TestResult> testedSet)
        {
            var res = testedSet.OrderByDescending(x => x.Score).Take(2).ToArray();

            return 
            from a in res
            from b in res
            select Tuple.Create(a.NeuralNetwork, b.NeuralNetwork);
        }

        private static bool IsSatisfying(IReadOnlyCollection<TestResult> testedSet)
        {
            return testedSet.Any(x => x.Score >= 4);
        }

        private class TestResult
        {
            public float Score { get; set; }
            public NeuralNetwork NeuralNetwork { get; set; }
        }
    }
}
