using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace ParallelTasksDemo
{
    public class Sink
    {
        private readonly ZmqContext _context;
        private readonly CancellationToken _token;
        private readonly string _endpoint;

        public Sink(ZmqContext context, CancellationToken token, string endpoint)
        {
            _context = context;
            _token = token;
            _endpoint = endpoint;
        }

        public void Start()
        {
            Task.Factory.StartNew(start, _token, 
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void start()
        {
            using (var socket = _context.CreateSocket(SocketType.PULL))
            {
                socket.Bind(_endpoint);
                int received = 0;

                while (received < 100 && _token.IsCancellationRequested == false)
                {
                    var result = socket.Receive(
                        Encoding.Unicode, TimeSpan.FromSeconds(1));

                    if (string.IsNullOrWhiteSpace(result) == false)
                        received++;
                }

                if(received == 100)
                    Console.WriteLine("Done all tasks");
            }
        } 
    }
}