using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace ParallelTasksDemo
{
    public class Worker
    {
        private readonly ZmqContext _context;
        private readonly CancellationToken _token;
        private readonly string _source;
        private readonly string _destination;

        public Worker(ZmqContext context, CancellationToken token, 
            string source, string destination)
        {
            _context = context;
            _token = token;
            _source = source;
            _destination = destination;
        }

        public void Start()
        {
            Task.Factory.StartNew(start, _token, 
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void start()
        {
            using (var sourceSocket = _context.CreateSocket(SocketType.PULL))
            using (var destinationSocket = _context.CreateSocket(SocketType.PUSH))
            {
                destinationSocket.Connect(_destination);
                sourceSocket.Connect(_source);

                while (_token.IsCancellationRequested == false)
                {
                    var input = sourceSocket.Receive(
                        Encoding.Unicode, TimeSpan.FromSeconds(1));

                    if (string.IsNullOrWhiteSpace(input) == false)
                        destinationSocket.Send(
                            string.Concat(input, " World"), Encoding.Unicode);
                }
            }
        }
    }
}