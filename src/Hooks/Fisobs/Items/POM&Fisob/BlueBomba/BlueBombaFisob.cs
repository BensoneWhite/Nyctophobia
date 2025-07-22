namespace Nyctophobia;

public class BlueBombaFisob : Fisob
{
    public BlueBombaFisob() : base(NTEnums.AbstractObjectTypes.Bluebomba)
    {
        Icon = new SimpleIcon("Symbol_StunBomb", Color.cyan);

        RegisterUnlock(NTEnums.SandboxUnlock.BlueBomba);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        BlueBombaAbstract result = new(world, entitySaveData.Pos, entitySaveData.ID);

        return result;
    }

    private static readonly BlueBombaProperties BlueBombaProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return BlueBombaProperties;
    }
}