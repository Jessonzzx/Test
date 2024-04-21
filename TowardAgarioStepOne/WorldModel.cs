using System;
using System.Numerics;
using Microsoft.Maui.Graphics;
using System;
using System.Diagnostics;

namespace TowardAgarioStepOne
{

    public class WorldModel
    {
        public Vector2 CircleCenter { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Radius { get; } = 10; // You can set the radius of the circle here
        public float Width { get; private set; }
        public float Height { get; private set; }

        private Stopwatch stopwatch;
        private int frameCount;
        private double lastFpsUpdateTime;
        private double fps;

        public double FPS => fps;

        public WorldModel(Vector2 startCenter, Vector2 startDirection, float width, float height)
        {
            CircleCenter = startCenter;
            Direction = startDirection;
            Width = width;
            Height = height;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void AdvanceGameOneStep()
        {
            // Update the circle's position
            CircleCenter += Direction;

            // Check for collision with the window borders and reverse direction if necessary
            if (CircleCenter.X - Radius < 0 || CircleCenter.X + Radius > Width)
            {
                Direction = new Vector2(-Direction.X, Direction.Y);
            }
            if (CircleCenter.Y - Radius < 0 || CircleCenter.Y + Radius > Height)
            {
                Direction = new Vector2(Direction.X, -Direction.Y);
            }

            UpdateFps();
        }

        private void UpdateFps()
        {
            frameCount++;
            var currentTime = stopwatch.Elapsed.TotalSeconds;
            if (currentTime - lastFpsUpdateTime > 1)
            {
                fps = frameCount / (currentTime - lastFpsUpdateTime);
                frameCount = 0;
                lastFpsUpdateTime = currentTime;
            }
        }

        public void UpdateWorldSize(float newWidth, float newHeight)
        {
            Width = newWidth;
            Height = newHeight;
        }
    }
}
    


