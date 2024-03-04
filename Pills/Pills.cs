using System;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using ServerEvents = Exiled.Events.Handlers.Server;

namespace FunnyPills
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; } = null!;

        public override string Name => "Funny Pills";
        public override string Author => "TagGodOnYT";

        public override Version RequiredExiledVersion { get; } = new(8, 0, 0);
        public override Version Version { get; } = new(1, 4, 1);

        private ServerHandler serverHandler = null!;

        public override void OnEnabled()
        {
            Instance = this;
            serverHandler = new ServerHandler();
            Config.LoadConfigs();
            if (Config.IsItemsEnabled)
            {
                CustomItem.RegisterItems(overrideClass: Config.ItemConfigs);
            }

            ServerEvents.ReloadedConfigs += serverHandler.OnReloadingConfigs;
        }

        public override void OnDisabled()
        {
            if (Config.IsItemsEnabled)
            {
                CustomItem.UnregisterItems();
            }
            Instance = null;
            serverHandler = null;
            ServerEvents.ReloadedConfigs -= serverHandler.OnReloadingConfigs;
            base.OnDisabled();
        }

    }

}
