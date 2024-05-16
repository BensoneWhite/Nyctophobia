namespace Nyctophobia;

public class Whiskerdata(Player player)
{
    public bool ready = false;
    public int initialfacewhiskerloc;
    public string sprite = "LizardScaleA0";
    public string facesprite = "LizardScaleA0";
    public WeakReference<Player> playerref = new(player);
    public Vector2[] headpositions = new Vector2[6];
    public Scale[] headScales = new Scale[6];
    public Color headcolor = new(1f, 1f, 0f);

    public int Facewhiskersprite(int side, int pair)
    {
        return initialfacewhiskerloc + side + pair + pair;
    }

    public class Scale(GraphicsModule cosmetics) : BodyPart(cosmetics)
    {
        public float length = 5f;
        public float width = 2f;

        public override void Update()
        {
            base.Update();
            if (owner.owner.room.PointSubmerged(pos))
            {
                vel *= 0.5f;
            }
            else
            {
                vel *= 0.9f;
            }

            lastPos = pos;
            pos += vel;
        }
    }
}

public class NWSmoke : CosmeticSprite
{
    public float lifeTime;
    public float life;
    public float lastLife;
    public Color color;
    public float size;

    public NWSmoke(Vector2 pos, Color color, float size)
    {
        base.pos = pos;
        this.color = color;
        this.size = size;
        lastPos = pos;
        vel = Custom.RNV() * 1.5f * Random.value;
        life = 10f;
        lifeTime = Mathf.Lerp(10f, 40f, Random.value);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        vel *= 0.8f;
        vel.y += 0.1f;
        vel += Custom.RNV() * Random.value * 0.5f;
        lastLife = life;
        life -= 1f / lifeTime;
        if (life < 0f)
        {
            Destroy();
        }
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("deerEyeB");
        AddToContainer(sLeaser, rCam, null);
        base.InitiateSprites(sLeaser, rCam);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        sLeaser.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, timeStacker) - camPos.x;
        sLeaser.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, timeStacker) - camPos.y;
        float lifetimeMult = Mathf.Lerp(lastLife, life, timeStacker);
        sLeaser.sprites[0].scale = size * lifetimeMult;
        sLeaser.sprites[0].color = color;
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }
}