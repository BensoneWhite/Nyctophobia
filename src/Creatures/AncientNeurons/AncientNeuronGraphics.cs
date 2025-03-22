namespace Nyctophobia;

public class AncientNeuronGraphics : GraphicsModule
{

    private readonly AncientNeuron aneuron;
    private readonly Vector2[,] body = new Vector2[2, 3];

    private readonly float sizeFac;
    public Vector2 lazyDirection;
    private RoomPalette roomPalette = default;

    public Vector2 lastLazyDirection;
    public Vector2 direction;
    public float rotation;
    public Vector2 lastDirection;

    public float lastRotation;

    public bool lastVisible;


    public AncientNeuronGraphics(PhysicalObject ow) : base(ow, false)
    {
        aneuron = ow as AncientNeuron;
        var state = Random.state;
        Random.InitState(aneuron.abstractCreature.ID.RandomSeed);
        sizeFac = Custom.ClampedRandomVariation(0.8f, 0.2f, 0.5f);
        body = new Vector2[2, 3];
        Random.state = state;
    }

    public override void Reset()
    {
        base.Reset();

        Vector2 dir = Custom.RNV();

        for (int i = 0; i < body.GetLength(0); i++)
        {
            body[i, 0] = aneuron.firstChunk.pos - dir * i;
            body[i, 1] = body[i, 0];
            body[i, 2] *= 0f;
        }
    }

