namespace Nyctophobia;

public class ImpalerObj : ManagedObjectType
{
    public ImpalerObj() : base("Impaler", "Nyctophobia", typeof(Impaler), typeof(PlacedObject.ResizableObjectData), typeof(ResizeableObjectRepresentation))
    {
    }

    public override UpdatableAndDeletable MakeObject(PlacedObject placedObject, Room room)
    {
        int m = room.roomSettings.placedObjects.IndexOf(placedObject);
        if (room.game.session is not StoryGameSession || !(room.game.session as StoryGameSession).saveState.ItemConsumed(room.world, false, room.abstractRoom.index, m))
        {
            room.AddObject(new Impaler(room, placedObject));
        }
        return null;
    }
}

public class Impaler : UpdatableAndDeletable, IDrawable
{
    private readonly PlacedObject po;
    private WorldCoordinate wc;
    private readonly Vector2[] stickPositions;
    private FloatRect impalePoint;
    private Creature impaledCreature;
    private BodyChunk impaledChunk;
    private float damageOverTime = 0.75f;
    private float slide = 0f;
    private float breakSize = 5f;
    private readonly float varA;
    private readonly float varB;
    private float tipWidth = 0.5f;
    private Color color;
    private bool broken = false;
    private bool snap = false;
    private bool updateFade = false;
    private readonly int index;
    private readonly float impaleChance = Random.value;
    private readonly float startAngle;

    public Impaler(Room room, PlacedObject pObj)
    {
        color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : new Color(1f, 1f, 1f);

        this.room = room;
        po = pObj;
        for (int i = 0; i < room.roomSettings.placedObjects.Count; i++)
        {
            if (room.roomSettings.placedObjects[i] == po)
            {
                index = i;
                break;
            }
        }
        wc = room.GetWorldCoordinate(room.GetTilePosition(po.pos));
        stickPositions = new Vector2[(int)Mathf.Clamp((pObj.data as PlacedObject.ResizableObjectData).handlePos.magnitude / 11f, 3f, 30f)];
        impalePoint = new FloatRect(po.pos.x - 13f, po.pos.y - 13f, po.pos.x + 13f, po.pos.y + 13f);
        varA = Random.Range(0.5f, 1f);
        varB = Random.Range(0.5f, 1f);
        if (room.game.IsStorySession)
        {
            snap = room.game.rainWorld.progression.currentSaveState.ItemConsumed(room.world, false, room.abstractRoom.index, index);
        }
        startAngle = Custom.AimFromOneVectorToAnother(po.pos, po.pos + (po.data as PlacedObject.ResizableObjectData).handlePos);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        if (broken || snap)
        {
            if (snap)
            {
                BreakImpaler();
                return;
            }
            return;
        }

        CheckForImpale();

        if (impaledCreature != null && impaledChunk != null)
        {
            HandleImpaledCreature();
        }
    }

    private void CheckForImpale()
    {
        foreach (var physicalObject in room.physicalObjects.SelectMany(room => room))
        {
            foreach (var chunk in physicalObject.bodyChunks)
            {
                var contactPoint = chunk.ContactPoint.ToVector2();
                var impalePosition = chunk.pos + contactPoint * (chunk.rad + 30f);

                if (impalePoint.Vector2Inside(impalePosition))
                {
                    var angle = Custom.AimFromOneVectorToAnother(po.pos, po.pos + (po.data as PlacedObject.ResizableObjectData).handlePos);
                    var impaleDirection = Custom.AimFromOneVectorToAnother(chunk.lastPos, chunk.pos);
                    var ang1 = Custom.DegToVec(angle);
                    var ang2 = Custom.DegToVec(impaleDirection);

                    if (Vector2.Distance(ang1, ang2) < 0.5f &&
                        Vector2.Distance(chunk.pos, chunk.lastLastPos) > 15f &&
                        physicalObject is Creature creature)
                    {
                        ImpaleOrBreak(chunk, creature);
                        return;
                    }
                }
            }
        }
    }

    private void ImpaleOrBreak(BodyChunk chunk, Creature creature)
    {
        var mass = chunk.mass;

        if ((mass > 0.75f && impaleChance > 0.15f) ||
            (mass <= 0.75 && mass > 0.5f && impaleChance > 0.7f))
        {
            SnapImpaler();
        }
        else if (impaledChunk == null)
        {
            ImpaleCreature(chunk, creature);
        }
    }

