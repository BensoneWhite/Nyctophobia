﻿namespace Nyctophobia;

public class BloodyFlowerFisob : Fisob
{
    public BloodyFlowerFisob() : base(NTEnums.AbstractObjectTypes.BloodyKarmaFlower)
    {
        Icon = IsPrideDay
            ? new SimpleIcon("Futile_White", new Color(Random.value, Random.value, Random.value))
            : (Icon)new SimpleIcon("Futile_White", Color.cyan);

        RegisterUnlock(NTEnums.SandboxUnlock.BloodyKarmaFlower);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        BloodyFlowerAbstract result = new(world, entitySaveData.Pos, entitySaveData.ID);

        return result;
    }

    private static readonly BloodyFlowerProperties BloodyFlowerProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return BloodyFlowerProperties;
    }
}