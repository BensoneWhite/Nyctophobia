namespace Nyctophobia;

public static class HueRemixMenu
{
    public static void Apply() => On.Menu.Remix.MenuModList.Update += MenuModList_Update;

    public class MenuModListModule
    {
        public int Timer { get; set; }
        public int MoveCounter { get; set; }
        public int Dir { get; set; }
        public float Hue { get; set; }
        public Queue<float> MouseVel { get; set; } = new();
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
        Color lerpedColor = Color.Lerp(Color.black, Color.red, module.Hue);

        thisModButton.SetColor(lerpedColor);
    }
}