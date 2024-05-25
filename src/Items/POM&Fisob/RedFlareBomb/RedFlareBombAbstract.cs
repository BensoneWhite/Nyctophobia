namespace Nyctophobia;

public class RedFlareBombAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableObjectData) : AbstractConsumable(world, NTEnums.AbstractObjectTypes.RedFlareBomb, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableObjectData)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new RedFlareBomb(this, world);
    }
}