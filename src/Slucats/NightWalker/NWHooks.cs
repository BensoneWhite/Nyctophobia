namespace Witness;

public static class NWHooks
{

    public static ConditionalWeakTable<Player, Whiskerdata> whiskerstorage = new();

    public class Whiskerdata
    {
        public bool ready = false;
        public int initialfacewhiskerloc;
        public string sprite = "LizardScaleA0";
        public string facesprite = "LizardScaleA0";
        public WeakReference<Player> playerref;
        public Whiskerdata(Player player) => playerref = new WeakReference<Player>(player);
        public Vector2[] headpositions = new Vector2[6];
        public Scale[] headScales = new Scale[6];
        public class Scale : BodyPart
        {
            public Scale(GraphicsModule cosmetics) : base(cosmetics)
            {
            }

            public override void Update()
            {
                base.Update();
                if (this.owner.owner.room.PointSubmerged(this.pos))
                {
                    this.vel *= 0.5f;
                }
                else
                {
                    this.vel *= 0.9f;
                }
                this.lastPos = this.pos;
                this.pos += this.vel;
            }
            public float length = 7f;
            public float width = 2f;
        }
        public Color headcolor = new(1f, 1f, 0f);

        public int Facewhiskersprite(int side, int pair) => initialfacewhiskerloc + side + pair + pair;
    }

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
        On.Player.UpdateMSC += Player_UpdateMSC;

        if (!ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
        {

        }

        On.PlayerGraphics.ctor += PlayerGraphics_ctor;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;

        On.PlayerGraphics.ctor += PlayerGraphics_ctor1;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites1;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer1;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites1;
        On.PlayerGraphics.Update += PlayerGraphics_Update1;
    }

    public static void PlayerGraphics_Update1(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
        if (((self.player).slugcatStats.name.value == "NightWalker") && whiskerstorage.TryGetValue(self.player, out Whiskerdata data))
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
                    float num4 = num2 / (float)(data.headScales.Length / 2);
                    if (i == 1)
                    {
                        num = 0f;
                        pos.x += 5f;
                    }
                    else
                    {
                        pos.x -= 5f;
                    }
                    Vector2 a = Custom.rotateVectorDeg(Custom.DegToVec(0f), (float)num3 * num4 - num2 / 2f + num + 90f);
                    float f = Custom.VecToDeg(self.lookDirection);
                    Vector2 vector = Custom.rotateVectorDeg(Custom.DegToVec(0f), (float)num3 * num4 - num2 / 2f + num);
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
    }

    public static void PlayerGraphics_DrawSprites1(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        if (((self.player).slugcatStats.name.value == "NightWalker") && whiskerstorage.TryGetValue(self.player, out Whiskerdata data))
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
                    sLeaser.sprites[data.Facewhiskersprite(i, j)].color = sLeaser.sprites[1].color;
                    index++;
                }
            }
        }
    }

    public static void PlayerGraphics_ctor1(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
    {
        orig(self, ow);

        if ((self.player).slugcatStats.name.value == "NightWalker")
        {
            whiskerstorage.Add(self.player, new Whiskerdata(self.player));
            whiskerstorage.TryGetValue(self.player, out Whiskerdata data);
            for (int i = 0; i < data.headScales.Length; i++)
            {
                data.headScales[i] = new Whiskerdata.Scale(self);
                data.headpositions[i] = new Vector2((i < data.headScales.Length / 2 ? 0.7f : -0.7f), i == 1 ? 0.035f : 0.026f);

            }
        }
    }

    public static void PlayerGraphics_AddToContainer1(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        if (((self.player).slugcatStats.name.value == "NightWalker") && whiskerstorage.TryGetValue(self.player, out Whiskerdata data) && data.ready)
        {
            FContainer container = rCam.ReturnFContainer("Midground");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    FSprite whisker = sLeaser.sprites[data.Facewhiskersprite(i, j)];
                    container.AddChild(whisker);
                }
            }
            data.ready = false;
        }
    }

    public static void PlayerGraphics_InitiateSprites1(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        if ((self.owner as Player).slugcatStats.name.value == "NightWalker")
        {
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
    }

    private static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
    {
        orig(self);

        if (!self.IsNightWalker(out var night))
        {
            return;
        }

        const float normalGravity = 0.9f;
        const float normalAirFriction = 0.999f;
        const float flightGravity = -0.25f;
        const float flightAirFriction = 0.83f;
        const float flightKickinDuration = 6f;

        if (night.CanFly)
        {
            if (self.animation == Player.AnimationIndex.HangFromBeam)
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
                if (night.wingStamina > 0)
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

        night.SetupTailTextureNW();
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