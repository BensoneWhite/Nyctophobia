namespace Nyctophobia;

public class CacaoFruitAbstract(World world, WorldCoordinate pos, EntityID ID) : AbstractPhysicalObject(world, NTEnums.AbstractObjectType.CacaoFruit, null, pos, ID)
{
    public override void Realize()
    {
        base.Realize();
        realizedObject ??= new CacaoFruit(this);
    }
}