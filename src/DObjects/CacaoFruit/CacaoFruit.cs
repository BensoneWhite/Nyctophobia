namespace Nyctophobia;

public class CacaoFruit : ICacaoFruit
{
    public LightSource Glow;
    public Color Color = new(.9f, .82f, .71f);
    public float Flicker;

    public Color cacaoColor = new Color(0.27f, 0.2f, 0.18f);

    public int FoodPoints => 5;

    public void Init(DangleFruit fruit)
    {
    }

    public void Update(DangleFruit fruit, bool eu)
    {
        Flicker = Mathf.Clamp(Flicker + Random.Range(-0.1f, 0.1f), 0.8f, 1.2f);

        if (Glow != null && Glow.room == fruit.room)
        {
            Glow.stayAlive = true;
            Glow.setRad = 50 * Flicker;
            Glow.setPos = fruit.firstChunk.pos;
            if (Glow.slatedForDeletetion)
            {
                Glow = null;
            }
        }
        else
        {
            Glow = new LightSource(fruit.firstChunk.pos, environmentalLight: true, Color, fruit);
            Glow.requireUpKeep = true;
            Glow.setRad = 50 * Flicker;
            Glow.setAlpha = 0.6f;
            fruit.room.AddObject(Glow);
        }
    }

    public void NewRoom(DangleFruit dangleFruit, Room newRoom)
    {
    }

    public void ThrowByPlayer(DangleFruit fruit)
    {
    }

    public void BitByPlayer(DangleFruit fruit, Creature.Grasp grasp, bool eu)
    {
    }

    public void InitiateSprites(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
    }

    public void AddToContainer(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
    }

    public void ApplyPalette(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = palette.blackColor;
        if (ModManager.MSC && rCam.room.game.session is StoryGameSession && rCam.room.world.name == "HR")
        {
            fruit.color = Color.Lerp(RainWorld.SaturatedGold, palette.blackColor, fruit.darkness);
        }
        else
        {
            fruit.color = Color.Lerp(cacaoColor, palette.blackColor, fruit.darkness);
        }
    }

    public void DrawSprites(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(fruit.firstChunk.lastPos, fruit.firstChunk.pos, timeStacker);
        Vector2 v = Vector3.Slerp(fruit.lastRotation, fruit.rotation, timeStacker);
        fruit.lastDarkness = fruit.darkness;
        fruit.darkness = rCam.room.Darkness(pos) * (1f - rCam.room.LightSourceExposure(pos));
        if (fruit.darkness != fruit.lastDarkness)
        {
            fruit.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
        }

        if (fruit.blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            sLeaser.sprites[1].color = RainWorld.SaturatedGold;
        }
        else
        {
            sLeaser.sprites[1].color = cacaoColor;
        }
    }
}
