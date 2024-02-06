using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
