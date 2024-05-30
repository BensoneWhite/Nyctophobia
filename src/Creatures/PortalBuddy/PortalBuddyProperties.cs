namespace Nyctophobia;

public class PortalBuddyProperties(PortalBuddy portalBuddy) : ItemProperties
{
    public PortalBuddy portalBuddy = portalBuddy;

    public override void Grabability(Player player, ref ObjectGrabability grabability) => grabability = ObjectGrabability.CantGrab;

    public override void Nourishment(Player player, ref int quarterPips) => quarterPips = 12;
}