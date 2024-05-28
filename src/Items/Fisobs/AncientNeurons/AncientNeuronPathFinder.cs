namespace Nyctophobia;

public class AncientNeuronPathFinder(ArtificialIntelligence artificialIntelligence, World world, AbstractCreature abstractCreature) : PathFinder(artificialIntelligence, world, abstractCreature), IUseARelationshipTracker
{
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