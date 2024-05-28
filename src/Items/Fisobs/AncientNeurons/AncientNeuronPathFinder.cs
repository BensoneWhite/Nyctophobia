using static Nyctophobia.AncientNeuronAI;

namespace Nyctophobia;

public class AncientNeuronPathFinder : PathFinder, IUseARelationshipTracker
{
    private AncientNeuronAI AncientNeuronAI { get; set; }

    private AncientNeuron AncientNeuron { get; set; }

    public AncientNeuronPathFinder(ArtificialIntelligence artificialIntelligence, World world, AbstractCreature abstractCreature) : base(artificialIntelligence, world, abstractCreature)
    {
        AncientNeuron = new AncientNeuron(abstractCreature);
        AncientNeuronAI = new AncientNeuronAI(abstractCreature, AncientNeuron);
    }

    AIModule IUseARelationshipTracker.ModuleToTrackRelationship(Relationship relationship)
    {
        if (relationship.type == Eats) return AncientNeuronAI.preyTracker;
        if (relationship.type == Afraid) return AncientNeuronAI.threatTracker;
        return null;
    }

    RelationshipTracker.TrackedCreatureState IUseARelationshipTracker.CreateTrackedCreatureState(RelationshipTracker.DynamicRelationship rel)
    {
        return new AncientNeuronTrackedState();
    }

    Relationship IUseARelationshipTracker.UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
    {
        if (dRelation.state is not AncientNeuronTrackedState state) return default;

        if (dRelation.trackerRep.VisualContact)
        {
            dRelation.state.alive = dRelation.trackerRep.representedCreature.state.alive;
        }

        if (!dRelation.state.alive)
        {
            return new(Ignores, 0f);
        }

        if (dRelation.trackerRep.representedCreature.realizedObject is Creature c && c.State.alive && AncientNeuronAI.aneuron.grasps[0]?.grabbed == c)
        {
            state.prickedTime += 2;
            AncientNeuronAI.preyTracker.ForgetPrey(AncientNeuronAI.tiredOfHuntingCreature);
        }
        else
        {
            state.prickedTime -= 1;
        }

        if (state.prickedTime > 0)
        {
            return new(Afraid, 0.5f);
        }

        return AncientNeuronAI.StaticRelationship(dRelation.trackerRep.representedCreature);
    }
}