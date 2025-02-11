namespace Nyctophobia;

public static class SelectMenuHooks
{
    public static bool IsNightwalker;
    public static bool IsNyctoCat;

    public class SelectMenuModule
    {
        public float Hue { get; set; }
        public bool Increasing { get; set; } = true;
        public Color Color { get; set; }
    }

    public static readonly ConditionalWeakTable<object, SelectMenuModule> MenuData = new();

    public static SelectMenuModule GetModule(this object obj) => MenuData.GetOrCreateValue(obj);

    public static void Apply()
    {
        On.Menu.SlugcatSelectMenu.Update += SlugcatSelectMenu_Update;
        On.Menu.SlugcatSelectMenu.ctor += SlugcatSelectMenu_ctor;
        On.Menu.HoldButton.MyColor += HoldButton_MyColor;
        On.Menu.HoldButton.GrafUpdate += HoldButton_GrafUpdate;
        On.Menu.SlugcatSelectMenu.SlugcatPageContinue.GrafUpdate += SlugcatPageContinue_GrafUpdate;
        IL.Menu.SlugcatSelectMenu.StartGame += SlugcatSelectMenu_StartGame;
        On.Menu.SlugcatSelectMenu.ContinueStartedGame += SlugcatSelectMenu_ContinueStartedGame;
    }

    #region Hooks
    private static void SlugcatSelectMenu_ContinueStartedGame(On.Menu.SlugcatSelectMenu.orig_ContinueStartedGame orig, SlugcatSelectMenu self, SlugcatStats.Name storyGameCharacter)
    {
        orig(self, storyGameCharacter);

        if (IsNyctoCat)
        {
            self.PlaySound(SoundID.Thunder, 1f, 0.7f, 1f);
        }
    }

    private static void SlugcatSelectMenu_StartGame(ILContext il)
    {
        ILCursor cursor = new(il);
        try
        {

            // Find the instruction that loads the original sound field.
            //This should be changed to 'typeof' 'nameof' instead of using strings
            if (!cursor.TryGotoNext((MoveType)2,
                [(Instruction i) => ILPatternMatchingExt.MatchLdsfld(i, "SoundID", "MENU_Start_New_Game")]))
            {
                Plugin.DebugError($"Failed to change start menu sound from {Plugin.MOD_NAME}");
                return;
            }
            cursor.MoveAfterLabels();

            // Cache the original sound.
            SoundID originalSound = SoundID.MENU_Start_New_Game;

            // Replace the original sound with a delegate that returns Thunder if NyctoCat.
            cursor.EmitDelegate<Func<SoundID, SoundID>>((_) =>
            {
                if (IsNyctoCat)
                {
                    return SoundID.Thunder;
                }
                return originalSound;
            });
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Plugin.DebugError(ex);
        }
    }

    private static void HoldButton_GrafUpdate(On.Menu.HoldButton.orig_GrafUpdate orig, HoldButton self, float timeStacker)
    {
        orig(self, timeStacker);

        var module = self.GetModule();

        // Compute a color by lerping from black to red.
        Color lerpedColor = Color.Lerp(Color.black, Color.red, module.Hue);

        // If it's PrideDay, use a custom RGB color.
        if (IsPrideDay)
            lerpedColor = Custom.HSL2RGB(module.Hue, 1.0f, 0.5f);

        if (IsNightwalker)
        {
            MethodHelpers.UpdateModule(module);
            // Loop over all circle sprites rather than setting each by index.
            foreach (var sprite in self.circleSprites)
            {
                sprite.color = lerpedColor;
            }
        }
        else
        {
            // Reset a fallback sprite if needed.
            if (self.circleSprites.Length > 2)
                self.circleSprites[2].color = Color.white;
        }
    }

    private static void SlugcatPageContinue_GrafUpdate(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_GrafUpdate orig, SlugcatSelectMenu.SlugcatPageContinue self, float timeStacker)
    {
        orig(self, timeStacker);

        var module = self.GetModule();

        if (MethodHelpers.IsNyctoCat(self))
        {
            MethodHelpers.UpdateModule(module);
            self.regionLabel.label.color = module.Color;
        }
    }

    private static Color HoldButton_MyColor(On.Menu.HoldButton.orig_MyColor orig, HoldButton self, float timeStacker)
    {
        var module = self.GetModule();

        if (IsNightwalker)
        {
            MethodHelpers.UpdateModule(module);
            return module.Color;
        }
        return orig(self, timeStacker);
    }

    private static void SlugcatSelectMenu_ctor(On.Menu.SlugcatSelectMenu.orig_ctor orig, SlugcatSelectMenu self, ProcessManager manager)
    {
        orig(self, manager);
        UpdateCharacterFlags(self);

        var module = self.GetModule();
        UpdateStartButtonIfNeeded(self, module);
    }

    private static void SlugcatSelectMenu_Update(On.Menu.SlugcatSelectMenu.orig_Update orig, SlugcatSelectMenu self)
    {
        orig(self);
        UpdateCharacterFlags(self);

        var module = self.GetModule();
        UpdateStartButtonIfNeeded(self, module);
    }

    /// <summary>
    /// Updates the global character flags based on the current slugcat page.
    /// </summary>
    private static void UpdateCharacterFlags(SlugcatSelectMenu self)
    {
        var currentSlugcat = self.slugcatPages[self.slugcatPageIndex].slugcatNumber;
        IsNightwalker = currentSlugcat == NTEnums.NightWalker;
        IsNyctoCat = currentSlugcat == NTEnums.NightWalker ||
                     currentSlugcat == NTEnums.Witness ||
                     currentSlugcat == NTEnums.Exile;
    }

    /// <summary>
    /// If the current slugcat is a NyctoCat and the start button is in NEW GAME mode,
    /// update its appearance to indicate warning mode.
    /// </summary>
    private static void UpdateStartButtonIfNeeded(SlugcatSelectMenu self, SelectMenuModule module)
    {
        if (MethodHelpers.IsNyctoCat(self) && self.startButton.menuLabel.text == self.Translate("NEW GAME"))
        {
            MethodHelpers.UpdateModule(module);
            self.startButton.warningMode = true;
            self.startButton.menuLabel.label.color = module.Color;
        }
    }
    #endregion

}