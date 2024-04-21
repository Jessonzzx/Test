using Microsoft.Maui.Graphics;
using System;

namespace TowardAgarioStepOne
{
    public class WorldDrawable : IDrawable
    {
        private readonly WorldModel worldModel;

        public WorldDrawable(WorldModel model)
        {
            worldModel = model ?? throw new ArgumentNullException(nameof(model));
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            // Set the fill color for the circle (this example uses red)
            canvas.FillColor = Colors.Red;

            // Draw the circle using the WorldModel's properties
            canvas.FillCircle(worldModel.CircleCenter.X, worldModel.CircleCenter.Y, worldModel.Radius);
        }
    }
}

