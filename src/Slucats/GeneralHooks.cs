using Sony.NP;

namespace Nyctophobia;

public class GeneralHooks
{
    public static WorldCoordinate GeneralPlayerPos;
    public static Vector2 GeneralPlayerMainPos;
    public static bool SpawnedBoyKisser;
    public static bool DroneCrafting;
    public static Player Player;

    public static void Apply()
    {
        On.PlayerGraphics.CosmeticPearl.Update += CosmeticPearl_Update_Hook;
        On.PlayerGraphics.CosmeticPearl.AddToContainer += CosmeticPearl_AddToContainer_Hook;
        On.PlayerGraphics.CosmeticPearl.InitiateSprites += CosmeticPearl_InitiateSprites_Hook;
        On.PlayerGraphics.CosmeticPearl.DrawSprites += CosmeticPearl_DrawSprites_Hook;
        On.PlayerGraphics.CosmeticPearl.ApplyPalette += CosmeticPearl_ApplyPalette_Hook;

        IL.Player.SlugcatGrab += Player_SlugcatGrab_ILHook;
        IL.Player.Collide += Player_Collide;
        On.Player.HeavyCarry += Player_HeavyCarry;
        On.Player.CanIPickThisUp += Player_CanIPickThisUp;

        On.Player.Update += Player_Update;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        On.AbstractCreatureAI.Update += AbstractCreatureAI_Update;
        On.Player.NewRoom += Player_NewRoom;
        On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManager_RequestMainProcessSwitch_ProcessID;
        On.AbstractCreatureAI.AbstractBehavior += AbstractCreatureAI_AbstractBehavior;
        On.ArtificialIntelligence.Update += ArtificialIntelligence_Update;
        On.Player.ThrownSpear += Player_ThrownSpear;

        // Hook property getter for slugPupMaxCount.
        var slugPupMaxCountGetter = typeof(StoryGameSession)
            .GetProperty(nameof(StoryGameSession.slugPupMaxCount))
            ?.GetGetMethod();
        if (slugPupMaxCountGetter != null)
        {
            _ = new Hook(slugPupMaxCountGetter, StoryGameSession_slugPupMaxCount_get);
        }

        // Attempt to get the "MainColor" property from the OverseerGraphics class using reflection.
        var mainColorProperty = typeof(OverseerGraphics).GetProperty("MainColor", BindingFlags.Instance | BindingFlags.Public) 
            ?? throw new Exception("MainColor property not found on OverseerGraphics!");

        // Retrieve the getter method (the 'get' accessor) for the MainColor property.
        var getMainColorMethod = mainColorProperty.GetGetMethod() 
            ?? throw new Exception("Getter for MainColor not found!");

        // Locate our custom hook method in the Plugin class.
        //The typeof "class" can be changed to the current class where the "OverseerGraphics_MainColor_get" method is located
        var hookMethod = typeof(Plugin).GetMethod("OverseerGraphics_MainColor_get", BindingFlags.Static | BindingFlags.Public) 
            ?? throw new Exception("Hook method OverseerGraphics_MainColor_get not found!");

        // Create a new hook that replaces the original getter of the MainColor property
        new Hook(getMainColorMethod, hookMethod);
    }

    // Define a delegate matching the signature of the original MainColor getter.
    public delegate Color orig_MainColor(OverseerGraphics self);

    // Custom hook method for the MainColor getter.
    // It receives two parameters:
    // 1. 'orig' is a delegate to the original getter method.
    // 2. 'self' is the instance of OverseerGraphics on which the getter was called.
    public static Color OverseerGraphics_MainColor_get(orig_MainColor orig, OverseerGraphics self)
    {
        // Check if the current region name is "SU" and the Story character is NightWalker.
        if (self.OwnerRoom.world.region.name == "DD" && self.owner.room.world.game.StoryCharacter == NTEnums.NightWalker)
        {
            // If the region is "SU" and StoryCharacter is NightWalker, return a custom color.
            return new Color(1f, 0.28f, 0.46f);
        }
        // Otherwise, call the original getter method to return the default color.
        return orig(self);
    }

