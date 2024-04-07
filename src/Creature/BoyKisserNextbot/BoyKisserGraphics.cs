using static RoomCamera;

namespace Nyctophobia;

public class BoyKisserGraphics : GraphicsModule
{
    public Boykisser boykisser;
    public int counter;

    public BoyKisserGraphics(PhysicalObject ow) : base(ow, false) => boykisser = ow as Boykisser;

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("BoyKisser", true)
        {
            color = Color.white
        };
        AddToContainer(sLeaser, rCam, null);
        base.InitiateSprites(sLeaser, rCam);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        Vector2 val = Vector2.Lerp(boykisser.bodyChunks[0].lastPos, boykisser.bodyChunks[0].pos, timeStacker);
        sLeaser.sprites[0].SetPosition(val + new Vector2(0f, 30f) - camPos);
        sLeaser.sprites[0].scaleX = 1f;
        sLeaser.sprites[0].scaleY = 1f;

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            if (boykisser.distanceToPlayer < 200f)
            {
                sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName("BoyKisser");
            }
            else
            {
                sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName("BoyKisserDancer_F" + counter / 3);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        counter++;
        if (counter > 3 * 51) 
        {
            counter = 0;
        }
    }
}
