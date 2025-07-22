namespace Nyctophobia;

public static class SelectMenuHooks
{
    public static bool IsNyctoSlugcat;

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
        On.Menu.SlugcatSelectMenu.SlugcatPageContinue.GrafUpdate += SlugcatPageContinue_GrafUpdate;
        IL.Menu.SlugcatSelectMenu.StartGame += SlugcatSelectMenu_StartGame;
        On.Menu.SlugcatSelectMenu.ContinueStartedGame += SlugcatSelectMenu_ContinueStartedGame;
    }

    private static void SlugcatSelectMenu_ContinueStartedGame(On.Menu.SlugcatSelectMenu.orig_ContinueStartedGame orig, SlugcatSelectMenu self, SlugcatStats.Name storyGameCharacter)
    {
        orig(self, storyGameCharacter);

        if (IsNyctoSlugcat)
        {
            self.PlaySound(SoundID.Thunder, 1f, 0.7f, 1f);
        }
    }

    private static void SlugcatSelectMenu_StartGame(ILContext il)
    {
        ILCursor cursor = new(il);
        try
        {
            if (!cursor.TryGotoNext((MoveType)2,
                [(Instruction i) => ILPatternMatchingExt.MatchLdsfld(i, nameof(SoundID), nameof(SoundID.MENU_Start_New_Game))]))
            {
                Plugin.DebugError($"Failed to change start menu sound from {Plugin.MOD_NAME}");
                return;
            }
            cursor.MoveAfterLabels();

            SoundID originalSound = SoundID.MENU_Start_New_Game;

            cursor.EmitDelegate<Func<SoundID, SoundID>>((_) =>
            {
                if (IsNyctoSlugcat)
                {
                    return SoundID.Thunder;
                }
                return originalSound;
            });
        }
        catch (Exception ex)
        {
            Plugin.DebugError(ex);
        }
    }

    private static void SlugcatPageContinue_GrafUpdate(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_GrafUpdate orig, SlugcatSelectMenu.SlugcatPageContinue self, float timeStacker)
    {
        orig(self, timeStacker);

        var module = self.GetModule();

        if (IsNyctoCat(self))
        {
            UpdateModule(module);
            self.regionLabel.label.color = module.Color;
        }
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

    private static void UpdateCharacterFlags(SlugcatSelectMenu self)
    {
        var currentSlugcat = self.slugcatPages[self.slugcatPageIndex].slugcatNumber;
        IsNyctoSlugcat = currentSlugcat == NTEnums.Witness ||
                         currentSlugcat == NTEnums.Exile;
    }

    private static void UpdateStartButtonIfNeeded(SlugcatSelectMenu self, SelectMenuModule module)
    {
        if (IsNyctoCat(self) && self.startButton.menuLabel.text == self.Translate("NEW GAME"))
        {
            UpdateModule(module);
            self.startButton.warningMode = true;
            self.startButton.menuLabel.label.color = module.Color;
        }
    }

    public static void UpdateModule(SelectMenuModule module)
    {
        if (module.Increasing)
        {
            module.Hue += 0.005f;
            if (module.Hue >= 1.0f)
            {
                module.Hue = 1.0f;
                module.Increasing = false;
            }
        }
        else
        {
            module.Hue -= 0.005f;
            if (module.Hue <= 0.0f)
            {
                module.Hue = 0.0f;
                module.Increasing = true;
            }
        }

        module.Color = Color.Lerp(new Color(0.592f, 0.22f, 0.22f), Color.red, module.Hue);
    }

    public static bool IsNyctoCat(SlugcatSelectMenu.SlugcatPageContinue self)
    {
        return self.slugcatNumber == NTEnums.Witness || 
               self.slugcatNumber == NTEnums.Exile;
    }

    public static bool IsNyctoCat(SlugcatSelectMenu self)
    {
        return self.slugcatPages[self.slugcatPageIndex].slugcatNumber == NTEnums.Witness || 
               self.slugcatPages[self.slugcatPageIndex].slugcatNumber == NTEnums.Exile;
    }
}