namespace Witness;

public class RedFlareBombsHooks
{
    //private readonly AbstractConsumable abstractPhysicalObject;

    //public AbstractConsumable AbstrConsumable => abstractPhysicalObject;

    public static void Apply()
    {
        On.FlareBomb.Update += FlareBomb_Update;
        On.FlareBomb.Stalk.Update += Stalk_Update;
        On.FlareBomb.Stalk.ctor += Stalk_ctor;
        On.FlareBomb.Stalk.InitiateSprites += Stalk_InitiateSprites;
        On.FlareBomb.Stalk.DrawSprites += Stalk_DrawSprites;
        On.FlareBomb.Stalk.ApplyPalette += Stalk_ApplyPalette;
        On.FlareBomb.Stalk.AddToContainer += Stalk_AddToContainer;
    }

    private static void Stalk_AddToContainer(On.FlareBomb.Stalk.orig_AddToContainer orig, FlareBomb.Stalk self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        try
        {
            if (self.fruit is RedFlareBomb)
            {
                sLeaser.sprites[0].RemoveFromContainer();
                rCam.ReturnFContainer("Background").AddChild(sLeaser.sprites[0]);
            }
            else
            {
                orig(self, sLeaser, rCam, newContatiner);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
        }
    }

    private static void Stalk_ApplyPalette(On.FlareBomb.Stalk.orig_ApplyPalette orig, FlareBomb.Stalk self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        try
        {
            if (self.fruit is RedFlareBomb)
            {
                for (int i = 0; i < (sLeaser.sprites[0] as TriangleMesh).verticeColors.Length; i++)
                {
                    float value = (float)i / (float)(sLeaser.sprites[0] as TriangleMesh).verticeColors.Length;
                    (sLeaser.sprites[0] as TriangleMesh).verticeColors[i] = Color.Lerp(palette.blackColor, self.color, Mathf.InverseLerp(0.3f, 1f, value));
                }
            }
            else
            {
                orig(self, sLeaser, rCam, palette);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
        }
    }

    private static void Stalk_DrawSprites(On.FlareBomb.Stalk.orig_DrawSprites orig, FlareBomb.Stalk self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        try
        {
            if(self.fruit is RedFlareBomb)
            {
                var segments = self.segments;
                Vector2 vector = Vector2.Lerp(segments[0, 1], segments[0, 0], timeStacker);
                float num = 4f;
                for (int i = 0; i < segments.GetLength(0); i++)
                {
                    Vector2 vector2 = Vector2.Lerp(segments[i, 1], segments[i, 0], timeStacker);
                    Vector2 normalized = (vector2 - vector).normalized;
                    Vector2 vector3 = Custom.PerpendicularVector(normalized);
                    float num2 = Vector2.Distance(vector2, vector) / 5f;
                    float num3 = Mathf.Lerp(4f, 1f, (float)i / (float)segments.GetLength(0));
                    (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4, vector - vector3 * (num + num3) * 0.5f + normalized * num2 - camPos);
                    (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 1, vector + vector3 * (num + num3) * 0.5f + normalized * num2 - camPos);
                    if (i < segments.GetLength(0) - 1)
                    {
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 2, vector2 - vector3 * num3 - normalized * num2 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 3, vector2 + vector3 * num3 - normalized * num2 - camPos);
                    }
                    else
                    {
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(i * 4 + 2, vector2 - camPos);
                    }

                    num = num3;
                    vector = vector2;

                    if (self.fruit == null)
                    {
                        sLeaser.sprites[0].alpha = 0f;
                    }
                }
            }
            else
            {
                orig(self, sLeaser, rCam, timeStacker, camPos);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
        }
    }

    private static void Stalk_InitiateSprites(On.FlareBomb.Stalk.orig_InitiateSprites orig, FlareBomb.Stalk self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        try
        {
            if (self.fruit is RedFlareBomb)
            {
                sLeaser.sprites = new FSprite[1];
                sLeaser.sprites[0] = TriangleMesh.MakeLongMesh(self.segments.GetLength(0), pointyTip: true, customColor: true);
                self.AddToContainer(sLeaser, rCam, null);
            }
            else
            {
                orig(self, sLeaser, rCam);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
        }
    }

    private static void Stalk_Update(On.FlareBomb.Stalk.orig_Update orig, FlareBomb.Stalk self, bool eu)
    {
        try
        {
            if (self.fruit is RedFlareBomb && (self is not null || self.fruit is not null))
            {
                var segments = self.segments;
                self.fruit = self.fruit as RedFlareBomb;

                for (int i = 0; i < segments.GetLength(0); i++)
                {
                    segments[i, 1] = segments[i, 0];
                    if (i == 0)
                    {
                        segments[i, 0] = self.rootPos;
                        segments[i, 2] *= 0f;
                    }
                    else if (i == segments.GetLength(0) - 1 && self.fruit != null)
                    {
                        segments[i, 0] = self.fruit.firstChunk.pos;
                        segments[i, 2] *= 0f;
                    }
                    else
                    {
                        segments[i, 0] += segments[i, 2];
                        segments[i, 2] *= 0.7f;
                        segments[i, 2].y += 1.8f * (1f - self.contracted);
                        segments[i, 2] += self.direction * 1.4f * (1f - ((float)i + 1f) / (float)segments.GetLength(0)) * (1f - self.contracted);
                    }

                    if (i < segments.GetLength(0) - 1)
                    {
                        Vector2 normalized = (segments[i, 0] - segments[i + 1, 0]).normalized;
                        float num = 15f * (1f - self.contracted);
                        float num2 = Vector2.Distance(segments[i, 0], segments[i + 1, 0]);
                        segments[i, 0] += normalized * (num - num2) * 0.5f;
                        segments[i, 2] += normalized * (num - num2) * 0.5f;
                        segments[i + 1, 0] -= normalized * (num - num2) * 0.5f;
                        segments[i + 1, 2] -= normalized * (num - num2) * 0.5f;
                    }

                    if (i < segments.GetLength(0) - 2)
                    {
                        Vector2 normalized2 = (segments[i, 0] - segments[i + 2, 0]).normalized;
                        segments[i, 2] += normalized2 * 1.5f * (1f - self.contracted);
                        segments[i + 2, 2] -= normalized2 * 1.5f * (1f - self.contracted);
                    }

                    if (i == 0)
                    {
                        segments[i, 0] = self.rootPos + new Vector2(0f, -100f * self.contracted);
                        segments[i, 2] *= 0f;
                    }

                    if (Custom.DistLess(segments[i, 1], segments[i, 0], 10f))
                    {
                        segments[i, 1] = segments[i, 0];
                    }
                }
                //AbstractConsumable abstractConsumable = self.fruit.AbstrConsumable;
                //self.fruit.room = self.room;

                if (self.fruit != null && self.fruit.AbstrConsumable is not null)
                {
                    if (!Custom.DistLess(self.fruitPos, self.fruit.firstChunk.pos, 240f) || self.fruit.slatedForDeletetion || self.fruit.firstChunk.vel.magnitude > 15f)
                    {
                        //Error here
                        //self.fruit.AbstrConsumable.isConsumed = true;
                        self.fruit = null;
                    }
                    else
                    {
                        self.fruit.firstChunk.vel.y += self.fruit.gravity;
                        self.fruit.firstChunk.vel *= 0.6f;
                        self.fruit.firstChunk.vel += (self.fruitPos - self.fruit.firstChunk.pos) / 20f;
                    }
                }
                else
                {
                    self.contracted = Mathf.Clamp(self.contracted + 1f / 140f, 0f, 1f);
                }
            }
            else
            {
                orig(self, eu);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
    }

    private static void Stalk_ctor(On.FlareBomb.Stalk.orig_ctor orig, FlareBomb.Stalk self, FlareBomb fruit, Room room)
    {
        try
        {
            if (self.fruit is RedFlareBomb)
            {
                self.fruit = fruit as RedFlareBomb;
                self.color = fruit.color;
                self.fruitPos = fruit.firstChunk.pos;
                self.room = room;
                IntVector2 tilePosition = room.GetTilePosition(fruit.firstChunk.pos);
                while (tilePosition.y >= 0 && !room.GetTile(tilePosition).Solid)
                {
                    tilePosition.y--;
                }

                self.rootPos = room.MiddleOfTile(tilePosition) + new Vector2(0f, -10f);
                self.segments = new Vector2[Custom.IntClamp((int)(Vector2.Distance(fruit.firstChunk.pos, self.rootPos) / 15f), 4, 60), 3];
                for (int i = 0; i < self.segments.GetLength(0); i++)
                {
                    self.segments[i, 0] = Vector2.Lerp(self.rootPos, fruit.firstChunk.pos, (float)i / (float)self.segments.GetLength(0));
                    self.segments[i, 1] = self.segments[i, 0];
                }

                self.direction = Custom.DegToVec(Mathf.Lerp(-90f, 90f, room.game.SeededRandom((int)(self.fruitPos.x + self.fruitPos.y))));
                for (int j = 0; j < 100; j++)
                {
                    self.Update(eu: false);
                }

                fruit.ChangeCollisionLayer(0);
            }
            else
            {
                orig(self, fruit, room);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
        }
    }

    private static void FlareBomb_Update(On.FlareBomb.orig_Update orig, FlareBomb self, bool eu)
    {
        try
        {
            if (self is RedFlareBomb && (self is not null || self.stalk.fruit is not null))
            {
                self = self.stalk.fruit as RedFlareBomb;
                self.color = Color.red;
                self.stalk.color = Color.red;
            }
                orig(self, eu);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogException(ex);
        }
    }
}
