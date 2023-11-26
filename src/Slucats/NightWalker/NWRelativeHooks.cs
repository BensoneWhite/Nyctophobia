using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Player;
using static PlayerGraphics;

namespace Witness
{
    public static class NWRelativeHooks
    {

        public static void Init()
        {
            On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            On.Lizard.ctor += Lizard_ctor;
            On.HUD.RainMeter.Update += RainMeter_Update;
            On.Player.ThrownSpear += Player_ThrownSpear;
            On.Player.UpdateBodyMode += Player_UpdateBodyMode;
            On.GhostWorldPresence.SpawnGhost += GhostWorldPresence_SpawnGhost;
            On.Player.Grabability += Player_Grabability;
            On.Player.Jump += Player_Jump;
            On.Player.WallJump += Player_WallJump;

            if (!ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
            {
                On.PlayerGraphics.ctor += PlayerGraphics_ctor;
                On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
                On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
                On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            }
        }

        private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig(self, sLeaser, rCam, newContatiner);
            if (!self.player.IsNightWalker(out _))
            {
                return;
            }
            sLeaser.sprites[2].MoveBehindOtherNode(sLeaser.sprites[1]);
        }

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (!self.player.IsNightWalker(out var night)) return;

            if (sLeaser.sprites[2] is TriangleMesh tail && night.TailAtlas.elements != null && night.TailAtlas.elements.Count > 0)
            {
                tail.element = night.TailAtlas.elements[0];

                for (var i = tail.vertices.Length - 1; i >= 0; i--)
                {
                    var perc = i / 2 / (float)(tail.vertices.Length / 2);

                    Vector2 uv;
                    if (i % 2 == 0)
                        uv = new Vector2(perc, 0f);
                    else if (i < tail.vertices.Length - 1)
                        uv = new Vector2(perc, 1f);
                    else
                        uv = new Vector2(1f, 0f);

                    uv.x = Mathf.Lerp(tail.element.uvBottomLeft.x, tail.element.uvTopRight.x, uv.x);
                    uv.y = Mathf.Lerp(tail.element.uvBottomLeft.y, tail.element.uvTopRight.y, uv.y);

                    tail.UVvertices[i] = uv;
                }
            }
            self.AddToContainer(sLeaser, rCam, null);
        }

        private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (!self.player.IsNightWalker(out var night))
            {
                return;
            }

            night.NWTail(self);

            night.SetupColorsNW(self);

