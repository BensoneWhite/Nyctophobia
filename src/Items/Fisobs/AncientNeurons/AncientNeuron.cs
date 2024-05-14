namespace Nyctophobia;

public class AncientNeuron : OracleSwarmer
{
    public AncientNeuron(AbstractConsumable abstractConsumable, World world) : base(abstractConsumable, world)
    {
        bodyChunks[0] = new BodyChunk(this, 0, default, 3f, 0.2f);
    }
}