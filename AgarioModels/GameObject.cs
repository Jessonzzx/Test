using System;
using System.Numerics;

namespace AgarioModels
{
        public class GameObject
        {
            public long Id { get; init; } // Unique identifier for each game object
        public Vector2 Position
        {
            get => _position;
            set
            {
                if (value.X < 0 || value.Y < 0 || value.X > World.Width || value.Y > World.Height)
                    throw new ArgumentException("Position is out of world bounds.");
                _position = value;
            }
        }
        private Vector2 _position;
        public int ArgbColor { get; set; } // Color of the object for rendering
            public float Mass { get; set; } // Mass of the object, can be used to calculate size


            // Constructor
            public GameObject(long id, Vector2 position, int color, float mass)
            {
                Id = id;
                Position = position;
                ArgbColor = color;
                Mass = mass;
            }

            // Calculated property for radius based on mass
            public float Radius => CalculateRadiusFromMass(Mass);

            private float CalculateRadiusFromMass(float mass)
            {
                // Implement the logic to calculate the radius based on mass
                // This is a placeholder formula and should be replaced with the actual game logic
                return (float)Math.Sqrt(mass / Math.PI);
            }

        public override string ToString()
        {
            return $"ID: {Id}, Position: {Position}, Mass: {Mass}, Color: {ArgbColor}";
        }

    }
}

