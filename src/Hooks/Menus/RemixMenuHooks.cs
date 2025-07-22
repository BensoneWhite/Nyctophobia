namespace Nyctophobia;

public static class HueRemixMenu
{
    public static void Apply() => On.Menu.Remix.MenuModList.Update += MenuModList_Update;

    public class MenuModListModule
    {
        public float Hue { get; set; }
        public bool Increasing { get; set; } = true;
    }

    public static readonly ConditionalWeakTable<MenuModList, MenuModListModule> MenuModListData = new();

    public static MenuModListModule GetMenuModListModule(this MenuModList self) => MenuModListData.GetOrCreateValue(self);

    private static void MenuModList_Update(On.Menu.Remix.MenuModList.orig_Update orig, MenuModList self)
    {
        orig(self);

        var module = self.GetMenuModListModule();

        var modButton = self.modButtons.FirstOrDefault(x => x.ModID == Plugin.MOD_ID);
        if (modButton == null)
            return;

        UpdateHue(module, modButton);
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

        Color baseColor = new(0.522f, 0.22f, 0.22f);
        Color lerpedColor = Color.Lerp(baseColor, Color.red, module.Hue);
        modButton.SetColor(lerpedColor);
    }
}