
using System.Drawing.Text;
using Novell.Directory.Ldap.Rfc2251;

namespace Nyctophobia;

public class AncientNeuronGraphics : GraphicsModule
{
    private const int meshSegs = 4;

    private readonly AncientNeuron aneuron;
    private readonly Vector2[,] body = new Vector2[2, 3];

    private readonly float sizeFac;
    public Vector2 lazyDirection;
    RoomPalette roomPalette;

    public Vector2 lastLazyDirection;
    public Vector2 direction;
    public float rotation;
    public Vector2 lastDirection;
    public float darkness;
    public float lastDarkness;

    public float lastRotation;

    public bool lastVisible;
    private readonly TriangleMesh[] m = new TriangleMesh[2]; // mesh sprites 0 and 1

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


    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = m[0] = TriangleMesh.MakeLongMesh(meshSegs, false, true);
        sLeaser.sprites[1] = m[1] = TriangleMesh.MakeLongMesh(meshSegs - 1, false, true);

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

        if (culled) {
            return;
        }

        Vector2 chk0Pos = Vector2.Lerp(aneuron.firstChunk.lastPos, aneuron.firstChunk.pos, timeStacker);
        Vector2 bodyPos = Vector2.Lerp(body[0, 1], body[0, 0], timeStacker);
        Vector2 headPos = Vector2.Lerp(body[1, 1], body[1, 0], timeStacker);
        Vector2 segmentDir = -Vector3.Slerp(aneuron.lastheaddir, aneuron.headdir, timeStacker);
        Vector2 chkDir = Custom.DirVec(chk0Pos, bodyPos);
        Vector2 bodyDir = Custom.DirVec(bodyPos, headPos);

        if (aneuron.room != null) {
            
            lastDarkness = darkness;
            darkness = aneuron.room.DarknessOfPoint(rCam, bodyPos);
            if (darkness != lastDarkness) {
                ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
        }

        chk0Pos -= segmentDir * 7f * sizeFac;
        headPos += chkDir * 7f * sizeFac;
        Vector2 vector4 = chk0Pos - segmentDir * 18f;
        Vector2 v = vector4;
        float num = 0f;
        float num2 = Custom.LerpMap(Vector2.Distance(chk0Pos, bodyPos) + Vector2.Distance(bodyPos, headPos), 20f, 140f, 1f, 0.3f, 2f);
        Vector2 a4 = Custom.DegToVec(-45f);

        for (int i = 0; i < meshSegs; i++) {
            float iN = Mathf.InverseLerp(1f, meshSegs - 1, i); // i, normalized
            float num5 = i < 2 ? (0.5f + i) : (Custom.LerpMap(iN, 0.5f, 1f, Mathf.Lerp(3f, 2.5f, iN), 1f, 3f) * num2);

            num5 *= sizeFac;

            Vector2 vector5;
            if (i == 0) {
                vector5 = chk0Pos - segmentDir * 4f;
            } else if (iN < 0.5f) {
                vector5 = Custom.Bezier(chk0Pos, chk0Pos + segmentDir * 2f, bodyPos, bodyPos - chkDir * 4f, Mathf.InverseLerp(0f, 0.5f, iN));
            } else {
                vector5 = Custom.Bezier(bodyPos, bodyPos + chkDir * 4f, headPos, headPos - bodyDir * 2f, Mathf.InverseLerp(0.5f, 1f, iN));
            }

            Vector2 vector6 = vector5;
            Vector2 a5 = Custom.PerpendicularVector(vector6, v);
            Vector2 a6 = Custom.PerpendicularVector(vector5, vector4);
            //body
            m[0].MoveVertice(i * 4, (vector4 + vector5) / 2f - a6 * (num5 + num) * 0.5f - camPos);
            m[0].MoveVertice(i * 4 + 1, (vector4 + vector5) / 2f + a6 * (num5 + num) * 0.5f - camPos);
            m[0].MoveVertice(i * 4 + 2, vector5 - a6 * num5 - camPos);
            m[0].MoveVertice(i * 4 + 3, vector5 + a6 * num5 - camPos);
            //wings it seems
            /*
            if (i is > 1 and < (meshSegs - 1)) 
            {
                float d = Mathf.Lerp(0.2f, 0.5f, Mathf.Sin(3.1415927f * Mathf.Pow(Mathf.InverseLerp(2f, meshSegs - 2, i), 0.5f)));
                m[1].MoveVertice((i - 2) * 4, (vector4 + a4 * num * d + vector5 + a4 * num5 * d) / 2f - a6 * (num5 + num) * 0.5f * d - camPos);
                m[1].MoveVertice((i - 2) * 4 + 1, (vector4 + a4 * num * d + vector5 + a4 * num5 * d) / 2f + a6 * (num5 + num) * 0.5f * d - camPos);
                m[1].MoveVertice((i - 2) * 4 + 2, vector5 + a4 * num5 * d - a6 * num5 * d - camPos);
                m[1].MoveVertice((i - 2) * 4 + 3, vector5 + a4 * num5 * d + a6 * num5 * d - camPos);
            }
            */

            vector4 = vector5;
            v = vector6;
            num = num5;
        }

        const float wingsSize = .7f;

        for (int m = 0; m < 2; m++) {
            Vector2 firstChunkPos = Vector2.Lerp(aneuron.firstChunk.lastPos, aneuron.firstChunk.pos, timeStacker);

            Vector2 wingVert = firstChunkPos;

            if (aneuron.Consious && aneuron.grasps[0] == null) {
                Vector2 wingVertOff = new(m == 0 ? 1f : -1f, 0);

                wingVert += (wingVertOff + segmentDir * .1f).normalized * wingsSize * 33f;
            } else {
                wingVert += (segmentDir + Custom.PerpendicularVector(segmentDir) * (m == 0 ? 1f : -1f) * .2f).normalized * wingsSize * 33f;
            }

            Vector2 offset2 = Vector3.Slerp(Custom.PerpendicularVector(segmentDir) * (m == 0 ? -1f : 1f), new Vector2(m == 0 ? -1f : 1f, 0f), num);

            
        }
        if (aneuron.qmode==AncientNeuron.nmode.red)
        {
        ApplyPalettered(sLeaser, rCam, roomPalette);
        }
        if (aneuron.qmode==AncientNeuron.nmode.white)
        {
        ApplyPalette(sLeaser, rCam, roomPalette);
        }
        if (aneuron.qmode==AncientNeuron.nmode.yellow)
        {
        ApplyPaletteyellow(sLeaser, rCam, roomPalette);
        }
        
    }

    public override void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        for (int j = 0; j < 2; j++)
        {
            
            sLeaser.sprites[j].color = new Color(1f,1f,1f);
        }
    }
    public void ApplyPalettered(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        for (int j = 0; j < 2; j++)
        {
            
            sLeaser.sprites[j].color = new Color(1f,0f,0f);
        }
    }
    public void ApplyPaletteyellow(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        for (int j = 0; j < 2; j++)
        {
            
            sLeaser.sprites[j].color = new Color(1f,1f,0f);
        }
    }
}
