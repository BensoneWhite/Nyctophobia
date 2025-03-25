namespace Nyctophobia;

public class CacaoFruitAbstract(World world, WorldCoordinate pos, EntityID ID) : AbstractPhysicalObject(world, NTEnums.AbstractObjectTypes.CacaoFruit, null, pos, ID)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new CacaoFruit(this);
    }
}