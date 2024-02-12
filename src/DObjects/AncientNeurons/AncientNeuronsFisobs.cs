namespace Nyctophobia;

public class AncientNeuronsFisobs : Fisob
{
    public AncientNeuronsFisobs() : base(NTEnums.AbstractObjectType.AncientNeuron)
    {
        Icon = new SimpleIcon("Symbol_Neuron", Color.yellow);

        RegisterUnlock(NTEnums.SandboxUnlock.AncientNeuron);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
    {
        int origRoom = 0;
        int placedObjectIndex = 0;
        var result = new AncientNeuronsAbstract(world, null, entitySaveData.Pos, entitySaveData.ID, origRoom, placedObjectIndex, null);

        return result;
    }

    private static readonly AncientNeuronProperties ancientNeuronProperties = new();

    public override ItemProperties Properties(PhysicalObject forObject) => ancientNeuronProperties;
}
