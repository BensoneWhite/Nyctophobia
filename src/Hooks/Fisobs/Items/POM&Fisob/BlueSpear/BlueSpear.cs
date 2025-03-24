namespace Nyctophobia;

public class BlueSpear : ExplosiveSpear
{
    private static readonly Color BlueColor = new Color(0.098f, 0.356f, 0.815f);
    private const float AirFriction = 0.999f;
    private const float GravityValue = 0.9f;
    private const float BounceValue = 0.4f;
    private const float SurfaceFrictionValue = 0.4f;
    private const int CollisionLayerValue = 2;
    private const float WaterFrictionValue = 0.98f;
    private const float BuoyancyValue = 0.4f;
    private const float FirstChunkLoudness = 7f;
    private const int RagMinCount = 9;
    private const int RagMaxCount = 15;
    private const int MiniExplosionInterval = 20;

    public BlueSpear(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        explodeAt = Random.Range(35, 55);

        int ragSegments = Random.Range(RagMinCount, Random.Range(RagMinCount, RagMaxCount));
        rag = new Vector2[ragSegments, 9];

        redColor = BlueColor;
        explodeColor = BlueColor;
        color = BlueColor;

        for (int i = 0; i < explodeAt / MiniExplosionInterval; i++)
        {
            miniExplosions.Add(Random.Range(i * MiniExplosionInterval, (i + 1) * MiniExplosionInterval));
        }

        bodyChunks = [new BodyChunk(this, 0, new Vector2(0, 0), 10f, 0.1f)];
        bodyChunkConnections = [];
        airFriction = AirFriction;
        gravity = GravityValue;
        bounce = BounceValue;
        surfaceFriction = SurfaceFrictionValue;
        collisionLayer = CollisionLayerValue;
        waterFriction = WaterFrictionValue;
        buoyancy = BuoyancyValue;
        firstChunk.loudness = FirstChunkLoudness;
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (stuckIns != null)
        {
            rCam.ReturnFContainer("HUD").AddChild(stuckIns.label);
        }

        sLeaser.sprites = new FSprite[3];
        sLeaser.sprites[1] = new FSprite("SmallSpear");
        sLeaser.sprites[0] = new FSprite("SpearRag");

        TriangleMesh ragMesh = TriangleMesh.MakeLongMesh(rag.GetLength(0), pointyTip: false, customColor: true);
        ragMesh.shader = rCam.game.rainWorld.Shaders["JaggedSquare"];
        ragMesh.alpha = rCam.game.SeededRandom(abstractPhysicalObject.ID.RandomSeed);
        sLeaser.sprites[2] = ragMesh;

        AddToContainer(sLeaser, rCam, null);
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        firstChunk.HardSetPosition(placeRoom.MiddleOfTile(abstractPhysicalObject.pos));
        rotation = Custom.RNV();
        lastRotation = rotation;
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

        sLeaser.sprites[0].color = BlueColor;
        sLeaser.sprites[2].color = BlueColor;

        if (blink > 0)
        {
            sLeaser.sprites[1].color = blink > 1 && Random.value < 0.5f ? Color.white : color;
        }
        else if (sLeaser.sprites[1].color != color)
        {
            sLeaser.sprites[1].color = color;
        }

        if (mode == Mode.Free && firstChunk.ContactPoint.y < 0)
        {
            sLeaser.sprites[0].anchorY += 0.2f;
        }

        UpdateRagMesh((TriangleMesh)sLeaser.sprites[2], timeStacker, camPos);
    }

    private void UpdateRagMesh(TriangleMesh ragMesh, float timeStacker, Vector2 camPos)
    {
        float accumulatedOffset = 0f;
        Vector2 attachPos = RagAttachPos(timeStacker);

        for (int i = 0; i < rag.GetLength(0); i++)
        {
            float t = i / (float)(rag.GetLength(0) - 1);
            Vector2 currentPos = Vector2.Lerp(rag[i, 1], rag[i, 0], timeStacker);
            float width = (2f + 2f * Mathf.Sin(Mathf.Pow(t, 2f) * Mathf.PI)) *
                          Vector3.Slerp(rag[i, 4], rag[i, 3], timeStacker).x;
            Vector2 dirToAttach = (attachPos - currentPos).normalized;
            Vector2 perpDir = Custom.PerpendicularVector(dirToAttach);
            float distanceFactor = Vector2.Distance(attachPos, currentPos) / 5f;

            ragMesh.MoveVertice(i * 4, attachPos - dirToAttach * distanceFactor - perpDir * (width + accumulatedOffset) * 0.5f - camPos);
            ragMesh.MoveVertice(i * 4 + 1, attachPos - dirToAttach * distanceFactor + perpDir * (width + accumulatedOffset) * 0.5f - camPos);
            ragMesh.MoveVertice(i * 4 + 2, currentPos + dirToAttach * distanceFactor - perpDir * width - camPos);
            ragMesh.MoveVertice(i * 4 + 3, currentPos + dirToAttach * distanceFactor + perpDir * width - camPos);

            attachPos = currentPos;
            accumulatedOffset = width;
        }
    }

    public override void Update(bool eu)
    {
        bool isThrown = mode == Mode.Thrown;
        base.Update(eu);

        UpdateRagPhysics();
        EnforceRagConstraints();

        HandleIgnition(isThrown);

        if (exploded)
        {
            destroyCounter++;
            for (int i = 0; i < 2; i++)
            {
                room.AddObject(new SpearFragment(firstChunk.pos, Custom.RNV() * Mathf.Lerp(20f, 40f, Random.value)));
            }

            room.AddObject(new PuffBallSkin(
                firstChunk.pos + rotation * (pivotAtTip ? 0f : 10f),
                Custom.RNV() * Mathf.Lerp(10f, 30f, Random.value),
                redColor,
                Color.Lerp(redColor, Color.black, 0.3f)));

            if (destroyCounter > 4)
            {
                Destroy();
            }
        }
    }

