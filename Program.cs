using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            const int maxRetries = 10;

            int retry = 0;

            while(retry < maxRetries)
            {
                try
                {
                    // This switch must be set before creating the GrpcChannel/HttpClient.
                    //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                                        
                    var channel = GrpcChannel.ForAddress("http://localhost:50051");                    

                    var client = new HelloGreeter.HelloGreeterClient(channel);
                    var reply = client.SayHello
                        (new HelloRequest { Name = "JK" });
                    Console.WriteLine($"Client received : {reply.Message}");
                    Console.WriteLine("Hit any key to exit");
                    Console.ReadLine();
                    break;
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
                {

                    Console.WriteLine($"Error message: {ex.Status.Detail}");
                    retry++;

                    if (retry < maxRetries)
                    {
                        Console.WriteLine($"Retrying (Attempt {retry}/{maxRetries})...");
                        await Task.Delay(1000);
                    } else
                    {
                        Console.WriteLine("Max retries reached");
                        break;
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    break;
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
           
        }
    }
}