    private void SnapImpaler()
    {
        if (room.game.IsStorySession)
        {
            room.game.rainWorld.progression.currentSaveState.ReportConsumedItem(room.world, false, room.abstractRoom.index, index, 2);
        }
        room.PlaySound(SoundID.Leviathan_Crush_Non_Organic_Object, po.pos, 1.5f, 0.8f);
        room.PlaySound(SoundID.Fire_Spear_Pop, po.pos, 0.5f, 0.8f);

        var impalePos = po.pos + (po.data as PlacedObject.ResizableObjectData).handlePos;
        var spearFragmentPos = impalePos + Custom.DirVec(impalePos, po.pos) * breakSize;

        for (int s = 0; s < 6; s++)
        {
            room.AddObject(new ExplosiveSpear.SpearFragment(spearFragmentPos, Custom.RNV() * 10f));
            room.AddObject(new Spark(spearFragmentPos, Custom.RNV() * 5f, IsPrideDay ? new Color(Random.value, Random.value, Random.value) : new Color(1f, 1f, 1f), null, 20, 60));
        }

        var explosionColor = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : new Color(1f, 1f, 1f);
        room.AddObject(new ExplosionSpikes(room, spearFragmentPos, 4, 4f, 7f, 2f, 50f, explosionColor));

        if (Random.value > 0.5f)
        {
            SpawnImpalerTip();
        }

        snap = true;
    }

    private void ImpaleCreature(BodyChunk chunk, Creature creature)
    {
        damageOverTime = Mathf.Lerp(0.65f, 2f, Mathf.InverseLerp(10f, 100f, Vector2.Distance(chunk.pos, chunk.lastLastPos)));
        impaledCreature = creature;
        impaledChunk = chunk;

        if (impaledCreature.stun == 0)
        {
            room.PlaySound(SoundID.Spear_Stick_In_Creature, impaledChunk);
        }

        for (int s = 0; s < 6; s++)
        {
            var color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : new Color(0.7f, 0.7f, 0.7f);
            var spark = new Spark(impaledChunk.pos, impaledChunk.vel, color, null, 10, 50);
            room.AddObject(spark);
        }

        if ((startAngle <= 0f && startAngle > -60f) || (startAngle >= 0f && startAngle < 60f))
        {
            slide = 0.07f;
        }

        impaledChunk.vel = Vector2.zero;
    }

    private void SpawnImpalerTip()
    {
        ImpalerAbstract apo = new ImpalerAbstract(room.world, null, wc, room.game.GetNewID());
        room.abstractRoom.AddEntity(apo);
        apo.RealizeInRoom();
    }

    private void HandleImpaledCreature()
    {
        if (!impaledCreature.dead)
        {
            impaledCreature.stun = 10;
            damageOverTime += (0.25f + (impaledCreature.Template.baseDamageResistance * 0.1f)) * Time.deltaTime;

            Debug.Log(damageOverTime + " | " + impaledCreature.Template.baseDamageResistance);

            if (damageOverTime > impaledCreature.Template.baseDamageResistance)
            {
                impaledCreature.Die();
            }
        }

        var angle = Custom.AimFromOneVectorToAnother(po.pos, po.pos + (po.data as PlacedObject.ResizableObjectData).handlePos);
        var impalePos = Vector2.Lerp(impalePoint.Center, po.pos + (po.data as PlacedObject.ResizableObjectData).handlePos, slide);

        impaledChunk.setPos = impalePos;

        if ((angle > 120f && angle <= 180f) || (angle < -120f && angle >= -180f))
        {
            slide += Mathf.Lerp(0.35f, 0.01f, Mathf.InverseLerp(0f, 0.35f, slide)) * Time.deltaTime;
            slide = Mathf.Clamp(slide, -0f, 0.35f);
        }
        else if ((angle <= 0f && angle > -60f) || (angle >= 0f && angle < 60f))
        {
            slide -= Mathf.Lerp(0.04f, 0.25f, Mathf.InverseLerp(0f, 0.35f, slide)) * Time.deltaTime;

            if (slide < -0.03f)
            {
                ResetImpaledCreature();
                slide = 0f;
                return;
            }

            slide = Mathf.Clamp(slide, -0.1f, 0.35f);
        }

        if (Vector2.Distance(impaledChunk.pos, impalePos) > 25f)
        {
            ResetImpaledCreature();
            slide = 0f;
        }
    }

