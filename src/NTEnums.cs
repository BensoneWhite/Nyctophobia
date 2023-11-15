using System.Runtime.CompilerServices;
using Rewired;
using SlugBase.DataTypes;

namespace Witness;

public class NTEnums
{
    public static SlugcatStats.Name NightWalker = new("NightWalker");
    public static SlugcatStats.Name Witness = new("Witness");

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
        public static SoundID Wind;
    }

    //Fibos
    public static class CreatureType
    {
    }

    public static class SandboxUnlock
    {
    }

    public static class Color
    {
        public static PlayerColor Body;
        public static PlayerColor Eyes;
        public static PlayerColor Tail;
        public static PlayerColor Whiskers;
    }

    //Fisobds
    public static class AbstractObjectType
    {
    }

    //Devtools objects
    public static class PlacedObjectType
    {
    }
}
