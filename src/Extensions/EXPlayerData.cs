namespace Nyctophobia;

public class EXPlayerData
{
    public int currentDashes = 0;
    public float dashCooldown = 0f;
    public Vector2 dashDirectionX;
    public Vector2 dashDirectionY;
    public float dashDistance = 100f;
    public float maxDashDistance = 0f;
    public bool Dashed;
    public float DoesThatPlayerDashedOrNoBOZO;

    public readonly bool IsExile;

    public int lastTail;

    public WeakReference<Player> PlayerRef;

    public Color BodyColor;
    public Color EyesColor;
    public Color WhiskersColor;
    public Color TailColor;

    public FAtlasElement TailElement;

    public FAtlas TailAtlas;

    public static Texture2D TailTextureEX;

    public EXPlayerData(Player player)
    {
        PlayerRef = new WeakReference<Player>(player);

        IsExile = player.slugcatStats.name == NTEnums.Exile;

        if (!IsExile)
        {
            return;
        }
    }

    public void SetupColorsEX(PlayerGraphics pg)
    {
        BodyColor = pg.GetColor(NTEnums.ColorEX.Body) ?? Custom.hexToColor("ffcf0d");
        EyesColor = pg.GetColor(NTEnums.ColorEX.Eyes) ?? Custom.hexToColor("010101");
    }

    public void EXTail(PlayerGraphics self)
    {
        TailSegment[] oldTail = self.tail;

        self.tail = new TailSegment[5];
        self.tail[0] = new TailSegment(self, 8f, 4f, null, 0.85f, 1f, 1f, true);
        self.tail[1] = new TailSegment(self, 6f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
        self.tail[2] = new TailSegment(self, 4f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
        self.tail[3] = new TailSegment(self, 2f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
        self.tail[4] = new TailSegment(self, 1f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);

        if (oldTail != self.tail)
        {
            for (int i = 0; i < self.tail.Length && i < oldTail.Length; i++)
            {
                self.tail[i].pos = oldTail[i].pos;
                self.tail[i].lastPos = oldTail[i].lastPos;
                self.tail[i].vel = oldTail[i].vel;
                self.tail[i].terrainContact = oldTail[i].terrainContact;
                self.tail[i].stretched = oldTail[i].stretched;
            }

            List<BodyPart> bp = self.bodyParts.ToList();
            _ = bp.RemoveAll(x => x is TailSegment);
            bp.AddRange(self.tail);

            self.bodyParts = [.. bp];
        }
    }

    public void SetupTailTextureEX()
    {
        TailTextureEX = new Texture2D(150, 75, TextureFormat.ARGB32, false);
        string ExitailTextureFile = AssetManager.ResolveFilePath("textures/exiletail.png");
        if (File.Exists(ExitailTextureFile))
        {
            byte[] rawData = File.ReadAllBytes(ExitailTextureFile);
            _ = TailTextureEX.LoadImage(rawData);
        }

        Texture2D tailTexture = new(TailTextureEX.width, TailTextureEX.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(TailTextureEX, tailTexture);

        NTUtils.MapTextureColor(tailTexture, 0, TailColor);

        if (PlayerRef.TryGetTarget(out Player player))
        {
            TailAtlas = Futile.atlasManager.LoadAtlasFromTexture("Exilecattailtexture_" + player.playerState.playerNumber + Time.time + Random.value, tailTexture, false);
        }
    }
}