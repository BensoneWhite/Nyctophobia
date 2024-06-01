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

        var thisModButton = self.modButtons.FirstOrDefault(x => x.ModID == Plugin.MOD_ID);
        if (thisModButton == null) return;

        if (IsPrideDay)
        {
            module.Hue += 0.01f;

            if (module.Hue > 1.0f)
                module.Hue = 0.0f;

            thisModButton.SetColor(Custom.HSL2RGB(module.Hue, thisModButton.selectEnabled ? 1.0f : 0.15f, 0.5f));
        }
        else
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
            Color lerpedColor = Color.Lerp(new Color(.522f, .22f, .22f), Color.red, module.Hue);

            thisModButton.SetColor(lerpedColor);
        }
    }
}