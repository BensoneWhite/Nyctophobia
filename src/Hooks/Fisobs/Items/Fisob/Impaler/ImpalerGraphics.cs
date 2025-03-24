namespace Nyctophobia;

public class ImpalerRealizer(AbstractPhysicalObject abstractPhysicalObject, World world) : Spear(abstractPhysicalObject, world)
{
    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("Impaler", true);
        sLeaser.sprites[1] = new FSprite("ImpalerCol", true)
        {
            alpha = 0.4f
        };
        AddToContainer(sLeaser, rCam, null);
    }

    public override void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (IsPrideDay)
        {
            color = new Color(Random.value, Random.value, Random.value);
            sLeaser.sprites[0].color = new Color(Random.value, Random.value, Random.value);
            sLeaser.sprites[1].color = new Color(Random.value, Random.value, Random.value);
        }
        else
        {
            color = palette.blackColor;
            sLeaser.sprites[0].color = palette.blackColor;
            sLeaser.sprites[1].color = palette.fogColor;
        }
    }

    public override void AddToContainer(SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        base.AddToContainer(sLeaser, rCam, newContatiner);
        rCam.ReturnFContainer("Background").AddChild(sLeaser.sprites[0]);
        rCam.ReturnFContainer("Background").AddChild(sLeaser.sprites[1]);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 a = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        if (vibrate > 0)
        {
            a += Custom.DegToVec(Random.value * 360f) * 2f * Random.value;
        }
        Vector3 v = Vector3.Slerp(lastRotation, rotation, timeStacker);
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = a.x - camPos.x;
            sLeaser.sprites[i].y = a.y - camPos.y;
            sLeaser.sprites[i].anchorY = Mathf.Lerp((!lastPivotAtTip) ? 0.5f : 0.85f, (!pivotAtTip) ? 0.5f : 0.85f, timeStacker);
            sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), v);
        }
        if (blink > 0 && Random.value < 0.5f)
        {
            if (IsPrideDay)
            {
                sLeaser.sprites[0].color = new Color(Random.value, Random.value, Random.value);
                sLeaser.sprites[1].color = new Color(Random.value, Random.value, Random.value);
            }
            else
            {
                sLeaser.sprites[0].color = blinkColor;
                sLeaser.sprites[1].color = blinkColor;
            }
        }
        else
        {
            if (IsPrideDay)
            {
                sLeaser.sprites[0].color = new Color(Random.value, Random.value, Random.value);
                sLeaser.sprites[1].color = new Color(Random.value, Random.value, Random.value);
            }
            else
            {
                sLeaser.sprites[0].color = color;
                sLeaser.sprites[1].color = rCam.currentPalette.fogColor;
            }
        }
        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        if ((abstractPhysicalObject as ImpalerAbstract).StuckInWall)
        {
            stuckInWall = new Vector2?(placeRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile));
            ChangeMode(Mode.StuckInWall);
        }
    }
}