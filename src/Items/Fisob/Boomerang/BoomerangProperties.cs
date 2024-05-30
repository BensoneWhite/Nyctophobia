namespace Nyctophobia;

public class BoomerangProperties : ItemProperties
{
    public override void ScavCollectScore(Scavenger scav, ref int score) => score = 5;

    public override void ScavWeaponPickupScore(Scavenger scav, ref int score) => score = 3;

    public override void ScavWeaponUseScore(Scavenger scav, ref int score) => score = 3;

    public override void LethalWeapon(Scavenger scav, ref bool isLethal) => isLethal = true;

    public override void Grabability(Player player, ref ObjectGrabability grabability) => grabability = ObjectGrabability.BigOneHand;

    public override void Throwable(Player player, ref bool throwable) => throwable = true;
}