    private static bool Player_CanIPickThisUp(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
    {
        if (!self.IsNightWalker())
        {
            return orig(self, obj);
        }
        if (obj is Player player && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Slugpup)
        {
            return true;
        }

        return orig(self, obj);
    }

    private static bool Player_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj)
    {
        if (!self.IsNightWalker())
        {
            return orig(self, obj);
        }
        if (obj.TotalMass <= self.TotalMass * 3f)
        {
            if (ModManager.CoopAvailable && obj is Player player && player != null)
            {
                return !player.isSlugpup;
            }
            return false;
        }

        return orig(self, obj);
    }

    /// <summary>
    /// This avoids a crash when meeting Moon in the Spearmaster campaign.
    /// </summary>
    #region CosmeticPearl Hooks

    private static void CosmeticPearl_Update_Hook(On.PlayerGraphics.CosmeticPearl.orig_Update orig, PlayerGraphics.CosmeticPearl self)
    {
        if (ShouldProcessCosmeticPearl(self))
            orig(self);
    }

    private static void CosmeticPearl_AddToContainer_Hook(On.PlayerGraphics.CosmeticPearl.orig_AddToContainer orig, PlayerGraphics.CosmeticPearl self, RoomCamera.SpriteLeaser leaser, RoomCamera cam, FContainer container)
    {
        if (ShouldProcessCosmeticPearl(self))
            orig(self, leaser, cam, container);
    }

    private static void CosmeticPearl_InitiateSprites_Hook(On.PlayerGraphics.CosmeticPearl.orig_InitiateSprites orig, PlayerGraphics.CosmeticPearl self, RoomCamera.SpriteLeaser leaser, RoomCamera cam)
    {
        if (ShouldProcessCosmeticPearl(self))
            orig(self, leaser, cam);
    }

    private static void CosmeticPearl_DrawSprites_Hook(On.PlayerGraphics.CosmeticPearl.orig_DrawSprites orig, PlayerGraphics.CosmeticPearl self, RoomCamera.SpriteLeaser leaser, RoomCamera cam, float stacker, Vector2 pos)
    {
        if (ShouldProcessCosmeticPearl(self))
            orig(self, leaser, cam, stacker, pos);
    }

    private static void CosmeticPearl_ApplyPalette_Hook(On.PlayerGraphics.CosmeticPearl.orig_ApplyPalette orig, PlayerGraphics.CosmeticPearl self, RoomCamera.SpriteLeaser leaser, RoomCamera cam, RoomPalette palette)
    {
        if (ShouldProcessCosmeticPearl(self))
            orig(self, leaser, cam, palette);
    }

    private static bool ShouldProcessCosmeticPearl(PlayerGraphics.CosmeticPearl cosmeticPearl)
    {
        var player = cosmeticPearl.pGraphics.player;
        return !player.IsNightWalker() || !player.IsWitness() || !player.IsExile();
    }

    #endregion

    #region IL Hooks

    private static void Player_SlugcatGrab_ILHook(ILContext il)
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

