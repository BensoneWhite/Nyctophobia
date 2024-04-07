namespace Nyctophobia;
public class BlueBombaAbstract : AbstractPhysicalObject
{
    public BlueBombaAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, NTEnums.AbstractObjectType.Bluebomba, null, pos, ID)
    {
    }

    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new BlueBomba(this, world);
    }
}
