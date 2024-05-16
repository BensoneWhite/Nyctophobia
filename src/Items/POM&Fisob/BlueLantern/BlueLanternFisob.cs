namespace Nyctophobia;

public class BlueLanternFisob : Fisob
{
    public BlueLanternFisob() : base(NTEnums.AbstractObjectType.BlueLantern)
    {
        Icon = new SimpleIcon("Symbol_Lantern", new Color(0.196f, 0.596f, 0.965f));

        RegisterUnlock(NTEnums.SandboxUnlock.BlueLantern);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        BlueLanternAbstract result = new(world, entitySaveData.Pos, entitySaveData.ID);

        return result;
    }

    private static readonly BlueLanternProperties BlueLanternProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return BlueLanternProperties;
    }
}