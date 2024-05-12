namespace Nyctophobia;

public static class NTPlayerExtensions
{
    public static Color? GetColor(this PlayerGraphics pg, PlayerColor color)
    {
        return color.GetColor(pg);
    }

    public static Color? GetColor(this Player player, PlayerColor color)
    {
        return (player.graphicsModule as PlayerGraphics)?.GetColor(color);
    }

    public static Player Get(this WeakReference<Player> weakRef)
    {
        _ = weakRef.TryGetTarget(out Player result);
        return result;
    }

    public static PlayerGraphics PlayerGraphics(this Player player)
    {
        return (PlayerGraphics)player.graphicsModule;
    }

    public static TailSegment[] Tail(this Player player)
    {
        return player.PlayerGraphics().tail;
    }

    private static readonly ConditionalWeakTable<Player, NWPlayerData> _cwtnw = new();

    public static NWPlayerData NightWalker(this Player player)
    {
        return _cwtnw.GetValue(player, _ => new NWPlayerData(player));
    }

    public static bool IsNightWalker(this Player player)
    {
        return player.NightWalker().IsNightWalker;
    }

    public static bool IsNightWalker(this Player player, out NWPlayerData NightWalker)
    {
        NightWalker = player.NightWalker();
        return NightWalker.IsNightWalker;
    }

    private static readonly ConditionalWeakTable<Player, EXPlayerData> _cwtex = new();

    public static EXPlayerData Exile(this Player player)
    {
        return _cwtex.GetValue(player, _ => new EXPlayerData(player));
    }

    public static bool IsExile(this Player player)
    {
        return player.Exile().IsExile;
    }

    public static bool IsExile(this Player player, out EXPlayerData Exile)
    {
        Exile = player.Exile();
        return Exile.IsExile;
    }

    private static readonly ConditionalWeakTable<Player, WSPlayerData> _cwtws = new();

    public static WSPlayerData Witness(this Player player)
    {
        return _cwtws.GetValue(player, _ => new WSPlayerData(player));
    }

    public static bool IsWitness(this Player player)
    {
        return player.Witness().IsWitness;
    }

    public static bool IsWitness(this Player player, out WSPlayerData Witness)
    {
        Witness = player.Witness();
        return Witness.IsWitness;
    }

    public static bool IsESP(this Oracle oracle)
    {
        return oracle.ID == NTEnums.Iterator.ESP;
    }

    public static bool IsESP(this OracleGraphics oracle)
    {
        return (oracle.owner as Oracle).IsESP();
    }

    public static readonly ConditionalWeakTable<Player, ItemData> _itctw = new();

    public static ItemData ItemData(this Player player)
    {
        return _itctw.GetValue(player, _ => new ItemData(player));
    }

    public static bool IsPlayer(this Player player)
    {
        return player.ItemData().IsAPlayer;
    }

    public static bool IsPlayer(this Player player, out ItemData itemData)
    {
        itemData = player.ItemData();
        return itemData.IsAPlayer;
    }
}