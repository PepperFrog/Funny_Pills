using System;
using Exiled.API.Features;
using Exiled.CustomItems;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using HarmonyLib;
using ServerEvents = Exiled.Events.Handlers.Server;
using Config = FunnyPills.Config;
using CustomRole = Exiled.CustomRoles.API.Features.CustomRole;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Scp049Events = Exiled.Events.Handlers.Scp049;
using System.Collections.Generic;
using Exiled.CustomRoles.API;
using CommandSystem;
using RemoteAdmin;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Linq;
using Exiled.API.Features.Doors;
using InventorySystem.Items.Keycards;
using Interactables.Interobjects.DoorUtils;

namespace FunnyPills
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; } = null!;

        public override string Name => "Funny Pills";
        public override string Author => "TagGodOnYT";

        public override Version RequiredExiledVersion { get; } = new(8, 0, 0);
        public override Version Version { get; } = new(1, 0, 0);

        public bool IsSpawnable = false;
        public bool IsForced = false;

        private ServerHandler serverHandler = null!;

        public override void OnEnabled()
        {
            Instance = this;
            serverHandler = new ServerHandler();
            if (Config.IsPillsEnabled)
            {
                CustomItem.RegisterItems(overrideClass: Config.PillConfigs);
            }

            ServerEvents.ReloadedConfigs += serverHandler.OnReloadingConfigs;
        }

        public override void OnDisabled()
        {
            if (Config.IsPillsEnabled)
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
