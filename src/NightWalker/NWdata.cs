using System;
using System.Linq;
using System.Runtime.CompilerServices;
using RWCustom;
using SlugBase.DataTypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Witness
{
    public class NightWalkerPlayerData
    {
        private static float WingStaminaMaxBase => 1000f;
        private static float WingStaminaRecoveryBase => 2f;
        private static float WingSpeedBase => 5f;

        public readonly bool IsNightWalker;

        public bool CanFly => WingStaminaMax > 0 && WingSpeed > 0;
        public float MinimumFlightStamina => WingStaminaMax * 0.1f;
        public double LowWingStamina => MinimumFlightStamina * 3;

        public int WingStaminaMax => (int)(UnlockedExtraStamina ? (int)(WingStaminaMaxBase * 1.6f) : WingStaminaMaxBase);
        public float WingStaminaRecovery => UnlockedExtraStamina ? WingStaminaRecoveryBase * 1.2f : WingStaminaRecoveryBase;
        public float WingSpeed => UnlockedFasterWings ? WingSpeedBase * 1.3f : WingSpeedBase;

        public bool UnlockedExtraStamina = false;
        public bool UnlockedVerticalFlight = false;
        public bool UnlockedFasterWings = false;

        public int preventGrabs;
        public bool isFlying;
        public float wingStamina;
        public int wingStaminaRecoveryCooldown;
        public int currentFlightDuration;
        public int timeSinceLastFlight;
        public int preventFlight;
        public int lastTail;

        public WeakReference<Player> playerRef;

        public DynamicSoundLoop Wind;

        public Color BodyColor;
        public Color EyesColor;
        public Color WhiskersColor;
        public Color TailColor;

        public FAtlas TailAtlas;

        public NightWalkerPlayerData(Player player)
        {
            IsNightWalker = player.slugcatStats.name == NWEnums.NightWalker;

            playerRef = new WeakReference<Player>(player);

            if (!IsNightWalker)
            {
                return;
            }

            SetupSounds(player);

            lastTail = -1;
            wingStamina = WingStaminaMax;
            timeSinceLastFlight = 200;
        }

        ~NightWalkerPlayerData()
        {
            try
            {
                TailAtlas.Unload();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void LoadTailAtlas()
        {
            var tailTexture = new Texture2D(Plugin.TailTexture.width, Plugin.TailTexture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(Plugin.TailTexture, tailTexture);

            NWUtils.MapTextureColor(tailTexture, 0, TailColor);

            if (playerRef.TryGetTarget(out var player))
            {
                TailAtlas = Futile.atlasManager.LoadAtlasFromTexture("NightWalkercattailtexture_" + player.playerState.playerNumber + Time.time + Random.value, tailTexture, false);
            }
        }

        private void SetupSounds(Player player)
        {
            Wind = new ChunkDynamicSoundLoop(player.bodyChunks[0]);
            Wind.sound = NWEnums.Sound.Wind;
            Wind.Pitch = 1f;
            Wind.Volume = 1f;
        }

        public void SetupColors(PlayerGraphics pg)
        {
            BodyColor = pg.GetColor(NWEnums.Color.Body) ?? Custom.hexToColor("ffcf0d");
            EyesColor = pg.GetColor(NWEnums.Color.Eyes) ?? Custom.hexToColor("010101");
            TailColor = pg.GetColor(NWEnums.Color.Tail) ?? Custom.hexToColor("ffcf0d");
            WhiskersColor = pg.GetColor(NWEnums.Color.Whiskers) ?? Custom.hexToColor("010101");
        }

        public void StopFlight()
        {
            currentFlightDuration = 0;
            timeSinceLastFlight = 0;
            isFlying = false;
        }

        public void InitiateFlight()
        {
            if (!playerRef.TryGetTarget(out var player))
            {
                return;
            }

            player.bodyMode = Player.BodyModeIndex.Default;
            player.animation = Player.AnimationIndex.None;
            player.wantToJump = 0;
            currentFlightDuration = 0;
            timeSinceLastFlight = 0;
            isFlying = true;
        }

        public bool CanSustainFlight()
        {
            if (!playerRef.TryGetTarget(out var player))
            {
                return false;
            }

            return wingStamina > 0 &&
                   preventFlight <= 0 &&
                   player.canJump <= 0 &&
                   player.canWallJump == 0 && //-- Equals zero is correct, is negative when jumping to the left
                   player.Consious &&
                   player.bodyMode != Player.BodyModeIndex.Crawl &&
                   player.bodyMode != Player.BodyModeIndex.CorridorClimb &&
                   player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut &&
                   player.animation != Player.AnimationIndex.HangFromBeam &&
                   player.animation != Player.AnimationIndex.ClimbOnBeam &&
                   player.bodyMode != Player.BodyModeIndex.WallClimb &&
                   player.bodyMode != Player.BodyModeIndex.Swimming &&
                   player.animation != Player.AnimationIndex.AntlerClimb &&
                   player.animation != Player.AnimationIndex.VineGrab &&
                   player.animation != Player.AnimationIndex.ZeroGPoleGrab;
        }

        public void RecreateTailIfNeeded(PlayerGraphics self)
        {
            var currentFood = self.player.CurrentFood;
            var oldTail = self.tail;

            if (currentFood < 6)
            {
                if (lastTail != 2)
                {
                    lastTail = 2;
                    self.tail = new TailSegment[5];
                    self.tail[0] = new TailSegment(self, 8f, 4f, null, 0.85f, 1f, 1f, true);
                    self.tail[1] = new TailSegment(self, 6f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
                    self.tail[2] = new TailSegment(self, 4f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
                    self.tail[3] = new TailSegment(self, 2f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
                    self.tail[4] = new TailSegment(self, 1f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);
                }
            }
            else if (currentFood < 11)
            {
                if (lastTail != 3)
                {
                    lastTail = 3;
                    self.tail = new TailSegment[6];
                    self.tail[0] = new TailSegment(self, 10f, 4f, null, 0.85f, 1f, 1f, true);
                    self.tail[1] = new TailSegment(self, 8f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
                    self.tail[2] = new TailSegment(self, 6f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
                    self.tail[3] = new TailSegment(self, 4f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
                    self.tail[4] = new TailSegment(self, 2f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);
                    self.tail[5] = new TailSegment(self, 1f, 7f, self.tail[4], 0.85f, 1f, 0.5f, true);
                }
            }
            else if (lastTail != 4)
            {
                lastTail = 4;
                self.tail = new TailSegment[7];
                self.tail[0] = new TailSegment(self, 11f, 4f, null, 0.85f, 1f, 1f, true);
                self.tail[1] = new TailSegment(self, 9f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
                self.tail[2] = new TailSegment(self, 7f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
                self.tail[3] = new TailSegment(self, 6f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
                self.tail[4] = new TailSegment(self, 4f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);
                self.tail[5] = new TailSegment(self, 3f, 7f, self.tail[4], 0.85f, 1f, 0.5f, true);
                self.tail[6] = new TailSegment(self, 2f, 7f, self.tail[5], 0.85f, 1f, 0.5f, true);
            }

            if (oldTail != self.tail)
            {
                for (var i = 0; i < self.tail.Length && i < oldTail.Length; i++)
                {
                    self.tail[i].pos = oldTail[i].pos;
                    self.tail[i].lastPos = oldTail[i].lastPos;
                    self.tail[i].vel = oldTail[i].vel;
                    self.tail[i].terrainContact = oldTail[i].terrainContact;
                    self.tail[i].stretched = oldTail[i].stretched;
                }

                var bp = self.bodyParts.ToList();
                bp.RemoveAll(x => x is TailSegment);
                bp.AddRange(self.tail);

                self.bodyParts = bp.ToArray();
            }
        }
    }

    public static class PlayerExtension
    {
        private static readonly ConditionalWeakTable<Player, NightWalkerPlayerData> _cwt = new();

        public static NightWalkerPlayerData NightWalker(this Player player) => _cwt.GetValue(player, _ => new NightWalkerPlayerData(player));

        public static Color? GetColor(this PlayerGraphics pg, PlayerColor color) => color.GetColor(pg);

        public static Color? GetColor(this Player player, PlayerColor color) => (player.graphicsModule as PlayerGraphics)?.GetColor(color);

        public static Player Get(this WeakReference<Player> weakRef)
        {
            weakRef.TryGetTarget(out var result);
            return result;
        }

        public static PlayerGraphics PlayerGraphics(this Player player) => (PlayerGraphics)player.graphicsModule;

        public static TailSegment[] Tail(this Player player) => player.PlayerGraphics().tail;

        public static bool IsNightWalker(this Player player) => player.NightWalker().IsNightWalker;

        public static bool IsNightWalker(this Player player, out NightWalkerPlayerData NightWalker)
        {
            NightWalker = player.NightWalker();
            return NightWalker.IsNightWalker;
        }

    }
}
