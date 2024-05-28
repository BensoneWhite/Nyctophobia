namespace Nyctophobia;
public static class SelectMenuHooks
{
    public static bool IsNightwalker;

    public class SelectMenuModule
    {
        public float Hue { get; set; }
        public bool Increasing { get; set; } = true;
        public Color Color { get; set; }
    }

    public static readonly ConditionalWeakTable<object, SelectMenuModule> SharedData = new();
    public static SelectMenuModule GetModule(this object obj) => SharedData.GetOrCreateValue(obj);

    public static void Apply()
    {
        On.Menu.SlugcatSelectMenu.Update += SlugcatSelectMenu_Update;
        On.Menu.SlugcatSelectMenu.ctor += SlugcatSelectMenu_ctor;
        On.Menu.HoldButton.MyColor += HoldButton_MyColor;
        On.Menu.HoldButton.GrafUpdate += HoldButton_GrafUpdate;
        On.Menu.SlugcatSelectMenu.SlugcatPageContinue.GrafUpdate += SlugcatPageContinue_GrafUpdate;
    }

    private static void HoldButton_GrafUpdate(On.Menu.HoldButton.orig_GrafUpdate orig, HoldButton self, float timeStacker)
    {
        orig(self, timeStacker);

        var module = self.GetModule();

        Color lerpedColor = Color.Lerp(Color.black, Color.red, module.Hue);

        if (IsNightwalker)
        {
            MethodHelpers.UpdateModule(module);
            self.circleSprites[0].color = lerpedColor;
            self.circleSprites[1].color = lerpedColor;
            self.circleSprites[2].color = lerpedColor;
            self.circleSprites[3].color = lerpedColor;
            self.circleSprites[4].color = lerpedColor;
        }
        else
        {
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
        else
        {
            return orig(self, timeStacker);
        }
    }

    private static void SlugcatSelectMenu_ctor(On.Menu.SlugcatSelectMenu.orig_ctor orig, SlugcatSelectMenu self, ProcessManager manager)
    {
        orig(self, manager);

        var module = self.GetModule();

        IsNightwalker = self.slugcatPages[self.slugcatPageIndex].slugcatNumber == NTEnums.NightWalker;

        if (MethodHelpers.IsNyctoCat(self) && self.startButton.menuLabel.text == self.Translate("NEW GAME"))
        {
            MethodHelpers.UpdateModule(module);
            self.startButton.warningMode = true;
            self.startButton.menuLabel.label.color = module.Color;
        }
    }

    private static void SlugcatSelectMenu_Update(On.Menu.SlugcatSelectMenu.orig_Update orig, SlugcatSelectMenu self)
    {
        orig(self);

        var module = self.GetModule();

        IsNightwalker = self.slugcatPages[self.slugcatPageIndex].slugcatNumber == NTEnums.NightWalker;

        if (MethodHelpers.IsNyctoCat(self) && self.startButton.menuLabel.text == self.Translate("NEW GAME"))
        {
            MethodHelpers.UpdateModule(module);
            self.startButton.warningMode = true;
            self.startButton.menuLabel.label.color = module.Color;
        }
    }
}