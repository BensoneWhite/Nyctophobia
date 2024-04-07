namespace Nyctophobia;

public class BlueLanternAbstract : AbstractPhysicalObject
{
    public BlueLanternAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, NTEnums.AbstractObjectType.BlueLantern, null, pos, ID)
    {
    }

    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new BlueLantern(this);
    }
}
