namespace Nyctophobia;

public class ScarletLizardAI : LizardAI
{
    public ScarletLizardAI(AbstractCreature creature) : base(creature, creature.world)
    {
        yellowAI = new YellowAI(this);
        AddModule(yellowAI);
    }

    public override PathCost TravelPreference(MovementConnection connection, PathCost cost)
    {
        return yellowAI.TravelPreference(connection, cost);
    }
}