using System;
using System.Numerics;

namespace AgarioModels
{
    public class Player : GameObject
    {
        public string Name { get; private set; }
        public float Score { get; private set; }
        public Vector2 Velocity { get; set; } // The current velocity for player movement
        World world;

        // We might want to track if the player is alive or has been consumed
        public bool IsAlive { get; private set; }
        public float ARGBColor { get; set; }
        public DateTime? TimeOfDeath { get; private set; }
        public float RespawnTime { get; set; } = 5.0f; // seconds after death before respawning
        public bool ShouldSplit { get; set; }

        // Constructor
        public Player(long id, Vector2 position, int color, float mass, string name)
            : base(id, position, color, mass)
        {
            Name = name;
            Score = 0;
            Velocity = Vector2.Zero;
            IsAlive = true;
        }

        public void Die()
        {
            IsAlive = false;
            TimeOfDeath = DateTime.Now;
        }
        // Call this method to update the player's score
        public void AddToScore(float amount)
        {
            Score += amount;
        }

        // Call this method when the player is consumed by another player
        public void Consume()
        {
            IsAlive = false;
        }

        // Call this method to respawn the player
        public void Respawn(Vector2 newPosition)
        {
            Position = newPosition;
            Mass = 10; //To be determined
            IsAlive = true;
        }

        public void Update(float deltaTime)
        {
            if (IsAlive)
            {
                // Apply the current velocity to the player's position
                Position += Velocity * deltaTime;

                // Ensure the player doesn't move outside the world boundaries
                Position = new Vector2(
                    Math.Clamp(Position.X, 0, World.Width),
                    Math.Clamp(Position.Y, 0, World.Height)
                );
                CheckForCollisions(world);

                // Splitting and other actions
                if (ShouldSplit) // ShouldSplit would be set based on player input
                {
                    HandleSplitting(world);
                    ShouldSplit = false;
                }

            }
            if (!IsAlive && TimeOfDeath.HasValue)
            {
                double timeSinceDeath = (DateTime.Now - TimeOfDeath.Value).TotalSeconds;
                if (timeSinceDeath > RespawnTime)
                {
                    HandleRespawn();
                }
            }

        }

        private void CheckForCollisions(World world)
        {
            // Assuming the World class has a method to return all food as IEnumerable<Food>
            foreach (var food in world.Foods)
            {
                float distance = Vector2.Distance(this.Position, food.Position);
                if (distance < this.Radius + food.Radius) // Simplified collision check
                {
                    // Handle collision
                    this.Mass += food.Mass; // Increase the player's mass
                    food.Eat(); // Mark the food as eaten
                                // You can add a scoring mechanism here as well
                    this.Score += food.Mass; // For example
                }
            }
        }

        private void HandleSplitting(World world)
        {
            // Splitting logic here. For simplicity, we split the player into two
            if (this.Mass > 100) // Check if the player is large enough to split
            {
                this.Mass /= 2; // Halve the mass
                                // Create a new player object representing the split part
                var newPlayer = new Player(
                    id: world.GetNextAvailableId(), // You'd need a method to get unique IDs
                    position: this.Position, // Start at the current position
                    color: this.ArgbColor,
                    mass: this.Mass,
                    name: this.Name + "_split" // Just an example name
                );
                newPlayer.Velocity = new Vector2(-this.Velocity.X, -this.Velocity.Y); // Move in opposite direction
                                                                                      // Add the new split player to the world
                world.AddPlayer(newPlayer);
            }
           
        }

        private void HandleRespawn()
        {
            
                // Respawn logic here
                this.Position = world.GetRandomPosition(); // You'd implement this method in World
                this.Mass = 50; // Reset mass
                IsAlive = true;
                TimeOfDeath = null;

        }

        public bool CollidesWith(Food food)
        {
            // Calculate the distance between the player and the food
            float distance = Vector2.Distance(this.Position, food.Position);
            // Check if the distance is less than the sum of the radii (assuming circles)
            return distance < (this.Radius + food.Radius);
        }

        public void EatFood(Food food)
        {
            if (this.CollidesWith(food))
            {
                this.Mass += food.Mass; // Increase player mass
                food.IsEaten = true; // Mark food as eaten
                                   // ... Additional logic for updating the game state
            }
        }
    }
}

