    using AgarioModels;
    using Communications;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;
    using TowardAgarioStepThree;
    class Program
    {
        private static Networking network;

        static async Task Main(string[] args)
        {
            ILogger consoleLogger = new ConsoleLogger();
            // Initialize networking with the console logger and callback methods
            network = new Networking(consoleLogger, OnConnect, OnDisconnect, OnMessage);

            await network.ConnectAsync("localhost", 11000);
       
            _ = network.HandleIncomingDataAsync();

            Console.ReadLine();
        }

        static void OnConnect(Networking networkingInstance)
        {
            Console.WriteLine("Connected to the server.");
        }

        static void OnDisconnect(Networking networkingInstance)
        {
            Console.WriteLine("Disconnected from the server.");
        }

        static void OnMessage(Networking networkingInstance, string message)
        {
            Console.WriteLine("Message received: " + message);
      
            if (message.StartsWith(Protocols.CMD_Food))
            {
                Console.WriteLine("Food data received.");
                string json = message[Protocols.CMD_Food.Length..];
                var foodArray = JsonSerializer.Deserialize<TowardAgarioStepThree.Food[]>(json);

                // Output the first 10 food items to verify
                for (int i = 0; i < Math.Min(10, foodArray.Length); i++)
                {
                    var food = foodArray[i];
                    Console.WriteLine($"Food {i}: X={food.X}, Y={food.Y}, Color={food.ARGBcolor}, Mass={food.Mass}");
                }
            }
        }

        public class ConsoleLogger : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log(string message)
            {
                Console.WriteLine(message);
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                throw new NotImplementedException();
            }
        }
    }