namespace Nyctophobia;

public class AncientNeuronsAbstract : AbstractConsumable
{
    public AncientNeuronsAbstract(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int placedObjectIndex, PlacedObject.ConsumableObjectData consumableObjectData) : base(world, NTEnums.AbstractObjectType.AncientNeuron, realizedObject, pos, ID, originRoom, placedObjectIndex, consumableObjectData)
    {
    }

    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new AncientNeuron(this, world);
    }
}
