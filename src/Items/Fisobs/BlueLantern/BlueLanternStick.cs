namespace Nyctophobia;

public class LanternStickObj : ManagedObjectType
{
    public LanternStickObj() : base("BlueLanternStick", Plugin.MOD_NAME, typeof(BlueLanternStick), typeof(PlacedObject.ResizableObjectData), typeof(ResizeableObjectRepresentation))
    {
    }

    public override UpdatableAndDeletable MakeObject(PlacedObject placedObject, Room room)
    {
        int m = room.roomSettings.placedObjects.IndexOf(placedObject);
        if (room.game.session is not StoryGameSession || !(room.game.session as StoryGameSession).saveState.ItemConsumed(room.world, false, room.abstractRoom.index, m))
        {
            room.AddObject(new BlueLanternStick(room, placedObject));
        }
        return null;
    }
}

public class BlueLanternStick : LanternStick
{
    public List<BlueLantern> lanterns;

    public BlueLanternStick(Room room, PlacedObject po) : base(room, po)
    {
        lanterns = [];

        for (int i = 0; i < 3; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = 2.0f;

            float roomX = room.GetWorldCoordinate(po.pos).x;
            float roomY = room.GetWorldCoordinate(po.pos).y;

            Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;
            Vector2 lanternPosition = new Vector2(roomX, roomY) + offset;

            lantern = new BlueLantern(new AbstractPhysicalObject(room.world, NTEnums.AbstractObjectType.BlueLantern, null, room.GetWorldCoordinate(lanternPosition), room.game.GetNewID()))
            {
                room = room,
                stick = this,
                setRotation = new Vector2(Random.Range(0f, 360f), Random.Range(0f, 360f))
            };

            lantern.firstChunk.HardSetPosition(lanternPosition);
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        foreach (BlueLantern lantern in lanterns)
        {
            lantern.Update(eu);
        }
    }
}