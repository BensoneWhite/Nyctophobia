namespace Nyctophobia;

public class BlueBombaAbstract(World world, WorldCoordinate pos, EntityID ID) : AbstractPhysicalObject(world, NTEnums.AbstractObjectType.Bluebomba, null, pos, ID)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new BlueBomba(this, world);
    }
}