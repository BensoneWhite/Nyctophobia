﻿namespace Nyctophobia;

public class WSPlayerData
{
    public bool HasSeenFirtsTutorial = false;

    public readonly bool IsWitness;

    public float DangerNum;

    public float power;

    public int lastTail;

    public int FlashCooldown;

    public bool wawaEat;

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

        if (!IsWitness) return;
    }

    public void SetupColorsWS(PlayerGraphics pg)
    {
        BodyColor = pg.GetColor(NTEnums.ColorWS.Body) ?? Custom.hexToColor("ffcf0d");
        EyesColor = pg.GetColor(NTEnums.ColorWS.Eyes) ?? Custom.hexToColor("010101");
    }

    public void SetupTailTextureWS()
    {
        WSPlayerData.TailTextureWS = new Texture2D(150, 75, TextureFormat.ARGB32, false);
        var WStailTextureFile = AssetManager.ResolveFilePath("textures/witnesstail.png");
        if (File.Exists(WStailTextureFile))
        {
            var rawData = File.ReadAllBytes(WStailTextureFile);
            WSPlayerData.TailTextureWS.LoadImage(rawData);
        }

        var tailTexture = new Texture2D(TailTextureWS.width, TailTextureWS.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(TailTextureWS, tailTexture);

        NTUtils.MapTextureColor(tailTexture, 0, TailColor);

        if (PlayerRef.TryGetTarget(out var player))
        {
            TailAtlas = Futile.atlasManager.LoadAtlasFromTexture("Witnesscattailtexture_" + player.playerState.playerNumber + Time.time + Random.value, tailTexture, false);
        }
    }
}