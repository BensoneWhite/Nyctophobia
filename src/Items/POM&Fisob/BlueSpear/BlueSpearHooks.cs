namespace Nyctophobia;

public static class BlueSpearHooks
{
    public static void Apply()
    {
        On.ExplosiveSpear.MiniExplode += OnMiniExplode;
        On.ExplosiveSpear.Explode += OnExplode;
    }

    private static void OnExplode(On.ExplosiveSpear.orig_Explode orig, ExplosiveSpear spear)
    {
        if (spear is not BlueSpear)
        {
            orig(spear);
            return;
        }

        if (spear.exploded)
            return;

        spear.exploded = true;

        ProcessStuckObject(spear);

        Vector2 explosionPos = spear.firstChunk.pos + spear.rotation * (spear.pivotAtTip ? 0f : 10f);

        spear.room.AddObject(new SootMark(spear.room, explosionPos, 100f, bigSprite: false));
        spear.room.AddObject(new Explosion(spear.room, spear, explosionPos, 5, 270f, 5f, 2f, 60f, 0.3f, spear.thrownBy, 0f, 0f, 0.7f));
        AddExplosionSmoke(spear, explosionPos, count: 14);
        spear.room.AddObject(new Explosion.ExplosionLight(explosionPos, 360f, 1f, 3, spear.explodeColor));
        spear.room.AddObject(new ExplosionSpikes(spear.room, explosionPos, 13, 20f, 5f, 7f, 110f, spear.explodeColor));
        spear.room.AddObject(new ShockWave(explosionPos, 180f, 0.045f, 4));
        AddExplosionSparks(spear, explosionPos, count: 20);
        spear.room.ScreenMovement(explosionPos, default, 0.7f);
        AddSmolderEffects(spear, iterations: 2);

        spear.abstractPhysicalObject.LoseAllStuckObjects();
        spear.room.PlaySound(SoundID.Fire_Spear_Explode, explosionPos);
        spear.room.InGameNoise(new InGameNoise(explosionPos, 8000f, spear, 1f));
        spear.Destroy();
    }

    private static void ProcessStuckObject(ExplosiveSpear spear)
    {
        if (spear.stuckInObject == null)
            return;

        if (spear.stuckInObject is Creature creature && spear.stuckInObject is not Player)
        {
            creature.Violence(spear.firstChunk, spear.rotation * 12f, spear.stuckInChunk, null, Creature.DamageType.Explosion, spear.stuckInAppendage != null ? 1.8f : 4.2f, 120f);
        }
        else
        {
            spear.stuckInChunk.vel += spear.rotation * 12f / spear.stuckInChunk.mass;
        }
    }

