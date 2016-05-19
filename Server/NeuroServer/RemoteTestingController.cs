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

        private readonly TimeSpan _timeOfExperiment;
        private readonly ImageManager _imageManager;
        private readonly ILogger _logger;

        private readonly ManualResetEventSlim _waiter = new ManualResetEventSlim(false);
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private NeuralNetwork _currentNeuralNetwork;
        private float _lastScore;
        private Response.MessageType _currentMessageType = Response.MessageType.Pause;

        public RemoteTestingController(TimeSpan timeOfExperiment, ImageManager imageManager, ILogger logger)
        {
            _timeOfExperiment = timeOfExperiment;
            _imageManager = imageManager;
            _logger = logger;
        }

        public float Test(NeuralNetwork neuralNetwork)
        {
            _currentNeuralNetwork = neuralNetwork;

            Signal(Response.MessageType.Start);

            Task.Delay(_timeOfExperiment).Wait();

            Signal(Response.MessageType.Result);

            return _lastScore;
        }

        public Response Compute(Request request)
        {
            _lock.EnterReadLock();

            var response = new Response(_currentMessageType);
            switch (_currentMessageType)
            {
                case Response.MessageType.Pause:
                    break;
                case Response.MessageType.Start:
                    _currentMessageType = Response.MessageType.Control;
                    break;
                case Response.MessageType.Control:
                    {
                        var image = ((ImageRequest)request).Image;
                        var input = _imageManager.ConvertFromPngToInput(image, InputNeuronsCount);
                        input = _imageManager.Normalize(input);
                        var output = _currentNeuralNetwork.Compute(input);
                        response = new Response(Response.MessageType.Control, (float)output[0], (float)output[1]);
                        _waiter.Set();
                        break;
                    }
                case Response.MessageType.Result:
                    _currentMessageType = Response.MessageType.Finish;
                    break;
                case Response.MessageType.Finish:
                    _lastScore = ((ScoreRequest)request).Score;
                    _currentMessageType = Response.MessageType.Pause;
                    _waiter.Set();
                    break;
                case Response.MessageType.End:
                    _currentMessageType = Response.MessageType.Shutdown;
                    break;

                case Response.MessageType.Shutdown:
                    _waiter.Set();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _logger.Debug($"Response({response}) Request({request})");

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
