using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using AgarioModels;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using System.Text;
using Communications;
using Microsoft.Extensions.Logging;


namespace ClientGUI
{
    public partial class MainPage : ContentPage
    {
        private string playerName;
        private string serverAddress;
        private int serverPort;
        private Entry serverPortEntry;
        private readonly Networking _networking;

        private HttpClient httpClient; // Used for server communication
        private World world;

        // New UI Control
        private GraphicsView gameCanvas;
        public MainPage()
        {
            InitializeComponent();

            ILogger logger = new CustomFileLogger("myApplicationLog");

            _networking = new Networking(
        logger, // This assumes your Networking class is expecting an ILogger, not ILogger<T>
        OnConnectedToServer,
        OnDisconnectedFromServer,
        OnMessageReceivedFromServer
    );
            httpClient = new HttpClient();
            world = new World(logger);

            // Initialization of UI components and HttpClient
            playerNameEntry.Placeholder = "Enter name";
            serverAddressEntry = new Entry { Placeholder = "Enter server address", Text = "localhost" };
            serverPortEntry = new Entry { Placeholder = "Enter port", Text = "11000" };
            connectButton = new Button { Text = "Connect To Server" };
            statusLabel = new Label();

            connectButton.Clicked += OnConnectButtonClicked;

            // Add UI elements to the layout
            Content = new StackLayout
            {
                Children =
                {
                    playerNameEntry,
                    serverAddressEntry,
                    serverPortEntry,
                    connectButton,
                    statusLabel
                }
            };

            // Initialize gameCanvas and add to layout
            gameCanvas = new GraphicsView
            {
                Drawable = new GameDrawable(world),
                WidthRequest = 800, // Set the desired width
                HeightRequest = 800, // Set the desired height
                BackgroundColor = Colors.Transparent
            };
            gameCanvas.WidthRequest = 500;
            gameCanvas.HeightRequest = 500;
            gameCanvas.Drawable = new GameDrawable(world);

            // Add gameCanvas to the children of the stack layout
            ((StackLayout)Content).Children.Add(gameCanvas);
        }

        private void DrawGame(ICanvas canvas, RectF dirtyRect)
        {
            // Here you would access the game world state and draw accordingly.
            // For now, we're just drawing a placeholder rectangle.
            canvas.FillColor = Colors.Blue;
            canvas.FillRectangle(dirtyRect);

            // Assuming you have a method to get the current player's position and size:
            var playerPosition = GetPlayerPosition();
            var playerSize = GetPlayerSize();

            // Calculate the scale factor based on the player's size (e.g., 10 to 20 times width)
            float scaleFactor = Math.Max(10 * playerSize.Width, dirtyRect.Width);

            // Calculate the offset to center the player in the middle of the view
            float offsetX = playerPosition.X - dirtyRect.Width / 2 * scaleFactor;
            float offsetY = playerPosition.Y - dirtyRect.Height / 2 * scaleFactor;

            // Apply transformations to scale and translate the canvas
            canvas.Scale(scaleFactor, scaleFactor);
            canvas.Translate(-offsetX, -offsetY);

            Device.StartTimer(TimeSpan.FromSeconds(1 / 30.0), () => { // Run 30 times per second
                UpdateGame();
                return true; // Keep timer running
            });
            // Now draw the player and other objects relative to the transformed canvas
            // ...
        }

        private PointF GetPlayerPosition()
        {
            // This should come from the game world state
            return new PointF(2500, 2500); // Example: center of the world
        }

        private SizeF GetPlayerSize()
        {
            // This should be calculated based on the player's mass
            return new SizeF(50, 50); // Example: some size
        }

        private void UpdateGame()
        {
            foreach (var player in world.Players)
            {
                player.Update(1 / 30.0f); // Assuming a delta time of approximately 1/30th of a second
            }
            gameCanvas.Invalidate();
        }

        private async void OnConnectButtonClicked(object sender, EventArgs e)
        {
            playerName = playerNameEntry.Text;
            serverAddress = serverAddressEntry.Text;

            // Use the port number as defined in your requirements
            if (!int.TryParse(serverPortEntry.Text, out int port))
            {
                statusLabel.Text = "Invalid port number.";
                return;
            }

            try
            {
                await _networking.ConnectAsync(serverAddress, port);
                statusLabel.Text = "Connected to server!";

                // Send a message to the server to start the game
                string startCommand = string.Format(Protocols.CMD_Start_Game, Uri.EscapeDataString(playerName));
                await _networking.SendAsync(startCommand);

                // Optionally start listening to incoming data
                _ = _networking.HandleIncomingDataAsync();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error in Networking: {ex.Message}";
            }
        }

        private void OnConnectedToServer(Networking networking)
        {
            // Use MainThread for UI thread access in MAUI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                statusLabel.Text = "Connected to server!";
                connectButton.IsEnabled = false;
                // Switch views here
                WelcomeView.IsVisible = false;
                GameView.IsVisible = true;
            });
        }

        private void OnDisconnectedFromServer(Networking networking)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update UI to reflect the disconnection
                statusLabel.Text = "Disconnected from server.";
                connectButton.IsEnabled = true; // Re-enable connect button if needed
                                                // Other UI updates...
            });
        }

        private void OnMessageReceivedFromServer(Networking networking, string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Process and display the message on the UI
                // You may want to deserialize JSON messages and update UI components
                // based on the content of the message.

                // Example: Update a label with the message
                statusLabel.Text = message;

                // If the message is JSON and you're using System.Text.Json,
                // you might deserialize it like so:
                // var myObject = JsonSerializer.Deserialize<MyObjectType>(message);

                // Then update the UI with the deserialized data
                // e.g., myLabel.Text = myObject.SomeProperty;
            });
        }

        // Add additional methods to handle game state updates and UI drawing...

        private void InvalidateBtnClicked(object sender, EventArgs e)
        {
            // Invalidate or redraw logic here
        }

        private void InvalidateAlwaysBtnClicked(object sender, CheckedChangedEventArgs e)
        {
            // Logic to handle the checkbox state change
        }
        private void OnSplitButtonClicked(object sender, EventArgs e)
        {
            if (world.CurrentPlayer != null)
            {
                world.CurrentPlayer.ShouldSplit = true; // Assuming a Split method exists in your Player class
                gameCanvas.Invalidate(); // Redraw the screen to reflect the split
            }
        }

    }




    public class GameDrawable : IDrawable
    {
        public World world { get; set; } // The World object that holds the game state
        public GameDrawable(World world)
        {
            this.world = world;
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (world == null)
                return;

            // Iterate over the players in the world
            foreach (var player in world.Players)
            {
                // Calculate the position and radius based on the player's data
                var position = new PointF(player.X, player.Y);
                var radius = player.Radius; // Assuming Radius is a defined property in Player class

                // Adjust position and radius based on your zoom logic here

                // Draw the player
                canvas.FillColor = new Color(player.ARGBColor);
                canvas.FillCircle(position.X, position.Y, radius);
            }


            // Draw Foods
            foreach (var food in world.Foods)
            {
                // The food drawing logic
                var foodPosition = new PointF(food.X, food.Y);
                var foodRadius = food.Radius; // If Radius is a property in the Food class

                // Draw the food
                canvas.FillColor = new Color(food.ARGBColor);
                canvas.FillCircle(foodPosition.X, foodPosition.Y, foodRadius);
            }

            foreach (var food in world.Foods.Where(f => !f.IsEaten))
            {
                canvas.FillColor = Color.FromArgb(food.ArgbColor);
                canvas.FillCircle(food.Position.X, food.Position.Y, food.Radius);
            }
        }
    }
}