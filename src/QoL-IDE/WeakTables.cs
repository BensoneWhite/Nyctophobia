namespace Nyctophobia;

public static class WeakTables
{
    public static ConditionalWeakTable<RainWorld, Shaders> NyctoShaders = new();

    public class Shaders
    {
        public FShader desaturation = null;

        public AssetBundle ShaderPack = null;
    }
}