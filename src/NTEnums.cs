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
        RuntimeHelpers.RunClassConstructor(typeof(Color).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(AbstractObjectType).TypeHandle);
    }

    public static void Unregister()
    {
        NTUtils.UnregisterEnums(typeof(Sound));
        NTUtils.UnregisterEnums(typeof(CreatureType));
        NTUtils.UnregisterEnums(typeof(SandboxUnlock));
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
    }

    public static class Color
    {
        public readonly static PlayerColor Body;
        public readonly static PlayerColor Eyes;
        public readonly static PlayerColor Tail;
        public readonly static PlayerColor Whiskers;
    }

    public static class AbstractObjectType
    {
        public static AbstractPhysicalObject.AbstractObjectType RedFlareBomb = new(nameof(RedFlareBomb), true);
        public static AbstractPhysicalObject.AbstractObjectType AncientNeuron = new(nameof(AncientNeuron), true);
    }

    public static class PlacedObjectType
    {
    }
}
