namespace Nyctophobia;

public class BloodyFlowerAbstract(World world, WorldCoordinate pos, EntityID ID) : AbstractPhysicalObject(world, NTEnums.AbstractObjectTypes.BloodyKarmaFlower, null, pos, ID)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new BloodyFlower(this);
    }
}