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
using Exiled.Events.Handlers;
using MEC;
using Item = Exiled.API.Features.Items.Item;
using Map = Exiled.API.Features.Map;
using Player = Exiled.API.Features.Player;
using Warhead = Exiled.API.Features.Warhead;

namespace FunnyPills
{
    [CustomItem(ItemType.SCP500)]
    public class Scp500 : CustomItem
    {
        public override uint Id { get; set; } = 500;

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
                Chance = 70,
                Location = SpawnLocationManager.GetRandomSpawnLocation()
                },
            },
        };
        public bool AffecAll500s { get; set; } = false;

        public Dictionary<Effects, Chance> EffectChances { get; set; } = new Dictionary<Effects, Chance>
        {
            { Effects.StartTheFuckingNuke, new Chance(1, 10)},
            { Effects.ReplaceInventory, new Chance(11, 50) },
            { Effects.AddRandomGoodEffect, new Chance(51, 150) },
            { Effects.AddRandomBadEffect, new Chance(151, 300) },
            { Effects.Die, new Chance(301, 350) },
            { Effects.Add5MoveBoost, new Chance(351, 450) },
            { Effects.OneSpec, new Chance(451, 500) },
            { Effects.AllSpec, new Chance(501, 525) },
            { Effects.BetrayTeam, new Chance(526, 700) },
            { Effects.SizeChange, new Chance(701, 800) },
            { Effects.Kaboom, new Chance(801, 850) },
            { Effects.Blackout, new Chance(851, 1000)},
            { Effects.invisible, new Chance(10, 35) },
            { Effects.Dimension106, new Chance(100, 450)},
            { Effects.Inventory, new Chance(200, 451)},
            { Effects.Teleport, new Chance(50, 100) },
            { Effects.CuffPlayer, new Chance(1, 5)},
            { Effects.MovementBoost, new Chance(100, 200) },
            { Effects.TeleportALlPlayer, new Chance(1, 2)}
        };

        public int MaxPlayerScale { get; set; } = 10;
        public int MinPlayerScale { get; set; } = 5;
        
        private CoroutineHandle _coroutine;


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
            if (Check(ev.Item) || AffecAll500s)
            {
                Log.Debug("Passed FP Check");
                if (ev.Item.Type == ItemType.SCP500)
                {
                    Random random = new Random();
                    int chance = random.Next(1, 1000);

                    foreach (var efctDict in EffectChances)
                    {
                        if (chance >= efctDict.Value.MinimumChance && chance <= efctDict.Value.MaximumChance)
                        {
                            Log.Warn($"FP Returned {efctDict.Key}");
                            ApplyEffect(ev.Player, efctDict.Key);
                            break;
                        }
                    }
                }
            }
        }

        private void Dimension106(Player player)
        {
            player.Broadcast(5, "Pas de chance <3");
            player.EnableEffect(EffectType.PocketCorroding);
        }

        public IEnumerator<float> MovementBost(Player player)
        {
            player.Broadcast(5, "Tu a de la chance mais plus pour longtemps");
            player.EnableEffect(EffectType.MovementBoost, 125, 30f);
            Timing.WaitForSeconds(20f);
            player.Kill("Tu a utilisé tous le carburant");
            yield break;
        }

        private void ApplyEffect(Player player, Effects effect)
        {
            switch (effect)
            {
                case Effects.StartTheFuckingNuke:
                    StartTheNuke();
                    break;
                case Effects.ReplaceInventory:
                    ReplacePlayerInventory(player);
                    break;
                case Effects.AddRandomGoodEffect:
                    ApplyRandomEffect(player, true);
                    break;
                case Effects.AddRandomBadEffect:
                    ApplyRandomEffect(player, false);
                    break;
                case Effects.Die:
                    ApplyDieEffect(player);
                    break;
                case Effects.Add5MoveBoost:
                    ApplyAdd5MoveBoostEffect(player);
                    break;
                case Effects.OneSpec:
                    ApplyOneSpecEffect(player);
                    break;
                case Effects.AllSpec:
                    ApplyAllSpecEffect(player);
                    break;
                case Effects.BetrayTeam:
                    ApplyBetrayTeamEffect(player);
                    break;
                case Effects.SizeChange:
                    ChangePlayerSize(player);
                    break;
                case Effects.Kaboom:
                    TriggerExplosion(player);
                    break;
                case Effects.Blackout:
                    CauseBlackout();
                    break;
                case Effects.invisible:
                    Invisible(player);
                    break;
                case Effects.Inventory:
                    ClearInventory(player);
                    break;
                case Effects.Dimension106:
                    Dimension106(player);
                    break;
                case Effects.Teleport:
                    Teleport(player);
                    break;
                case Effects.CuffPlayer:
                    CuffPlayer(player);
                    break;
                case Effects.MovementBoost:
                    MovementBost(player);
                    break;
                case Effects.TeleportALlPlayer:
                    TeleportALlPlayer(player);
                    break;
            }
        }

    private void ApplyAllSpecEffect(Player player)
    {
        foreach (var p in Player.List.Where(o => o.Role == RoleTypeId.Spectator))
        {
        RoleSpawnFlags spawnFlags = RoleSpawnFlags.AssignInventory;
        p.RoleManager.ServerSetRole(player.Role, RoleChangeReason.Revived, spawnFlags);
        p.Teleport(player.Position);
        player.Broadcast(5, "You Have Revived All Spectators");
        p.Broadcast(5, $"You Have Been Revived By {player.Nickname}");
        }
    }

        private void ApplyOneSpecEffect(Player player)
        {
            var spectators = Player.List.Where(p => p.Role.Type == RoleTypeId.Spectator).ToList();

            if (spectators.Count == 0)
            {
                Log.Warn("No Spectators To Revive");
            }

            var random = new System.Random();
            var selectedSpectator = spectators[random.Next(spectators.Count)];
            player.Broadcast(5, $"You Have Revived {selectedSpectator.Nickname}");
            RoleSpawnFlags spawnFlags = RoleSpawnFlags.AssignInventory;

            selectedSpectator.Broadcast(5, $"You Have Been Revived By {player.Nickname}");

            selectedSpectator.RoleManager.ServerSetRole(player.Role, RoleChangeReason.Revived, spawnFlags);
            selectedSpectator.Teleport(player.Position);
        }

        private void Teleport(Player player)
        {
            player.Broadcast(5, "Tu est vraiment stupide!!");
            player.Teleport(Room.Random().Position);
        }

        private void ApplyAdd5MoveBoostEffect(Player player)
        {
            if (player.GetEffect(EffectType.MovementBoost).IsEnabled)
            {
                byte intensity = player.GetEffectIntensity<MovementBoost>();
                if (intensity < 100)
                {
                    intensity += 25;
                    player.Broadcast(5, "<color=green>You Have Gained +25% Movement Speed</color>");
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
                player.Broadcast(5, "<color=green>You Have Gained A 25% Movement Boost</color>");
            }
        }
        
        private void ApplyDieEffect(Player player)
        {
            player.Broadcast(5, "<color=red>You F***ed Around And Found Out...</color>");

            player.Kill("Tu est nul!!!");
        }

        private void ClearInventory(Player player)
        {
            player.ClearInventory();
        }

        private void ApplyRandomEffect(Player player, bool IsBad)
        {
            if (!IsBad)
            {
                player.ApplyRandomEffect(EffectCategory.Positive, 255, 90);
                player.Broadcast(5, "<color=green>You Feel Stronger</color>");


            }
            else if (IsBad)
            {
                player.ApplyRandomEffect(EffectCategory.Negative, 255, 90);
                player.Broadcast(5, "<color=green>You Feel Weaker</color>");

            }
        }

        private void CuffPlayer(Player player)
        {
            player.Broadcast(5, "Tu as vraiment pas de chance");
            _coroutine = Timing.RunCoroutine(CuffPlayers(player));
        }

        private IEnumerator<float> CuffPlayers(Player player)
        {
            player.Handcuff();
            Timing.WaitForSeconds(25f);
            player.RemoveHandcuffs();
            yield break;
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
            player.Broadcast(5, $"Your Inventory Has Been Replaced By <color=red>{randomItemType}</color>.");
        }

        private void ChangePlayerSize(Player player)
        {
            Random random = new Random();
            var newScale = random.Next(MinPlayerScale, MaxPlayerScale)/10;
            player.Scale = new UnityEngine.Vector3(newScale, newScale, newScale);
            string sizeMessage = player.Scale.x < 1.0f ? "Shorter" : "Taller";
            player.Broadcast(5, $"<color=green>You Begin To Feel {sizeMessage}</color>");
        }

        private void TriggerExplosion(Player player)
        {
            player.Broadcast(5, "<color=green>Kaboom</color>");
            _coroutine = Timing.RunCoroutine(Kaboomm(player));
        }
        
        private IEnumerator<float> Kaboomm(Player player)
        {
            Timing.WaitForSeconds(30f);
            var pos = player.Position;
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0.5f;
            grenade.SpawnActive(pos);
            yield break;
        }

        private void Invisible(Player player)
        {
            player.Broadcast(5, "Tu a de la chance");

            player.EnableEffect(EffectType.Invisible, 1 , 15f);
        }

        private void CauseBlackout()
        {
            Map.TurnOffAllLights(15, ZoneType.Unspecified);
        }

        private void TeleportALlPlayer(Player player)
        {
            player.Broadcast(5, "GG tu as réussi a tp toute la map sur toi (1%)");
            foreach (Player players in Player.List)
            {
                players.Teleport(player.Position);
            }
        }

        private void ApplyBetrayTeamEffect(Player player)
        {
            player.Broadcast(5, "<color=red>You Let Your Intrusive Thoughts Win, And Betrayed Your Comrades In Battle</color>.");
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
            RandomItem, 
            invisible,
            Dimension106,
            Inventory,
            Teleport,
            CuffPlayer,
            MovementBoost,
            TeleportALlPlayer
        }
    }
}