namespace Nyctophobia;

public class BlueSpearFisob : Fisob
{
    public static readonly BlueSpearFisob Instance = new();
    public static readonly BlueSpearProperties BlueSpearProperties = new();

    public BlueSpearFisob() : base(NTEnums.AbstractObjectTypes.BlueSpear)
    {
        Icon = new SimpleIcon("Symbol_FireSpear", Color.cyan);

        RegisterUnlock(NTEnums.SandboxUnlock.BlueSpear, SandboxUnlockID.Slugcat);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        BlueSpearAbstract bluespear = new(world, null, entitySaveData.Pos, entitySaveData.ID, true, 0f);

        return bluespear;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return BlueSpearProperties;
    }
}