namespace Nyctophobia;

public class BlueLanternProperties : ItemProperties
{
    public override void Throwable(Player player, ref bool throwable)
    {
        throwable = true;
    }

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = ObjectGrabability.OneHand;
    }
}