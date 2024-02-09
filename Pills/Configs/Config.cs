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
        public bool IsItemsEnabled { get; set; } = true;

        [YamlIgnore] public Items ItemConfigs { get; private set; } = null!;

        public string ItemFolderPath { get; set; } = Path.Combine(Paths.Configs, "FunnyPills");
        public string ItemFilePath { get; set; } = "Funny_Pill.yml";


        public void LoadConfigs()
        {
            if (!Directory.Exists(ItemFolderPath))
                Directory.CreateDirectory(ItemFolderPath);

            if (IsItemsEnabled)
            {
                string itemFilePath = Path.Combine(ItemFolderPath, ItemFilePath);
                Log.Info($"{itemFilePath}");
                if (!File.Exists(itemFilePath))
                {
                    ItemConfigs = new Items();
                    File.WriteAllText(itemFilePath, Loader.Serializer.Serialize(ItemConfigs));
                }
                else
                {
                    ItemConfigs = Loader.Deserializer.Deserialize<Items>(File.ReadAllText(itemFilePath));
                    File.WriteAllText(itemFilePath, Loader.Serializer.Serialize(ItemConfigs));
                }
            }
        }
    }
}
