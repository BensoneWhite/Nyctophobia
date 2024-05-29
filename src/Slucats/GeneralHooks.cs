namespace Nyctophobia;

public class GeneralHooks
{
    public static WorldCoordinate generalPlayerPos;

    public static Vector2 generalPlayerMainPos;

    public static bool SpawnedBoyKisser;

    public static void Apply()
    {
        On.PlayerGraphics.CosmeticPearl.Update += (orig, self) =>
        {
            if (!self.pGraphics.player.IsNightWalker() || !self.pGraphics.player.IsWitness() || !self.pGraphics.player.IsExile()) orig(self);
        };

        On.PlayerGraphics.CosmeticPearl.AddToContainer += (orig, self, leaser, cam, contatiner) =>
        {
            if (!self.pGraphics.player.IsNightWalker() || !self.pGraphics.player.IsWitness() || !self.pGraphics.player.IsExile()) orig(self, leaser, cam, contatiner);
        };

        On.PlayerGraphics.CosmeticPearl.InitiateSprites += (orig, self, leaser, cam) =>
        {
            if (!self.pGraphics.player.IsNightWalker() || !self.pGraphics.player.IsWitness() || !self.pGraphics.player.IsExile()) orig(self, leaser, cam);
        };

        On.PlayerGraphics.CosmeticPearl.DrawSprites += (orig, self, leaser, cam, stacker, pos) =>
        {
            if (!self.pGraphics.player.IsNightWalker() || !self.pGraphics.player.IsWitness() || !self.pGraphics.player.IsExile()) orig(self, leaser, cam, stacker, pos);
        };

        On.PlayerGraphics.CosmeticPearl.ApplyPalette += (orig, self, leaser, cam, palette) =>
        {
            if (!self.pGraphics.player.IsNightWalker() || !self.pGraphics.player.IsWitness() || !self.pGraphics.player.IsExile()) orig(self, leaser, cam, palette);
        };

        IL.Player.SlugcatGrab += Player_SlugcatGrab;
        On.Player.Update += Player_Update;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        On.Player.ObjectEaten += Player_ObjectEaten;
        On.AbstractCreatureAI.Update += AbstractCreatureAI_Update;
        On.Player.NewRoom += Player_NewRoom;
        On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManager_RequestMainProcessSwitch_ProcessID;

        _ = new Hook(typeof(StoryGameSession).GetProperty(nameof(StoryGameSession.slugPupMaxCount))!.GetGetMethod(), StoryGameSession_slugPupMaxCount_get);
    }