    private static void Player_Collide(ILContext il)
    {
        ILCursor cursor = new(il);

        while (cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdarg(0),
            x => x.MatchCall<Player>("get_isSlugpup")))
        {
            cursor.Emit(OpCodes.Ldarg_0);

            cursor.EmitDelegate<Func<bool, Player, bool>>((isSlugPup, player) =>
            {
                return isSlugPup && !player.IsNightWalker();
            });
        }
    }

    #endregion

    #region Player Hooks

    private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);
        if (self.room == null)
            return;

        self.IsPlayer(out var playerData);
        if (playerData != null && playerData.Berserker)
        {
            spear.spearDamageBonus += 1f;
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.IsPlayer(out GeneralPlayerData player))
            return;
        if (self.room == null || self.mainBodyChunk == null)
            return;

        // Update global references.
        FlashWigHooks.Player = self;
        Player = self;

        try
        {
            UpdatePlayerPositions(self, player);

            // Process fear if a BoyKisser is the most interesting object.
            if (!self.room.game.paused && self.Consious &&
                self.graphicsModule is PlayerGraphics playerGraphics &&
                playerGraphics.objectLooker.currentMostInteresting is Boykisser boykisser && !boykisser.dead)
            {
                Relationship relationship = self.abstractCreature.creatureTemplate.CreatureRelationship(boykisser.abstractCreature.creatureTemplate);
                // Assumes "Eats" and "Afraid" are defined constants or enum members.
                if ((relationship.type == Eats || relationship.type == Afraid))
                {
                    float lookDistance = Vector2.Distance(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint);
                    float contactMultiplier = self.room.VisualContact(self.mainBodyChunk.pos, playerGraphics.objectLooker.mostInterestingLookPoint) ? 1f : 1.5f;
                    player.afraid = Mathf.InverseLerp(Mathf.Lerp(40f, 250f, relationship.intensity), 10f, lookDistance * contactMultiplier);
                }
            }
            else
            {
                player.afraid = Custom.LerpAndTick(player.afraid, 0f, 0.001f, 0.3f);
            }

            // Update danger level based on fear.
            player.DangerNum = player.afraid > 0
                ? Custom.LerpAndTick(player.DangerNum, 100f, 0.01f, 0.03f)
                : Custom.LerpAndTick(player.DangerNum, 0f, 0.001f, 0.3f);

            // Process delayed deafen.
            player = self.ItemData();
            if (player.DelayedDeafen > 0)
            {
                player.DelayedDeafen--;
                if (player.DelayedDeafen <= 0)
                {
                    self.Deafen(player.DelayedDeafenDuration);
                    player.DelayedDeafen = 0;
                    player.DelayedDeafenDuration = 0;
                }
            }

            // Process berserker duration.
            if (player.BerserkerDuration > 0)
            {
                player.BerserkerDuration--;
                if (player.BerserkerDuration <= 0)
                {
                    player.Berserker = false;
                    player.BerserkerDuration = 0;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        // The following block is commented out pending further updates.
        /*
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
        */
    }

    private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
    {
        orig(self);
        if (!self.IsPlayer(out GeneralPlayerData player))
            return;
        if (self.room == null)
            return;

        // Smooth out cacao speed and update dynamic run speed.
        player.cacaoSpeed = Custom.LerpAndTick(player.cacaoSpeed, 0f, 0.001f, 0.0001f);
        self.dynamicRunSpeed[0] += player.cacaoSpeed;
        self.dynamicRunSpeed[1] += player.cacaoSpeed;

        // Update player power based on danger level.
        player.power = player.DangerNum > 10f
            ? Custom.LerpAndTick(player.power, 5f, 0.1f, 0.03f)
            : Custom.LerpAndTick(player.power, 0f, 0.01f, 0.3f);
        self.dynamicRunSpeed[0] += player.power;
        self.dynamicRunSpeed[1] += player.power;
    }

    private static void Player_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
    {
        orig(self, newRoom);
        if (self.room == null)
            return;

        // Conditions to spawn a BoyKisser.
        if (!self.room.world.game.IsArenaSession &&
            !SpawnedBoyKisser &&
            !NTOptionsMenu.DisableBoykisser.Value &&
            !newRoom.abstractRoom.gate &&
            !newRoom.abstractRoom.shelter &&
            !newRoom.abstractRoom.isAncientShelter &&
            self.room.game.GetStorySession.saveState.cycleNumber != 0 &&
            Random.value <= (1f / 150000) &&
            self.room.game.world.rainCycle.timer > ((self.room.game.GetStorySession.saveState.cycleNumber == 0) ? 2000f : 1000f))
        {
            Room randomRoom = self.room.world.activeRooms[Random.Range(0, self.room.world.activeRooms.Count)];
            int tileX = Random.Range(0, randomRoom.Width);
            int tileY = Random.Range(0, randomRoom.TileHeight);
            if (randomRoom.GetTile(tileX, tileY).Terrain == 0 &&
                !RayTraceTilesForTerrain(randomRoom, new IntVector2(tileX, tileY), new IntVector2(tileX, tileY - 1000)))
            {
                AbstractCreature boyKisserCreature = new(
                    self.room.world,
                    StaticWorld.GetCreatureTemplate(NTEnums.CreatureType.BoyKisser),
                    null,
                    randomRoom.GetWorldCoordinate(new IntVector2(tileX, tileY)),
                    self.room.game.GetNewID());

                randomRoom.abstractRoom.AddEntity(boyKisserCreature);
                boyKisserCreature.RealizeInRoom();
                SpawnedBoyKisser = true;
            }
        }
    }

    #endregion

    #region AI Hooks

    private static void ArtificialIntelligence_Update(On.ArtificialIntelligence.orig_Update orig, ArtificialIntelligence self)
    {
        orig(self);
        if (!Player.IsPlayer(out var playerData))
            return;

        if (self.tracker != null &&
            self.creature.world.game.IsStorySession &&
            Player != null &&
            playerData.BerserkerDuration != 0)
        {
            var creatureRepresentation = self.tracker.RepresentationForObject(Player, AddIfMissing: false);
            if (creatureRepresentation == null)
            {
                self.tracker.SeeCreature(Player.abstractCreature);
            }
        }
    }

    private static void AbstractCreatureAI_AbstractBehavior(On.AbstractCreatureAI.orig_AbstractBehavior orig, AbstractCreatureAI self, int time)
    {
        orig(self, time);
        if (!Player.IsPlayer(out var playerData) || playerData.BerserkerDuration == 0)
            return;

        AbstractRoom abstractRoom = self.world.GetAbstractRoom(GeneralPlayerPos);
        if (abstractRoom == null)
            return;

        if (GeneralPlayerPos.NodeDefined &&
            self.parent.creatureTemplate.mappedNodeTypes[(int)abstractRoom.nodes[GeneralPlayerPos.abstractNode].type])
        {
            self.SetDestination(GeneralPlayerPos);
            return;
        }

        // Gather all valid node positions.
        List<WorldCoordinate> validCoordinates = new List<WorldCoordinate>();
        for (int i = 0; i < abstractRoom.nodes.Length; i++)
        {
            if (self.parent.creatureTemplate.mappedNodeTypes[(int)abstractRoom.nodes[i].type])
            {
                validCoordinates.Add(new WorldCoordinate(GeneralPlayerPos.room, -1, -1, i));
            }
        }
        if (validCoordinates.Count > 0)
        {
            self.SetDestination(validCoordinates[Random.Range(0, validCoordinates.Count)]);
        }
    }

    private static void AbstractCreatureAI_Update(On.AbstractCreatureAI.orig_Update orig, AbstractCreatureAI self, int time)
    {
        try
        {
            orig(self, time);
            if (self.parent?.creatureTemplate.type == NTEnums.CreatureType.BoyKisser && self.world.game.Players.Count > 0)
            {
                self.followCreature = self.world.game.Players[0];
            }

            if (self.parent.Room != null &&
                self.parent.Room.realizedRoom != null &&
                self.parent.Room.realizedRoom.regionGate == null &&
                self.parent.creatureTemplate.type == NTEnums.CreatureType.BoyKisser)
            {
                self.SetDestination(GeneralPlayerPos);
            }
        }
        catch (Exception e)
        {
            Plugin.DebugError(e);
        }
    }

    #endregion

    #region Process Manager Hook

    private static void ProcessManager_RequestMainProcessSwitch_ProcessID(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        //Reseting this values every cycle
        if (ID == ProcessManager.ProcessID.Game)
        {
            SpawnedBoyKisser = false;
            DroneCrafting = true;
        }
        orig(self, ID);
    }

    #endregion

    #region Story Game Session Hook

    private static int StoryGameSession_slugPupMaxCount_get(Func<StoryGameSession, int> orig, StoryGameSession self)
    {
        if (self.saveStateNumber == NTEnums.NightWalker)
            return 3;
        if (self.saveStateNumber == NTEnums.Exile)
            return 1;
        if (self.saveStateNumber == NTEnums.Witness)
            return 0;
        return orig(self);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Updates global and player-specific position data.
    /// </summary>
    private static void UpdatePlayerPositions(Player playerObj, GeneralPlayerData playerData)
    {
        if (playerObj.room == null || playerObj.mainBodyChunk == null)
            return;

        playerData.playerPos = playerObj.room.GetWorldCoordinate(playerObj.mainBodyChunk.pos);
        GeneralPlayerPos = playerData.playerPos;

        playerData.playerMainPos = playerObj.mainBodyChunk.pos;
        GeneralPlayerMainPos = playerData.playerMainPos;

        playerData.distanceToPlayer = Vector2.Distance(playerData.playerMainPos, Boykisser.boykisserPos);
    }

    #endregion
}
