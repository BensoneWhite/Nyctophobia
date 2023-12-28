namespace Witness;

public class WSPlayerData
{
    public readonly bool IsWitness;

    public int lastTail;

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

        if (!IsWitness)
        {
            return;
        }
    }

    public void SetupColorsWS(PlayerGraphics pg)
    {
        BodyColor = pg.GetColor(NTEnums.Color.Body) ?? Custom.hexToColor("ffcf0d");
        EyesColor = pg.GetColor(NTEnums.Color.Eyes) ?? Custom.hexToColor("010101");
        TailColor = pg.GetColor(NTEnums.Color.Tail) ?? Custom.hexToColor("ffcf0d");
    }

    public void WSTail(PlayerGraphics self)
    {
        var currentFood = self.player.CurrentFood;
        var oldTail = self.tail;

        if (currentFood < 6)
        {
            if (lastTail != 2)
            {
                lastTail = 2;
                self.tail = new TailSegment[5];
                self.tail[0] = new TailSegment(self, 8f, 4f, null, 0.85f, 1f, 1f, true);
                self.tail[1] = new TailSegment(self, 6f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
                self.tail[2] = new TailSegment(self, 4f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
                self.tail[3] = new TailSegment(self, 2f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
                self.tail[4] = new TailSegment(self, 1f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);
            }
        }
        else if (currentFood < 11)
        {
            if (lastTail != 3)
            {
                lastTail = 3;
                self.tail = new TailSegment[6];
                self.tail[0] = new TailSegment(self, 10f, 4f, null, 0.85f, 1f, 1f, true);
                self.tail[1] = new TailSegment(self, 8f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
                self.tail[2] = new TailSegment(self, 6f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
                self.tail[3] = new TailSegment(self, 4f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
                self.tail[4] = new TailSegment(self, 2f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);
                self.tail[5] = new TailSegment(self, 1f, 7f, self.tail[4], 0.85f, 1f, 0.5f, true);
            }
        }
        else if (lastTail != 4)
        {
            lastTail = 4;
            self.tail = new TailSegment[7];
            self.tail[0] = new TailSegment(self, 11f, 4f, null, 0.85f, 1f, 1f, true);
            self.tail[1] = new TailSegment(self, 9f, 7f, self.tail[0], 0.85f, 1f, 0.5f, true);
            self.tail[2] = new TailSegment(self, 7f, 7f, self.tail[1], 0.85f, 1f, 0.5f, true);
            self.tail[3] = new TailSegment(self, 6f, 7f, self.tail[2], 0.85f, 1f, 0.5f, true);
            self.tail[4] = new TailSegment(self, 4f, 7f, self.tail[3], 0.85f, 1f, 0.5f, true);
            self.tail[5] = new TailSegment(self, 3f, 7f, self.tail[4], 0.85f, 1f, 0.5f, true);
            self.tail[6] = new TailSegment(self, 2f, 7f, self.tail[5], 0.85f, 1f, 0.5f, true);
        }

        if (oldTail != self.tail)
        {
            for (var i = 0; i < self.tail.Length && i < oldTail.Length; i++)
            {
                self.tail[i].pos = oldTail[i].pos;
                self.tail[i].lastPos = oldTail[i].lastPos;
                self.tail[i].vel = oldTail[i].vel;
                self.tail[i].terrainContact = oldTail[i].terrainContact;
                self.tail[i].stretched = oldTail[i].stretched;
            }

            var bp = self.bodyParts.ToList();
            bp.RemoveAll(x => x is TailSegment);
            bp.AddRange(self.tail);

            self.bodyParts = bp.ToArray();
        }
    }

    public void SetupTailTextureWS(WSPlayerData nt)
    {
        if (nt.TailElement != null) return;

        var tailTexture = new Texture2D(Plugin.TailTextureWS.width, Plugin.TailTextureWS.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(Plugin.TailTextureWS, tailTexture);

        NTUtils.MapTextureColor(tailTexture, 0, TailColor);

        if (PlayerRef.TryGetTarget(out var player))
        {
            TailAtlas = Futile.atlasManager.LoadAtlasFromTexture("Witnesscattailtexture_" + player.playerState.playerNumber + Time.time + Random.value, tailTexture, false);
        }
    }
}
