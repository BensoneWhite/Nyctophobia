namespace Nyctophobia;

public class BloodyFlowerProperties : ItemProperties
{
    public override void Throwable(Player player, ref bool throwable)
    {
        throwable = true;
    }

    public override void Grabability(Player player, ref ObjectGrabability grabability)
    {
        grabability = ObjectGrabability.OneHand;
    }
}