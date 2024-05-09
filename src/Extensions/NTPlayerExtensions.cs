namespace Nyctophobia;

public static class NTPlayerExtensions
{
    public static Color? GetColor(this PlayerGraphics pg, PlayerColor color) => color.GetColor(pg);
    public static Color? GetColor(this Player player, PlayerColor color) => (player.graphicsModule as PlayerGraphics)?.GetColor(color);
    public static Player Get(this WeakReference<Player> weakRef)
    {
        weakRef.TryGetTarget(out var result);
        return result;
    }
    public static PlayerGraphics PlayerGraphics(this Player player) => (PlayerGraphics)player.graphicsModule;
    public static TailSegment[] Tail(this Player player) => player.PlayerGraphics().tail;

    private static readonly ConditionalWeakTable<Player, NWPlayerData> _cwtnw = new();

    public static NWPlayerData NightWalker(this Player player) => _cwtnw.GetValue(player, _ => new NWPlayerData(player));

    public static bool IsNightWalker(this Player player) => player.NightWalker().IsNightWalker;

    public static bool IsNightWalker(this Player player, out NWPlayerData NightWalker)
    {
        NightWalker = player.NightWalker();
        return NightWalker.IsNightWalker;
    }

    private static readonly ConditionalWeakTable<Player, EXPlayerData> _cwtex = new();

    public static EXPlayerData Exile(this Player player) => _cwtex.GetValue(player, _ => new EXPlayerData(player));

    public static bool IsExile(this Player player) => player.Exile().IsExile;

    public static bool IsExile(this Player player, out EXPlayerData Exile)
    {
        Exile = player.Exile();
        return Exile.IsExile;
    }

    private static readonly ConditionalWeakTable<Player, WSPlayerData> _cwtws = new();

    public static WSPlayerData Witness(this Player player) => _cwtws.GetValue(player, _ => new WSPlayerData(player));

    public static bool IsWitness(this Player player) => player.Witness().IsWitness;

    public static bool IsWitness(this Player player, out WSPlayerData Witness)
    {
        Witness = player.Witness();
        return Witness.IsWitness;
    }

    public static bool IsESP(this Oracle oracle) => oracle.ID == NTEnums.Iterator.ESP;

    public static bool IsESP(this OracleGraphics oracle) => (oracle.owner as Oracle).IsESP();

    public static readonly ConditionalWeakTable<Player, ItemData> _itctw = new();

    public static ItemData ItemData(this Player player) => _itctw.GetValue(player, _ => new ItemData(player));

    public static bool IsPlayer(this Player player) => player.ItemData().IsAPlayer;

    public static bool IsPlayer(this Player player, out ItemData itemData)
    {
        itemData = player.ItemData();
        return itemData.IsAPlayer;
    }
}