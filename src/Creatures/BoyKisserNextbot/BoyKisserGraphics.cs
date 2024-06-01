namespace Nyctophobia;

public class BoyKisserGraphics(PhysicalObject ow) : GraphicsModule(ow, false)
{
    public Boykisser boykisser = ow as Boykisser;
    public int counter;

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("BoyKisser", true)
        {
            color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : Color.white
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
            sLeaser.sprites[i].element = boykisser.distanceToPlayer < 200f
                ? Futile.atlasManager.GetElementWithName("BoyKisser")
                : Futile.atlasManager.GetElementWithName("BoyKisserDancer_F" + (counter / 3));
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