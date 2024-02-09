using System.IO;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;

using YamlDotNet.Serialization;

namespace FunnyPills
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
        public bool IsPillsEnabled { get; set; } = true;

        [YamlIgnore]
        public Items PillConfigs { get; private set; } = null!;

        public string PillFolderPath { get; set; } = Path.Combine(Paths.Configs, "FunnyPills");
        public string PillFilePath { get; set; } = "Funny_Pill.yml";


        public void LoadConfigs()
        {
            if (!Directory.Exists(PillFolderPath))
                Directory.CreateDirectory(PillFolderPath);

            if (IsPillsEnabled)
            {
                string PillPath = Path.Combine(PillFolderPath, PillFilePath);
                Log.Info($"{PillPath}");
                if (!File.Exists(PillPath))
                {
                    PillConfigs = new Items();
                    File.WriteAllText(PillPath, Loader.Serializer.Serialize(PillConfigs));
                }
                else
                {
                    PillConfigs = Loader.Deserializer.Deserialize<Items>(File.ReadAllText(PillPath));
                    File.WriteAllText(PillPath, Loader.Serializer.Serialize(PillConfigs));
                }
            }
        }
    }
}
