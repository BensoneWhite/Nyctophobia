namespace Nyctophobia;

//Move Player DataClass into Slugcats folder?
public class WSPlayerData
{
    public bool HasSeenFirtsTutorial = false;

    public readonly bool IsWitness;

    public float DangerNum;

    public float power;

    public int lastTail;

    public int FlashCooldown;

    public bool wawaEat;

    public bool DroneCrafting;

    public WeakReference<Player> PlayerRef;

    public Color BodyColor;
    public Color EyesColor;
    public Color WhiskersColor;
    public Color TailColor;

    public FAtlasElement TailElement;

    public FAtlas TailAtlas;

    public static Texture2D TailTextureWS;

    public WSPlayerData(Player player)
    {
        PlayerRef = new WeakReference<Player>(player);

        IsWitness = player.slugcatStats.name == NTEnums.Witness;

        if (!IsWitness)
        {
            return;
        }
    }

    public void SetupColorsWS(PlayerGraphics pg)
    {
        BodyColor = pg.GetColor(NTEnums.ColorWS.Body) ?? Custom.hexToColor("ffcf0d");
        EyesColor = pg.GetColor(NTEnums.ColorWS.Eyes) ?? Custom.hexToColor("010101");
    }

    public void SetupTailTextureWS()
    {
        TailTextureWS = new Texture2D(150, 75, TextureFormat.ARGB32, false);
        string WStailTextureFile = AssetManager.ResolveFilePath("textures/witnesstail.png");
        if (File.Exists(WStailTextureFile))
        {
            byte[] rawData = File.ReadAllBytes(WStailTextureFile);
            _ = TailTextureWS.LoadImage(rawData);
        }

        Texture2D tailTexture = new(TailTextureWS.width, TailTextureWS.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(TailTextureWS, tailTexture);

        NTUtils.MapTextureColor(tailTexture, 0, TailColor);

        if (PlayerRef.TryGetTarget(out Player player))
        {
            TailAtlas = Futile.atlasManager.LoadAtlasFromTexture("Witnesscattailtexture_" + player.playerState.playerNumber + Time.time + Random.value, tailTexture, false);
        }
    }
}