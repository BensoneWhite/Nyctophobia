namespace Witness;

public class RedFlareBombFisob : Fisob
{
    public RedFlareBombFisob() : base(NTEnums.AbstractObjectType.RedFlareBomb)
    {
        Icon = new SimpleIcon("Symbol_FlashBomb", Color.red);

        RegisterUnlock(NTEnums.SandboxUnlock.RedFlareBomb);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        int origRoom = 0;
        int placedObjectIndex = 0;
        var result = new RedFlareBombsAbstract(world, null, entitySaveData.Pos, entitySaveData.ID, origRoom, placedObjectIndex, null);

        return result;
    }

    private static readonly RedFlareBombsProperties redFlareBombsProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject) => redFlareBombsProperties;
}
