using System.Security.Policy;

namespace Nyctophobia;

public static class NTEnums
{
    public static readonly SlugcatStats.Name NightWalker = new("NightWalker");
    public static readonly SlugcatStats.Name Witness = new("Witness");
    public static readonly SlugcatStats.Name Exile = new("Exile");

    public const string DevToolsCategory = Plugin.MOD_NAME;

    public static void Init()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Sound).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(CreatureType).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(SandboxUnlock).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ColorNW).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ColorWS).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ColorEX).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(AbstractObjectType).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(SpecialItemType).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(PlacedObjectType).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(Iterator).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ESPBehaviorAction).TypeHandle);
        RuntimeHelpers.RunClassConstructor(typeof(ESPBehaviorSubBehavID).TypeHandle);
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
        NTUtils.UnregisterEnums(typeof(PlacedObjectType));
        NTUtils.UnregisterEnums(typeof(Iterator));
        NTUtils.UnregisterEnums(typeof(ESPBehaviorAction));
        NTUtils.UnregisterEnums(typeof(ESPBehaviorSubBehavID));
    }

    public static class Sound
    {
        public static readonly SoundID wind = new(nameof(wind), true);
        public static readonly SoundID wawaWit = new(nameof(wawaWit), true);
        public static readonly SoundID TryAgain = new(nameof(TryAgain), true);

        public static SoundID BoyKisserKiss = new(nameof(BoyKisserKiss), true);
        public static SoundID BoyKisserChase = new(nameof(BoyKisserChase), true);
        public static SoundID BoyKisserSilly = new(nameof(BoyKisserSilly), true);
    }

    public static class CreatureType
    {
        public static CreatureTemplate.Type ScarletLongLegs = new(nameof(ScarletLongLegs), true);
        public static CreatureTemplate.Type ScarletLizard = new(nameof(ScarletLizard), true);
        public static CreatureTemplate.Type BlackLightMouse = new(nameof(BlackLightMouse), true);
        public static CreatureTemplate.Type CicadaDron = new(nameof(CicadaDron), true);
        public static CreatureTemplate.Type MiroAlbino = new(nameof(MiroAlbino), true);

        public static CreatureTemplate.Type WitnessPup = new(nameof(WitnessPup), true);

        public static CreatureTemplate.Type BoyKisser = new(nameof(BoyKisser), true);
    }

    public static class SandboxUnlock
    {
        public static SandboxUnlockID ScarletLongLegs = new(nameof(ScarletLongLegs), true);
        public static SandboxUnlockID ScarletLizards = new(nameof(ScarletLizards), true);
        public static SandboxUnlockID BlackLightMouse = new(nameof(BlackLightMouse), true);
        public static SandboxUnlockID CicadaDron = new(nameof(CicadaDron), true);
        public static SandboxUnlockID MiroAlbino = new(nameof(MiroAlbino), true);

        public static SandboxUnlockID RedFlareBomb = new(nameof(RedFlareBomb), true);
        public static SandboxUnlockID AncientNeuron = new(nameof(AncientNeuron), true);

        public static SandboxUnlockID WitnessPup = new(nameof(WitnessPup), true);

        public static SandboxUnlockID BlueLantern = new(nameof(BlueLantern), true);
        public static SandboxUnlockID BoyKisser = new(nameof(BoyKisser), true);
        public static SandboxUnlockID BlueBomba = new(nameof(BlueBomba), true);
        public static SandboxUnlockID BlueSpear = new(nameof(BlueSpear), true);
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

        public static AbstractPhysicalObject.AbstractObjectType BlueLantern = new(nameof(BlueLantern), true);
        public static AbstractPhysicalObject.AbstractObjectType Bluebomba = new(nameof(Bluebomba), true);
        public static AbstractPhysicalObject.AbstractObjectType BlueSpear = new(nameof(BlueSpear), true);
    }

    public class SpecialItemType(string value, bool register = false) : ExtEnum<SpecialItemType>(value, register)
    {
        public static SpecialItemType CacaoFruit = new(nameof(CacaoFruit), true);
        public static SpecialItemType BoodyKarmaFlower = new(nameof(BoodyKarmaFlower), true);
        public static SpecialItemType RedFlareBomb = new(nameof(RedFlareBomb), true);
        public static SpecialItemType BlueSpear = new(nameof(BlueSpear), true);
    }

    public static class PlacedObjectType
    {
    }

    public static class Iterator
    {
        public static Oracle.OracleID ESP = new(nameof(ESP), true);
    }

    public class ESPBehaviorAction
    {
        public static ESPBehavior.Action MeetPurple_Init;
        public static ESPBehavior.Action MeetPurple_GetPearl;
        public static ESPBehavior.Action MeetPurple_InspectPearl;
        public static ESPBehavior.Action MeetPurple_anger;
        public static ESPBehavior.Action MeetPurple_killoverseer;
        public static ESPBehavior.Action MeetPurple_getout;
        public static ESPBehavior.Action MeetPurple_markeddialog;
        public static ESPBehavior.Action Moon_SlumberParty;
        public static ESPBehavior.Action Moon_BeforeGiveMark;
        public static ESPBehavior.Action Moon_AfterGiveMark;
        public static ESPBehavior.Action MeetWhite_ThirdCurious;
        public static ESPBehavior.Action MeetWhite_SecondImages;
        public static ESPBehavior.Action MeetWhite_StartDialog;
        public static ESPBehavior.Action MeetInv_Init;
        public static ESPBehavior.Action MeetArty_Init;
        public static ESPBehavior.Action MeetArty_Talking;
        public static ESPBehavior.Action Pebbles_SlumberParty;
        public static ESPBehavior.Action ThrowOut_Singularity;
        public static ESPBehavior.Action MeetGourmand_Init;
        public static ESPBehavior.Action Rubicon;
    }

    public class ESPBehaviorSubBehavID
    {
        public static ESPBehavior.SubBehavior.SubBehavID MeetPurple;
        public static ESPBehavior.SubBehavior.SubBehavID SlumberParty;
        public static ESPBehavior.SubBehavior.SubBehavID Commercial;
        public static ESPBehavior.SubBehavior.SubBehavID MeetArty;
        public static ESPBehavior.SubBehavior.SubBehavID MeetGourmand;
        public static ESPBehavior.SubBehavior.SubBehavID Rubicon;
    }

    public static class Passage
    {
    }

    public static class RoomEffect
    {
        public static RoomSettings.RoomEffect.Type VoidMeltCustom = new(nameof(VoidMeltCustom), true);
    }
}