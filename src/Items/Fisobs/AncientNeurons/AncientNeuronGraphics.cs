namespace Nyctophobia;

public class AncientNeuronGraphics : GraphicsModule
{
    private const int meshSegs = 9;

    private const float squeeze = -0.1f;
    private const float squirmAdd = 0;
    private const float squirmWidth = 0;
    private const float squirmAmp = 0;

    private readonly AncientNeuron aneuron;
    private Vector2[,] body = new Vector2[2, 3];
    private float[,] squirm = new float[meshSegs, 3];
    private readonly float sizeFac;

    private float squirmOffset;
    private float darkness;
    private float lastDarkness;
    private Color yellow;

    private RoomPalette roomPalette;

    private ChunkSoundEmitter soundLoop;

    private TriangleMesh[] m = new TriangleMesh[2]; // mesh sprites 0 and 1

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

        aneuron.headdir = (aneuron.headdir + Custom.DirVec(body[0, 0], aneuron.firstChunk.pos) * 0.2f).normalized;

        squirmOffset += squirmAdd * 0.2f;
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = m[0] = TriangleMesh.MakeLongMesh(meshSegs, false, true);
        sLeaser.sprites[1] = m[1] = TriangleMesh.MakeLongMesh(meshSegs - 3, false, true);

        AddToContainer(sLeaser, rCam, null);

        base.InitiateSprites(sLeaser, rCam);
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
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

        if (culled)
        {
            return;
        }

        if (sLeaser.sprites == null || sLeaser.sprites.Length == 0)
        {
            Plugin.DebugError("Sprites array is null or empty in DrawSprites");
            return;
        }

        Vector2 chk0Pos = Vector2.Lerp(aneuron.firstChunk.lastPos, aneuron.firstChunk.pos, timeStacker);
        Vector2 bodyPos = Vector2.Lerp(body[0, 1], body[0, 0], timeStacker);
        Vector2 headPos = Vector2.Lerp(body[1, 1], body[1, 0], timeStacker);
        Vector2 segmentDir = -Vector3.Slerp(aneuron.lastheaddir, aneuron.headdir, timeStacker);
        Vector2 chkDir = Custom.DirVec(chk0Pos, bodyPos);
        Vector2 bodyDir = Custom.DirVec(bodyPos, headPos);

        if (aneuron.room != null)
        {
            lastDarkness = darkness;
            darkness = aneuron.room.DarknessOfPoint(rCam, bodyPos);
            if (darkness != lastDarkness)
            {
                ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
        }

        chk0Pos -= segmentDir * 7f * sizeFac;
        Vector2 vector4 = chk0Pos - segmentDir * 18f;
        Vector2 v = vector4;
        float num = 0f;
        float num2 = Custom.LerpMap(Vector2.Distance(chk0Pos, bodyPos) + Vector2.Distance(bodyPos, headPos), 20f, 140f, 1f, 0.3f, 2f);
        Vector2 a4 = Custom.DegToVec(-45f);

        for (int i = 0; i < meshSegs; i++)
        {
            float iN = Mathf.InverseLerp(1f, meshSegs - 1, i); // i, normalized
            float num5 = i < 2 ? (0.5f + i) : (Custom.LerpMap(iN, 0f, 1f, 1f, 0.5f));
            num += 1f;
            v -= segmentDir * (7f * num5 * sizeFac);
            Vector2 vector5 = v - Custom.PerpendicularVector(segmentDir) * squirmWidth * 7f * squirm[i, 2] * sizeFac;
            Vector2 vector6 = v + Custom.PerpendicularVector(segmentDir) * squirmWidth * 7f * squirm[i, 2] * sizeFac;

            float num6 = squirmAmp * Mathf.Sin(squirmOffset + squirm[i, 2] * squirmWidth * squirm[i, 2] * sizeFac);

            (sLeaser.sprites[0] as TriangleMesh)?.MoveVertice(i * 4, vector5 - segmentDir * (num6 + 1f));
            (sLeaser.sprites[0] as TriangleMesh)?.MoveVertice(i * 4 + 1, vector5 + segmentDir * (num6 + 1f));
            (sLeaser.sprites[0] as TriangleMesh)?.MoveVertice(i * 4 + 2, vector6 - segmentDir * (num6 + 1f));
            (sLeaser.sprites[0] as TriangleMesh)?.MoveVertice(i * 4 + 3, vector6 + segmentDir * (num6 + 1f));

            if (i < meshSegs - 2)
            {
                float num7 = Mathf.InverseLerp(0f, meshSegs - 3, i);

                Vector2 a5 = chk0Pos + segmentDir * (squirmWidth * 7f * squirm[i, 2] * sizeFac);
                Vector2 vector7 = a5 + Custom.PerpendicularVector(segmentDir) * squirmWidth * 7f * squirm[i, 2] * sizeFac;
                Vector2 vector8 = a5 - Custom.PerpendicularVector(segmentDir) * squirmWidth * 7f * squirm[i, 2] * sizeFac;

                float num8 = squirmAmp * Mathf.Sin(squirmOffset + squirm[i, 2] * squirmWidth * squirm[i, 2] * sizeFac);

                (sLeaser.sprites[1] as TriangleMesh)?.MoveVertice(i * 4, vector7 - segmentDir * (num8 + 1f));
                (sLeaser.sprites[1] as TriangleMesh)?.MoveVertice(i * 4 + 1, vector7 + segmentDir * (num8 + 1f));
                (sLeaser.sprites[1] as TriangleMesh)?.MoveVertice(i * 4 + 2, vector8 - segmentDir * (num8 + 1f));
                (sLeaser.sprites[1] as TriangleMesh)?.MoveVertice(i * 4 + 3, vector8 + segmentDir * (num8 + 1f));
            }
        }
    }

    public override void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        for (int j = 0; j < 2; j++)
        {
            sLeaser.sprites[j].color = palette.blackColor;
        }
    }
}