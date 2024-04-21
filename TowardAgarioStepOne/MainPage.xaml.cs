using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
namespace TowardAgarioStepOne;

public partial class MainPage : ContentPage
{
    // Assume WorldModel and WorldDrawable are already implemented
    private WorldModel worldModel;
    private WorldDrawable worldDrawable;
    private bool initialized = false;
    private Timer gameTimer;

    public MainPage()
    {
        InitializeComponent();

        // Initialize the world model with starting values
        worldModel = new WorldModel(new Vector2(100, 100), new Vector2(50, 25), 800, 800);

        // Initialize the drawable with the world model
        worldDrawable = new WorldDrawable(worldModel);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (!initialized)
        {
            initialized = true;
            InitializeGameLogic();
        }
    }

    private void InitializeGameLogic()
    {
        // Assign the WorldDrawable to the PlaySurface.Drawable property
        PlaySurface.Drawable = worldDrawable;

        // Start the timer
        StartGameTimer();
    }

    private void StartGameTimer()
    {
        // Set the timer to tick every 33 milliseconds (~30 FPS)
        gameTimer = new Timer(GameStep, null, 0, 33);
    }

    private void GameStep(object state)
    {
        // Advance the game one step
        worldModel.AdvanceGameOneStep();

        // Invalidate the GraphicsView on the UI thread to trigger a redraw.
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PlaySurface.Invalidate();
        });

        // Update the labels also on the UI thread.
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        // Run on the UI thread because we're updating UI elements
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Format the FPS to show only two decimal places
            fpsLabel.Text = $"FPS: {worldModel.FPS:0.00}";

            // Assuming CircleCenter is a Vector2, format it to show the X and Y with two decimal places
            centerLabel.Text = $"Center: {worldModel.CircleCenter.X:0.00}, {worldModel.CircleCenter.Y:0.00}";

            // Format the Direction in the same way
            directionLabel.Text = $"Direction: {worldModel.Direction.X:0.00}, {worldModel.Direction.Y:0.00}";
        });
    }
}




