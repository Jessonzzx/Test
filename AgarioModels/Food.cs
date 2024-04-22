using System;
using System.Numerics;

namespace AgarioModels
{
    public class Food : GameObject
    {
        public bool IsEaten { get; set; }

        // Constructor
        public Food(long id, Vector2 position, int color, float mass)
            : base(id, position, color, mass)
        {
            IsEaten = false;
        }

        // Method to mark the food as eaten
        public void Eat()
        {
            IsEaten = true;
        }
    }
}

    