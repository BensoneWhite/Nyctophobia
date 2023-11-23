using System.Runtime.CompilerServices;
using Rewired;
using SlugBase.DataTypes;

namespace Witness;

public static class NTEnums
{
    public readonly static SlugcatStats.Name NightWalker = new("NightWalker");
    public readonly static SlugcatStats.Name Witness = new("Witness");

    public static SlugcatStats.Name Exile { get; internal set; }

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
    }

    //Fibos
    public static class CreatureType
    {
        //Empty
    }

    public static class SandboxUnlock
    {
        //Empty
    }

    public static class Color
    {
        public readonly static PlayerColor Body;
        public readonly static PlayerColor Eyes;
        public readonly static PlayerColor Tail;
        public readonly static PlayerColor Whiskers;
    }

    //Fisobds
    public static class AbstractObjectType
    {
        //Empty
    }

    //Devtools objects
    public static class PlacedObjectType
    {
        //Empty
    }
}
