﻿using DressMySlugcat;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Creature;
using static DressMySlugcat.SpriteDefinitions;
using static MoreSlugcats.SingularityBomb;

namespace Witness
{
    internal class WSRelativeHooks
    {
        internal static ConditionalWeakTable<Player, FlareData> FlareData { get; } = new();

        public static void Init()
        {
            On.Player.ThrownSpear += Player_ThrownSpear;
            On.Player.Die += Player_Die;
            On.Player.Jump += Player_Jump;
            On.Player.Update += Player_Update;

            if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
            {
                SetupDMSSprites();
            }

        }

        public static void SetupDMSSprites()
        {
            //SpriteDefinitions.AddSprite(new AvailableSprite
            //{
            //    Name = "WHISKERS",
            //    Description = "Whiskers",
            //    GallerySprite = "LizardScaleA0",
            //    RequiredSprites = new List<string> { "LizardScaleA0" },
            //    Slugcats = new List<string>
            //{
            //((ExtEnumBase)Plugin.NightWalkerName).value,
            //((ExtEnumBase)Plugin.WitnessName).value
            //}
            //});
            //SpriteDefinitions.AddSprite(new AvailableSprite
            //{
            //    Name = "SPLATTER",
            //    Description = "Splatter",
            //    GallerySprite = "PhotoSplatter",
            //    RequiredSprites = new List<string> { "PhotoSplatter" },
            //    Slugcats = new List<string> { ((ExtEnumBase)Plugin.WitnessName).value }
            //});

            var sheetID = "Nankh.Witness";

            for (int index = 0; index < 4; index++)
            {
                SpriteDefinitions.AddSlugcatDefault(new Customization()
                {
                    Slugcat = "Witness",
                    PlayerNumber = index,
                    CustomSprites = new List<CustomSprite>
                    {
                        new CustomSprite() { Sprite = "TAIL", SpriteSheetID = sheetID, Color = Color.white }
                    },

                    CustomTail = new CustomTail()
                    {
                        Length = 7,
                        Wideness = 2.4f,
                        Roundness = 1.2f
                    }
                });
            }
        }

        private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);

            if (self.SlugCatClass.value == "Witness" && self.room.game.IsStorySession)
            {
                spear.spearDamageBonus = UnityEngine.Random.Range(1f, 1.2f);
            }

