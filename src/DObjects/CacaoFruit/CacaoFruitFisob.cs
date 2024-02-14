namespace Nyctophobia;

public class CacaoFruitFisob : Fisob
{
    public CacaoFruitFisob() : base(NTEnums.AbstractObjectType.CacaoFruit)
    {
        Icon = new SimpleIcon("Symbol_DangleFruit", new Color(0.44f, 0.22f, 0.06f));

        RegisterUnlock(NTEnums.SandboxUnlock.CacaoFruit);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        int origRoom = 0;
        int placedObjectIndex = 0;
        var result = new CacaoFruitAbstract(world, null, entitySaveData.Pos, entitySaveData.ID, origRoom, placedObjectIndex, null);

        return result;
    }

    private static readonly CacaoFruitProperties cacaoFruitProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject) => cacaoFruitProperties;
}
