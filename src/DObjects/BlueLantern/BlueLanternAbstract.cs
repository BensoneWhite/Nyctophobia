namespace Nyctophobia;

public class BlueLanternAbstract(World world, WorldCoordinate pos, EntityID ID) : AbstractPhysicalObject(world, NTEnums.AbstractObjectType.BlueLantern, null, pos, ID)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new BlueLantern(this);
    }
}