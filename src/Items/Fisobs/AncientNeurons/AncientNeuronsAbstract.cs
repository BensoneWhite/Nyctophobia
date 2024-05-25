namespace Nyctophobia;

public class AncientNeuronsAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableObjectData) : AbstractConsumable(world, NTEnums.AbstractObjectTypes.AncientNeuron, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableObjectData)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new AncientNeuron(this, world);
    }
}