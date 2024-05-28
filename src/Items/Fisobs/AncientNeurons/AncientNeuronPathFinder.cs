namespace Nyctophobia;

public class AncientNeuronPathFinder : PathFinder, IUseARelationshipTracker
{
    public AncientNeuronPathFinder(ArtificialIntelligence artificialIntelligence, World world, AbstractCreature abstractCreature) : base(artificialIntelligence, world, abstractCreature)
    {
    }

    AIModule IUseARelationshipTracker.ModuleToTrackRelationship(Relationship relationship)
    {
        return default;
    }

    RelationshipTracker.TrackedCreatureState IUseARelationshipTracker.CreateTrackedCreatureState(RelationshipTracker.DynamicRelationship rel)
    {
        return default;
    }

    Relationship IUseARelationshipTracker.UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
    {
        return default;
    }
}