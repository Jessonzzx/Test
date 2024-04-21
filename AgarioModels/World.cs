using System;
using System.Numerics;

namespace AgarioModels
{
	public class World
    {
        private readonly Dictionary<long, GameObject> _gameObjects = new Dictionary<long, GameObject>();
        private static readonly Random _random = new Random();
        private readonly List<Player> _players = new List<Player>();
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        public IEnumerable<Food> Foods => _gameObjects.Values.OfType<Food>();
        public const int Width = 5000;
        public const int Height = 5000;
        private long _nextId = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(Player player)
        {
            _players.Add(player);
            _gameObjects[player.Id] = player; // Add to the general game objects as well
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRandomPosition()
        {
            return new Vector2(
                _random.Next(0, Width),
                _random.Next(0, Height)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="food"></param>
        public void AddFood(Food food)
        {
            _gameObjects[food.Id] = food; // Add directly to game objects
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetNextAvailableId()
        {
            return _nextId++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerId"></param>
        public void RemovePlayer(long playerId)
        {
            if (_gameObjects.TryGetValue(playerId, out var player))
            {
                _players.Remove(player as Player);
                _gameObjects.Remove(playerId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foodId"></param>
        public void RemoveFood(long foodId)
        {
            _gameObjects.Remove(foodId); // Remove directly from game objects
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            foreach (var player in _players)
            {
                
                player.Position += player.Velocity * deltaTime;

                
                player.Position = new Vector2(
                    Math.Clamp(player.Position.X, 0, Width),
                    Math.Clamp(player.Position.Y, 0, Height)
                );
            }

            // Check for collisions between players and food
            foreach (var food in Foods.ToList()) // ToList to avoid collection modified exceptions
            {
                foreach (var player in _players)
                {
                    if (IsCollision(player, food))
                    {
                        // Implement logic for when a player consumes food.
                        // For example, increase the player's mass and remove the food from the world.
                        player.Mass += food.Mass;
                        RemoveFood(food.Id); // You'll need to properly implement RemoveFood to handle this
                    }
                }
            }
        }

        /// <summary>
        /// A simple collision detection method that checks if two circles (game objects) are overlapping.
        /// It assumes that each GameObject has a calculated Radius property.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool IsCollision(GameObject a, GameObject b)
        {
            
            float distance = Vector2.Distance(a.Position, b.Position);
            return distance < (a.Radius + b.Radius);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Cleanup()
        {
            
            var eatenFoodIds = _gameObjects.Values
                                .OfType<Food>()
                                .Where(food => food.IsEaten) 
                                .Select(food => food.Id)
                                .ToList();

            foreach (var id in eatenFoodIds)
            {
                _gameObjects.Remove(id);
            }

           
        }
    }
}

