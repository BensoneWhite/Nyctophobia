namespace Nyctophobia;

public class BoomerangAbstract(World world, WorldCoordinate pos, EntityID id) : AbstractPhysicalObject(world, NTEnums.AbstractObjectTypes.GenericBoomerang, null, pos, id)
{
    public string DataString;

    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new Boomerang(this);
    }

    public override string ToString()
    {
        return this.SaveToString();
    }
}