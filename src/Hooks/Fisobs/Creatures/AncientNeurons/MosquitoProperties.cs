namespace Nyctophobia;

sealed class AncientNeuronProperties(AncientNeuron mosquito) : ItemProperties
{
    private readonly AncientNeuron mosquito = mosquito;

    public override void Grabability(Player player, ref ObjectGrabability grabability)
    {
        if (mosquito.State.alive)
        {
            grabability = ObjectGrabability.Drag;
        }
        else
        {
            grabability = ObjectGrabability.BigOneHand;
        }
    }
}
