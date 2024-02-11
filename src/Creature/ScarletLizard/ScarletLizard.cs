namespace Witness
{
    public class ScarletLizard : Lizard
    {
        public ScarletLizard(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
            effectColor = Custom.HSL2RGB(Custom.WrappedRandomVariation(0.0025f, 0.02f, 0.6f), 1f, Custom.ClampedRandomVariation(0.5f, 0.15f, 0.1f));
        }
    }
}
