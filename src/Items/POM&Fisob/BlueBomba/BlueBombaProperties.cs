﻿namespace Nyctophobia;

public class BlueBombaProperties : ItemProperties
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