    private static void AddExplosionSmoke(ExplosiveSpear spear, Vector2 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float randomFactor = Random.value;
            spear.room.AddObject(new Explosion.ExplosionSmoke(pos, Custom.RNV() * 5f * randomFactor, 2f));
        }
    }

    private static void AddExplosionSparks(ExplosiveSpear spear, Vector2 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomDir = Custom.RNV();
            float randomSpeed = Mathf.Lerp(4f, 30f, Random.value);
            spear.room.AddObject(new Spark(pos + randomDir * Random.value * 40f, randomDir * randomSpeed, spear.explodeColor, null, 4, 18));
        }
    }

    private static void AddSmolderEffects(ExplosiveSpear spear, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            Smolder smolder = null;
            if (spear.stuckInObject != null)
            {
                smolder = new Smolder(spear.room, spear.stuckInChunk.pos, spear.stuckInChunk, spear.stuckInAppendage);
            }
            else
            {
                Vector2? terrainPos = ExactTerrainRayTracePos(spear.room, spear.firstChunk.pos, spear.firstChunk.pos + (i == 0 ? spear.rotation * 20f : Custom.RNV() * 20f));
                if (terrainPos.HasValue)
                {
                    smolder = new Smolder(spear.room, terrainPos.Value + Custom.DirVec(terrainPos.Value, spear.firstChunk.pos) * 3f, null, null);
                }
            }

            if (smolder != null)
                spear.room.AddObject(smolder);
        }
    }

    public static float ConchunkWeight(Vector2 pushDir, BodyChunkConnection con, ExplosiveSpear spear)
    {
        BodyChunk stuckChunk = spear.stuckInChunk;
        if (con.chunk1 == stuckChunk)
        {
            return Custom.LerpMap(Vector2.Dot(pushDir, Custom.DirVec(stuckChunk.pos, con.chunk2.pos)), -0.5f, 1f, 0f, 7f, 1.5f);
        }
        if (con.chunk2 == stuckChunk)
        {
            return Custom.LerpMap(Vector2.Dot(pushDir, Custom.DirVec(stuckChunk.pos, con.chunk1.pos)), -0.5f, 1f, 0f, 7f, 1.5f);
        }
        return 0f;
    }

    private static void OnMiniExplode(On.ExplosiveSpear.orig_MiniExplode orig, ExplosiveSpear spear)
    {
        if (spear is not BlueSpear)
        {
            orig(spear);
            return;
        }

        if (spear.stuckInObject != null)
        {
            const float baseForce = 7f;
            float totalWeight = 0f;
            Vector2 pushDir = spear.rotation;

            if (spear.room.readyForAI && spear.room.aimap.getAItile(spear.firstChunk.pos).floorAltitude < 3)
            {
                pushDir = Vector3.Slerp(pushDir, new Vector2(0f, 1f), 0.2f);
            }

            foreach (var connection in spear.stuckInObject.bodyChunkConnections)
            {
                if (connection.type != BodyChunkConnection.Type.Pull &&
                    (connection.chunk1 == spear.stuckInChunk || connection.chunk2 == spear.stuckInChunk))
                {
                    totalWeight += ConchunkWeight(pushDir, connection, spear);
                }
            }

            if (totalWeight > 0f)
            {
                float forceReduction = Mathf.Clamp(totalWeight * 2f, 0f, 6f);
                float appliedForce = baseForce - forceReduction;

                foreach (var connection in spear.stuckInObject.bodyChunkConnections)
                {
                    if (connection.type == BodyChunkConnection.Type.Pull)
                        continue;

                    float weight = ConchunkWeight(pushDir, connection, spear);
                    if (connection.chunk2 == spear.stuckInChunk)
                    {
                        ApplyForceToChunk(connection.chunk1, pushDir, forceReduction, weight, totalWeight);
                    }
                    else if (connection.chunk1 == spear.stuckInChunk)
                    {
                        ApplyForceToChunk(connection.chunk2, pushDir, forceReduction, weight, totalWeight);
                    }
                }

                if (spear.stuckInObject is Creature creature && !(spear.stuckInObject is Player))
                {
                    creature.Violence(
                        spear.firstChunk,
                        pushDir * appliedForce,
                        spear.stuckInChunk,
                        null,
                        Creature.DamageType.Explosion,
                        spear.stuckInAppendage != null ? 0.4f : 1.2f,
                        0f);
                }
                else
                {
                    spear.stuckInChunk.vel += pushDir * appliedForce / spear.stuckInChunk.mass;
                }
                spear.stuckInChunk.pos += pushDir * appliedForce / spear.stuckInChunk.mass;
            }
        }

        Vector2 miniExplosionPos = spear.firstChunk.pos + spear.rotation * (spear.pivotAtTip ? 0f : 15f);
        spear.room.AddObject(new Explosion.ExplosionLight(miniExplosionPos, 160f, 1f, 2, spear.explodeColor));
        AddMiniExplosionSparks(spear, miniExplosionPos, count: 8);
        spear.room.AddObject(new ShockWave(miniExplosionPos, 120f, 0.035f, 2));
        spear.room.PlaySound(SoundID.Fire_Spear_Pop, miniExplosionPos);

        int ragLength = spear.rag.GetLength(0);
        for (int i = 0; i < ragLength; i++)
        {
            Vector2 dir = Custom.DirVec(miniExplosionPos, spear.rag[i, 0]);
            Vector2 randomVec = Custom.RNV();
            Vector2 force = (dir + randomVec - spear.rotation) * Random.value * 5f;
            spear.rag[i, 2] += force;
            spear.rag[i, 0] += force;
        }

        if (spear.mode == Mode.Free && spear.stuckInObject == null)
        {
            if (spear.firstChunk.vel.y < 0f)
                spear.firstChunk.vel.y *= 0.5f;

            spear.firstChunk.vel -= (Custom.RNV() + spear.rotation) * Random.value *
                                      (spear.firstChunk.ContactPoint.y < 0 ? 5f : 15f);
            spear.SetRandomSpin();
        }

        spear.room.InGameNoise(new InGameNoise(miniExplosionPos, 800f, spear, 1f));
        spear.vibrate = Math.Max(spear.vibrate, 6);
    }

    private static void ApplyForceToChunk(BodyChunk chunk, Vector2 pushDir, float forceReduction, float weight, float totalWeight)
    {
        float force = forceReduction * weight / (totalWeight * chunk.mass);
        chunk.vel += pushDir * force;
        chunk.pos += pushDir * force;
    }

    private static void AddMiniExplosionSparks(ExplosiveSpear spear, Vector2 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomDir = Custom.RNV();
            float randomSpeed = Mathf.Lerp(6f, 18f, Random.value);
            spear.room.AddObject(new Spark(pos + randomDir * Random.value * 10f, randomDir * randomSpeed, spear.explodeColor, null, 4, 18));
        }
    }
}