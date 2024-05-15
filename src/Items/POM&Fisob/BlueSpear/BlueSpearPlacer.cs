﻿namespace Nyctophobia;

public class BlueSpearData(PlacedObject owner) : ManagedData(owner, [new ExtEnumField<NTEnums.SpecialItemType>(nameof(Type), NTEnums.SpecialItemType.BlueSpear, displayName: nameof(Type))])
{
    public NTEnums.SpecialItemType Type => GetValue<NTEnums.SpecialItemType>(nameof(Type));

    [IntegerField(nameof(MinCycles), 0, 20, 0, ManagedFieldWithPanel.ControlType.slider, "Min Cycles")]
    public int MinCycles;

    [IntegerField(nameof(MaxCycles), 0, 20, 0, ManagedFieldWithPanel.ControlType.slider, "Max Cycles")]
    public int MaxCycles;
}

public class BlueSpearPlacer : UpdatableAndDeletable
{
    public BlueSpearPlacer(Room room, PlacedObject placedObject)
    {
        if (room.abstractRoom.firstTimeRealized)
        {
            int objIndex = room.roomSettings.placedObjects.IndexOf(placedObject);
            BlueSpearData data = (BlueSpearData)placedObject.data;

            if (room.game.session is not StoryGameSession session || !session.saveState.ItemConsumed(room.world, false, room.abstractRoom.index, objIndex))
            {
                AbstractConsumable obj = new(room.world, AbstractPhysicalObject.AbstractObjectType.Spear, null, room.GetWorldCoordinate(placedObject.pos), room.game.GetNewID(), room.abstractRoom.index, objIndex, new PlacedObject.ConsumableObjectData(placedObject))
                {
                    isConsumed = false,
                    minCycles = data.MinCycles,
                    maxCycles = data.MaxCycles
                };

                BlueSpearHooks.MakeSpear(obj, data.Type, obj, room.world);

                room.abstractRoom.entities.Add(obj);
            }
        }

        Destroy();
    }
}