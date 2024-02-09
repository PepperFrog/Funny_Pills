using Exiled.API.Enums;
using System;

namespace FunnyPills
{
    public class SpawnLocationManager
    {
        private static readonly Random random = new Random();

        public static SpawnLocationType GetRandomSpawnLocation()
        {
            var spawnLocations = Enum.GetValues(typeof(SpawnLocationType));
            SpawnLocationType randomSpawnLocation = (SpawnLocationType)spawnLocations.GetValue(random.Next(spawnLocations.Length));
            return randomSpawnLocation;
        }
    }
}
