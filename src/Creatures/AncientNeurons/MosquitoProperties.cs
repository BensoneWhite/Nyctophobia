using Fisobs.Properties;
using MoreSlugcats;

namespace Mosquitoes;

sealed class AncientNeuronProperties : ItemProperties
{
    private readonly AncientNeuron mosquito;

    public AncientNeuronProperties(AncientNeuron mosquito)
    {
        this.mosquito = mosquito;
    }

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        if (mosquito.State.alive) {
            grabability = Player.ObjectGrabability.CantGrab;
        } else {
            grabability = Player.ObjectGrabability.OneHand;
        }
    }

    public override void Nourishment(Player player, ref int quarterPips)
    {
        if (player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint) {
            quarterPips = -1;
        } else {
            quarterPips = 4 * mosquito.FoodPoints;
        }
    }
}
