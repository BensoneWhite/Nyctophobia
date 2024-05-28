namespace Nyctophobia;

public class WSHooks
{
    public static void Init()
    {
        On.Player.ctor += Player_ctor;
        On.Player.ThrownSpear += Player_ThrownSpear;
        On.Player.Die += Player_Die;
        On.Player.Jump += Player_Jump;
        On.Player.Update += Player_Update;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        On.Player.UpdateMSC += Player_UpdateMSC;
        On.Player.ObjectEaten += Player_ObjectEaten;

        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.ctor += PlayerGraphics_ctor;

        On.Player.SpitUpCraftedObject += Player_SpitUpCraftedObject;
        On.Player.SwallowObject += Player_SwallowObject;
    }

    private static void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
    {
        if (!self.IsWitness(out _))
        {
            orig(self, grasp);
            return;
        }

        if (grasp < 0 || self.grasps[grasp] == null)
        {
            return;
        }

        AbstractPhysicalObject abstractPhysicalObject2 = self.grasps[grasp].grabbed.abstractPhysicalObject;
        if (abstractPhysicalObject2 is AbstractSpear)
        {
            (abstractPhysicalObject2 as AbstractSpear).stuckInWallCycles = 0;
        }
        self.objectInStomach = abstractPhysicalObject2;
        if (ModManager.MMF && self.room.game.session is StoryGameSession)
        {
            (self.room.game.session as StoryGameSession).RemovePersistentTracker(self.objectInStomach);
        }
        self.ReleaseGrasp(grasp);
        self.objectInStomach.realizedObject.RemoveFromRoom();
        self.objectInStomach.Abstractize(self.abstractCreature.pos);
        self.objectInStomach.Room.RemoveEntity(self.objectInStomach);

        if (abstractPhysicalObject2.type == AbstractObjectType.FlareBomb && self.FoodInStomach >= 3)
        {
            abstractPhysicalObject2 = new BlueLanternAbstract(self.room.world, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID());
            self.SubtractFood(3);
        }
        if (abstractPhysicalObject2.type == AbstractObjectType.ScavengerBomb && self.FoodInStomach >= 3)
        {
            abstractPhysicalObject2 = new BlueBombaAbstract(self.room.world, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID());
            self.SubtractFood(3);
        }
        if (abstractPhysicalObject2.type == NTEnums.AbstractObjectTypes.RedFlareBomb && self.FoodInStomach >= 2)
        {
            abstractPhysicalObject2 = new AbstractPhysicalObject(self.room.world, AbstractObjectType.ScavengerBomb, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID());
            self.SubtractFood(2);
        }
        if (abstractPhysicalObject2.type == AbstractObjectType.Spear && !(abstractPhysicalObject2 as BlueSpearAbstract).explosive && !(abstractPhysicalObject2 as AbstractSpear).electric && self.FoodInStomach >= 3)
        {
            abstractPhysicalObject2 = new BlueSpearAbstract(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), true, 0f);
            self.SubtractFood(3);
        }

