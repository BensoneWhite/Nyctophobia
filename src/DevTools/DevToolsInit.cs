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
        //look this is a terrible idea but its for testing
        //or smth like that idrk
        Hookfun.Apply();
    }
}