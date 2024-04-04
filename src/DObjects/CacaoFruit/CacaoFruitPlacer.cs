namespace Nyctophobia;

public class CacaoFruitData : Pom.Pom.ManagedData
{
    public CacaoFruitType Type => GetValue<CacaoFruitType>(nameof(Type));

    [Pom.Pom.IntegerField(nameof(MinCycles), 0, 20, 0, Pom.Pom.ManagedFieldWithPanel.ControlType.slider, "Min Cycles")]
    public int MinCycles;

    [Pom.Pom.IntegerField(nameof(MaxCycles), 0, 20, 0, Pom.Pom.ManagedFieldWithPanel.ControlType.slider, "Max Cycles")]

    public int MaxCycles;

    public CacaoFruitData(PlacedObject owner) : base(owner, [new Pom.Pom.ExtEnumField<CacaoFruitType>(nameof(Type), CacaoFruitType.CacaoFruit, displayName: nameof(Type))])
    {
    }
}

public sealed class CacaoFruitPlacer : UpdatableAndDeletable
{

    public CacaoFruitPlacer(Room room, PlacedObject placedObject)
    {
        if (room.abstractRoom.firstTimeRealized)
        {
            var objIndex = room.roomSettings.placedObjects.IndexOf(placedObject);
            var data = (CacaoFruitData)placedObject.data;

            if (room.game.session is not StoryGameSession session || !session.saveState.ItemConsumed(room.world, false, room.abstractRoom.index, objIndex))
            {
                var obj = new AbstractConsumable(room.world, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null, room.GetWorldCoordinate(placedObject.pos), room.game.GetNewID(), room.abstractRoom.index, objIndex, new PlacedObject.ConsumableObjectData(placedObject));
                obj.isConsumed = false;
                obj.minCycles = data.MinCycles;
                obj.maxCycles = data.MaxCycles;

                CacaoFruitHooks.MakeCacao(obj, data.Type);

                room.abstractRoom.entities.Add(obj);
            }
        }

        Destroy();
    }

}