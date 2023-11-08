using SlugBase.DataTypes;

namespace Witness
{
    public static class NWEnums
    {
        public static SlugcatStats.Name NightWalker = new("NightWalker");

        public static class Sound
        {
            public static SoundID Wind;
        }

        public static class Color
        {
            public static PlayerColor Body;
            public static PlayerColor Eyes;
            public static PlayerColor Tail;
            public static PlayerColor Whiskers;
        }

        public static void RegisterVaules()
        {
            Sound.Wind = new SoundID("wind", true);
            Color.Body = new PlayerColor("Body");
            Color.Eyes = new PlayerColor("Eyes");
            Color.Tail = new PlayerColor("Tail");
            Color.Whiskers = new PlayerColor("Whiskers");
        }

        public static void UnregisterVaules()
        {
            Sound.Wind.Unregister();
        }
    }
}
