namespace Nyctophobia;

//TODO: Revert changes from the IClass and make it a new object instead
public class BlueLantern : Lantern, IBlueLantern
{
    public BlueLantern(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        color = new(0.196f, 0.596f, 0.965f);
    }

    public void Init(Lantern lantern)
    { }

    public void NewRoom(Lantern lantern, Room room)
    { }

    public void Update(Lantern lantern, bool eu)
    {
        base.Update(eu);
        for (int i = 0; i < flicker.GetLength(0); i++)
        {
            flicker[i, 1] = flicker[i, 0];
            flicker[i, 0] += Mathf.Pow(Random.value, 3f) * 0.09f * ((Random.value < 0.5f) ? (-1f) : 1f);
            flicker[i, 0] = Custom.LerpAndTick(flicker[i, 0], flicker[i, 2], 0.060f, 71f / (678f * (float)Math.PI));
            if (Random.value < 0.2f)
            {
                flicker[i, 2] = 1f + (Mathf.Pow(Random.value, 3f) * 0.2f * ((Random.value < 0.5f) ? (-1f) : 1f));
            }

            flicker[i, 2] = Mathf.Lerp(flicker[i, 2], 1f, 0.019f);
        }

        if (lightSource == null)
        {
            Color color = new Color(0.196f, 0.596f, 0.965f);
            lightSource = new LightSource(firstChunk.pos, environmentalLight: false, color, this)
            {
                affectedByPaletteDarkness = 0.5f
            };
            room.AddObject(lightSource);
        }
        else
        {
            lightSource.setPos = firstChunk.pos;
            lightSource.setRad = 370f * flicker[0, 0];
            lightSource.setAlpha = 1f;
            if (lightSource.slatedForDeletetion || lightSource.room != room)
            {
                lightSource = null;
            }
        }

        lastRotation = rotation;
        if (stick != null)
        {
            firstChunk.pos = stick.po.pos;
            firstChunk.vel *= 0f;
            rotation = (stick.po.data as PlacedObject.ResizableObjectData).handlePos.normalized;
            firstChunk.collideWithTerrain = false;
            firstChunk.collideWithObjects = false;
            canBeHitByWeapons = false;
            return;
        }

        firstChunk.collideWithTerrain = grabbedBy.Count == 0;
        if (grabbedBy.Count > 0)
        {
            rotation = Custom.PerpendicularVector(Custom.DirVec(firstChunk.pos, grabbedBy[0].grabber.mainBodyChunk.pos));
            rotation.y = 0f - Mathf.Abs(rotation.y);
        }

        if (setRotation.HasValue)
        {
            rotation = setRotation.Value;
            setRotation = null;
        }

        rotation = (rotation - (Custom.PerpendicularVector(rotation) * ((firstChunk.ContactPoint.y < 0) ? 0.15f : 0.05f) * firstChunk.vel.x)).normalized;
        if (firstChunk.ContactPoint.y < 0)
        {
            firstChunk.vel.x *= 0.8f;
        }

        if (abstractPhysicalObject is AbstractConsumable && grabbedBy.Count > 0 && !(abstractPhysicalObject as AbstractConsumable).isConsumed)
        {
            (abstractPhysicalObject as AbstractConsumable).Consume();
        }
    }

    public void PlaceInRoom(Lantern lantern, Room room)
    { }

    public void HitByWeapon(Lantern lantern, Weapon weapon)
    { }

    public void TerrainImpact(Lantern lantern, int chunk, IntVector2 direction, float speed, bool firstContact)
    { }

    public void InitiateSprites(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam)
    { }

    public void DrawSprites(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    { }

    public void ApplyPalette(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    { }

    public void AddToContainer(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    { }
}