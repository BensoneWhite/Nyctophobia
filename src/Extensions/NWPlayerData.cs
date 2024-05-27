namespace Nyctophobia;

public class NWPlayerData
{
    private static float WingStaminaMaxBase => 1000f;
    private static float WingStaminaRecoveryBase => 2f;
    private static float WingSpeedBase => 5f;

    public int currentDashes = 0;
    public float dashCooldown = 0f;
    public Vector2 dashDirectionX;
    public Vector2 dashDirectionY;
    public float dashDistance = 100f;
    public float maxDashDistance = 0f;
    public bool Dashed;
    public float DoesThatPlayerDashedOrNoBOZO;

    public bool AlwaysFalse;
    public bool AlwaysTrue;

    public readonly bool IsNightWalker;

    public Dictionary<Player, bool> focus = [];
    public Dictionary<Player, bool> canFocus = [];
    public Dictionary<Player, bool> DarkMode = [];

    public Color interpolatedColor;

    public bool CanFly => WingStaminaMax > 0 && WingSpeed > 0;
    public float MinimumFlightStamina => WingStaminaMax * 0.1f;
    public double LowWingStamina => MinimumFlightStamina * 3;

    public int WingStaminaMax => (int)(UnlockedExtraStamina ? (int)(WingStaminaMaxBase * 1.6f) : WingStaminaMaxBase);
    public float WingStaminaRecovery => UnlockedExtraStamina ? WingStaminaRecoveryBase * 1.2f : WingStaminaRecoveryBase;
    public float WingSpeed => UnlockedFasterWings ? WingSpeedBase * 1.3f : WingSpeedBase;

    public bool UnlockedExtraStamina = false;
    public bool UnlockedVerticalFlight = false;
    public bool UnlockedFasterWings = false;

    public int preventGrabs;
    public bool isFlying;
    public float wingStamina;
    public int wingStaminaRecoveryCooldown;
    public int currentFlightDuration;
    public int timeSinceLastFlight;
    public int preventFlight;

    public DynamicSoundLoop Wind;

    public int lastTail;

    public readonly Player player;

    public Color BodyColor;
    public Color EyesColor;
    public Color TailColor;
    public Color Corruption;
    public Color WhiskersColor;

    public FAtlas TailAtlas;

    public static Texture2D TailTextureNW;

    public NWPlayerData(Player player)
    {
        IsNightWalker = player.slugcatStats.name == NTEnums.NightWalker;
        this.player = player;
        interpolatedColor = player.ShortCutColor();

        if (!IsNightWalker)
        {
            return;
        }

        SetupSounds(player);

        lastTail = -1;
        wingStamina = WingStaminaMax;
        timeSinceLastFlight = 200;

        AlwaysFalse = !IsNightWalker;
        AlwaysTrue = IsNightWalker;
    }
    ~NWPlayerData()
    {
        try
        {
            TailAtlas.Unload();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Plugin.DebugError(e);
        }
    }

    private void SetupSounds(Player player)
    {
        Wind = new ChunkDynamicSoundLoop(player.bodyChunks[0])
        {
            sound = NTEnums.Sound.wind,
            Pitch = 1f,
            Volume = 1f
        };
    }

    public void SetupColors(PlayerGraphics pg)
    {
        BodyColor = pg.GetColor(NTEnums.ColorNW.Body) ?? Custom.hexToColor("ffcf0d");
        EyesColor = pg.GetColor(NTEnums.ColorNW.Eyes) ?? Custom.hexToColor("010101");
        TailColor = pg.GetColor(NTEnums.ColorNW.Tail) ?? Custom.hexToColor("ffcf0d");
        Corruption = pg.GetColor(NTEnums.ColorNW.Corruption) ?? Custom.hexToColor("b99999");
        WhiskersColor = pg.GetColor(NTEnums.ColorNW.Whiskers) ?? Custom.hexToColor("010101");
    }

    public void LoadTailAtlas()
    {
        TailTextureNW = new Texture2D(150, 75, TextureFormat.ARGB32, false);
        string tailTextureFile = AssetManager.ResolveFilePath("textures/nightwalkertail.png");
        if (File.Exists(tailTextureFile))
        {
            byte[] rawData = File.ReadAllBytes(tailTextureFile);
            _ = TailTextureNW.LoadImage(rawData);
        }

        Texture2D tailTexture = new(TailTextureNW.width, TailTextureNW.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(TailTextureNW, tailTexture);

        NTUtils.MapTextureColor(tailTexture, 255, Corruption, false);
        NTUtils.MapTextureColor(tailTexture, 0, TailColor);

        TailAtlas = Futile.atlasManager.LoadAtlasFromTexture("Nightwalkertailtexture_" + player.playerState.playerNumber + Time.time + Random.value, tailTexture, false);
    }

    public void NWTailLonger(PlayerGraphics self)
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

            List<BodyPart> bp = [.. self.bodyParts];
            _ = bp.RemoveAll(x => x is TailSegment);
            bp.AddRange(self.tail);

            self.bodyParts = [.. bp];
        }
    }

    public void StopFlight()
    {
        currentFlightDuration = 0;
        timeSinceLastFlight = 0;
        isFlying = false;
    }

    public void InitiateFlight()
    {
        player.bodyMode = BodyModeIndex.Default;
        player.animation = AnimationIndex.None;
        player.wantToJump = 0;
        currentFlightDuration = 0;
        timeSinceLastFlight = 0;
        isFlying = true;
    }

    public bool CanSustainFlight()
    {
        return wingStamina > 0 &&
               preventFlight <= 0 &&
               player.canJump <= 0 &&
               player.canWallJump == 0 &&
               player.Consious &&
               player.bodyMode != BodyModeIndex.Crawl &&
               player.bodyMode != BodyModeIndex.CorridorClimb &&
               player.bodyMode != BodyModeIndex.ClimbIntoShortCut &&
               player.animation != AnimationIndex.HangFromBeam &&
               player.animation != AnimationIndex.ClimbOnBeam &&
               player.bodyMode != BodyModeIndex.WallClimb &&
               player.bodyMode != BodyModeIndex.Swimming &&
               player.animation != AnimationIndex.AntlerClimb &&
               player.animation != AnimationIndex.VineGrab &&
               player.animation != AnimationIndex.ZeroGPoleGrab;
    }
}