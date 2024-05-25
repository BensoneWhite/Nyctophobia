namespace Nyctophobia;

public class BlueBombaAbstract(World world, WorldCoordinate pos, EntityID ID) : AbstractPhysicalObject(world, NTEnums.AbstractObjectTypes.Bluebomba, null, pos, ID)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new BlueBomba(this, world);
    }
}