namespace Nyctophobia;

public class RedFlareBombFisob : Fisob
{
    private static readonly RedFlareBombsProperties redFlareBombsProperties = new();

    public RedFlareBombFisob() : base(NTEnums.AbstractObjectTypes.RedFlareBomb)
    {
        Icon = new SimpleIcon("Symbol_FlashBomb", Color.red);

        RegisterUnlock(NTEnums.SandboxUnlock.RedFlareBomb);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        int origRoom = 0;
        int placedObjectIndex = 0;
        RedFlareBombAbstract result = new(world, null, entitySaveData.Pos, entitySaveData.ID, origRoom, placedObjectIndex, null);

        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return redFlareBombsProperties;
    }
}