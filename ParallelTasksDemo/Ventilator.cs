using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace ParallelTasksDemo
{
    public class Ventilator
    {
        private readonly ZmqContext _context;
        private readonly CancellationToken _token;
        private readonly string _endpoint;

        public Ventilator(ZmqContext context, CancellationToken token, string endpoint)
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
            using (var socket = _context.CreateSocket(SocketType.PUSH))
            {
                socket.Bind(_endpoint);
                //hack...there are better ways of doing this.
                Task.Delay(1000, _token).Wait(_token); 


                for (int i = 0; i < 100; i++)
                {
                    socket.Send("Hello", Encoding.Unicode);
                }
            }
        }
    }
}