    private void UpdateRagPhysics()
    {
        int ragLength = rag.GetLength(0);
        for (int i = 0; i < ragLength; i++)
        {
            float t = i / (float)(ragLength - 1);
            rag[i, 1] = rag[i, 0];
            rag[i, 0] += rag[i, 2];
            rag[i, 2] -= rotation * Mathf.InverseLerp(1f, 0f, i) * 0.8f;

            rag[i, 4] = rag[i, 3];
            rag[i, 3] = (rag[i, 3] + rag[i, 5] * Custom.LerpMap(Vector2.Distance(rag[i, 0], rag[i, 1]), 1f, 18f, 0.05f, 0.3f)).normalized;
            rag[i, 5] = (rag[i, 5] + Custom.RNV() * Random.value * Mathf.Pow(Mathf.InverseLerp(1f, 18f, Vector2.Distance(rag[i, 0], rag[i, 1])), 0.3f)).normalized;

            if (room.PointSubmerged(rag[i, 0]))
            {
                rag[i, 2] *= Custom.LerpMap(rag[i, 2].magnitude, 1f, 10f, 1f, 0.5f, Mathf.Lerp(1.4f, 0.4f, t));
                rag[i, 2].y += 0.05f;
                rag[i, 2] += Custom.RNV() * 0.1f;
                continue;
            }

            rag[i, 2] *= Custom.LerpMap(Vector2.Distance(rag[i, 0], rag[i, 1]), 1f, 6f, 0.999f, 0.7f, Mathf.Lerp(1.5f, 0.5f, t));
            rag[i, 2].y -= room.gravity * Custom.LerpMap(Vector2.Distance(rag[i, 0], rag[i, 1]), 1f, 6f, 0.6f, 0f);

            if (i % 3 == 2 || i == ragLength - 1)
            {
                TerrainCollisionData cd = scratchTerrainCollisionData.Set(rag[i, 0], rag[i, 1], rag[i, 2], 1f, new IntVector2(0, 0), goThroughFloors: false);
                cd = HorizontalCollision(room, cd);
                cd = VerticalCollision(room, cd);
                cd = SlopesVertically(room, cd);
                rag[i, 0] = cd.pos;
                rag[i, 2] = cd.vel;

                if (cd.contactPoint.x != 0)
                    rag[i, 2].y *= 0.6f;
                if (cd.contactPoint.y != 0)
                    rag[i, 2].x *= 0.6f;
            }
        }
    }

    private void EnforceRagConstraints()
    {
        int ragLength = rag.GetLength(0);
        for (int j = 0; j < ragLength; j++)
        {
            if (j > 0)
            {
                Vector2 delta = rag[j, 0] - rag[j - 1, 0];
                Vector2 normalized = delta.normalized;
                float distance = delta.magnitude;
                float correctionFactor = distance > conRad ? 0.5f : 0.25f;

                Vector2 correction = normalized * (conRad - distance) * correctionFactor;
                rag[j, 0] += correction;
                rag[j, 2] += correction;
                rag[j - 1, 0] -= correction;
                rag[j - 1, 2] -= correction;

                if (j > 1)
                {
                    normalized = (rag[j, 0] - rag[j - 2, 0]).normalized;
                    rag[j, 2] += normalized * 0.2f;
                    rag[j - 2, 2] -= normalized * 0.2f;
                }

                if (j < ragLength - 1)
                {
                    rag[j, 3] = Vector3.Slerp(rag[j, 3], (rag[j - 1, 3] * 2f + rag[j + 1, 3]) / 3f, 0.1f);
                    rag[j, 5] = Vector3.Slerp(rag[j, 5], (rag[j - 1, 5] * 2f + rag[j + 1, 5]) / 3f,
                        Custom.LerpMap(Vector2.Distance(rag[j, 1], rag[j, 0]), 1f, 8f, 0.05f, 0.5f));
                }
            }
            else
            {
                rag[j, 0] = RagAttachPos(1f);
                rag[j, 2] = Vector2.zero;
            }
        }
    }

    private void HandleIgnition(bool isThrown)
    {
        if (isThrown && mode != Mode.Thrown && igniteCounter < 1)
        {
            Ignite();
        }

        if (Submersion > 0.2f && room.waterObject.WaterIsLethal && igniteCounter < 1)
        {
            Ignite();
        }

        if (igniteCounter > 0)
        {
            int prevCounter = igniteCounter;
            igniteCounter++;
            if (stuckInObject == null)
            {
                igniteCounter++;
            }

            room.AddObject(new Spark(firstChunk.pos + rotation * 15f,
                -rotation * Mathf.Lerp(6f, 11f, Random.value) + Custom.RNV() * Random.value * 1.5f,
                explodeColor, null, 8, 18));
            room.MakeBackgroundNoise(0.5f);

            if (miniExplosions.Count > 0 && prevCounter < miniExplosions[0] && igniteCounter >= miniExplosions[0])
            {
                miniExplosions.RemoveAt(0);
                MiniExplode();
            }

            if (igniteCounter > explodeAt && !exploded)
            {
                Explode();
            }
        }
    }
}
