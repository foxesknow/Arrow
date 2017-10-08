using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Threading;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var dispatcher = new SimpleWorkDispatcher())
            {
                var t1 = new Thread(dispatcher.DispatcherLoop);
                t1.Start();
                var t2 = new Thread(dispatcher.DispatcherLoop);
                t2.Start();
                
                var context = new WorkDispatcherSynchronizationContext(dispatcher);
                using(var disposer = WorkDispatcherSynchronizationContext.Install(context))
                {
                    var task = Sum(10,20);
                    Console.WriteLine("running");
                    task.Wait();
                    Console.WriteLine("done");
                }

                dispatcher.Stop();

                t1.Join();
                t2.Join();
            }
        }

        static async Task<int> Sum(int x, int y)
        {
            await Task.Delay(2000);
            return x + y;
        }
    }
}
