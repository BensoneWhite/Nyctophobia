using EffExt;

namespace Nyctophobia;

public static class DevToolsInit
{
    public static void Apply()
    {
        BlueVoidMelt.Apply();
        new EffectDefinitionBuilder(nameof(NTEnums.RoomEffect.VoidMeltController))
        .SetCategory(NTEnums.DevToolsCategory)
        .Register();
    }
}