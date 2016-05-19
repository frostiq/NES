using System;
using System.Threading;
using Common.Contracts;
using Common.Utility;
using NeuroEngine;
using System.Threading.Tasks;
using NLog;

namespace NeuroServer
{
    public interface IRemoteTestingController
    {
        float Test(NeuralNetwork neuralNetwork);
    }

    public class RemoteTestingController : IRemoteTestingController, IDisposable
    {
        private const int InputNeuronsCount = 9;
        private readonly TimeSpan _timeOfExperiment = TimeSpan.FromSeconds(20);

        private readonly ImageManager _imageManager;
        private readonly ILogger _logger;

        private readonly ManualResetEventSlim _waiter = new ManualResetEventSlim(false);
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private NeuralNetwork _currentNeuralNetwork;
        private Response.MessageType _currentMessageType = Response.MessageType.Pause;
        private float _lastScore;

        public RemoteTestingController(ImageManager imageManager, ILogger logger)
        {
            _imageManager = imageManager;
            _logger = logger;
        }

        public float Test(NeuralNetwork neuralNetwork)
        {
            Signal(Response.MessageType.Control);

            Task.Delay(_timeOfExperiment).Wait();

            Signal(Response.MessageType.Start);

            return _lastScore;
        }

        public Response Compute(Request request)
        {
            Response response;

            _lock.EnterReadLock();
            switch (_currentMessageType)
            {
                case Response.MessageType.Control:
                {
                    var image = ((ImageRequest)request).Image;
                    var input = _imageManager.ConvertFromPngToInput(image, InputNeuronsCount);
                    input = _imageManager.Normalize(input);
                    var output = _currentNeuralNetwork.Compute(input);
                    response = new Response(Response.MessageType.Control, (float)output[0], (float)output[1]);
                    break;
                }
                case Response.MessageType.Start:
                    _lastScore = ((ScoreRequest) request).Score;
                    response = new Response(Response.MessageType.Control);
                    break;

                default:
                    response = new Response(_currentMessageType);
                    break;
            }
            _waiter.Set();
            _logger.Info($"Response({response}) Request({request})");
            _lock.ExitReadLock();

            return response;
        }

        public void Dispose()
        {
            Signal(Response.MessageType.End);
        }

        private void Signal(Response.MessageType messageType)
        {
            _lock.EnterWriteLock();
            _currentMessageType = messageType;
            _waiter.Reset();
            _logger.Info($"{messageType}. Waiting for request...");
            _lock.ExitWriteLock();
            _waiter.Wait();
        }
    }
}
