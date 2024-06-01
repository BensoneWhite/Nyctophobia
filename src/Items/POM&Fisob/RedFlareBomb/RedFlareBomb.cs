namespace Nyctophobia;

public class RedFlareBomb : FlareBomb, IRedFlareBomb
{
    public RedFlareBomb(AbstractConsumable abstractConsumable, World world) : base(abstractConsumable, world)
    {
        if (IsPrideDay)
            color = new Color(Random.value, Random.value, Random.value);
        else
            color = new(Random.Range(0.6f, 1f), 0f, Random.Range(0.2f, 0.3f));
    }

    public Color flareColor = new(Random.Range(0.6f, 1f), 0f, Random.Range(0.2f, 0.3f));

    public void Init(FlareBomb flare)
    {
        if (IsPrideDay)
            flare.color = new Color(Random.value, Random.value, Random.value);
        else
            flare.color = flareColor;
    }

    public void Update(FlareBomb flare, bool eu)
    {
    }

    public void NewRoom(FlareBomb flare, Room newRoom)
    {
    }

    public void HitSomething(FlareBomb flare, CollisionResult result, bool eu)
    {
    }

    public void Thrown(FlareBomb flare, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
    }

    public void PickedUp(FlareBomb flare, Creature upPicker)
    {
    }

    public void InitiateSprites(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam)
    {
    }

    public void AddToContainer(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
    }

    public void ApplyPalette(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (IsPrideDay)
        {
            sLeaser.sprites[0].color = new Color(Random.value, Random.value, Random.value);
            sLeaser.sprites[2].color = new Color(Random.value, Random.value, Random.value);
        }
        else
        {
            sLeaser.sprites[0].color = new Color(1f, 1f, 1f);
            sLeaser.sprites[2].color = flareColor;
        }
    }

    public void DrawSprites(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(flare.firstChunk.lastPos, flare.firstChunk.pos, timeStacker);
        if (flare.vibrate > 0)
        {
            vector += Custom.DegToVec(Random.value * 360f) * 2f * Random.value;
        }

        sLeaser.sprites[0].x = vector.x - camPos.x;
        sLeaser.sprites[0].y = vector.y - camPos.y;
        if (flare.burning == 0f)
        {
            sLeaser.sprites[2].shader = rCam.room.game.rainWorld.Shaders["FlatLightBehindTerrain"];
            sLeaser.sprites[2].x = vector.x - camPos.x;
            sLeaser.sprites[2].y = vector.y - camPos.y;
        }
        else
        {
            sLeaser.sprites[2].shader = rCam.room.game.rainWorld.Shaders["FlareBomb"];
            sLeaser.sprites[2].x = vector.x - camPos.x + Mathf.Lerp(flare.lastFlickerDir.x, flare.flickerDir.x, timeStacker);
            sLeaser.sprites[2].y = vector.y - camPos.y + Mathf.Lerp(flare.lastFlickerDir.y, flare.flickerDir.y, timeStacker);
            sLeaser.sprites[2].scale = Mathf.Lerp(flare.lastFlashRad, flare.flashRad, timeStacker) / 16f;
            sLeaser.sprites[2].alpha = Mathf.Lerp(flare.lastFlashAlpha, flare.flashAplha, timeStacker);
        }

        if (flare.mode == Mode.Thrown)
        {
            sLeaser.sprites[1].isVisible = true;
            Vector2 vector2 = Vector2.Lerp(flare.tailPos, flare.firstChunk.lastPos, timeStacker);
            Vector2 vector3 = Custom.PerpendicularVector((vector - vector2).normalized);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(0, vector + vector3 * 3f - camPos);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(1, vector - vector3 * 3f - camPos);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(2, vector2 - camPos);
            if (IsPrideDay)
                (sLeaser.sprites[1] as TriangleMesh).verticeColors[2] = new Color(Random.value, Random.value, Random.value);
            else
                (sLeaser.sprites[1] as TriangleMesh).verticeColors[2] = flareColor;
        }
        else
        {
            sLeaser.sprites[1].isVisible = false;
        }

        if (flare.slatedForDeletetion || flare.room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}