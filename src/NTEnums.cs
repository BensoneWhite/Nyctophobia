﻿namespace Nyctophobia;

public static class NTEnums
{
    public static readonly SlugcatStats.Name Witness = new(nameof(Witness));
    public static readonly SlugcatStats.Name Exile = new(nameof(Exile));

    public const string DevToolsCategory = Plugin.MOD_NAME;

    private static readonly System.Type[] _enumTypes =
    [
        typeof(Sound),
        typeof(CreatureType),
        typeof(SandboxUnlock),
        typeof(ColorNW),
        typeof(ColorWS),
        typeof(ColorEX),
        typeof(AbstractObjectTypes),
        typeof(SpecialItemType),
        typeof(PlacedObjectType),
        typeof(Iterator),
        typeof(ESPBehaviorAction),
        typeof(RoomEffect)
    ];

    public static void Init()
    {
        foreach (var type in _enumTypes)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }

    public static void Unregister()
    {
        foreach (var type in _enumTypes)
        {
            NTUtils.UnregisterEnums(type);
        }
    }

    public static class Sound
    {
        public static SoundID wind = new(nameof(wind), true);
        public static SoundID wawaWit = new(nameof(wawaWit), true);
        public static SoundID TryAgain = new(nameof(TryAgain), true);
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
        public static SandboxUnlockID ScarletLongLegs = new(nameof(ScarletLongLegs), true);
        public static SandboxUnlockID ScarletLizards = new(nameof(ScarletLizards), true);
        public static SandboxUnlockID BlackLightMouse = new(nameof(BlackLightMouse), true);
        public static SandboxUnlockID CicadaDron = new(nameof(CicadaDron), true);
        public static SandboxUnlockID MiroAlbino = new(nameof(MiroAlbino), true);

        public static SandboxUnlockID RedFlareBomb = new(nameof(RedFlareBomb), true);

        public static SandboxUnlockID WitnessPup = new(nameof(WitnessPup), true);

        public static SandboxUnlockID BlueLantern = new(nameof(BlueLantern), true);
        public static SandboxUnlockID BlueBomba = new(nameof(BlueBomba), true);
        public static SandboxUnlockID BlueSpear = new(nameof(BlueSpear), true);
        public static SandboxUnlockID CacaoFruit = new(nameof(CacaoFruit), true);
        public static SandboxUnlockID BloodyKarmaFlower = new(nameof(BloodyKarmaFlower), true);
        public static SandboxUnlockID Impaler = new(nameof(Impaler), true);
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

    public static class AbstractObjectTypes
    {
        public static AbstractObjectType RedFlareBomb = new(nameof(RedFlareBomb), true);
        public static AbstractObjectType BlueLantern = new(nameof(BlueLantern), true);
        public static AbstractObjectType Bluebomba = new(nameof(Bluebomba), true);
        public static AbstractObjectType BlueSpear = new(nameof(BlueSpear), true);
        public static AbstractObjectType CacaoFruit = new(nameof(CacaoFruit), true);
        public static AbstractObjectType BloodyKarmaFlower = new(nameof(BloodyKarmaFlower), true);
        public static AbstractObjectType Impaler = new(nameof(Impaler), true);
    }

    public class SpecialItemType(string value, bool register = false) : ExtEnum<SpecialItemType>(value, register)
    {
        public static SpecialItemType CacaoFruit = new(nameof(CacaoFruit), true);
        public static SpecialItemType BoodyKarmaFlower = new(nameof(BoodyKarmaFlower), true);
        public static SpecialItemType RedFlareBomb = new(nameof(RedFlareBomb), true);
        public static SpecialItemType BlueSpear = new(nameof(BlueSpear), true);
        public static SpecialItemType Bluebomba = new(nameof(Bluebomba), true);
        public static SpecialItemType BlueLantern = new(nameof(BlueLantern), true);
    }

    public static class PlacedObjectType
    {
        public static PlacedObject.Type Impaler = new(nameof(Impaler), true);
    }

    public static class Iterator
    {
        public static Oracle.OracleID ESP = new(nameof(ESP), true);
    }

    public static class ESPBehaviorAction
    {
        public static ESPBehavior.Action MeetWhite_ThirdCurious;
        public static ESPBehavior.Action MeetWhite_SecondImages;
        public static ESPBehavior.Action MeetWhite_StartDialog;
        public static ESPBehavior.Action ThrowOut_Singularity;
    }

    public static class Passage
    {
        public static WinState.EndgameID EggHatcher = new($"{Plugin.MOD_ID}_{nameof(EggHatcher)}", true);
        public static WinState.EndgameID TheGreatMother = new($"{Plugin.MOD_ID}_{nameof(TheGreatMother)}", true);
    }

    public static class RoomEffect
    {
        public static RoomSettings.RoomEffect.Type VoidMeltController = new(nameof(VoidMeltController), true);
    }
}