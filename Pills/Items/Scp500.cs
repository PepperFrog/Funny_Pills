using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using YamlDotNet.Serialization;
using System;
using Random = System.Random;
using PlayerRoles;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features.Items;
namespace FunnyPills
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500 : CustomItem
    {
        public override uint Id { get; set; } = 1;

        public override string Name { get; set; } = "Funny Pills";

        public override string Description { get; set; } = "Do Something Fun When Eaten";

        public override float Weight { get; set; }

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
            },
        };
        public bool AffecAll500s { get; set; } = false;
        public int MaxPlayerScale { get; set; } = 10;
        public int MinPlayerScale { get; set; } = 5;


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
            if ((Check(ev.Item) && AffecAll500s == false) || AffecAll500s == true)
            {
                var effects = Enum.GetValues(typeof(Effects));
                var randomEffect = (Effects)effects.GetValue(new Random().Next(effects.Length));

                Random random = new Random();
                int chance = random.Next(1, 1000);

                if (ev.Item.Type == ItemType.SCP500)
                {
                    switch (randomEffect)
                    {
                        case Effects.AllSpec:
                            if (chance <= 525 && chance > 500)
                            {
                                ApplyAllSpecEffect(ev.Player);
                            }
                            break;
                        case Effects.OneSpec:
                            if (chance <= 500 && chance > 450)
                            {
                                ApplyOneSpecEffect(ev.Player);
                            }
                            break;
                        case Effects.Add5MoveBoost:
                            if (chance <= 450 && chance > 350)
                            {
                                ApplyAdd5MoveBoostEffect(ev.Player);
                            }
                            break;
                        case Effects.Die:
                            if (chance <= 350 && chance > 300)
                            {
                                ApplyDieEffect(ev.Player);
                            }
                            break;
                        case Effects.BetrayTeam:
                            if (chance <= 700 && chance > 525)
                            {
                                ApplyBetrayTeamEffect(ev.Player);
                            }
                            break;
                        case Effects.AddRandomGoodEffect:
                            if (chance <= 300 && chance > 150)
                            {
                                ApplyRandomEffect(ev.Player, false);
                            }
                            break;
                        case Effects.AddRandomBadEffect:
                            if (chance <= 150 && chance > 50)
                            {
                                ApplyRandomEffect(ev.Player, true);
                            }
                            break;
                        case Effects.StartTheFuckingNuke:
                            if (chance <= 10 && chance > 1)
                            {
                                StartTheNuke();
                            }
                            break;
                        case Effects.ReplaceInventory:
                            if (chance <= 50 && chance > 11)
                            {
                                ReplacePlayerInventory(ev.Player);
                            }
                            break;
                        case Effects.SizeChange:
                            if (chance <= 800 && chance > 700)
                            {
                                ChangePlayerSize(ev.Player);
                            }
                            break;
                        case Effects.Kaboom:
                            if (chance <= 850 && chance > 800)
                            {
                                TriggerExplosion(ev.Player);
                            }
                            break;
                        case Effects.Blackout:
                            if (chance <= 1000 && chance > 850)
                            {
                                CauseBlackout();
                            }
                            break;
                    }
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
                    player.ChangeEffectIntensity<MovementBoost>(intensity);
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

        private void ApplyRandomEffect(Player player, bool IsBad)
        {
            if (!IsBad)
            {
                player.ApplyRandomEffect(EffectCategory.Positive, 255, 90);
            }
            else if (IsBad)
            {
                player.ApplyRandomEffect(EffectCategory.Negative, 255, 90);
            }
        }

        private void StartTheNuke()
        {
            Warhead.Start();
        }

        private void ReplacePlayerInventory(Player player)
        {
            Random random = new Random();
            var itemTypes = Enum.GetValues(typeof(ItemType));
            var randomItemType = (ItemType)itemTypes.GetValue(random.Next(itemTypes.Length));

            player.ClearInventory();
            player.AddItem(randomItemType, 8);
        }

        private void ChangePlayerSize(Player player)
        {
            Random random = new Random();
            var newScale = random.Next(MinPlayerScale, MaxPlayerScale)/10;
            player.Scale = new UnityEngine.Vector3(newScale, newScale, newScale);
        }

        private void TriggerExplosion(Player player)
        {
            var pos = player.Position;
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0.5f;
            grenade.SpawnActive(pos);
        }

        private void CauseBlackout()
        {
            Map.TurnOffAllLights(15, ZoneType.Unspecified);
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
            AddRandomGoodEffect,
            AddRandomBadEffect,
            StartTheFuckingNuke,
            ReplaceInventory,
            SizeChange,
            Kaboom,
            Blackout,
            From,
            To,
            RandomItem
        }
    }
}