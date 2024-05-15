﻿namespace Nyctophobia;

public class GeneralHooks
{
    public static WorldCoordinate generalPlayerPos;

    public static Vector2 generalPlayerMainPos;

    public static bool SpawnedBoyKisser;

    public static void Apply()
    {
        On.Player.Update += Player_Update;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        On.Player.ObjectEaten += Player_ObjectEaten;
        On.AbstractCreatureAI.Update += AbstractCreatureAI_Update;
        On.Player.NewRoom += Player_NewRoom;
        On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManager_RequestMainProcessSwitch_ProcessID;

        On.Player.Update += Player_Update1;
    }

    private static void Player_Update1(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        //self.room.physicalObjects
        //    .SelectMany(list => list)
        //    .OfType<Creature>()
        //    .Where(creature => creature != self && (creature.mainBodyChunk.pos - self.mainBodyChunk.pos).magnitude < 100f)
        //    .ToList()
        //    .ForEach(creature =>
        //    {
        //        self.SaintStagger(10);
        //        self.lungsExhausted = true;
        //        self.exhausted = true;
        //    });
    }

    private static void ProcessManager_RequestMainProcessSwitch_ProcessID(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        if (ID == ProcessManager.ProcessID.Game)
        {
            SpawnedBoyKisser = false;
            Plugin.DebugLog("BoyKisser is not spawned");
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
            Plugin.DebugLog("Generating BoyKisser, RUUUNNNNNN");
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

        if (edible is CacaoFruit && self.IsWitness())
        {
            player.cacaoSpeed = 7;
        }

        if (edible is CacaoFruit && !self.IsWitness() && !self.room.game.IsArenaSession)
        {
            player.cacaoSpeed = 5;
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        _ = self.IsPlayer(out ItemData player);

        if (self is null || self.room is null || self.mainBodyChunk == null)
        {
            return;
        }

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
                if ((relationship.type == Relationship.Type.Eats || relationship.type == Relationship.Type.Afraid) && !(playerGraphics.objectLooker.currentMostInteresting as Boykisser).dead)
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
    }
}

//public static void Apply()
//{
//    _ = new Hook(typeof(StoryGameSession).GetProperty(nameof(StoryGameSession.slugPupMaxCount)).GetGetMethod(), StoryGameSession_slugPupMaxCount_get);
//}

//private static int StoryGameSession_slugPupMaxCount_get(Func<StoryGameSession, int> orig, StoryGameSession self)
//{
//    if (self.saveStateNumber == NTEnums.NightWalker)
//    {
//        return 2;
//    }
//    if (self.saveStateNumber == NTEnums.Exile)
//    {
//        return 1;
//    }
//    if (self.saveStateNumber == NTEnums.Witness)
//    {
//        return 0;
//    }
//    return orig(self);
//}