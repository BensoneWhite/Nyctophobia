namespace Nyctophobia;

public static class NTEnums
{
    public readonly static SlugcatStats.Name NightWalker = new("NightWalker");
    public readonly static SlugcatStats.Name Witness = new("Witness");
    public readonly static SlugcatStats.Name Exile = new("Exile");

    public static void Init()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Sound).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(CreatureType).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(SandboxUnlock).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ColorNW).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ColorWS).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ColorEX).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(AbstractObjectType).TypeHandle);
    }

    public static void Unregister()
    {
        NTUtils.UnregisterEnums(typeof(Sound));
        NTUtils.UnregisterEnums(typeof(CreatureType));
        NTUtils.UnregisterEnums(typeof(SandboxUnlock));
        NTUtils.UnregisterEnums(typeof(ColorNW));
        NTUtils.UnregisterEnums(typeof(ColorWS));
        NTUtils.UnregisterEnums(typeof(ColorEX));
        NTUtils.UnregisterEnums(typeof(AbstractObjectType));
    }

    public static class Sound
    {
        public readonly static SoundID Wind;
        public readonly static SoundID wawa_Wit;
    }

    public static class CreatureType
    {
        public static CreatureTemplate.Type ScarletLongLegs = new(nameof(ScarletLongLegs), true);
        public static CreatureTemplate.Type ScarletLizard = new(nameof(ScarletLizard), true);
        public static CreatureTemplate.Type BlackLightMouse = new(nameof(BlackLightMouse), true);
        public static CreatureTemplate.Type CicadaDron = new(nameof(CicadaDron), true);
        public static CreatureTemplate.Type MiroAlbino = new(nameof(MiroAlbino), true);

        public static CreatureTemplate.Type WitnessPup = new(nameof(WitnessPup), true);
    }

    public static class SandboxUnlock
    {
        public static MultiplayerUnlocks.SandboxUnlockID ScarletLongLegs = new(nameof(ScarletLongLegs), true);
        public static MultiplayerUnlocks.SandboxUnlockID ScarletLizards = new(nameof(ScarletLizards), true);
        public static MultiplayerUnlocks.SandboxUnlockID BlackLightMouse = new(nameof(BlackLightMouse), true);
        public static MultiplayerUnlocks.SandboxUnlockID CicadaDron = new(nameof(CicadaDron), true);
        public static MultiplayerUnlocks.SandboxUnlockID MiroAlbino = new(nameof(MiroAlbino), true);

        public static MultiplayerUnlocks.SandboxUnlockID RedFlareBomb = new(nameof(RedFlareBomb), true);
        public static MultiplayerUnlocks.SandboxUnlockID AncientNeuron = new(nameof(AncientNeuron), true);
        public static MultiplayerUnlocks.SandboxUnlockID CacaoFruit = new(nameof(CacaoFruit), true);

        public static MultiplayerUnlocks.SandboxUnlockID WitnessPup = new(nameof(WitnessPup), true);
    }

    public static class ColorNW
    {
        public static PlayerColor Body = new(nameof(Body));
        public static PlayerColor Eyes = new(nameof(Eyes));
        public static PlayerColor Tail = new(nameof(Tail));
        public static PlayerColor Whiskers = new(nameof(Whiskers));
        public static PlayerColor Corruption = new(nameof(Corruption));
    }

    public static class ColorWS
    {
        public static PlayerColor Body = new(nameof(Body));
        public static PlayerColor Eyes = new(nameof(Eyes));
    }

    public static class ColorEX
    {
        public static PlayerColor Body = new(nameof(Body));
        public static PlayerColor Eyes = new(nameof(Eyes));
    }

    public static class AbstractObjectType
    {
        public static AbstractPhysicalObject.AbstractObjectType RedFlareBomb = new(nameof(RedFlareBomb), true);
        public static AbstractPhysicalObject.AbstractObjectType AncientNeuron = new(nameof(AncientNeuron), true);
        public static AbstractPhysicalObject.AbstractObjectType CacaoFruit = new(nameof(CacaoFruit), true);
    }

    public static class PlacedObjectType
    {
    }
}