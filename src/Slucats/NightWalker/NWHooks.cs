using UnityEngine;

namespace Nyctophobia;

public static class NWHooks
{
    public static ConditionalWeakTable<Player, Whiskerdata> whiskerstorage = new();

    public static void Init()
    {
        On.Player.ctor += Player_ctor;
        On.Player.Update += Player_Update;
        On.Lizard.ctor += Lizard_ctor;
        On.HUD.RainMeter.Update += RainMeter_Update;
        On.Player.ThrownSpear += Player_ThrownSpear;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        On.Player.Grabability += Player_Grabability;
        On.Player.Jump += Player_Jump;
        On.Player.WallJump += Player_WallJump;
        On.Player.UpdateMSC += Player_UpdateMSC;

        On.World.SpawnGhost += World_SpawnGhost;
        On.GhostHunch.Update += GhostHunch_Update;

        if (!ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
        {
        }

        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        On.PlayerGraphics.Update += PlayerGraphics_Update;
        On.Player.Collide += Player_Collide;
        On.PlayerGraphics.ctor += PlayerGraphics_ctor;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
    }

    private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
    {
        orig(self, ow);

        if (!self.player.IsNightWalker(out var night))
        {
            return;
        }

        night.SetupColors(self);
        night.LoadTailAtlas();
        night.NWTailLonger(self);

        whiskerstorage.Add(self.player, new Whiskerdata(self.player));
        whiskerstorage.TryGetValue(self.player, out Whiskerdata data);

        for (int i = 0; i < data.headScales.Length; i++)
        {
            data.headScales[i] = new Whiskerdata.Scale(self);
            data.headpositions[i] = new Vector2((i < data.headScales.Length / 2 ? 0.7f : -0.7f), i == 1 ? 0.035f : 0.026f);

        }
    }

    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        if (!self.player.IsNightWalker(out var night))
        {
            return;
        }

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

        whiskerstorage.TryGetValue(self.player, out var thedata);
        thedata.initialfacewhiskerloc = sLeaser.sprites.Length;

        Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 6);

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                sLeaser.sprites[thedata.Facewhiskersprite(i, j)] = new FSprite(thedata.facesprite)
                {
                    scaleY = 17f / Futile.atlasManager.GetElementWithName(thedata.sprite).sourcePixelSize.y,
                    anchorY = 0.1f
                };
            }
        }
        thedata.ready = true;

        self.AddToContainer(sLeaser, rCam, null);
    }

    private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (!self.player.IsNightWalker(out var night))
        {
            return;
        }

        Color[] actualColors = new Color[10];
        for (int i = 0; i < 10; i++)
        {
            actualColors[i] = sLeaser.sprites[i].color;
        }

        sLeaser.sprites[2].color = Color.white;

        var realColor = self.player.ShortCutColor();

        Color black = new(0.009f, 0.009f, 0.009f, 1f);
        Color white = Color.white;

        float colorChangeSpeed = 0.5f;

        var colorChangeProgress = Mathf.Clamp01(0 + Time.deltaTime * colorChangeSpeed);

        if (!night.DarkMode[self.player])
        {
            sLeaser.sprites[0].color = Color.Lerp(actualColors[0], realColor, colorChangeProgress);
            sLeaser.sprites[1].color = Color.Lerp(actualColors[1], realColor, colorChangeProgress);
            sLeaser.sprites[2].color = Color.Lerp(actualColors[2], white,     colorChangeProgress);
            sLeaser.sprites[3].color = Color.Lerp(actualColors[3], realColor, colorChangeProgress);
            sLeaser.sprites[4].color = Color.Lerp(actualColors[4], realColor, colorChangeProgress);
            sLeaser.sprites[5].color = Color.Lerp(actualColors[5], realColor, colorChangeProgress);
            sLeaser.sprites[6].color = Color.Lerp(actualColors[6], realColor, colorChangeProgress);
            sLeaser.sprites[7].color = Color.Lerp(actualColors[7], realColor, colorChangeProgress);
            sLeaser.sprites[8].color = Color.Lerp(actualColors[8], realColor, colorChangeProgress);
            sLeaser.sprites[9].color = Color.Lerp(actualColors[9], black,     colorChangeProgress);
        }
        else
        {
            sLeaser.sprites[0].color = Color.Lerp(actualColors[0], black, colorChangeProgress);
            sLeaser.sprites[1].color = Color.Lerp(actualColors[1], black, colorChangeProgress);
            sLeaser.sprites[2].color = Color.Lerp(actualColors[2], black, colorChangeProgress);
            sLeaser.sprites[3].color = Color.Lerp(actualColors[3], black, colorChangeProgress);
            sLeaser.sprites[4].color = Color.Lerp(actualColors[4], black, colorChangeProgress);
            sLeaser.sprites[5].color = Color.Lerp(actualColors[5], black, colorChangeProgress);
            sLeaser.sprites[6].color = Color.Lerp(actualColors[6], black, colorChangeProgress);
            sLeaser.sprites[7].color = Color.Lerp(actualColors[7], black, colorChangeProgress);
            sLeaser.sprites[8].color = Color.Lerp(actualColors[8], black, colorChangeProgress);
            sLeaser.sprites[9].color = Color.Lerp(actualColors[9], white, colorChangeProgress);
        }

        FSprite fSprite = sLeaser.sprites[3];

        fSprite.SetElementByName("slugcula" + fSprite.element.name);

        if (whiskerstorage.TryGetValue(self.player, out Whiskerdata data))
        {
            int index = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2 vector = new(sLeaser.sprites[9].x + camPos.x, sLeaser.sprites[9].y + camPos.y);
                    float f = 0f;
                    float num = 0f;
                    if (i == 0)
                    {
                        vector.x -= 5f;
                    }
                    else
                    {
                        num = 180f;
                        vector.x += 5f;
                    }
                    sLeaser.sprites[data.Facewhiskersprite(i, j)].x = vector.x - camPos.x;
                    sLeaser.sprites[data.Facewhiskersprite(i, j)].y = vector.y - camPos.y;
                    sLeaser.sprites[data.Facewhiskersprite(i, j)].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(data.headScales[index].lastPos, data.headScales[index].pos, timeStacker)) + num;
                    sLeaser.sprites[data.Facewhiskersprite(i, j)].scaleX = 0.4f * Mathf.Sign(f);
                    sLeaser.sprites[data.Facewhiskersprite(i, j)].color = night.WhiskersColor;
                    index++;
                }
            }
        }
    }

    private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
        if (!self.player.IsNightWalker(out var _)) return;

        if (whiskerstorage.TryGetValue(self.player, out Whiskerdata data))
        {
            int index = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2 pos = self.owner.bodyChunks[0].pos;
                    Vector2 pos2 = self.owner.bodyChunks[1].pos;
                    float num = 0f;
                    float num2 = 90f;
                    int num3 = index % (data.headScales.Length / 2);
                    float num4 = num2 / (data.headScales.Length / 2);
                    if (i == 1)
                    {
                        num = 0f;
                        pos.x += 5f;
                    }
                    else
                    {
                        pos.x -= 5f;
                    }
                    Vector2 a = Custom.rotateVectorDeg(Custom.DegToVec(0f), num3 * num4 - num2 / 2f + num + 90f);
                    float f = Custom.VecToDeg(self.lookDirection);
                    Vector2 vector = Custom.rotateVectorDeg(Custom.DegToVec(0f), num3 * num4 - num2 / 2f + num);
                    Vector2 a2 = Vector2.Lerp(vector, Custom.DirVec(pos2, pos), Mathf.Abs(f));
                    if (data.headpositions[index].y < 0.2f)
                    {
                        a2 -= a * Mathf.Pow(Mathf.InverseLerp(0.2f, 0f, data.headpositions[index].y), 2f) * 2f;
                    }
                    a2 = Vector2.Lerp(a2, vector, Mathf.Pow(0.0875f, 1f)).normalized;
                    Vector2 vector2 = pos + a2 * data.headScales.Length;
                    if (!Custom.DistLess(data.headScales[index].pos, vector2, data.headScales[index].length / 2f))
                    {
                        Vector2 a3 = Custom.DirVec(data.headScales[index].pos, vector2);
                        float num5 = Vector2.Distance(data.headScales[index].pos, vector2);
                        float num6 = data.headScales[index].length / 2f;
                        data.headScales[index].pos += a3 * (num5 - num6);
                        data.headScales[index].vel += a3 * (num5 - num6);
                    }
                    data.headScales[index].vel += Vector2.ClampMagnitude(vector2 - data.headScales[index].pos, 10f) / Mathf.Lerp(5f, 1.5f, 0.5873646f);
                    data.headScales[index].vel *= Mathf.Lerp(1f, 0.8f, 0.5873646f);
                    data.headScales[index].ConnectToPoint(pos, data.headScales[index].length, true, 0f, new Vector2(0f, 0f), 0f, 0f);
                    data.headScales[index].Update();
                    index++;
                }
            }
        }

        if (self.player.animation == AnimationIndex.Roll)
        {
            for (int i = 1; i < self.tail.Length; i++)
            {
                float startVel = Custom.VecToDeg(Custom.DirVec(self.tail[i].pos, self.tail[i - 1].pos));
                startVel += 25f * -self.player.flipDirection;
                self.tail[i].vel = Custom.DegToVec(startVel) * 7.5f;
                if (self.player.bodyChunks[0].pos.y >= self.player.bodyChunks[1].pos.y)
                {
                    self.tail[i].vel.x *= 1.20f;
                    self.tail[i].vel.y *= 0.25f;
                }
            }
        }
    }



    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        if (!self.player.IsNightWalker(out _)) return;

        sLeaser.sprites[2].MoveBehindOtherNode(sLeaser.sprites[1]);

        if (whiskerstorage.TryGetValue(self.player, out Whiskerdata data) && data.ready)
        {
            newContatiner ??= rCam.ReturnFContainer("Midground");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    FSprite whisker = sLeaser.sprites[data.Facewhiskersprite(i, j)];
                    newContatiner.AddChild(whisker);
                }
            }
            data.ready = false;
        }
    }

    private static void GhostHunch_Update(On.GhostHunch.orig_Update orig, GhostHunch self, bool eu)
    {
        if (self.room?.game?.session is StoryGameSession storySession && storySession.saveStateNumber.value == "NightWalker")
        {
            self.Destroy();
        }

        orig(self, eu);
    }

    private static void World_SpawnGhost(On.World.orig_SpawnGhost orig, World self)
    {
        if (self.game.session is StoryGameSession storySession && storySession.saveStateNumber.value == "NightWalker")
        {
            return;
        }

        orig(self);
    }

    private static void Player_Collide(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        orig(self, otherObject, myChunk, otherChunk);

        if (!self.IsNightWalker(out var _)) return;

        if (otherObject is Creature)
        {

        }
    }

    private static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
    {
        orig(self);

        if (!self.IsNightWalker(out var night)) return;
        const float normalGravity = 0.9f;
        const float normalAirFriction = 0.999f;
        const float flightGravity = -0.25f;
        const float flightAirFriction = 0.83f;
        const float flightKickinDuration = 6f;

        if (night.CanFly && (self.Karma == 10 || self.room.game.IsArenaSession))
        {
            if (self.animation == AnimationIndex.HangFromBeam)
            {
                night.preventFlight = 15;
            }
            else if (night.preventFlight > 0)
            {
                night.preventFlight--;
            }

            if (night.isFlying)
            {
                night.Wind.Volume = Mathf.Lerp(0f, 1f, night.currentFlightDuration / flightKickinDuration);

                night.currentFlightDuration++;

                self.AerobicIncrease(0.00001f);

                self.gravity = Mathf.Lerp(normalGravity, flightGravity, night.currentFlightDuration / flightKickinDuration);
                self.airFriction = Mathf.Lerp(normalAirFriction, flightAirFriction, night.currentFlightDuration / flightKickinDuration);


                if (self.input[0].x > 0)
                {
                    self.bodyChunks[0].vel.x += night.WingSpeed;
                    self.bodyChunks[1].vel.x -= 1f;
                }
                else if (self.input[0].x < 0)
                {
                    self.bodyChunks[0].vel.x -= night.WingSpeed;
                    self.bodyChunks[1].vel.x += 1f;
                }

                if (self.room.gravity <= 0.5)
                {
                    if (self.input[0].y > 0)
                    {
                        self.bodyChunks[0].vel.y += night.WingSpeed;
                        self.bodyChunks[1].vel.y -= 1f;
                    }
                    else if (self.input[0].y < 0)
                    {
                        self.bodyChunks[0].vel.y -= night.WingSpeed;
                        self.bodyChunks[1].vel.y += 1f;
                    }
                }
                else if (night.UnlockedVerticalFlight)
                {
                    if (self.input[0].y > 0)
                    {
                        self.bodyChunks[0].vel.y += night.WingSpeed * 0.75f;
                        self.bodyChunks[1].vel.y -= 0.3f;
                    }
                    else if (self.input[0].y < 0)
                    {
                        self.bodyChunks[0].vel.y -= night.WingSpeed;
                        self.bodyChunks[1].vel.y += 0.3f;
                    }
                }

                night.wingStaminaRecoveryCooldown = 40;
                night.wingStamina--;

                if (!self.input[0].jmp || !night.CanSustainFlight())
                {
                    night.StopFlight();
                }
                if (night.wingStamina <= 0)
                {
                    self.SubtractFood(1);
                }
            }
            else
            {
                night.Wind.Volume = Mathf.Lerp(1f, 0f, night.timeSinceLastFlight / flightKickinDuration);

                night.timeSinceLastFlight++;

                night.Wind.Volume = 0f;

                if (night.wingStaminaRecoveryCooldown > 0)
                {
                    night.wingStaminaRecoveryCooldown--;
                }
                else
                {
                    night.wingStamina = Mathf.Min(night.wingStamina + night.WingStaminaRecovery, night.WingStaminaMax);
                }

                if (self.wantToJump > 0 && night.wingStamina > night.MinimumFlightStamina && night.CanSustainFlight())
                {
                    night.InitiateFlight();
                }

                self.airFriction = normalAirFriction;
                self.gravity = normalGravity;
            }
        }
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        if (!self.IsNightWalker(out var NW)) return;

        NW.focus[self] = false;
        NW.DarkMode[self] = false;
        NW.canFocus[self] = true;
        

        if ((self.redsIllness == null || self.redsIllness.cycle <= 3) && !self.playerState.isGhost && self.room.game.IsStorySession)
        {
            self.redsIllness = new RedsIllness(self, 3);
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.IsNightWalker(out var NW)) return;

        if (self is not null && self.room is not null)
        {
            PlayerGraphics selfGraphics = self.graphicsModule as PlayerGraphics;
            InputPackage inputPackage = self.input[0];

            float inputValx = self.input[0].x;
            float inputValy = self.input[0].y;

            Vector2 combinedDirection = new Vector2(inputValx, inputValy).normalized;

            NW.dashDirectionX = combinedDirection;
            NW.dashDirectionY = combinedDirection;

            Vector2 newPosition = self.bodyChunks[0].pos + combinedDirection * NW.dashDistance;

            Vector2 currentVelocity = newPosition - self.bodyChunks[0].pos;
            Vector2 newVelocity = currentVelocity * 0.9f;

            Vector2 smokePos =
            selfGraphics.tail is null ? self.bodyChunks[1].pos :
            selfGraphics.tail.Length > 3 ? selfGraphics.tail[Random.Range(selfGraphics.tail.Length - 1, (int)(selfGraphics.tail.Length / 2f))].pos :
            selfGraphics.tail.Length > 2 ? selfGraphics.tail[Random.Range(selfGraphics.tail.Length - 1, selfGraphics.tail.Length - 2)].pos :
            selfGraphics.tail.Length > 0 ? selfGraphics.tail[1].pos : self.bodyChunks[1].pos;

            if (inputPackage.mp && NW.canFocus[self])
            {
                NW.focus[self] = !NW.focus[self];
                NW.DarkMode[self] = !NW.DarkMode[self];
            }
            if (NW.focus[self])
            {
                //self.mushroomEffect = 1f;
                self.room.AddObject(new NWSmoke(smokePos, new Color(0.008f, 0.008f, 0.008f, 1f), 0.1f));
            }
            else
            {
                self.mushroomEffect = 0f;
                self.mushroomCounter = 0;
            }
            NW.canFocus[self] = !inputPackage.mp;

            if ((NW.dashCooldown <= 0 && NW.currentDashes == 3) || NW.DoesThatPlayerDashedOrNoBOZO < 0)
            {
                NW.currentDashes = 0;
                NW.dashCooldown = 0;
            }

            if (inputPackage.mp && (inputValx != 0 || inputValy != 0) && NW.dashCooldown <= 0 && NW.currentDashes <= 3)
            {
                float DashDelay = 0.35f;
                NW.dashCooldown = (int)(DashDelay * 40f);

                float MaxDistance = 0.04f;
                NW.maxDashDistance = (int)(MaxDistance * 40f);

                float TIMERISTIMEHOHOHO = 20f;
                NW.DoesThatPlayerDashedOrNoBOZO = (int)(TIMERISTIMEHOHOHO * 40f);

                NW.currentDashes++;

                self.room.PlaySound(SoundID.Slugcat_Flip_Jump, self.mainBodyChunk, false, 1f, 1f);
                self.room.PlaySound(SoundID.Leaves, self.mainBodyChunk, false, 1f, 1f);

                NW.Dashed = true;
            }

            if (NW.Dashed)
            {
                newPosition = self.bodyChunks[0].pos + newVelocity;

                self.bodyChunks[0].pos = newPosition;
                self.bodyChunks[1].pos = newPosition;

                if (inputValx == 0 || NW.dashCooldown <= 0 || NW.maxDashDistance <= 0)
                {
                    NW.Dashed = false;
                }
            }

            if (NW.currentDashes >= 3)
            {
                NW.currentDashes = 0;

                float FlashDelay = 5f;
                NW.dashCooldown = (int)(FlashDelay * 40f);
            }

            if (NW.DoesThatPlayerDashedOrNoBOZO > 0) NW.DoesThatPlayerDashedOrNoBOZO--;

            if (NW.dashCooldown > 0) NW.dashCooldown--;

            if (NW.maxDashDistance > 0) NW.maxDashDistance--;

            if (self.room.game.IsStorySession && self.dead && !self.dead && !self.KarmaIsReinforced && self.Karma == 0)
            {
                self.room.game.rainWorld.progression.WipeSaveState(self.room.game.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat);
                self.room.PlaySound(SoundID.MENU_Enter_Death_Screen, self.mainBodyChunk, false, 1f, 1f);
                self.room.game.GetStorySession.saveState.deathPersistentSaveData.ascended = true;
                self.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Statistics, 5f);
                self.room.game.GetStorySession.saveState.AppendCycleToStatistics(self, self.room.game.GetStorySession, death: true, 0);
                self.room.game.GoToRedsGameOver();
            }

            if (self.redsIllness != null && self.room.game.IsStorySession)
            {
                self.redsIllness.Update();
            }
        }
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

        if (!player.IsNightWalker(out var _)) return;

        self.fade = 0f;
        self.tickCounter = 1;
        self.fade = 0f;
    }

    private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if (!self.IsNightWalker(out var _)) return;

        if (self.room.game.IsStorySession)
        {
            spear.spearDamageBonus = Random.Range(0.6f, 2.4f);
        }

        if (self.room.game.IsArenaSession)
        {
            spear.spearDamageBonus = Random.Range(0.9f, 1.4f);
        }
    }

    private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
    {
        orig(self);

        if (!self.IsNightWalker(out var _)) return;

        float crawlPower = 0.75f;

        self.dynamicRunSpeed[0] += crawlPower;
        self.dynamicRunSpeed[1] += crawlPower;

        if (self.animation == AnimationIndex.BellySlide)
        {

            float vector1 = 7f;
            float vector2 = 13f;

            self.bodyChunks[0].vel.x += (self.longBellySlide ? vector1 : vector2) * self.rollDirection * Mathf.Sin(self.rollCounter / (self.longBellySlide ? 29f : 15f) * (float)Math.PI);
            if (self.IsTileSolid(0, 0, -1) || self.IsTileSolid(0, 0, -2))
            {
                self.bodyChunks[0].vel.y -= 2f;
            }
        }
    }

    private static ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
    {
        orig(self, obj);

        if (self.slugcatStats.name.value == "NightWalker" && obj is Weapon)
        {
            return ObjectGrabability.OneHand;
        }
        return orig(self, obj);
    }

    private static void Player_Jump(On.Player.orig_Jump orig, Player self)
    {
        orig(self);

        if (!self.IsNightWalker(out var _)) return;

        float jumpboost = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

        self.jumpBoost += 0.75f * jumpboost;

        if (self.animation == AnimationIndex.Flip)
        {

            self.bodyChunks[0].vel.y = 10f * jumpboost;
            self.bodyChunks[1].vel.y = 9f * jumpboost;


            self.bodyChunks[0].vel.x = 5f * self.flipDirection * jumpboost;
            self.bodyChunks[1].vel.x = 4f * self.flipDirection * jumpboost;
        }

        if (self.animation == AnimationIndex.RocketJump)
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

        if (!self.IsNightWalker(out var _)) return;

        float num = Mathf.Lerp(0.9f, 1.10f, self.Adrenaline);

        self.bodyChunks[0].vel.y = 10f * num;
        self.bodyChunks[1].vel.y = 9f * num;
        self.bodyChunks[0].vel.x = 8f * num * direction;
        self.bodyChunks[1].vel.x = 6f * num * direction;
    }
}