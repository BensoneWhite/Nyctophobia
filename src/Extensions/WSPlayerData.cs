namespace Witness;

public class WSPlayerData
{
    public bool HasSeenFirtsTutorial = false;

    public readonly bool IsWitness;

    public float DangerNum;

    public float power;

    public int lastTail;

    public int FlashCooldown;

    public WeakReference<Player> PlayerRef;

    public Color BodyColor;
    public Color EyesColor;
    public Color WhiskersColor;
    public Color TailColor;

    public FAtlasElement TailElement;

    public FAtlas TailAtlas;

    public WSPlayerData(Player player)
    {
        PlayerRef = new WeakReference<Player>(player);

        IsWitness = player.slugcatStats.name == NTEnums.Witness;

        if (!IsWitness) return;
    }

    public void SetupColorsWS(PlayerGraphics pg)
    {
        BodyColor = pg.GetColor(NTEnums.Color.Body) ?? Custom.hexToColor("ffcf0d");
        EyesColor = pg.GetColor(NTEnums.Color.Eyes) ?? Custom.hexToColor("010101");
        TailColor = pg.GetColor(NTEnums.Color.Tail) ?? Custom.hexToColor("ffcf0d");
    }

    public void SetupTailTextureWS()
    {
        var tailTexture = new Texture2D(Plugin.TailTextureWS.width, Plugin.TailTextureWS.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(Plugin.TailTextureWS, tailTexture);

        NTUtils.MapTextureColor(tailTexture, 0, TailColor);

        if (PlayerRef.TryGetTarget(out var player))
        {
            TailAtlas = Futile.atlasManager.LoadAtlasFromTexture("Witnesscattailtexture_" + player.playerState.playerNumber + Time.time + Random.value, tailTexture, false);
        }
    }
}
