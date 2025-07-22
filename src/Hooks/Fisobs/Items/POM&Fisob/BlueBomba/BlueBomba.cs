namespace Nyctophobia;

//TODO: Rever Iclass and follow the same template as other objects
public class BlueBomba : ScavengerBomb, IBlueBomba
{
    public BlueBomba(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
    {
        color = new(0.098f, 0.356f, 0.815f);
        explodeColor = new(0.098f, 0.356f, 0.815f);

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

    public void Init(ScavengerBomb bomb)
    { }

    public void NewRoom(ScavengerBomb bom, Room room)
    { }

    public void InitiateBurn(ScavengerBomb bomb)
    { }

    public void Update(ScavengerBomb bomb, bool eu)
    { }

    public void TerrainImpact(ScavengerBomb bomb, int chunk, IntVector2 direction, float speed, bool firstContact)
    { }

    public void HitSomething(ScavengerBomb bomb, CollisionResult result, bool eu)
    { }

    public void Thrown(ScavengerBomb bomb, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    { }

    public void PickedUp(ScavengerBomb bomb, Creature upPicker)
    { }

    public void HitByWeapon(ScavengerBomb bomb, Weapon weapon)
    { }

    public void WeaponDeflect(ScavengerBomb bomb, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    { }

    public void HitByExplosion(ScavengerBomb bomb, float hitFac, Explosion explosion, int hitChunk)
    { }

    public void Explode(ScavengerBomb self, BodyChunk hitChunk)
    {
        if (self.slatedForDeletetion)
        {
            return;
        }

        Vector2 vector = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
        self.room.AddObject(new SootMark(self.room, vector, 104f, bigSprite: true));
        if (!self.explosionIsForShow)
        {
            for (int j = 0; j < self.room.physicalObjects.Length; j++)
            {
                for (int k = 0; k < self.room.physicalObjects[j].Count; k++)
                {
                    for (int l = 0; l < self.room.physicalObjects[j][k].bodyChunks.Length; l++)
                    {
                        if (self.room.physicalObjects[j][k] as Creature is Player)
                        {
                            self.room.AddObject(new Explosion(self.room, self, vector, 7, 300f, 6.2f, 2f, 280f, 0.25f, self.thrownBy, 0f, 208f, 1f));
                        }
                        else
                        {
                            self.room.AddObject(new Explosion(self.room, self, vector, 7, 350f, 6.2f, 2f, 280f, 0.25f, self.thrownBy, 0f, 208f, 1f));
                        }
                    }
                }
            }
        }
        self.room.AddObject(new Explosion.ExplosionLight(vector, 464f, 1f, 9, self.explodeColor));
        self.room.AddObject(new Explosion.ExplosionLight(vector, 399f, 1f, 4, new Color(1f, 1f, 1f)));
        self.room.AddObject(new ExplosionSpikes(self.room, vector, 28, 49f, 21.7f, 9.1f, 221f, self.explodeColor));
        self.room.AddObject(new ShockWave(vector, 330f, 0.045f, 5));
        for (int i = 0; i < 25; i++)
        {
            Vector2 vector2 = Custom.RNV();
            if (self.room.GetTile(vector + (vector2 * 20f)).Solid)
            {
                if (!self.room.GetTile(vector - (vector2 * 20f)).Solid)
                {
                    vector2 *= -1f;
                }
                else
                {
                    vector2 = Custom.RNV();
                }
            }
            for (int j = 0; j < 3; j++)
            {
                self.room.AddObject(new Spark(vector + (vector2 * Mathf.Lerp(30f, 60f, Random.value)), (vector2 * Mathf.Lerp(7f, 38f, Random.value)) + (Custom.RNV() * 20f * Random.value), Color.Lerp(self.explodeColor, new Color(1f, 1f, 1f), Random.value), null, 11, 28));
            }
            self.room.AddObject(new Explosion.FlashingSmoke(vector + (vector2 * 52f * Random.value), vector2 * Mathf.Lerp(4f, 20f, Mathf.Pow(Random.value, 2f)), 1f + (0.05f * Random.value), new Color(1f, 1f, 1f), self.explodeColor, Random.Range(3, 11)));
        }
        if (self.smoke != null)
        {
            for (int k = 0; k < 8; k++)
            {
                self.smoke.EmitWithMyLifeTime(vector + Custom.RNV(), Custom.RNV() * Random.value * 17f);
            }
        }
        for (int l = 0; l < 6; l++)
        {
            self.room.AddObject(new BombFragment(vector, Custom.DegToVec((l + Random.value) / 6f * 360f) * Mathf.Lerp(18f, 38f, Random.value)));
        }
        self.room.ScreenMovement(vector, default, 1.3f);
        for (int m = 0; m < self.abstractPhysicalObject.stuckObjects.Count; m++)
        {
            self.abstractPhysicalObject.stuckObjects[m].Deactivate();
        }
        self.room.PlaySound(SoundID.Bomb_Explode, vector);
        self.room.InGameNoise(new InGameNoise(vector, 9000f, self, 1f));
        bool flag = hitChunk != null;
        for (int n = 0; n < 5; n++)
        {
            if (self.room.GetTile(vector + (Custom.fourDirectionsAndZero[n].ToVector2() * 20f)).Solid)
            {
                flag = true;
                break;
            }
        }
        if (flag)
        {
            if (self.smoke == null)
            {
                self.smoke = new BombSmoke(self.room, vector, null, self.explodeColor);
                self.room.AddObject(self.smoke);
            }
            if (hitChunk != null)
            {
                self.smoke.chunk = hitChunk;
            }
            else
            {
                self.smoke.chunk = null;
                self.smoke.fadeIn = 1f;
            }
            self.smoke.pos = vector;
            self.smoke.stationary = true;
            self.smoke.DisconnectSmoke();
        }
        else
        {
            self.smoke?.Destroy();
        }
        self.Destroy();
    }

    public void InitiateSprites(ScavengerBomb bomb, SpriteLeaser sLeaser, RoomCamera rCam)
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

    public void DrawSprites(ScavengerBomb bomb, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
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

    public void ApplyPalette(ScavengerBomb bomb, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    { }

    public void UpdateColor(ScavengerBomb bomb, SpriteLeaser sLeaser, Color col)
    { }
}