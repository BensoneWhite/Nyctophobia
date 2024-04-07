namespace Nyctophobia;

public class BlueSpear : ExplosiveSpear
{
    public BlueSpear(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        explodeAt = Random.Range(35, 55);
        rag = new Vector2[Random.Range(9, Random.Range(9, 15)), 9];

        redColor = new(0.098f, 0.356f, 0.815f);
        explodeColor = new(0.098f, 0.356f, 0.815f);
        color = new(0.098f, 0.356f, 0.815f);

        int num = 20;
        for (int i = 0; i < explodeAt / num; i++)
        {
            miniExplosions.Add(Random.Range(i * num, (i + 1) * num));
        }

        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0, 0), 10f, 0.1f);
        bodyChunkConnections = Array.Empty<BodyChunkConnection>();
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
        if (stuckIns != null)
        {
            rCam.ReturnFContainer("HUD").AddChild(stuckIns.label);
        }

        sLeaser.sprites = new FSprite[3];
        sLeaser.sprites[1] = new FSprite("SmallSpear");
        sLeaser.sprites[0] = new FSprite("SpearRag");
        sLeaser.sprites[2] = TriangleMesh.MakeLongMesh(rag.GetLength(0), pointyTip: false, customColor: true);
        sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["JaggedSquare"];
        sLeaser.sprites[2].alpha = rCam.game.SeededRandom(abstractPhysicalObject.ID.RandomSeed);
        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        sLeaser.sprites[0].color = new(0.098f, 0.356f, 0.815f);
        sLeaser.sprites[2].color = new(0.098f, 0.356f, 0.815f);
        if (blink > 0)
        {
            if (blink > 1 && Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = new Color(1f, 1f, 1f);
            }
            else
            {
                sLeaser.sprites[1].color = color;
            }
        }
        else if (sLeaser.sprites[1].color != color)
        {
            sLeaser.sprites[1].color = color;
        }

        if (mode == Mode.Free && firstChunk.ContactPoint.y < 0)
        {
            sLeaser.sprites[0].anchorY += 0.2f;
        }

        float num = 0f;
        Vector2 vector = RagAttachPos(timeStacker);
        for (int i = 0; i < rag.GetLength(0); i++)
        {
            float f = (float)i / (float)(rag.GetLength(0) - 1);
            Vector2 vector2 = Vector2.Lerp(rag[i, 1], rag[i, 0], timeStacker);
            float num2 = (2f + 2f * Mathf.Sin(Mathf.Pow(f, 2f) * (float)Math.PI)) * Vector3.Slerp(rag[i, 4], rag[i, 3], timeStacker).x;
            Vector2 normalized = (vector - vector2).normalized;
            Vector2 vector3 = Custom.PerpendicularVector(normalized);
            float num3 = Vector2.Distance(vector, vector2) / 5f;
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4, vector - normalized * num3 - vector3 * (num2 + num) * 0.5f - camPos);
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 1, vector - normalized * num3 + vector3 * (num2 + num) * 0.5f - camPos);
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 2, vector2 + normalized * num3 - vector3 * num2 - camPos);
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 3, vector2 + normalized * num3 + vector3 * num2 - camPos);
            vector = vector2;
            num = num2;
        }
    }

    public override void Update(bool eu)
    {
        bool flag = mode == Mode.Thrown;
        base.Update(eu);
        for (int i = 0; i < rag.GetLength(0); i++)
        {
            float t = i / (float)(rag.GetLength(0) - 1);
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
            if (i % 3 == 2 || i == rag.GetLength(0) - 1)
            {
                SharedPhysics.TerrainCollisionData cd = scratchTerrainCollisionData.Set(rag[i, 0], rag[i, 1], rag[i, 2], 1f, new IntVector2(0, 0), goThroughFloors: false);
                cd = SharedPhysics.HorizontalCollision(room, cd);
                cd = SharedPhysics.VerticalCollision(room, cd);
                cd = SharedPhysics.SlopesVertically(room, cd);
                rag[i, 0] = cd.pos;
                rag[i, 2] = cd.vel;
                if (cd.contactPoint.x != 0)
                {
                    rag[i, 2].y *= 0.6f;
                }

                if (cd.contactPoint.y != 0)
                {
                    rag[i, 2].x *= 0.6f;
                }
            }
        }

        for (int j = 0; j < rag.GetLength(0); j++)
        {
            if (j > 0)
            {
                Vector2 normalized = (rag[j, 0] - rag[j - 1, 0]).normalized;
                float num = Vector2.Distance(rag[j, 0], rag[j - 1, 0]);
                float num2 = ((num > conRad) ? 0.5f : 0.25f);
                rag[j, 0] += normalized * (conRad - num) * num2;
                rag[j, 2] += normalized * (conRad - num) * num2;
                rag[j - 1, 0] -= normalized * (conRad - num) * num2;
                rag[j - 1, 2] -= normalized * (conRad - num) * num2;
                if (j > 1)
                {
                    normalized = (rag[j, 0] - rag[j - 2, 0]).normalized;
                    rag[j, 2] += normalized * 0.2f;
                    rag[j - 2, 2] -= normalized * 0.2f;
                }

                if (j < rag.GetLength(0) - 1)
                {
                    rag[j, 3] = Vector3.Slerp(rag[j, 3], (rag[j - 1, 3] * 2f + rag[j + 1, 3]) / 3f, 0.1f);
                    rag[j, 5] = Vector3.Slerp(rag[j, 5], (rag[j - 1, 5] * 2f + rag[j + 1, 5]) / 3f, Custom.LerpMap(Vector2.Distance(rag[j, 1], rag[j, 0]), 1f, 8f, 0.05f, 0.5f));
                }
            }
            else
            {
                rag[j, 0] = RagAttachPos(1f);
                rag[j, 2] *= 0f;
            }
        }

        if (flag && mode != Mode.Thrown && igniteCounter < 1)
        {
            Ignite();
        }

        if (Submersion > 0.2f && room.waterObject.WaterIsLethal && igniteCounter < 1)
        {
            Ignite();
        }

        if (igniteCounter > 0)
        {
            int num3 = igniteCounter;
            igniteCounter++;
            if (stuckInObject == null)
            {
                igniteCounter++;
            }

            room.AddObject(new Spark(firstChunk.pos + rotation * 15f, -rotation * Mathf.Lerp(6f, 11f, Random.value) + Custom.RNV() * Random.value * 1.5f, explodeColor, null, 8, 18));
            room.MakeBackgroundNoise(0.5f);
            if (miniExplosions.Count > 0 && num3 < miniExplosions[0] && igniteCounter >= miniExplosions[0])
            {
                miniExplosions.RemoveAt(0);
                MiniExplode();
            }

            if (igniteCounter > explodeAt && !exploded)
            {
                Explode();
            }
        }

        if (exploded)
        {
            destroyCounter++;
            for (int k = 0; k < 2; k++)
            {
                room.AddObject(new SpearFragment(firstChunk.pos, Custom.RNV() * Mathf.Lerp(20f, 40f, Random.value)));
            }

            room.AddObject(new PuffBallSkin(firstChunk.pos + rotation * (pivotAtTip ? 0f : 10f), Custom.RNV() * Mathf.Lerp(10f, 30f, Random.value), redColor, Color.Lerp(redColor, new Color(0f, 0f, 0f), 0.3f)));
            if (destroyCounter > 4)
            {
                Destroy();
            }
        }
    }
}
