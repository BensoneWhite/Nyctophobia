namespace Nyctophobia;

public static class HueRemixMenu
{
    public static void Apply() => 
        On.Menu.Remix.MenuModList.Update += MenuModList_Update;

    public class MenuModListModule
    {
        public float Hue { get; set; }
        public bool Increasing { get; set; } = true;
    }

    public static readonly ConditionalWeakTable<MenuModList, MenuModListModule> MenuModListData = new();

    public static MenuModListModule GetMenuModListModule(this MenuModList self) =>
        MenuModListData.GetOrCreateValue(self);

    private static void MenuModList_Update(On.Menu.Remix.MenuModList.orig_Update orig, MenuModList self)
    {
        orig(self);

        var module = self.GetMenuModListModule();

        // Find Nyctophobia mod's button; if not found, nothing to do.
        var modButton = self.modButtons.FirstOrDefault(x => x.ModID == Plugin.MOD_ID);
        if (modButton == null)
            return;

        if (IsPrideDay)
        {
            UpdateHuePrideDay(module, modButton);
        }
        else
        {
            UpdateHue(module, modButton);
        }
    }

    private static void UpdateHuePrideDay(MenuModListModule module, MenuModList.ModButton modButton)
    {
        const float HueStep = 0.01f;
        const float SaturationFull = 1.0f;
        const float SaturationDim = 0.15f;
        const float Lightness = 0.5f;

        // Use modulo arithmetic so hue wraps around naturally
        module.Hue = (module.Hue + HueStep) % 1.0f;

        // Set the color using HSL; use a higher saturation if the button is enabled.
        float saturation = modButton.selectEnabled ? SaturationFull : SaturationDim;
        modButton.SetColor(Custom.HSL2RGB(module.Hue, saturation, Lightness));
    }

    private static void UpdateHue(MenuModListModule module, MenuModList.ModButton modButton)
    {
        const float HueStep = 0.005f;
        const float MinHue = 0.0f;
        const float MaxHue = 1.0f;

        if (module.Increasing)
        {
            module.Hue += HueStep;
            if (module.Hue >= MaxHue)
            {
                module.Hue = MaxHue;
                module.Increasing = false;
            }
        }
        else
        {
            module.Hue -= HueStep;
            if (module.Hue <= MinHue)
            {
                module.Hue = MinHue;
                module.Increasing = true;
            }
        }

        // Lerp from a custom color to red.
        Color baseColor = new(0.522f, 0.22f, 0.22f);
        Color lerpedColor = Color.Lerp(baseColor, Color.red, module.Hue);
        modButton.SetColor(lerpedColor);
    }
}