namespace Nyctophobia;

public class CacaoFruitFisob : Fisob
{
    public CacaoFruitFisob() : base(NTEnums.AbstractObjectTypes.CacaoFruit)
    {
        Icon = IsPrideDay
            ? new SimpleIcon("Symbol_DangleFruit", new Color(Random.value, Random.value, Random.value))
            : (Icon)new SimpleIcon("Symbol_DangleFruit", new Color(0.27f, 0.2f, 0.18f));

        RegisterUnlock(NTEnums.SandboxUnlock.CacaoFruit);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        CacaoFruitAbstract result = new(world, entitySaveData.Pos, entitySaveData.ID);

        return result;
    }

    public static readonly CacaoFruitProperties cacaoFruitProperties;

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return cacaoFruitProperties;
    }
}