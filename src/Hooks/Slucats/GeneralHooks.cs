namespace Nyctophobia;

public class GeneralHooks
{
    public static WorldCoordinate GeneralPlayerPos;
    public static Vector2 GeneralPlayerMainPos;
    public static bool DroneCrafting;
    public static Player Player;

    public static void Apply()
    {
        On.PlayerGraphics.CosmeticPearl.Update += CosmeticPearl_Update_Hook;
        On.PlayerGraphics.CosmeticPearl.AddToContainer += CosmeticPearl_AddToContainer_Hook;
        On.PlayerGraphics.CosmeticPearl.InitiateSprites += CosmeticPearl_InitiateSprites_Hook;
        On.PlayerGraphics.CosmeticPearl.DrawSprites += CosmeticPearl_DrawSprites_Hook;
        On.PlayerGraphics.CosmeticPearl.ApplyPalette += CosmeticPearl_ApplyPalette_Hook;

        On.Player.Update += Player_Update;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;

        On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManager_RequestMainProcessSwitch_ProcessID;

        var slugPupMaxCountGetter = typeof(StoryGameSession)
            .GetProperty(nameof(StoryGameSession.slugPupMaxCount))
            ?.GetGetMethod();
        if (slugPupMaxCountGetter != null)
        {
            _ = new Hook(slugPupMaxCountGetter, StoryGameSession_slugPupMaxCount_get);
        }
    }

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
        return !player.IsWitness() || !player.IsExile();
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.IsPlayer(out var player))
            return;
        if (self.room == null || self.mainBodyChunk == null)
            return;

        Player = self;
    }

    private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
    {
        orig(self);
        if (!self.IsPlayer(out GeneralPlayerData player))
            return;
        if (self.room == null)
            return;

        player.cacaoSpeed = Custom.LerpAndTick(player.cacaoSpeed, 0f, 0.001f, 0.0001f);
        self.dynamicRunSpeed[0] += player.cacaoSpeed;
        self.dynamicRunSpeed[1] += player.cacaoSpeed;

        player.power = player.DangerNum > 10f
            ? Custom.LerpAndTick(player.power, 5f, 0.1f, 0.03f)
            : Custom.LerpAndTick(player.power, 0f, 0.01f, 0.3f);
        self.dynamicRunSpeed[0] += player.power;
        self.dynamicRunSpeed[1] += player.power;
    }

    private static void ProcessManager_RequestMainProcessSwitch_ProcessID(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        if (ID == ProcessManager.ProcessID.Game)
        {
            DroneCrafting = true;
        }
        orig(self, ID);
    }

    private static int StoryGameSession_slugPupMaxCount_get(Func<StoryGameSession, int> orig, StoryGameSession self)
    {
        if (self.saveStateNumber == NTEnums.Exile)
            return 1;
        if (self.saveStateNumber == NTEnums.Witness)
            return 0;
        return orig(self);
    }
}