namespace Nyctophobia;

public class CreatureData
{
    public readonly bool IsCreature;
    public AbstractCreature creature;

    public int lifes;

    public CreatureData(AbstractCreature creature)
    {
        creature = this.creature;

        this.creature = creature;
        IsCreature = creature == this.creature;

        if (!IsCreature) return;

        if(creature.realizedCreature is Lizard)
            lifes = 2;
        if (creature.realizedCreature is Player)
            lifes = 1;
        if (creature.realizedCreature is Vulture vulture)
        {
            if(vulture.IsMiros)
                lifes = 3;
            else
                lifes = 2;
        }
        if(creature.realizedCreature is MirosBird)
            lifes = 3;
    }
}