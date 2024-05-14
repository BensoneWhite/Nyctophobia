namespace Nyctophobia;

public class BlueBomba : ScavengerBomb
{
    public BlueBomba(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0, 0), 10f, 0.1f);
        bodyChunkConnections = [];
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.4f;
        surfaceFriction = 0.4f;
        collisionLayer = 2;
        waterFriction = 0.98f;
        buoyancy = 0.4f;
        firstChunk.loudness = 7f;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[spikes.Length + 4];
        Random.State state = Random.state;
        Random.InitState(abstractPhysicalObject.ID.RandomSeed);
        for (int i = 0; i < 2; i++)
        {
            sLeaser.sprites[i] = new FSprite("pixel")
            {
                scaleX = Mathf.Lerp(1f, 2f, Mathf.Pow(Random.value, 1.8f)),
                scaleY = Mathf.Lerp(4f, 7f, Random.value)
            };
        }
        for (int j = 0; j < spikes.Length; j++)
        {
            sLeaser.sprites[2 + j] = new FSprite("pixel")
            {
                scaleX = Mathf.Lerp(1.5f, 3f, Random.value),
                scaleY = Mathf.Lerp(5f, 7f, Random.value),
                anchorY = 0f
            };
        }
        sLeaser.sprites[spikes.Length + 2] = new FSprite("Futile_White")
        {
            shader = rCam.game.rainWorld.Shaders["JaggedCircle"],
            scale = (firstChunk.rad + 0.75f) / 10f
        };
        Random.state = state;
        sLeaser.sprites[spikes.Length + 2].alpha = Mathf.Lerp(0.2f, 0.4f, Random.value);
        TriangleMesh.Triangle[] tris =
        [
        new TriangleMesh.Triangle(0, 1, 2)
        ];
        TriangleMesh triangleMesh = new("Futile_White", tris, customColor: true);
        sLeaser.sprites[spikes.Length + 3] = triangleMesh;
        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        if (vibrate > 0)
        {
            vector += Custom.DegToVec(Random.value * 360f) * 2f * Random.value;
        }
        Vector2 vector2 = Vector3.Slerp(lastRotation, rotation, timeStacker);
        sLeaser.sprites[2].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), vector2);
        sLeaser.sprites[spikes.Length + 2].x = vector.x - camPos.x;
        sLeaser.sprites[spikes.Length + 2].y = vector.y - camPos.y;
        for (int i = 0; i < spikes.Length; i++)
        {
            sLeaser.sprites[2 + i].x = vector.x - camPos.x;
            sLeaser.sprites[2 + i].y = vector.y - camPos.y;
            sLeaser.sprites[2 + i].rotation = Custom.VecToDeg(vector2) + spikes[i];
        }
        Color b = new(0.098f, 0.356f, 0.815f);
        Color a = Color.Lerp(b, b, 0.4f + (0.2f * Mathf.Pow(Random.value, 0.2f)));
        a = Color.Lerp(a, new Color(1f, 1f, 1f), Mathf.Pow(Random.value, ignited ? 3f : 30f));
        for (int j = 0; j < 2; j++)
        {
            sLeaser.sprites[j].x = vector.x - camPos.x;
            sLeaser.sprites[j].y = vector.y - camPos.y;
            sLeaser.sprites[j].rotation = Custom.VecToDeg(vector2) + (j * 90f);
            sLeaser.sprites[j].color = a;
        }
        if (mode == Mode.Thrown)
        {
            sLeaser.sprites[spikes.Length + 3].isVisible = true;
            Vector2 vector3 = Vector2.Lerp(tailPos, firstChunk.lastPos, timeStacker);
            Vector2 vector4 = Custom.PerpendicularVector((vector - vector3).normalized);
            (sLeaser.sprites[spikes.Length + 3] as TriangleMesh).MoveVertice(0, vector + (vector4 * 2f) - camPos);
            (sLeaser.sprites[spikes.Length + 3] as TriangleMesh).MoveVertice(1, vector - (vector4 * 2f) - camPos);
            (sLeaser.sprites[spikes.Length + 3] as TriangleMesh).MoveVertice(2, vector3 - camPos);
            (sLeaser.sprites[spikes.Length + 3] as TriangleMesh).verticeColors[0] = color;
            (sLeaser.sprites[spikes.Length + 3] as TriangleMesh).verticeColors[1] = color;
            (sLeaser.sprites[spikes.Length + 3] as TriangleMesh).verticeColors[2] = explodeColor;
        }
        else
        {
            sLeaser.sprites[spikes.Length + 3].isVisible = false;
        }
        if (blink > 0)
        {
            if (blink > 1 && Random.value < 0.5f)
            {
                UpdateColor(sLeaser, blinkColor);
            }
            else
            {
                UpdateColor(sLeaser, color);
            }
        }
        else if (sLeaser.sprites[spikes.Length + 2].color != color)
        {
            UpdateColor(sLeaser, color);
        }
        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}