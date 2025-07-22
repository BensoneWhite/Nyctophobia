namespace Nyctophobia;

public static class NTPlayerExtensions
{
    public static Color? GetColor(this PlayerGraphics pg, PlayerColor color) => color.GetColor(pg);

    public static Color? GetColor(this Player player, PlayerColor color) => (player.graphicsModule as PlayerGraphics)?.GetColor(color);

    public static Player Get(this WeakReference<Player> weakRef)
    {
        _ = weakRef.TryGetTarget(out Player result);
        return result;
    }

    public static PlayerGraphics PlayerGraphics(this Player player) => (PlayerGraphics)player.graphicsModule;

    public static TailSegment[] Tail(this Player player) => player.PlayerGraphics().tail;

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

    public static readonly ConditionalWeakTable<Player, GeneralPlayerData> _itctw = new();

    public static GeneralPlayerData ItemData(this Player player)
    {
        if (player == null)
        {
            Plugin.DebugWarning($"Player is null for ItemData");
            return null;
        }
        return _itctw.GetValue(player, _ => new GeneralPlayerData(player));
    }

    public static bool IsPlayer(this Player player)
    {
        if (player == null)
            return false;
        var data = player.ItemData();
        return data != null && data.IsAPlayer;
    }

    public static bool IsPlayer(this Player player, out GeneralPlayerData itemData)
    {
        if (player == null)
        {
            itemData = null;
            return false;
        }
        itemData = player.ItemData();
        return itemData != null && itemData.IsAPlayer;
    }
}