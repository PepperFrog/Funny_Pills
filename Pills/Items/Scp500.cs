using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;

using MEC;

using YamlDotNet.Serialization;
using UnityEngine;
using System;
using Exiled.API.Extensions;
using Mirror;
using System.IO;
using NVorbis;
using PlayerStatsSystem;
using PluginAPI.Enums;
using UnityEngine.Networking.Types;
using AdminTools.Commands.Kill;
using AdminTools.Commands.Jail;
using Random = System.Random;
using PlayerRoles;
using System.Linq;
using CustomPlayerEffects;
using AdminTools.Commands.Position;
using Exiled.CustomItems.API.Features;

namespace FunnyPills
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500 : CustomItem
    {
        public override uint Id { get; set; } = 3;

        public override string Name { get; set; } = "Funny Pills";

        public override string Description { get; set; } = "Do Something Fun When Eaten";

        public override float Weight { get; set; }

        //[YamlIgnore]
        //public static Vector3[] Spawns { get; set; } = Instance.GeneratePositions().ToArray();

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 50,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
                new()
                {
                Chance = 50,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.UsedItem += OnUsedItem;

            base.SubscribeEvents();
        }

        [YamlIgnore]
        private Player _player;

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Player.UsedItem -= OnUsedItem;

            base.UnsubscribeEvents();
        }

        protected void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            _player = ev.Player;
        }
        protected void OnUsedItem(UsedItemEventArgs ev)
        {
            var effects = Enum.GetValues(typeof(Effects));
            var randomEffect = (Effects)effects.GetValue(new Random().Next(effects.Length));
            if (ev.Item.Type == ItemType.SCP500)
            {
                switch (randomEffect)
                {
                    case Effects.AllSpec:
                        //revive  all spec
                        ApplyAllSpecEffect(ev.Player);
                        break;
                    case Effects.OneSpec:
                        //revive one spec
                        ApplyOneSpecEffect(ev.Player);
                        break;
                    case Effects.Add5MoveBoost:
                        //add 25 movement boost (Limit is 100)
                        ApplyAdd5MoveBoostEffect(ev.Player);
                        break;
                    case Effects.Die:
                        //vaporize the player
                        ApplyDieEffect(ev.Player);
                        break;
                    case Effects.BetrayTeam:
                        //betray the team and switch sides
                        ApplyBetrayTeamEffect(ev.Player);
                        break;
                    case Effects.AddRandomEffect:
                        //add a random effect to the player WIP
                        ApplyRandomEffect(ev.Player);
                            break;
                }
            }


        }

        private void ApplyAllSpecEffect(Player player)
        {
            foreach (var p in Player.List)
            {
                
                if (p.Role.Type == RoleTypeId.Spectator)
                {
                    RoleSpawnFlags spawnFlags = RoleSpawnFlags.AssignInventory;
                    p.RoleManager.ServerSetRole(player.Role, RoleChangeReason.Revived, spawnFlags);
                    p.Teleport(player.Position);
                    player.ShowHint("You Have Revived All Spectators", 5);
                    p.ShowHint("You Have Been Revived By " + player.Nickname, 5);
                }
            }
        }

        private void ApplyOneSpecEffect(Player player)
        {
            var spectators = Player.List.Where(p => p.Role.Type == RoleTypeId.Spectator).ToList();

            if (spectators.Count == 0)
            {
                return;
            }

            var random = new System.Random();
            var selectedSpectator = spectators[random.Next(spectators.Count)];
            RoleSpawnFlags spawnFlags = RoleSpawnFlags.AssignInventory;

            player.ShowHint("You Have Revived A Spectator", 5);
            selectedSpectator.ShowHint("You Have Been Revived By " + player.Nickname, 5);

            selectedSpectator.RoleManager.ServerSetRole(player.Role, RoleChangeReason.Revived, spawnFlags);
            selectedSpectator.Teleport(player.Position);
        }

        private void ApplyAdd5MoveBoostEffect(Player player)
        {
            if (player.GetEffect(EffectType.MovementBoost).IsEnabled)
            {
                byte intensity = player.GetEffectIntensity<MovementBoost>();
                if (intensity < 100)
                {
                    intensity += 25;
                    player.ShowHint("<color=green>You Have Gained +25% Movement Speed</color>");
                }
                else
                {
                    Log.Debug("Player: " + player.Nickname + " (" + player.Id + ") | MVB Intensity > +100");
                }
            }
            else
            {
                player.EnableEffect<MovementBoost>();
                player.ShowHint("<color=green>You Have Gained A 25% Movement Boost</color>");
            }
        }

        private void ApplyDieEffect(Player player)
        {
            player.Vaporize(player);
        }

        private void ApplyRandomEffect(Player player)
        {
            player.ApplyRandomEffect(EffectCategory.Positive, 255, 90);
        }

        private void ApplyBetrayTeamEffect(Player player)
        {
            if (player.Role == RoleTypeId.NtfCaptain || player.Role == RoleTypeId.NtfSergeant || player.Role == RoleTypeId.NtfPrivate || player.Role == RoleTypeId.NtfSpecialist)
            {
                player.RoleManager.ServerSetRole(RoleTypeId.ChaosConscript, RoleChangeReason.Revived, RoleSpawnFlags.None);
            }
            else if (player.Role == RoleTypeId.ChaosRepressor || player.Role == RoleTypeId.ChaosRifleman || player.Role == RoleTypeId.ChaosConscript || player.Role == RoleTypeId.ChaosMarauder)
            {
                player.RoleManager.ServerSetRole(RoleTypeId.NtfSpecialist, RoleChangeReason.Revived, RoleSpawnFlags.None);
            }
            else if (player.Role == RoleTypeId.ClassD)
            {
                player.RoleManager.ServerSetRole(RoleTypeId.Scientist, RoleChangeReason.Revived, RoleSpawnFlags.None);
            }
            else if (player.Role == RoleTypeId.Scientist)
            {
                player.RoleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.Revived, RoleSpawnFlags.None);
            }
        }


        public enum Effects
        {
            AllSpec,
            OneSpec,
            Add5MoveBoost,
            Die,
            BetrayTeam,
            AddRandomEffect
        }
    }
}