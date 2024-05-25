namespace Nyctophobia;

public class BlueLanternData(PlacedObject owner) : ManagedData(owner, [new ExtEnumField<NTEnums.SpecialItemType>(nameof(Type), NTEnums.SpecialItemType.BlueLantern, displayName: nameof(Type))])
{
    public NTEnums.SpecialItemType Type => GetValue<NTEnums.SpecialItemType>(nameof(Type));

    [IntegerField(nameof(MinCycles), 0, 20, 0, ManagedFieldWithPanel.ControlType.slider, "Min Cycles")]
    public int MinCycles;

    [IntegerField(nameof(MaxCycles), 0, 20, 0, ManagedFieldWithPanel.ControlType.slider, "Max Cycles")]
    public int MaxCycles;
}

public class BlueLanternPlacer : UpdatableAndDeletable
{
    public BlueLanternPlacer(Room room, PlacedObject placedObject)
    {
        if (room.abstractRoom.firstTimeRealized)
        {
            int objIndex = room.roomSettings.placedObjects.IndexOf(placedObject);
            BlueLanternData data = (BlueLanternData)placedObject.data;

            if (room.game.session is not StoryGameSession session || !session.saveState.ItemConsumed(room.world, false, room.abstractRoom.index, objIndex))
            {
                AbstractPhysicalObject obj = new(room.world, NTEnums.AbstractObjectType.BlueLantern, null, room.GetWorldCoordinate(placedObject.pos), room.game.GetNewID())
                {
                };

                BlueLanternHooks.MakeLantern(obj, data.Type, obj);

                room.abstractRoom.entities.Add(obj);
            }
        }

        Destroy();
    }
}