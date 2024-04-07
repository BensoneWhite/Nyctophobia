using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace Nyctophobia;

public class BlueSpearFisob : Fisob
{
    public static readonly BlueSpearFisob Instance = new();
    public static readonly BlueSpearProperties BlueSpearProperties = new();

    public BlueSpearFisob() : base(NTEnums.AbstractObjectType.BlueSpear)
    {
        Icon = new SimpleIcon("Symbol_FireSpear", Color.cyan);

        RegisterUnlock(NTEnums.SandboxUnlock.BlueSpear);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        var bluespear = new BlueSpearAbstract(world, null, entitySaveData.Pos, entitySaveData.ID, true, 0f);

        return bluespear;
    }

    public override ItemProperties Properties(PhysicalObject forObject) => BlueSpearProperties;
}