    public override void Update()
    {
        base.Update();
        if (culled)
        {
            return;
        }

        for (int i = 0; i < body.GetLength(0); i++)
        {
            body[i, 1] = body[i, 0];
            body[i, 0] += body[i, 2];
            body[i, 2] *= aneuron.airFriction;
        }

        for (int j = 0; j < body.GetLength(0); j++)
        {
            TerrainCollisionData terrainCollisionData = new(body[j, 0], body[j, 1], body[j, 2], (2.5f - j * 0.5f) * sizeFac, default, aneuron.firstChunk.goThroughFloors);
            terrainCollisionData = VerticalCollision(aneuron.room, terrainCollisionData);
            terrainCollisionData = HorizontalCollision(aneuron.room, terrainCollisionData);
            terrainCollisionData = SlopesVertically(aneuron.room, terrainCollisionData);
            body[j, 0] = terrainCollisionData.pos;
            body[j, 2] = terrainCollisionData.vel;

            if (terrainCollisionData.contactPoint.y < 0)
            {
                body[j, 2].x *= 0.4f;
            }

            if (j == 0)
            {
                Vector2 a = Custom.DirVec(body[j, 0], aneuron.firstChunk.pos) * (Vector2.Distance(body[j, 0], aneuron.firstChunk.pos) - 5f * sizeFac);
                body[j, 0] += a;
                body[j, 2] += a;
            }
            else
            {
                Vector2 a = Custom.DirVec(body[j, 0], body[j - 1, 0]) * (Vector2.Distance(body[j, 0], body[j - 1, 0]) - 5f * sizeFac);
                body[j, 0] += a * 0.5f;
                body[j, 2] += a * 0.5f;
                body[j - 1, 0] -= a * 0.5f;
                body[j - 1, 2] -= a * 0.5f;
            }
        }

        float d = Mathf.Pow(Mathf.InverseLerp(0.25f, -0.75f, Vector2.Dot((aneuron.firstChunk.pos - body[0, 0]).normalized, (body[0, 0] - body[1, 0]).normalized)), 2f);
        body[1, 2] -= Custom.DirVec(body[1, 0], aneuron.firstChunk.pos) * d * 3f * sizeFac;
        body[1, 0] -= Custom.DirVec(body[1, 0], aneuron.firstChunk.pos) * d * 3f * sizeFac;

        aneuron.needleDir = (aneuron.needleDir + Custom.DirVec(body[0, 0], aneuron.firstChunk.pos) * 0.2f).normalized;
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {

        sLeaser.sprites = new FSprite[5];
        sLeaser.sprites[0] = new FSprite("Futile_White");
        sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["FlatLightBehindTerrain"];
        sLeaser.sprites[0].scale = 1.5f;
        sLeaser.sprites[0].alpha = 0.2f;
        sLeaser.sprites[1] = new FSprite("JetFishEyeA");
        sLeaser.sprites[1].scaleY = 1.2f * 1.1f;
        sLeaser.sprites[1].scaleX = 0.75f * 1.1f;
        for (int i = 0; i < 2; i++)
        {
            sLeaser.sprites[2 + i] = new FSprite("deerEyeA2");
            sLeaser.sprites[2 + i].anchorX = 0f;
        }
        sLeaser.sprites[4] = new FSprite("JetFishEyeB");
        sLeaser.sprites[4].color = new Color(1f, 1f, 1f);
        AddToContainer(sLeaser, rCam, null);
    }

    public override void AddToContainer(SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Midground");

        if (newContainer != null)
        {
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (sLeaser.sprites[i] != null)
                {
                    newContainer.AddChild(sLeaser.sprites[i]);
                }
            }
        }
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {

        Vector2 pos = Vector2.Lerp(aneuron.firstChunk.lastPos, aneuron.firstChunk.pos, timeStacker);
        bool flag = rCam.room.ViewedByAnyCamera(pos, 48f);
        if (flag != lastVisible)
        {
            for (int i = 0; i <= 4; i++)
            {
                sLeaser.sprites[i].isVisible = flag;
            }
            lastVisible = flag;
        }
        if (flag)
        {
            Vector2 vector = Vector3.Slerp(lastDirection, direction, timeStacker);
            Vector2 vector2 = Vector3.Slerp(lastLazyDirection, lazyDirection, timeStacker);
            Vector3 vector3 = Custom.PerpendicularVector(vector);
            float num = Mathf.Sin(Mathf.Lerp(lastRotation, rotation, timeStacker) * (float)Math.PI * 2f);
            float num2 = Mathf.Cos(Mathf.Lerp(lastRotation, rotation, timeStacker) * (float)Math.PI * 2f);
            sLeaser.sprites[0].x = pos.x - camPos.x;
            sLeaser.sprites[0].y = pos.y - camPos.y;
            sLeaser.sprites[1].x = pos.x - camPos.x;
            sLeaser.sprites[1].y = pos.y - camPos.y;
            sLeaser.sprites[4].x = pos.x + vector3.x * 2f * num2 * Mathf.Sign(num) - camPos.x;
            sLeaser.sprites[4].y = pos.y + vector3.y * 2f * num2 * Mathf.Sign(num) - camPos.y;
            sLeaser.sprites[1].rotation = Custom.VecToDeg(vector);
            sLeaser.sprites[4].rotation = Custom.VecToDeg(vector);
            sLeaser.sprites[4].scaleX = 1f - Mathf.Abs(num2);
            sLeaser.sprites[1].isVisible = true;
            sLeaser.sprites[0].isVisible = true;
            for (int j = 0; j < 2; j++)
            {
                sLeaser.sprites[2 + j].x = pos.x - vector.x * 4f - camPos.x;
                sLeaser.sprites[2 + j].y = pos.y - vector.y * 4f - camPos.y;
                sLeaser.sprites[2 + j].rotation = Custom.VecToDeg(vector2) + 90f + ((j == 0) ? (-1f) : 1f) * Custom.LerpMap(Vector2.Distance(vector, vector2), 0.06f, 0.7f, 10f, 45f, 2f) * num;
            }
            sLeaser.sprites[2].scaleY = -1f * num;
            sLeaser.sprites[3].scaleY = num;
            if (aneuron.slatedForDeletetion || aneuron.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        if (aneuron.qmode == AncientNeuron.Col.red)
        {
            ApplyPalettered(sLeaser, rCam, roomPalette);
        }

        if (aneuron.qmode == AncientNeuron.Col.white)
        {
            ApplyPalette(sLeaser, rCam, roomPalette);
        }
        if (aneuron.qmode == AncientNeuron.Col.yellow)
        {
            ApplyPaletteyellow(sLeaser, rCam, roomPalette);
        }

    }

    public override void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        for (int j = 0; j < 2; j++)
        {
            sLeaser.sprites[j].color = new Color(1f, 1f, 1f);
        }
    }

    public void ApplyPalettered(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        for (int j = 0; j < 2; j++)
        {
            sLeaser.sprites[j].color = new Color(1f, 0f, 0f);
        }
    }

    public void ApplyPaletteyellow(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        for (int j = 0; j < 2; j++)
        {
            sLeaser.sprites[j].color = new Color(1f, 1f, 0f);
        }
    }
}