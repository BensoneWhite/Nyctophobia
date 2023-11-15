using SlugBase.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Witness;

public static class NTPlayerExtensions
{
    private static readonly ConditionalWeakTable<Player, NTPlayerData> _cwt = new();

    public static NTPlayerData NightWalker(this Player player) => _cwt.GetValue(player, _ => new NTPlayerData(player));

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

    public static bool IsNightWalker(this Player player, out NTPlayerData NightWalker)
    {
        NightWalker = player.NightWalker();
        return NightWalker.IsNightWalker;
    }

}
