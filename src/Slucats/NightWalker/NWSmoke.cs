namespace Nyctophobia;

//Vulture mask like the scav boss

//if (base.slatedForDeletetion)
//		{
//			return;
//		}
//		Forbid();
//if (grabbedBy.Count == 0)
//{
//base.bodyChunks[0].pos = new Vector2(-10000f, -10000f);
//waitCount--;
//    if (waitCount <= 0)
//    {
//        Destroy();
//    }
//}
//else
//{
//    base.bodyChunks[0].pos = grabbedBy[0].grabber.mainBodyChunk.pos;
//    lastRotationA = rotationA;
//    lastRotationB = rotationB;
//    rotationA = grabbedBy[0].grabber.mainBodyChunk.Rotation;
//    donned = 1f;
//    lastDonned = 1f;
//}
//int waitCount = 5;
//VultureMask.AbstractVultureMask abstractVultureMask = new VultureMask.AbstractVultureMask(world, null, abstractCreature.pos, world.game.GetNewID(), 0, king: true);
//abstractVultureMask.realizedObject = (mask = new CustomVultMask(abstractVultureMask, world));
//Grab(mask, 2, 0, Grasp.Shareability.CanNotShare, 9999f, overrideEquallyDominant: true, pacifying: false);
//if (base.grasps[2] == null)
//{
//	Grab(mask, 2, 0, Grasp.Shareability.CanNotShare, 9999f, overrideEquallyDominant: true, pacifying: false);
//}

//new HSLColor(Time.realtimeSinceStartup, 1f, 0.5f).rgb; //RGB color

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
        if (life < 0f) Destroy();
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("deerEyeB");
        AddToContainer(sLeaser, rCam, null);
        base.InitiateSprites(sLeaser, rCam);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        sLeaser.sprites[0].x = Mathf.Lerp(lastPos.x, pos.x, timeStacker) - camPos.x;
        sLeaser.sprites[0].y = Mathf.Lerp(lastPos.y, pos.y, timeStacker) - camPos.y;
        float lifetimeMult = Mathf.Lerp(lastLife, life, timeStacker);
        sLeaser.sprites[0].scale = size * lifetimeMult;
        sLeaser.sprites[0].color = color;
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }
}