using System;
using System.Threading.Tasks;

using Arrow.Application;
using Arrow.Application.DaemonHosting;
using Arrow.InsideOut;
using Arrow.InsideOut.Transport;
using Arrow.InsideOut.Transport.Http.Client;
using Arrow.InsideOut.Transport.Tcp.Client;

namespace InsideOutClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ApplicationRunner.RunAsync(() => Run(args));
        }

        private static async Task Run(string[] args)
        {
            var publisherID = new PublisherID("fuji", "InsideOutHost");

            //using(var manager = new HttpClientManager())
            using(var manager = new TcpClientManager())
            {
                //var node = manager.Subscribe(publisherID, new("http://localhost:8080/InsideOut"));
                var node = manager.Register(publisherID, new("tcp://localhost:12345"));

                while(true)
                {
                    Console.WriteLine("press any key to exeute");
                    Console.ReadLine();
                    var details = await node.GetDetails(default);

                    Console.WriteLine(details);

                    var executeRequest = new ExecuteRequest("Calculator/Divide")
                    {
                        Arguments =
                        {
                            new DecimalArgument("lhs"){Value = 10},
                            new DecimalArgument("rhs"){Value = 0}
                        }
                    };

                    var result = await node.Execute(executeRequest, default);
                    Console.WriteLine(result);
                }
            }
        }
    }
}