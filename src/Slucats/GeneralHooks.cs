namespace Nyctophobia;

public class GeneralHooks
{
    public static ConditionalWeakTable<Player, ItemData> ItemData = new();

    public static WorldCoordinate generalPlayerPos;

    public static Vector2 generalPlayerMainPos;

    public static void Apply()
    {
        On.Player.ctor += Player_ctor;
        On.Player.Update += Player_Update;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        On.Player.ObjectEaten += Player_ObjectEaten;
        On.AbstractCreatureAI.Update += AbstractCreatureAI_Update;
        On.Player.NewRoom += Player_NewRoom;
    }

    private static void Player_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
    {
        orig(self, newRoom);

        if(!newRoom.abstractRoom.gate && !newRoom.abstractRoom.shelter && !newRoom.abstractRoom.isAncientShelter && !(self.room.game.GetStorySession.saveState.cycleNumber == 0) && Random.value <= (1f / 1500))
        {
            Room val = self.room.world.activeRooms[Random.Range(0, self.room.world.activeRooms.Count)];
            int num = Random.Range(0, val.Width);
            int num2 = Random.Range(0, val.TileHeight);
            if (val.GetTile(num, num2).Terrain == 0 && !SharedPhysics.RayTraceTilesForTerrain(val, new IntVector2(num, num2), new IntVector2(num, num2 - 1000)))
            {
                AbstractCreature val2 = new(self.room.world, StaticWorld.GetCreatureTemplate(NTEnums.CreatureType.BoyKisser), null, val.GetWorldCoordinate(new IntVector2(num, num2)), self.room.game.GetNewID());
                val.abstractRoom.AddEntity(val2);
                val2.RealizeInRoom();
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

        if (!ItemData.TryGetValue(self, out var player)) return;

        if (self is null || self.room is null) return;

        player.cacaoSpeed = Custom.LerpAndTick(player.cacaoSpeed, 0f, 0.001f, 0.0001f);

        self.dynamicRunSpeed[0] += player.cacaoSpeed;

        if (player.DangerNum > 10f) player.power = Custom.LerpAndTick(player.power, 5f, 0.1f, 0.03f);

        else player.power = Custom.LerpAndTick(player.power, 0f, 0.01f, 0.3f);

        self.dynamicRunSpeed[0] += player.power;
        self.dynamicRunSpeed[1] += player.power;
    }

    private static void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
    {
        orig(self, edible);

        if (!ItemData.TryGetValue(self, out var player)) return;

        if (ModManager.ActiveMods.Any(mod => mod.id == "willowwisp.bellyplus") && edible is CacaoFruit)
        {
            self.AddFood(5);
            self.room.PlaySound(SoundID.Death_Lightning_Spark_Object, self.mainBodyChunk, false, 1f, 1f);
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
        else
        {
            player.cacaoSpeed = 5;
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!ItemData.TryGetValue(self, out var player)) return;

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

            if (self.Consious && playerGraphics.objectLooker.currentMostInteresting != null && playerGraphics.objectLooker.currentMostInteresting is Boykisser)
            {
                CreatureTemplate.Relationship relationship = self.abstractCreature.creatureTemplate.CreatureRelationship((playerGraphics.objectLooker.currentMostInteresting as Boykisser).abstractCreature.creatureTemplate);
                if ((relationship.type == CreatureTemplate.Relationship.Type.Eats || relationship.type == CreatureTemplate.Relationship.Type.Afraid) && !(playerGraphics.objectLooker.currentMostInteresting as Boykisser).dead)
                {
                    player.afraid = Mathf.InverseLerp(Mathf.Lerp(40f, 250f, relationship.intensity), 10f, Vector2.Distance(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint) * (self.room.VisualContact(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint) ? 1f : 1.5f));
                }
            }
            else player.afraid = Custom.LerpAndTick(player.afraid, 0f, 0.001f, 0.3f);

            if (player.afraid > 0) player.DangerNum = Custom.LerpAndTick(player.DangerNum, 100f, 0.01f, 0.03f);

            else player.DangerNum = Custom.LerpAndTick(player.DangerNum, 0f, 0.001f, 0.3f);

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        ItemData.Add(self, new ItemData(self));
    }
}
