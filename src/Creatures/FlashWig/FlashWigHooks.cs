namespace Nyctophobia;

public static class FlashWigHooks
{
    //Tricky but it works, thre's no legal way to convert an field inside the FlashWig hook
    public static Player Player;

    public static void Apply()
    {
        On.DropBug.JumpFromCeiling += DropBugOnJumpFromCeiling;
        On.HUD.HUD.InitMultiplayerHud += HUDOnInitMultiplayerHud;
        On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
        IL.DropBug.JumpFromCeiling += DropBug_ReplaceVoice;
        IL.DropBug.Violence += DropBug_ReplaceVoice;
        IL.DropBug.Attack += DropBug_ReplaceVoice;
    }

    private static void DropBug_ReplaceVoice(ILContext il)
    {
        var cursor = new ILCursor(il);

        cursor.GotoNext(MoveType.After, i => i.MatchLdsfld<SoundID>(nameof(SoundID.Drop_Bug_Voice)));

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate((SoundID oldValue, DropBug self) => self is FlashWig ? NTEnums.Sound.FlashWigVoice : oldValue);
    }

    private static void HUDOnInitMultiplayerHud(On.HUD.HUD.orig_InitMultiplayerHud orig, HUD.HUD self, ArenaGameSession session)
    {
        orig(self, session);
        
        self.AddPart(new FlashBangHUD(self));
    }

    private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);
        
        self.AddPart(new FlashBangHUD(self));
    }

    private static void DropBugOnJumpFromCeiling(On.DropBug.orig_JumpFromCeiling orig, DropBug self, BodyChunk target, Vector2 dir)
    {
        orig(self, target, dir);

        if (self is not FlashWig flashWig) return;

        //I caught you yummy player
        if (target?.owner is Player)
        {
            //Fear my power eat this sound
            flashWig.room.PlaySound(SoundID.Bomb_Explode, flashWig.mainBodyChunk);

            if(Player != null)
            {
                var data = Player?.ItemData();

                //JAJA now you are deafen temporaly
                data.DelayedDeafen = 10;
                data.DelayedDeafenDuration = 320;
            }

            //Eat this flash bang, I'm the FlashWig!
            if (self.room.game.cameras.FirstOrDefault()?.hud?.parts?.FirstOrDefault(x => x is FlashBangHUD) is FlashBangHUD hud)
            {
                hud.Flash(ScreenCapture.CaptureScreenshotAsTexture(), 160, flashWig.FlashColor);
            }
        }
        //Oppsss I failed I wasn't able to catch the player with my jump and hit the bodyChunk
        if (target is null)
        {
            //Well now you will eat my explosion
            Player.room.physicalObjects
            .SelectMany(list => list)
            .OfType<Creature>()
            .Where(creature => creature != Player && (creature.mainBodyChunk.pos - Player.mainBodyChunk.pos).magnitude < 1000f && creature is FlashWig)
            .ToList()
            .ForEach(creature =>
            {
                //Boom!
                flashWig.room.PlaySound(SoundID.Bomb_Explode, flashWig.mainBodyChunk);

                if (Player != null)
                {
                    var data = Player?.ItemData();

                    //JAJA now you are deafen temporaly
                    data.DelayedDeafen = 10;
                    data.DelayedDeafenDuration = 320;
                }

                //You can't scape from the light!
                if (self.room.game.cameras.FirstOrDefault()?.hud?.parts?.FirstOrDefault(x => x is FlashBangHUD) is FlashBangHUD hud)
                {
                    hud.Flash(ScreenCapture.CaptureScreenshotAsTexture(), 160, flashWig.FlashColor);
                }
            });
        }
    }
}