    private static void Player_SlugcatGrab(ILContext il)
    {
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.After,
            x => x.MatchLdarg(0),
            x => x.MatchCall<Player>("get_isSlugpup"));
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate((bool isSlugPup, Player player) =>
        {
            return isSlugPup && !player.IsNightWalker();
        });
    }

    private static int StoryGameSession_slugPupMaxCount_get(Func<StoryGameSession, int> orig, StoryGameSession self)
    {
        if (self.saveStateNumber == NTEnums.NightWalker)
        {
            return 3;
        }
        if (self.saveStateNumber == NTEnums.Exile)
        {
            return 1;
        }
        if (self.saveStateNumber == NTEnums.Witness)
        {
            return 0;
        }
        return orig(self);
    }

    private static void ProcessManager_RequestMainProcessSwitch_ProcessID(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        if (ID == ProcessManager.ProcessID.Game)
        {
            SpawnedBoyKisser = false;
        }
        orig(self, ID);
    }

    private static void Player_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
    {
        orig(self, newRoom);

        if (self.room != null &&
           !self.room.world.game.IsArenaSession &&
           !SpawnedBoyKisser &&
           !NTOptionsMenu.Boykisser.Value &&
           !newRoom.abstractRoom.gate &&
           !newRoom.abstractRoom.shelter &&
           !newRoom.abstractRoom.isAncientShelter &&
           self.room.game.GetStorySession.saveState.cycleNumber != 0 &&
           Random.value <= (1f / 150000) &&
           self.room.game.world.rainCycle.timer > ((self.room.game.GetStorySession.saveState.cycleNumber == 0) ? 2000f : 1000f))
        {
            Room val = self.room.world.activeRooms[Random.Range(0, self.room.world.activeRooms.Count)];
            int num = Random.Range(0, val.Width);
            int num2 = Random.Range(0, val.TileHeight);
            if (val.GetTile(num, num2).Terrain == 0 && !RayTraceTilesForTerrain(val, new IntVector2(num, num2), new IntVector2(num, num2 - 1000)))
            {
                AbstractCreature val2 = new(self.room.world, StaticWorld.GetCreatureTemplate(NTEnums.CreatureType.BoyKisser), null, val.GetWorldCoordinate(new IntVector2(num, num2)), self.room.game.GetNewID());
                val.abstractRoom.AddEntity(val2);
                val2.RealizeInRoom();
                SpawnedBoyKisser = true;
            }
        }
    }

    private static void AbstractCreatureAI_Update(On.AbstractCreatureAI.orig_Update orig, AbstractCreatureAI self, int time)
    {
        try
        {
            if (self is not null && self.parent.creatureTemplate.type == NTEnums.CreatureType.BoyKisser && self.world.game.Players.Count > 0)
            {
                self.followCreature = self.world.game.Players[0];
            }

            if (self.parent.Room != null && self.parent.Room.realizedRoom != null && self.parent.Room.realizedRoom.regionGate == null && self.parent.creatureTemplate.type == NTEnums.CreatureType.BoyKisser)
            {
                self.SetDestination(generalPlayerPos);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        orig(self, time);
    }

    private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
    {
        orig(self);

        _ = self.IsPlayer(out ItemData player);

        if (self is null || self.room is null)
        {
            return;
        }

        player.cacaoSpeed = Custom.LerpAndTick(player.cacaoSpeed, 0f, 0.001f, 0.0001f);

        self.dynamicRunSpeed[0] += player.cacaoSpeed;
        self.dynamicRunSpeed[1] += player.cacaoSpeed;

        player.power = player.DangerNum > 10f ? Custom.LerpAndTick(player.power, 5f, 0.1f, 0.03f) : Custom.LerpAndTick(player.power, 0f, 0.01f, 0.3f);

        self.dynamicRunSpeed[0] += player.power;
        self.dynamicRunSpeed[1] += player.power;
    }

    private static void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
    {
        orig(self, edible);

        _ = self.IsPlayer(out ItemData player);

        if (ModManager.ActiveMods.Any(mod => mod.id == "willowwisp.bellyplus") && edible is CacaoFruit)
        {
            self.AddFood(5);
            _ = self.room.PlaySound(SoundID.Death_Lightning_Spark_Object, self.mainBodyChunk, false, 1f, 1f);
            player.cacaoSpeed = 10;
        }

        if (edible is CacaoFruit && self.IsWitness() && !self.room.game.IsArenaSession)
        {
            player.cacaoSpeed = 7;
        }
        else
        {
            player.cacaoSpeed = 5;
        }

        if (edible is CacaoFruit && !self.IsWitness() && !self.room.game.IsArenaSession)
        {
            player.cacaoSpeed = 5;
        }
        else
        {
            player.cacaoSpeed = 3;
        }

        if (edible is BloodyFlower && (!self.room.game.IsArenaSession || self.room.game.rainWorld.ExpeditionMode) && self.Karma != 10)
        {
            (self.abstractCreature.world.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma += 1;
        }
        else
        {
            player.cacaoSpeed = 3;
            // death immune
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        _ = self.IsPlayer(out ItemData player);

        if (self is null || self.room is null || self.mainBodyChunk == null) return;

        try
        {
            PlayerGraphics playerGraphics = self.graphicsModule as PlayerGraphics;

            if (ModManager.JollyCoop && self.mainBodyChunk != null && (self.room.game.Players.Count <= 1 || !self.dead))
            {
                player.playerPos = self.room.GetWorldCoordinate(self.mainBodyChunk.pos);
                generalPlayerPos = player.playerPos;
                player.playerMainPos = self.mainBodyChunk.pos;
                generalPlayerMainPos = player.playerMainPos;
                player.distanceToPlayer = Vector2.Distance(player.playerMainPos, Boykisser.boykisserPos);
            }
            else
            {
                player.playerPos = self.room.GetWorldCoordinate(self.mainBodyChunk.pos);
                generalPlayerPos = player.playerPos;
                player.playerMainPos = self.mainBodyChunk.pos;
                generalPlayerMainPos = player.playerMainPos;
                player.distanceToPlayer = Vector2.Distance(player.playerMainPos, Boykisser.boykisserPos);
            }

            if (self != null && self.room != null && !self.room.game.paused && self.Consious && playerGraphics.objectLooker.currentMostInteresting != null && playerGraphics.objectLooker.currentMostInteresting is Boykisser boykisser && boykisser != null)
            {
                Relationship relationship = self.abstractCreature.creatureTemplate.CreatureRelationship((playerGraphics.objectLooker.currentMostInteresting as Boykisser).abstractCreature.creatureTemplate);
                if ((relationship.type == Eats || relationship.type == Afraid) && !(playerGraphics.objectLooker.currentMostInteresting as Boykisser).dead)
                {
                    player.afraid = Mathf.InverseLerp(Mathf.Lerp(40f, 250f, relationship.intensity), 10f, Vector2.Distance(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint) * (self.room.VisualContact(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint) ? 1f : 1.5f));
                }
            }
            else
            {
                player.afraid = Custom.LerpAndTick(player.afraid, 0f, 0.001f, 0.3f);
            }

            player.DangerNum = player.afraid > 0
                ? Custom.LerpAndTick(player.DangerNum, 100f, 0.01f, 0.03f)
                : Custom.LerpAndTick(player.DangerNum, 0f, 0.001f, 0.3f);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        try
        {
            self.room.physicalObjects
                .SelectMany(list => list)
                .OfType<Creature>()
                .Where(creature => creature != self && (creature.mainBodyChunk.pos - self.mainBodyChunk.pos).magnitude < 100f && creature is Boykisser)
                .ToList()
                .ForEach(creature =>
                {
                    self.room.AddObject(new RedsIllness.RedsIllnessEffect(self.redsIllness, self.room));
                });
        }
        catch (Exception ex)
        {
            Plugin.DebugError(ex);
            Debug.LogError(ex);
        }
    }
}

//self.objectLooker.LookAtPoint(new Vector2(((UpdatableAndDeletable)self.player).room.PixelWidth * Random.value, ((UpdatableAndDeletable)self.player).room.PixelHeight + 100f), (1f - ((UpdatableAndDeletable)self.player).room.world.rainCycle.RainApproaching) * 0.6f);