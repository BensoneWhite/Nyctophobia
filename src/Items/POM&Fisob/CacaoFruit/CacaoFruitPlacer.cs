﻿namespace Nyctophobia;

public class CacaoFruitData(PlacedObject owner) : ManagedData(owner, [new ExtEnumField<NTEnums.SpecialItemType>(nameof(Type), NTEnums.SpecialItemType.CacaoFruit, displayName: nameof(Type))])
{
    public NTEnums.SpecialItemType Type => GetValue<NTEnums.SpecialItemType>(nameof(Type));

    [IntegerField(nameof(MinCycles), 0, 20, 0, ManagedFieldWithPanel.ControlType.slider, "Min Cycles")]
    public int MinCycles;

    [IntegerField(nameof(MaxCycles), 0, 20, 0, ManagedFieldWithPanel.ControlType.slider, "Max Cycles")]
    public int MaxCycles;
}

public sealed class CacaoFruitPlacer : UpdatableAndDeletable
{
    public CacaoFruitPlacer(Room room, PlacedObject placedObject)
    {
        if (room.abstractRoom.firstTimeRealized)
        {
            int objIndex = room.roomSettings.placedObjects.IndexOf(placedObject);
            CacaoFruitData data = (CacaoFruitData)placedObject.data;

            if (room.game.session is not StoryGameSession session || !session.saveState.ItemConsumed(room.world, false, room.abstractRoom.index, objIndex))
            {
                AbstractConsumable obj = new(room.world, AbstractObjectType.DangleFruit, null, room.GetWorldCoordinate(placedObject.pos), room.game.GetNewID(), room.abstractRoom.index, objIndex, new PlacedObject.ConsumableObjectData(placedObject))
                {
                    isConsumed = false,
                    minCycles = data.MinCycles,
                    maxCycles = data.MaxCycles
                };

                CacaoFruitHooks.MakeCacao(obj, data.Type, room.world);

                room.abstractRoom.entities.Add(obj);
            }
        }

        Destroy();
    }
}