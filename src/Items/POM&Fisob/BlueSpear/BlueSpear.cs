namespace Nyctophobia;

public class BlueSpear : ExplosiveSpear, IBlueSpear
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

        collisionLayer = 2;
    }

    public void Init(ExplosiveSpear spear)
    {
        spear.explodeAt = Random.Range(35, 55);

        spear.rag = new Vector2[Random.Range(9, Random.Range(9, 15)), 9];

        spear.redColor = new(0.098f, 0.356f, 0.815f);
        spear.explodeColor = new(0.098f, 0.356f, 0.815f);
        spear.color = new(0.098f, 0.356f, 0.815f);

        int num = 20;
        for (int i = 0; i < explodeAt / num; i++)
        {
            spear.miniExplosions.Add(Random.Range(i * num, (i + 1) * num));
        }
    }

    public void PlaceInRoom(ExplosiveSpear spear, Room room)
    { }

    public void NewRoom(ExplosiveSpear spear, Room room)
    { }

    public void Update(ExplosiveSpear spear, bool eu)
    { }

    public void WeaponDeflect(ExplosiveSpear spear, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    { }

    public float ConchunkWeight(ExplosiveSpear spear, Vector2 pushDir, BodyChunkConnection con)
    {
        return con.chunk1 == spear.stuckInChunk
    ? Custom.LerpMap(Vector2.Dot(pushDir, Custom.DirVec(spear.stuckInChunk.pos, con.chunk2.pos)), -0.5f, 1f, 0f, 7f, 1.5f)
    : con.chunk2 == spear.stuckInChunk
    ? Custom.LerpMap(Vector2.Dot(pushDir, Custom.DirVec(spear.stuckInChunk.pos, con.chunk1.pos)), -0.5f, 1f, 0f, 7f, 1.5f)
    : 0f;
    }

    public void MiniExplode(ExplosiveSpear self)
    {
        if (self.stuckInObject != null)
        {
            float num = 7f;
            float num2 = 0f;
            Vector2 vector = self.rotation;
            if (self.room.readyForAI && self.room.aimap.getAItile(self.firstChunk.pos).floorAltitude < 3)
            {
                vector = Vector3.Slerp(vector, new Vector2(0f, 1f), 0.2f);
            }

            for (int i = 0; i < self.stuckInObject.bodyChunkConnections.Length; i++)
            {
                if (self.stuckInObject.bodyChunkConnections[i].type != BodyChunkConnection.Type.Pull && (self.stuckInObject.bodyChunkConnections[i].chunk1 == self.stuckInChunk || self.stuckInObject.bodyChunkConnections[i].chunk2 == self.stuckInChunk))
                {
                    num2 += ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[i]);
                }
            }

            if (num2 > 0f)
            {
                float num3 = Mathf.Clamp(num2 * 2f, 0f, 6f);
                num -= num3;
                for (int j = 0; j < self.stuckInObject.bodyChunkConnections.Length; j++)
                {
                    if (self.stuckInObject.bodyChunkConnections[j].type != BodyChunkConnection.Type.Pull)
                    {
                        if (self.stuckInObject.bodyChunkConnections[j].chunk2 == self.stuckInChunk)
                        {
                            self.stuckInObject.bodyChunkConnections[j].chunk1.vel += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j]) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk1.mass);
                            self.stuckInObject.bodyChunkConnections[j].chunk1.pos += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j]) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk1.mass);
                        }
                        else if (self.stuckInObject.bodyChunkConnections[j].chunk1 == self.stuckInChunk)
                        {
                            self.stuckInObject.bodyChunkConnections[j].chunk2.vel += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j]) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk2.mass);
                            self.stuckInObject.bodyChunkConnections[j].chunk2.pos += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j]) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk2.mass);
                        }
                    }
                }
            }

            if (self.stuckInObject is Creature and not Player)
            {
                (self.stuckInObject as Creature).Violence(self.firstChunk, vector * num, self.stuckInChunk, null, Creature.DamageType.Explosion, (self.stuckInAppendage != null) ? 0.4f : 1.2f, 0f);
            }
            else
            {
                self.stuckInChunk.vel += vector * num / self.stuckInChunk.mass;
            }

            self.stuckInChunk.pos += vector * num / self.stuckInChunk.mass;
        }

        Vector2 vector2 = self.firstChunk.pos + (self.rotation * (self.pivotAtTip ? 0f : 15f));
        self.room.AddObject(new Explosion.ExplosionLight(vector2, 160f, 1f, 2, self.explodeColor));
        for (int k = 0; k < 8; k++)
        {
            Vector2 vector3 = Custom.RNV();
            self.room.AddObject(new Spark(vector2 + (vector3 * Random.value * 10f), vector3 * Mathf.Lerp(6f, 18f, Random.value), self.explodeColor, null, 4, 18));
        }

        self.room.AddObject(new ShockWave(vector2, 120f, 0.035f, 2));
        self.room.PlaySound(SoundID.Fire_Spear_Pop, vector2);
        for (int l = 0; l < self.rag.GetLength(0); l++)
        {
            self.rag[l, 2] += (Custom.DirVec(vector2, self.rag[l, 0]) + Custom.RNV() - self.rotation) * Random.value * 5f;
            self.rag[l, 0] += (Custom.DirVec(vector2, self.rag[l, 0]) + Custom.RNV() - self.rotation) * Random.value * 5f;
        }

        if (self.mode == Mode.Free && self.stuckInObject == null)
        {
            if (self.firstChunk.vel.y < 0f)
            {
                self.firstChunk.vel.y *= 0.5f;
            }

            self.firstChunk.vel -= (Custom.RNV() + self.rotation) * Random.value * ((self.firstChunk.ContactPoint.y < 0) ? 5f : 15f);
            self.SetRandomSpin();
        }

        self.room.InGameNoise(new InGameNoise(vector2, 800f, self, 1f));
        self.vibrate = Math.Max(self.vibrate, 6);
    }

    public void Explode(ExplosiveSpear self)
    {
        if (self.exploded)
        {
            return;
        }

        self.exploded = true;
        if (self.stuckInObject != null)
        {
            if (self.stuckInObject is Creature and not Player)
            {
                (self.stuckInObject as Creature).Violence(self.firstChunk, self.rotation * 12f, self.stuckInChunk, null, Creature.DamageType.Explosion, (self.stuckInAppendage != null) ? 1.8f : 4.2f, 120f);
            }
            else
            {
                self.stuckInChunk.vel += self.rotation * 12f / self.stuckInChunk.mass;
            }
        }

        Vector2 vector = self.firstChunk.pos + (self.rotation * (self.pivotAtTip ? 0f : 10f));
        self.room.AddObject(new SootMark(self.room, vector, 100f, bigSprite: false));
        self.room.AddObject(new Explosion(self.room, self, vector, 5, 270f, 5f, 2f, 60f, 0.3f, self.thrownBy, 0f, 0f, 0.7f));
        for (int i = 0; i < 14; i++)
        {
            self.room.AddObject(new Explosion.ExplosionSmoke(vector, Custom.RNV() * 5f * Random.value, 2f));
        }

        self.room.AddObject(new Explosion.ExplosionLight(vector, 360f, 1f, 3, self.explodeColor));
        self.room.AddObject(new ExplosionSpikes(self.room, vector, 13, 20f, 5f, 7f, 110f, self.explodeColor));
        self.room.AddObject(new ShockWave(vector, 180f, 0.045f, 4));
        for (int j = 0; j < 20; j++)
        {
            Vector2 vector2 = Custom.RNV();
            self.room.AddObject(new Spark(vector + (vector2 * Random.value * 40f), vector2 * Mathf.Lerp(4f, 30f, Random.value), self.explodeColor, null, 4, 18));
        }

        self.room.ScreenMovement(vector, default, 0.7f);
        for (int k = 0; k < 2; k++)
        {
            Smolder smolder = null;
            if (self.stuckInObject != null)
            {
                smolder = new Smolder(self.room, self.stuckInChunk.pos, self.stuckInChunk, self.stuckInAppendage);
            }
            else
            {
                Vector2? vector3 = ExactTerrainRayTracePos(self.room, self.firstChunk.pos, self.firstChunk.pos + ((k == 0) ? (self.rotation * 20f) : (Custom.RNV() * 20f)));
                if (vector3.HasValue)
                {
                    smolder = new Smolder(self.room, vector3.Value + (Custom.DirVec(vector3.Value, self.firstChunk.pos) * 3f), null, null);
                }
            }

            if (smolder != null)
            {
                self.room.AddObject(smolder);
            }
        }

        self.abstractPhysicalObject.LoseAllStuckObjects();
        self.room.PlaySound(SoundID.Fire_Spear_Explode, vector);
        self.room.InGameNoise(new InGameNoise(vector, 8000f, self, 1f));
        self.Destroy();
    }

    public void HitByExplosion(ExplosiveSpear spear, float hitFac, Explosion explosion, int hitChunk)
    { }

    public void InitiateSprites(ExplosiveSpear spear, SpriteLeaser sLeaser, RoomCamera rCam)
    { }

    public void DrawSprites(ExplosiveSpear spear, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        sLeaser.sprites[0].color = new(0.098f, 0.356f, 0.815f);
        sLeaser.sprites[2].color = new(0.098f, 0.356f, 0.815f);
        if (blink > 0)
        {
            sLeaser.sprites[1].color = blink > 1 && Random.value < 0.5f ? new Color(1f, 1f, 1f) : color;
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
            float f = i / (float)(rag.GetLength(0) - 1);
            Vector2 vector2 = Vector2.Lerp(rag[i, 1], rag[i, 0], timeStacker);
            float num2 = (2f + (2f * Mathf.Sin(Mathf.Pow(f, 2f) * (float)Math.PI))) * Vector3.Slerp(rag[i, 4], rag[i, 3], timeStacker).x;
            Vector2 normalized = (vector - vector2).normalized;
            Vector2 vector3 = Custom.PerpendicularVector(normalized);
            float num3 = Vector2.Distance(vector, vector2) / 5f;
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4, vector - (normalized * num3) - (vector3 * (num2 + num) * 0.5f) - camPos);
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i * 4) + 1, vector - (normalized * num3) + (vector3 * (num2 + num) * 0.5f) - camPos);
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i * 4) + 2, vector2 + (normalized * num3) - (vector3 * num2) - camPos);
            (sLeaser.sprites[2] as TriangleMesh).MoveVertice((i * 4) + 3, vector2 + (normalized * num3) + (vector3 * num2) - camPos);
            vector = vector2;
            num = num2;
        }
    }

    public void ApplyPallete(ExplosiveSpear spear, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    { }
}