            night.SetupTailTextureNW(night);
        }

        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            FSprite fSprite = sLeaser.sprites[3];
            if (self.player.SlugCatClass.value == "NightWalker")
            {
                fSprite.SetElementByName("slugcula" + fSprite.element.name);
            }
        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.SlugCatClass.value == "NightWalker" && (self.redsIllness == null || self.redsIllness.cycle <= 3) && !self.playerState.isGhost && self.room.game.IsStorySession)
            {
                self.redsIllness = new RedsIllness(self, 3);
            }
        }

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            if(self.SlugCatClass.value == "NightWalker" && self is not null && self.room is not null)
            {
                if (self.SlugCatClass.value == "NightWalker" && self.room.game.IsStorySession && self.dead && !self.dead && !self.KarmaIsReinforced && self.Karma == 0)
                {
                    self.room.game.rainWorld.progression.WipeSaveState(self.room.game.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat);
                    self.room.PlaySound(SoundID.MENU_Enter_Death_Screen, self.mainBodyChunk, false, 1f, 1f);
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ascended = true;
                    self.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Statistics, 5f);
                    self.room.game.GetStorySession.saveState.AppendCycleToStatistics(self, self.room.game.GetStorySession, death: true, 0);
                    self.room.game.GoToRedsGameOver();
                }

                if (self.SlugCatClass.value == "NightWalker" && self.redsIllness != null && self.room.game.IsStorySession)
                {
                    self.redsIllness.Update();
                }
            }
            orig(self, eu);
        }

        private static void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            float angryness = 100f;
            orig(self, abstractCreature, world);

            if (world.game.session.characterStats.name.value == "NightWalker")
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, angryness);
            }
        }

        private static void RainMeter_Update(On.HUD.RainMeter.orig_Update orig, HUD.RainMeter self)
        {
            orig(self);
            Player player = (self.hud.owner as Player);

            if (player.SlugCatClass.value == "NightWalker")
            {
                self.fade = 0f;
                self.tickCounter = 1;
                self.fade = 0f;
            }
        }

        private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (self.SlugCatClass.value == "NightWalker")
            {
                spear.spearDamageBonus = UnityEngine.Random.Range(0.6f, 2.4f);
            }
        }

        private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            float crawlPower = 0.75f;
            orig(self);
            if (self.SlugCatClass.value == "NightWalker")
            {
                self.dynamicRunSpeed[0] += crawlPower;
                self.dynamicRunSpeed[1] += crawlPower;
            }

            if (self.slugcatStats.name.value == "NightWalker" && self.animation == AnimationIndex.BellySlide)
            {

                float vector1 = 7f;
                float vector2 = 13f;

                self.bodyChunks[0].vel.x += (self.longBellySlide ? vector1 : vector2) * (float)self.rollDirection * Mathf.Sin((float)self.rollCounter / (self.longBellySlide ? 29f : 15f) * (float)Math.PI);
                if (self.IsTileSolid(0, 0, -1) || self.IsTileSolid(0, 0, -2))
                {
                    self.bodyChunks[0].vel.y -= 2f;
                }
            }
        }

        private static bool GhostWorldPresence_SpawnGhost(On.GhostWorldPresence.orig_SpawnGhost orig, GhostWorldPresence.GhostID ghostID, int karma, int karmaCap, int ghostPreviouslyEncountered, bool playingAsRed)
        {
            orig(ghostID, karma, karmaCap, ghostPreviouslyEncountered, playingAsRed);

            if (Custom.rainWorld.processManager.currentMainLoop is RainWorldGame game && game.session.characterStats.name.value == "NightWalker")
            {
                return false;
            }
            return orig(ghostID, karma, karmaCap, ghostPreviouslyEncountered, playingAsRed);
        }

        private static ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            orig(self, obj);

            if(self.slugcatStats.name.value == "NightWalker" && obj is Weapon)
            {
                return Player.ObjectGrabability.OneHand;
            }
            return orig(self, obj);
        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            float jumpboost = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

            if (self.slugcatStats.name.value == "NightWalker" && self.animation == Player.AnimationIndex.Flip)
            {

                self.bodyChunks[0].vel.y = 10f * jumpboost;
                self.bodyChunks[1].vel.y = 9f * jumpboost;


                self.bodyChunks[0].vel.x = 5f * (float)self.flipDirection * jumpboost;
                self.bodyChunks[1].vel.x = 4f * (float)self.flipDirection * jumpboost;
            }

            if (self.slugcatStats.name.value == "NightWalker" && self.animation == Player.AnimationIndex.RocketJump)
            {

                self.bodyChunks[0].vel.y += 4f + jumpboost;
                self.bodyChunks[1].vel.y += 3f + jumpboost;

                self.bodyChunks[0].vel.x += 3f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
                self.bodyChunks[1].vel.x += 2f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
            }
        }

        private static void Player_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            orig(self, direction);
            float num = Mathf.Lerp(0.9f, 1.10f, self.Adrenaline);

            if (self.slugcatStats.name.value == "NightWalker")
            {
                self.bodyChunks[0].vel.y = 10f * num;
                self.bodyChunks[1].vel.y = 9f * num;
                self.bodyChunks[0].vel.x = 8f * num * (float)direction;
                self.bodyChunks[1].vel.x = 6f * num * (float)direction;
            }
        }
    }
}