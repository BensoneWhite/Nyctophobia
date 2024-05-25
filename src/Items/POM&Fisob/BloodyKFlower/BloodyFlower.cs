namespace Nyctophobia;

public class BloodyFlower : KarmaFlower, IBloodyFlower
{
    public LightSource Glow;
    public Color Color = new(.969f, .749f, .749f);
    public float Flicker;

    public Color flowerColor = new(.741f, .251f, .251f);

    public new int FoodPoints => 1;

    public BloodyFlower(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        color = flowerColor;
    }

    public void Init(KarmaFlower flower)
    {
    }

    public void Update(KarmaFlower flower, bool eu)
    {
        Flicker = Mathf.Clamp(Flicker + Random.Range(-0.1f, 0.1f), 0.8f, 1.2f);

        if (Glow != null && Glow.room == flower.room)
        {
            Glow.stayAlive = true;
            Glow.setRad = 50 * Flicker;
            Glow.setPos = flower.firstChunk.pos;
            if (Glow.slatedForDeletetion)
            {
                Glow = null;
            }
        }
        else
        {
            Glow = new LightSource(flower.firstChunk.pos, environmentalLight: true, Color, flower)
            {
                requireUpKeep = true,
                setRad = 50 * Flicker,
                setAlpha = 0.6f
            };
            flower.room.AddObject(Glow);
        }
    }

    public void NewRoom(KarmaFlower flower, Room newRoom)
    {
    }

    public void ThrowByPlayer(KarmaFlower flower)
    {
    }

    public void BitByPlayer(KarmaFlower flower, Creature.Grasp grasp, bool eu)
    {
        if (flower.bites < 1)
        {
            flower.room.PlaySound(SoundID.Bomb_Explode, flower.bodyChunks[0].pos);
        }
    }

    public void InitiateSprites(KarmaFlower flower, SpriteLeaser sLeaser, RoomCamera rCam)
    {
    }

    public void AddToContainer(KarmaFlower flower, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
    }

    public void ApplyPalette(KarmaFlower flower, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = palette.blackColor;
        flower.color = ModManager.MSC && rCam.room.game.session is StoryGameSession && rCam.room.world.name == "HR"
            ? Color.Lerp(RainWorld.SaturatedGold, palette.blackColor, flower.darkness)
            : Color.Lerp(flowerColor, palette.blackColor, flower.darkness);
    }

    public void DrawSprites(KarmaFlower flower, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(flower.firstChunk.lastPos, flower.firstChunk.pos, timeStacker);
        flower.lastDarkness = flower.darkness;
        flower.darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
        if (flower.darkness != flower.lastDarkness)
        {
            flower.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }

        sLeaser.sprites[1].color = flower.blink > 0 && Random.value < 0.5f ? RainWorld.SaturatedGold : flowerColor;
    }
}