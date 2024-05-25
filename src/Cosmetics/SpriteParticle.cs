namespace Nyctophobia;

public class SpriteParticle : CosmeticSprite
{
    private readonly int duration;
    private readonly string sprite;
    private readonly bool fade;
    private readonly string container;

    private int lifeTime;


    public SpriteParticle(string sprite, Vector2 pos, Vector2 vel, int duration, bool fade = false, string container = "Midground")
    {
        this.sprite = sprite;
        this.pos = pos;
        lastPos = pos;
        this.vel = vel;
        this.duration = duration;
        this.fade = fade;
        this.container = container;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        lifeTime++;
        if (lifeTime > duration)
        {
            Destroy();
        }
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);

        sLeaser.sprites =
        [
            new(sprite)
        ];

        AddToContainer(sLeaser, rCam, rCam.ReturnFContainer(container));
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

        sLeaser.sprites[0].SetPosition(Vector2.Lerp(lastPos, pos, timeStacker) - camPos);
        if (fade)
        {
            sLeaser.sprites[0].alpha = 1 - Mathf.Lerp(lifeTime - 1, lifeTime, timeStacker) / duration;
        }
    }
}