            if (self.SlugCatClass.value == "Witness" && self.room.game.IsArenaSession)
            {
                spear.spearDamageBonus = UnityEngine.Random.Range(0.6f, 1f);
            }
        }

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self.slugcatStats.name.value == "Witness" && !self.dead && (self is not null || self.room is not null))
            {

                if (self.slugcatStats.name.value == "Witness" && self.input[0].thrw && self.grasps[0] != null && self.grasps[0].grabbed is Creature && self.FoodInStomach >= 3)
                {
                    self.SubtractFood(3);
                    Creature shockObject = self.grasps[0].grabbed as Creature;
                    for (int i = 0; i < (int)Mathf.Lerp(4f, 8f, 6f); i++)
                    {
                        self.room.AddObject(new Spark(self.bodyChunks[0].pos, Custom.RNV() * Mathf.Lerp(4f, 14f, UnityEngine.Random.value), new Color(1f, 0.7f, 0f), null, 8, 14));

                    }
                    shockObject.Violence(shockObject.mainBodyChunk, new Vector2?(new Vector2(0f, 0f)), shockObject.mainBodyChunk, null, Creature.DamageType.Electric, 2f, 200f);

                    self.room.AddObject(new CreatureSpasmer(shockObject, false, shockObject.stun));
                    self.room.PlaySound(SoundID.Centipede_Shock, self.bodyChunks[0].pos, 1f, 1f);
                    self.grasps[0].Release();
                }

                if (self.slugcatStats.name.value == "Witness" && self.input[0].thrw && self.grabbedBy.Count > 0 && !self.dead && self.FoodInStomach >= 3)
                {
                    self.SubtractFood(3);
                    Creature shockObject = self.grabbedBy[0].grabber;
                    for (int i = 0; i < (int)Mathf.Lerp(4f, 8f, 6f); i++)
                    {
                        self.room.AddObject(new Spark(self.bodyChunks[0].pos, Custom.RNV() * Mathf.Lerp(4f, 14f, UnityEngine.Random.value), new Color(1f, 0.7f, 0f), null, 8, 14));

                    }
                    shockObject.Violence(shockObject.mainBodyChunk, new Vector2?(new Vector2(0f, 0f)), shockObject.mainBodyChunk, null, Creature.DamageType.Electric, 2f, 200f);

                    self.room.AddObject(new CreatureSpasmer(shockObject, false, shockObject.stun));
                    self.room.PlaySound(SoundID.Centipede_Shock, self.bodyChunks[0].pos, 1f, 1f);

                    shockObject.LoseAllGrasps();
                    if (shockObject.Submersion > 0f)
                    {
                        self.room.AddObject(new UnderwaterShock(self.room, self, self.bodyChunks[0].pos, 14, Mathf.Lerp(ModManager.MMF ? 0f : 200f, 1200f, 6f), 0.2f + 1.9f * 6f, self, new Color(0.7f, 0.7f, 1f)));
                    }
                }

                if (self.slugcatStats.name.value == "Witness")
                {
                    self.setPupStatus(true);
                }

                var data = FlareData.GetValue(self, _ => new());

                if (self.slugcatStats.name.value == "Witness" && self.input[0].jmp && !self.input[1].jmp && self.input[0].pckp && data.flashCooldown <= 0)
                {
                    AbstractConsumable abstractFlareBomb = new(self.room.world, AbstractPhysicalObject.AbstractObjectType.FlareBomb, null, self.coord, self.room.game.GetNewID(), -1, -1, null);
                    self.room.abstractRoom.AddEntity(abstractFlareBomb);
                    abstractFlareBomb.RealizeInRoom();
                    FlareBomb flareBomb = (FlareBomb)abstractFlareBomb.realizedObject;

                    flareBomb.firstChunk.HardSetPosition(self.bodyChunks[0].pos);
                    flareBomb.StartBurn();
                    float FlashDelay = 10;
                    data.flashCooldown = (int)(FlashDelay * 40f);

                    AbstractPhysicalObject abstractObject = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null, self.coord, self.room.game.GetNewID(), -1, -1, null);
                    self.room.abstractRoom.AddEntity(abstractObject);
                    abstractObject.RealizeInRoom();
                }

                if (self.slugcatStats.name.value == "Witness" && data.flashCooldown > 0)
                {
                    data.flashCooldown--;
                }
            }
        }

        private static void Player_Die(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;

            orig(self);

            if (self.SlugCatClass.value == "Witness" && !wasDead && self.dead && self.room is not null && self is not null)
            {
                var room = self.room;
                var pos = self.mainBodyChunk.pos;
                Vector2 vector = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
                room.AddObject(new SparkFlash(self.firstChunk.pos, 700f, new Color(0f, 0f, 1f)));
                room.AddObject(new Explosion(room, self, vector, 7, 4500f, 6.2f, 100f, 280f, 0.25f, self as Creature, 0f, 160f, 1f));
                room.AddObject(new Explosion(room, self, vector, 7, 20000f, 4f, 100f, 400f, 0.25f, self, 0.3f, 200f, 1f));
                room.AddObject(new Explosion.ExplosionLight(vector, 280f, 1f, 7, new Color(1f, 1f, 1f)));
                room.AddObject(new Explosion.ExplosionLight(vector, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new Explosion.ExplosionLight(vector, 2000f, 2f, 60, new Color(1f, 1f, 1f)));
                room.AddObject(new ShockWave(vector, 750f, 0.485f, 300, highLayer: true));
                room.AddObject(new ShockWave(vector, 5000f, 0.185f, 180));

                room.ScreenMovement(pos, default, 2.3f);
                room.PlaySound(SoundID.Bomb_Explode, pos);
                room.InGameNoise(new Noise.InGameNoise(pos, 18000f, self, 10f));
            }
        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            float jumpPower = 0.25f;

            if (self.SlugCatClass.value == "Witness")
            {
                self.jumpBoost *= 1f + jumpPower;
            }
        }
    }

    internal class FlareData
    {
        public int flashCooldown { get; internal set; }
    }
}