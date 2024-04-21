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
using AndroidX.Lifecycle;

namespace ClientGUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Event handler for the Connect button
        private async void OnConnectClicked(object sender, EventArgs e)
        {
            // Basic validation on user input
            if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(ServerEntry.Text))
            {
                await DisplayAlert("Input Required", "Please enter your name and server IP", "OK");
                return;
            }

            // Here you would initiate the connection to the server
            // This is a placeholder for the connection logic
            bool isConnected = await ConnectToServer(ServerEntry.Text, PortEntry.Text, NameEntry.Text);

            // If connected, switch views
            if (isConnected)
            {
                WelcomeView.IsVisible = false;
                GameView.IsVisible = true;
                // Set up the game view (attach drawable, start game loop, etc.)
                SetupGameView();
            }
            else
            {
                await DisplayAlert("Connection Failed", "Could not connect to the server.", "OK");
            }
        }

        // Placeholder method to simulate server connection
        private Task<bool> ConnectToServer(string serverIP, string port, string playerName)
        {
            // This is where you would have your actual server connection logic
            // For now, this method just simulates a successful connection
            return Task.FromResult(true);
        }

        // Set up the game view, attach drawable, and start the game loop
        private void SetupGameView()
        {
            // Create an instance of your drawable object to render the game state
            GameView.Drawable = new MyDrawableThing();

            // Start a game loop timer
            Device.StartTimer(TimeSpan.FromSeconds(1.0 / 30.0), () =>
            {
                GameView.Invalidate();
               
                return true; // Return true to keep the timer running
            });
        }

        // Handle taps on the game view
        private void OnGameViewTapped(object sender, EventArgs e)
        {
            // Handle tap gestures
        }

        // Handle pan updates on the game view
        private void OnGameViewPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            // Handle pan gestures
        }
    }

    // A simple drawable class as an example
    public class MyDrawableThing : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.Blue;
            canvas.FillRectangle(dirtyRect);
            // More drawing logic here...
        }
    }
}