        self.objectInStomach = abstractPhysicalObject2;
        self.objectInStomach.Abstractize(self.abstractCreature.pos);
        self.mainBodyChunk.vel.y += 2f;
        self.room.PlaySound(SoundID.Slugcat_Swallow_Item, self.mainBodyChunk);
    }

    private static void Player_SpitUpCraftedObject(On.Player.orig_SpitUpCraftedObject orig, Player self)
    {
        orig(self);

        if (!self.IsWitness(out _))
        {
            return;
        }

        for (int i = 0; i < self.grasps.Length; i++)
        {
            if (self.grasps[i] == null) continue;

            AbstractPhysicalObject abstractPhysicalObject = self.grasps[i].grabbed.abstractPhysicalObject;
            if (!(abstractPhysicalObject.type == AbstractObjectType.Spear) || (abstractPhysicalObject as BlueSpearAbstract).explosive)
            {
                continue;
            }

            if ((abstractPhysicalObject as AbstractSpear).electric && (abstractPhysicalObject as AbstractSpear).electricCharge > 0)
            {
                self.room.AddObject(new ZapCoil.ZapFlash(self.firstChunk.pos, 10f));
                self.room.PlaySound(SoundID.Zapper_Zap, self.firstChunk.pos, 1f, 1.5f + Random.value * 1.5f);
                if (self.Submersion > 0.5f)
                {
                    self.room.AddObject(new UnderwaterShock(self.room, null, self.firstChunk.pos, 10, 800f, 2f, self, new Color(1f, .3f, .3f)));
                }
                self.Stun(400);
                self.room.AddObject(new CreatureSpasmer(self, allowDead: false, 200));
                (abstractPhysicalObject as AbstractSpear).electricCharge = 0;
                return;
            }

            self.ReleaseGrasp(i);

            abstractPhysicalObject.realizedObject.RemoveFromRoom();
            self.room.abstractRoom.RemoveEntity(abstractPhysicalObject);

            BlueSpearAbstract spearAbstract = new(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), true, 0f);
            self.room.abstractRoom.AddEntity(spearAbstract);
            if (self.FreeHand() != -1)
            {
                self.SlugcatGrab(spearAbstract.realizedObject, self.FreeHand());
            }
            return;
        }
    }

    private static void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
    {
        orig(self, edible);

        if (!self.IsWitness(out _))
        {
            return;
        }

        if (Random.value < 0.01f)
        {
            _ = self.room.PlaySound(NTEnums.Sound.wawaWit, self.mainBodyChunk, false, 0.5f, 1f);
        }
        if (edible is CacaoFruit && Random.value < 0.15f)
        {
            _ = self.room.PlaySound(NTEnums.Sound.wawaWit, self.mainBodyChunk, false, 0.5f, 1f);
        }
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        if (!self.IsWitness(out WSPlayerData WS))
        {
            return;
        }

        if (WS.IsWitness && self.myRobot == null && self.room != null && self.room.game.session is StoryGameSession)
        {
            self.myRobot = new AncientBot(self.mainBodyChunk.pos, new Color(Random.Range(0.5f, 1f), 0f, Random.Range(0f, 0.2f)), self, online: true);
            self.room.AddObject(self.myRobot);
        }
    }

    private static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
    {
        orig(self);
        if (!self.IsWitness(out WSPlayerData WS))
        {
            return;
        }

        if (WS.IsWitness && self.myRobot == null && self.room != null && self.room.game.session is StoryGameSession)
        {
            self.myRobot = new AncientBot(self.mainBodyChunk.pos, new Color(Random.Range(0.5f, 1f), 0f, Random.Range(0f, 0.2f)), self, online: true);
            self.room.AddObject(self.myRobot);
        }
    }

    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        if (!self.player.IsWitness(out WSPlayerData _))
        {
            return;
        }

        sLeaser.sprites[2].MoveBehindOtherNode(sLeaser.sprites[1]);
    }

    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        if (!self.player.IsWitness(out WSPlayerData ws))
        {
            return;
        }

        if (sLeaser.sprites[2] is TriangleMesh tail && ws.TailAtlas.elements != null && ws.TailAtlas.elements.Count > 0)
        {
            tail.element = ws.TailAtlas.elements[0];

            for (int i = tail.vertices.Length - 1; i >= 0; i--)
            {
                float perc = i / 2 / (float)(tail.vertices.Length / 2);

                Vector2 uv = i % 2 == 0 ? new Vector2(perc, 0f) : i < tail.vertices.Length - 1 ? new Vector2(perc, 1f) : new Vector2(1f, 0f);
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
        if (!self.player.IsWitness(out WSPlayerData ws))
        {
            return;
        }

        ws.SetupTailTextureWS();
        ws.SetupColorsWS(self);
    }

    private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
    {
        orig(self);

        if (!self.IsWitness(out WSPlayerData witness))
        {
            return;
        }

        witness.power = witness.DangerNum > 10f
            ? Custom.LerpAndTick(witness.power, 5f, 0.1f, 0.03f)
            : Custom.LerpAndTick(witness.power, 0f, 0.01f, 0.3f);

        if (self.SlugCatClass.value == "Witness")
        {
            self.dynamicRunSpeed[0] += witness.power;
            self.dynamicRunSpeed[1] += witness.power;
        }
    }

    private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if (!self.IsWitness(out WSPlayerData _))
        {
            return;
        }

        if (self.room.game.IsStorySession)
        {
            spear.spearDamageBonus = Random.Range(1f, 1.2f);
        }

        if (self.room.game.IsArenaSession)
        {
            spear.spearDamageBonus = Random.Range(0.6f, 1f);
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.IsWitness(out WSPlayerData witness))
        {
            return;
        }

        if (!self.dead && self.room is not null)
        {
            PlayerGraphics playerGraphics = self.graphicsModule as PlayerGraphics;
            //string roomname = self.room.abstractRoom.name;
            self.setPupStatus(true);
            float afraid = 0f;

            //if (self.room.game.IsStorySession)
            //{
            //    if (roomname == "DD_A05" && self.room.game.GetStorySession.saveState.cycleNumber == 0 && !witness.HasSeenFirtsTutorial)
            //    {
            //        self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.rainWorld.inGameTranslator.Translate("You are hungry, look for food."), 48, 120, true, true);

            //        witness.HasSeenFirtsTutorial = true;
            //    }
            //}

            if (self.input[0].thrw && self.grasps[0] != null && self.grasps[0].grabbed is Creature && self.FoodInStomach >= 3)
            {
                self.SubtractFood(3);
                Creature shockObject = self.grasps[0].grabbed as Creature;
                for (int i = 0; i < (int)Mathf.Lerp(4f, 8f, 6f); i++)
                {
                    shockObject.room.AddObject(new Spark(shockObject.bodyChunks[0].pos, Custom.RNV() * Mathf.Lerp(4f, 14f, Random.value), new Color(1f, 0.7f, 0f), null, 8, 14));
                }
                shockObject.Violence(shockObject.mainBodyChunk, new Vector2?(new Vector2(0f, 0f)), shockObject.mainBodyChunk, null, Creature.DamageType.Electric, 2f, 200f);

                shockObject.room.AddObject(new CreatureSpasmer(shockObject, false, shockObject.stun));
                self.room.PlaySound(SoundID.Centipede_Shock, self.bodyChunks[0].pos, 1f, 1f);
                shockObject.grasps[0].Release();
            }

            if (self.input[0].thrw && self.grabbedBy.Count > 0 && !self.dead && self.FoodInStomach >= 3)
            {
                self.SubtractFood(3);
                Creature shockObject = self.grabbedBy[0].grabber;
                for (int i = 0; i < (int)Mathf.Lerp(4f, 8f, 6f); i++)
                {
                    self.room.AddObject(new Spark(self.bodyChunks[0].pos, Custom.RNV() * Mathf.Lerp(4f, 14f, Random.value), new Color(1f, 0.7f, 0f), null, 8, 14));
                }
                shockObject.Violence(shockObject.mainBodyChunk, new Vector2?(new Vector2(0f, 0f)), shockObject.mainBodyChunk, null, Creature.DamageType.Electric, 2f, 200f);

                self.room.AddObject(new CreatureSpasmer(shockObject, false, shockObject.stun));
                self.room.PlaySound(SoundID.Centipede_Shock, self.bodyChunks[0].pos, 1f, 1f);

                shockObject.LoseAllGrasps();
                if (shockObject.Submersion > 0f)
                {
                    self.room.AddObject(new UnderwaterShock(self.room, self, self.bodyChunks[0].pos, 14, Mathf.Lerp(ModManager.MMF ? 0f : 200f, 1200f, 6f), 0.2f + (1.9f * 6f), self, new Color(0.7f, 0.7f, 1f)));
                }
            }

            if (self.input[0].jmp && !self.input[1].jmp && self.input[0].pckp && witness.FlashCooldown <= 0)
            {
                if (Random.value < 0.5f)
                {
                    AbstractConsumable abstractFlareBomb = new(self.room.world, AbstractObjectType.FlareBomb, null, self.coord, self.room.game.GetNewID(), -1, -1, null);
                    self.room.abstractRoom.AddEntity(abstractFlareBomb);
                    abstractFlareBomb.RealizeInRoom();

                    FlareBomb flareBomb = (FlareBomb)abstractFlareBomb.realizedObject;
                    flareBomb.firstChunk.HardSetPosition(self.bodyChunks[0].pos);
                    flareBomb.StartBurn();
                    flareBomb.burning += 0.3f;
                }
                else
                {
                    AbstractConsumable abstractFlareBomb = new(self.room.world, NTEnums.AbstractObjectTypes.RedFlareBomb, null, self.coord, self.room.game.GetNewID(), -1, -1, null);
                    self.room.abstractRoom.AddEntity(abstractFlareBomb);
                    abstractFlareBomb.RealizeInRoom();

                    RedFlareBomb flareBomb = new(abstractFlareBomb, self.room.world);
                    flareBomb.abstractPhysicalObject.RealizeInRoom();
                    flareBomb.firstChunk.HardSetPosition(self.bodyChunks[0].pos);
                    flareBomb.StartBurn();
                    flareBomb.burning += 0.3f;
                }
                float FlashDelay = 10f;
                witness.FlashCooldown = (int)(FlashDelay * 40f);
            }

            if (witness.FlashCooldown is > 0 and not 0)
            {
                witness.FlashCooldown--;
            }

            if (self.Consious && playerGraphics.objectLooker.currentMostInteresting != null && playerGraphics.objectLooker.currentMostInteresting is Creature)
            {
                Relationship relationship = self.abstractCreature.creatureTemplate.CreatureRelationship((playerGraphics.objectLooker.currentMostInteresting as Creature).abstractCreature.creatureTemplate);
                if ((relationship.type == Eats || relationship.type == Afraid) && !(playerGraphics.objectLooker.currentMostInteresting as Creature).dead)
                {
                    afraid = Mathf.InverseLerp(Mathf.Lerp(40f, 250f, relationship.intensity), 10f, Vector2.Distance(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint) * (self.room.VisualContact(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint) ? 1f : 1.5f));
                }
            }

            witness.DangerNum = afraid > 0
                ? Custom.LerpAndTick(witness.DangerNum, 100f, 0.01f, 0.03f)
                : Custom.LerpAndTick(witness.DangerNum, 0f, 0.001f, 0.3f);
        }
    }

    private static void Player_Die(On.Player.orig_Die orig, Player self)
    {
        orig(self);

        if (!self.IsWitness(out WSPlayerData _))
        {
            return;
        }

        bool wasDead = self.dead;
        Room room = self.room;
        Vector2 pos = self.mainBodyChunk.pos;

        if (!wasDead && self.dead && self.room is not null)
        {
            Vector2 vector = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
            room.AddObject(new SparkFlash(self.firstChunk.pos, 700f, new Color(0f, 0f, 1f)));
            if (self is Creature creature && creature is not Player)
            {
                room.AddObject(new Explosion(room, self, vector, 7, 4500f, 6.2f, 100f, 280f, 0.25f, self, 0f, 160f, 1f));
                room.AddObject(new Explosion(room, self, vector, 7, 20000f, 4f, 100f, 400f, 0.25f, self, 0.3f, 200f, 1f));
            }
            room.AddObject(new Explosion.ExplosionLight(vector, 280f, 1f, 7, new Color(1f, 1f, 1f)));
            room.AddObject(new Explosion.ExplosionLight(vector, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room.AddObject(new Explosion.ExplosionLight(vector, 2000f, 2f, 60, new Color(1f, 1f, 1f)));
            room.AddObject(new ShockWave(vector, 750f, 0.485f, 300, highLayer: true));
            room.AddObject(new ShockWave(vector, 5000f, 0.185f, 180));

            room.ScreenMovement(pos, default, 2.3f);
            room.PlaySound(SoundID.Bomb_Explode, pos);
            room.InGameNoise(new InGameNoise(pos, 18000f, self, 10f));
        }
    }

    private static void Player_Jump(On.Player.orig_Jump orig, Player self)
    {
        orig(self);

        if (!self.IsWitness(out WSPlayerData _))
        {
            return;
        }

        float jumpPower = 0.25f;

        self.jumpBoost *= 1f + jumpPower;
    }
}