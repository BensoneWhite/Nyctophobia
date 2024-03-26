namespace Nyctophobia;

public static class NTPlayerExtensions
{
    private static readonly ConditionalWeakTable<Player, NWPlayerData> _cwtnw = new();
    private static readonly ConditionalWeakTable<Player, EXPlayerData> _cwtex = new();
    private static readonly ConditionalWeakTable<Player, WSPlayerData> _cwtws = new();

    public static NWPlayerData NightWalker(this Player player) => _cwtnw.GetValue(player, _ => new NWPlayerData(player));

    public static EXPlayerData Exile(this Player player) => _cwtex.GetValue(player, _ => new EXPlayerData(player));

    public static WSPlayerData Witness(this Player player) => _cwtws.GetValue(player, _ => new WSPlayerData(player));

    public static Color? GetColor(this PlayerGraphics pg, PlayerColor color) => color.GetColor(pg);

    public static Color? GetColor(this Player player, PlayerColor color) => (player.graphicsModule as PlayerGraphics)?.GetColor(color);

    public static Player Get(this WeakReference<Player> weakRef)
    {
        weakRef.TryGetTarget(out var result);
        return result;
    }

    public static PlayerGraphics PlayerGraphics(this Player player) => (PlayerGraphics)player.graphicsModule;

    public static TailSegment[] Tail(this Player player) => player.PlayerGraphics().tail;

    public static bool IsNightWalker(this Player player) => player.NightWalker().IsNightWalker;

    public static bool IsNightWalker(this Player player, out NWPlayerData NightWalker)
    {
        NightWalker = player.NightWalker();
        return NightWalker.IsNightWalker;
    }

    public static bool IsExile(this Player player) => player.Exile().IsExile;

    public static bool IsExile(this Player player, out EXPlayerData ws)
    {
        ws = player.Exile();
        return ws.IsExile;
    }

    public static bool IsWitness(this Player player) => player.Witness().IsWitness;

    public static bool IsWitness(this Player player, out WSPlayerData ws)
    {
        ws = player.Witness();
        return ws.IsWitness;
    }

    //Iterator extension
    public static bool IsESP(this Oracle oracle) => oracle.ID == NTEnums.Iterator.ESP;
    public static bool IsESP(this OracleGraphics oracle) => (oracle.owner as Oracle).IsESP();
}