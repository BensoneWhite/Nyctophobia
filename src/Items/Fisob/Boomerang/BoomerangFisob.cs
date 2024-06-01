namespace Nyctophobia;

public class BoomerangFisob : Fisob
{
    public BoomerangFisob() : base(NTEnums.AbstractObjectTypes.GenericBoomerang)
    {
        Icon = IsPrideDay
            ? new SimpleIcon("LizardArm_05", new Color(Random.value, Random.value, Random.value))
            : (Icon)new SimpleIcon("LizardArm_05", Ext.MenuGrey);

        RegisterUnlock(NTEnums.SandboxUnlock.GenericBoomerang);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        var boomerang = new BoomerangAbstract(world, entitySaveData.Pos, entitySaveData.ID);

        return boomerang;
    }

    private static readonly BoomerangProperties BoomerangProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject) => BoomerangProperties;
}