    private void BreakImpaler()
    {
        breakSize = -45f;
        tipWidth = 2f;
        broken = true;
    }

    private void ResetImpaledCreature()
    {
        foreach (var chunk in impaledCreature.bodyChunks)
        {
            chunk.lastLastPos = chunk.pos;
            chunk.lastPos = chunk.pos;
            chunk.vel = Vector2.zero;
        }

        impaledChunk = null;
        impaledCreature = null;
    }

    public void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (IsPrideDay)
            color = new Color(Random.value, Random.value, Random.value);
        else
            color = rCam.currentPalette.blackColor;
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = TriangleMesh.MakeLongMesh(stickPositions.Length, false, true);
        sLeaser.sprites[1] = new FSprite("mouseEyeB5", true)
        {
            scale = 1f,
            alpha = 0f
        };
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 a = po.pos;
        Vector2 vector = po.pos + (po.data as PlacedObject.ResizableObjectData).handlePos;
        Vector2 vector2 = a + Custom.DirVec(vector, po.pos) * breakSize;
        Vector2 a2 = vector + Custom.DirVec(vector2, vector) * 5f;
        float num = 1f;
        float baseWidth = 2.7f;
        for (int j = 0; j < stickPositions.Length; j++)
        {
            float t = (float)j / (stickPositions.Length - 1);
            float num2 = Mathf.Lerp(baseWidth + Mathf.Min((po.data as PlacedObject.ResizableObjectData).handlePos.magnitude / 190f, 3f), tipWidth, t);
            Vector2 vector3 = Vector2.Lerp(vector, vector2, t) + stickPositions[j] * Mathf.Lerp(num2 * 0.6f, 1f, t);
            Vector2 normalized = (a2 - vector3).normalized;
            Vector2 a3 = Custom.PerpendicularVector(normalized);
            float d = Vector2.Distance(a2, vector3) / 5f;
            (sLeaser.sprites[0] as TriangleMesh).MoveVertice(j * 4, a2 - normalized * d - a3 * (num2 + num) * varA - camPos);
            (sLeaser.sprites[0] as TriangleMesh).MoveVertice(j * 4 + 1, a2 - normalized * d + a3 * (num2 + num) * varB - camPos);
            (sLeaser.sprites[0] as TriangleMesh).MoveVertice(j * 4 + 2, vector3 + normalized * d - a3 * num2 - camPos);
            (sLeaser.sprites[0] as TriangleMesh).MoveVertice(j * 4 + 3, vector3 + normalized * d + a3 * num2 - camPos);
            a2 = vector3;
            num = num2;
        }
        sLeaser.sprites[1].x = impalePoint.Center.x - camPos.x;
        sLeaser.sprites[1].y = impalePoint.Center.y - camPos.y;
        if (broken && !updateFade)
        {
            ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            updateFade = true;
        }
        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void AddToContainer(SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Background");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
        }
        newContatiner.AddChild(sLeaser.sprites[0]);
        rCam.ReturnFContainer("HUD").AddChild(sLeaser.sprites[1]);
    }

    public void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : palette.blackColor;

        for (int i = 0; i < (sLeaser.sprites[0] as TriangleMesh).verticeColors.Length; i++)
        {
            float fade;
            if (broken)
            {
                fade = Mathf.InverseLerp((sLeaser.sprites[0] as TriangleMesh).verticeColors.Length / 2, (sLeaser.sprites[0] as TriangleMesh).verticeColors.Length * 2, i);
                if (IsPrideDay)
                    (sLeaser.sprites[0] as TriangleMesh).verticeColors[i] = new Color(Random.value, Random.value, Random.value);
                else
                    (sLeaser.sprites[0] as TriangleMesh).verticeColors[i] = Color.Lerp(palette.blackColor, Color.Lerp(palette.blackColor, palette.fogColor, 0.5f), fade);
            }
            else
            {
                fade = Mathf.InverseLerp((sLeaser.sprites[0] as TriangleMesh).verticeColors.Length / 2, (sLeaser.sprites[0] as TriangleMesh).verticeColors.Length, i);

                (sLeaser.sprites[0] as TriangleMesh).verticeColors[i] = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : Color.Lerp(palette.blackColor, Color.Lerp(palette.blackColor, palette.fogColor, 0.5f), fade);
            }
        }
        sLeaser.sprites[1].color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : new Color(1f, 1f, 1f);
    }
}