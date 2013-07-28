using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace ParallelTasksDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancel = new CancellationTokenSource();
            var context = ZmqContext.Create();

            const string source = "inproc://source";
            const string destination = "inproc://destination";
            //const string source = @"tcp://127.0.0.1:9000";
            //const string destination = @"tcp://127.0.0.1:9001";

            var ventilator = new Ventilator(context, cancel.Token, source);
            var worker = new Worker(context, cancel.Token, source, destination);
            var sink = new Sink(context, cancel.Token, destination);


            sink.Start();
            Console.WriteLine("Sink ready.");
            Task.Delay(100).Wait();
            worker.Start();
            worker.Start();
            worker.Start();
            Console.WriteLine("Workers ready.");
            ventilator.Start();
            Console.WriteLine("Ventilator started.");


            Console.ReadLine();
            cancel.Cancel();
            context.Dispose();
        }
    }
}
