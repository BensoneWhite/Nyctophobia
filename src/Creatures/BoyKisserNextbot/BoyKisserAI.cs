namespace Nyctophobia;

public class BoyKisserAI : ArtificialIntelligence, IUseARelationshipTracker
{
    public float currentUtility;
    public Boykisser boykisser;

    public BoyKisserAI(AbstractCreature acrit, Boykisser boykisser) : base(acrit, acrit.world)
    {
        this.boykisser = boykisser;
        boykisser.AI = this;
        AddModule(new StandardPather(this, acrit.world, creature));
        AddModule(new Tracker(this, 10, 10, -1, 0.5f, 10, 10, 10));
        AddModule(new PreyTracker(this, 5, 10f, 0f, 200f, 0.01f));
        AddModule(new RelationshipTracker(this, tracker));
    }

    public AIModule ModuleToTrackRelationship(Relationship relationship)
    {
        return preyTracker;
    }

    public Relationship UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
    {
        return StaticRelationship(dRelation.trackerRep.representedCreature);
    }

    public RelationshipTracker.TrackedCreatureState CreateTrackedCreatureState(RelationshipTracker.DynamicRelationship rel)
    {
        return new();
    }

    public override void Update()
    {
        base.Update();
        if (boykisser.room != null && boykisser.room.regionGate == null)
        {
            _ = GeneralHooks.GeneralPlayerPos;
            if (boykisser.room != null)
            {
                creature.abstractAI.SetDestination(GeneralHooks.GeneralPlayerPos);
            }
        }
    }
}