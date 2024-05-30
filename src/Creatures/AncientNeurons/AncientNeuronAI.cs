//repurposing hunt for attach nuke kinda, crashfish subnautica aaa shi
namespace Nyctophobia;

public class AncientNeuronAI : ArtificialIntelligence, IUseARelationshipTracker
{
    private enum Behavior
    {
        Patrol,
        OnBreak
    }

    public class AncientNeuronTrackedState : RelationshipTracker.TrackedCreatureState
    {
        public int prickedTime;
    }

    public AncientNeuron aneuron;
    public int tiredOfHuntingCounter;

    //
    private new readonly AncientNeuronPathFinder pathFinder;
    private new readonly DenFinder denFinder;
    private new readonly ThreatTracker threatTracker;

    //former nullable    public AbstractCreature? tiredOfHuntingCreature;
    public AbstractCreature tiredOfHuntingCreature;

    private Behavior behavior;
    private int behaviorCounter;
    private WorldCoordinate tempIdlePos;

    public AncientNeuronAI(AbstractCreature acrit, AncientNeuron aneuron) : base(acrit, acrit.world)
    {
        this.aneuron = aneuron;
        aneuron.AI = this;
        pathFinder = new AncientNeuronPathFinder(this, acrit.world, acrit);
        AddModule(new StandardPather(this, acrit.world, acrit));
        pathFinder.stepsPerFrame = 20;
        AddModule(new DenFinder(this, acrit));
        denFinder = new(this, acrit);
        AddModule(new Tracker(this, 10, 10, 600, 0.5f, 5, 5, 10));
        AddModule(new NoiseTracker(this, tracker));
        AddModule(new ThreatTracker(this, 3));
        threatTracker = new(this, 3);
        AddModule(new PreyTracker(this, 5, 1f, 5f, 150f, 0.05f));
        AddModule(new UtilityComparer(this));
        AddModule(new RelationshipTracker(this, tracker));
        var smoother = new FloatTweener.FloatTweenUpAndDown(new FloatTweener.FloatTweenBasic(FloatTweener.TweenType.Lerp, 0.5f), new FloatTweener.FloatTweenBasic(FloatTweener.TweenType.Tick, 0.005f));
        utilityComparer.AddComparedModule(threatTracker, smoother, 1f, 1.1f);
        utilityComparer.AddComparedModule(rainTracker, null, 1f, 1.1f);
        utilityComparer.AddComparedModule(preyTracker, null, 0.4f, 1.1f);
        noiseTracker.hearingSkill = 0.5f;
        behavior = Behavior.Patrol;
    }

    AIModule IUseARelationshipTracker.ModuleToTrackRelationship(Relationship relationship)
    //was AIModule? IUseARelationshipTracker.ModuleToTrackRelationship(CreatureTemplate.Relationship relationship)
    {
        if (relationship.type == Eats) return preyTracker;
        if (relationship.type == Afraid) return threatTracker;
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

        if (dRelation.trackerRep.representedCreature.realizedObject is Creature c && c.State.alive && aneuron.grasps[0]?.grabbed == c)
        {
            state.prickedTime += 2;
            preyTracker.ForgetPrey(tiredOfHuntingCreature);
        }
        else
        {
            state.prickedTime -= 1;
        }

        if (state.prickedTime > 0)
        {
            return new(Afraid, 0.5f);
        }

        return StaticRelationship(dRelation.trackerRep.representedCreature);
    }

    public override void Update()
    {
        base.Update();

        if (aneuron.room == null)
        {
            return;
        }

        if (pathFinder != null && denFinder != null && threatTracker != null)
        {
            pathFinder.walkPastPointOfNoReturn = stranded
                || (denFinder.GetDenPosition() is WorldCoordinate denPos && !pathFinder.CoordinatePossibleToGetBackFrom(denPos))
                || threatTracker.Utility() > 0.95f;
        }
        else
        {
            Plugin.DebugError("PathFinder AI from AncientNeuron is null");
        }

        utilityComparer.GetUtilityTracker(threatTracker).weight = Custom.LerpMap(threatTracker.ThreatOfTile(creature.pos, true), 0.1f, 2f, 0.1f, 1f, 0.5f);

        if (utilityComparer.HighestUtility() < 0.02f && (behavior != Behavior.Patrol || preyTracker.MostAttractivePrey == null))
        {
            behavior = Behavior.OnBreak;
        }
        else
        {
            behavior = Behavior.Patrol;
        }

        switch (behavior)
        {
            case Behavior.OnBreak:
                aneuron.runSpeed = Custom.LerpAndTick(aneuron.runSpeed, 0.6f + 0.4f * threatTracker.Utility(), 0.01f, 0.016666668f);

                WorldCoordinate coord = new(aneuron.room.abstractRoom.index, Random.Range(0, aneuron.room.TileWidth), Random.Range(0, aneuron.room.TileHeight), -1);
                if (IdleScore(tempIdlePos) > IdleScore(coord))
                {
                    tempIdlePos = coord;
                }

                if (IdleScore(tempIdlePos) < IdleScore(pathFinder.GetDestination) + Custom.LerpMap(behaviorCounter, 0f, 300f, 100f, -300f))
                {
                    SetDestination(tempIdlePos);
                    behaviorCounter = Random.Range(100, 400);
                    tempIdlePos = new WorldCoordinate(aneuron.room.abstractRoom.index, Random.Range(0, aneuron.room.TileWidth), Random.Range(0, aneuron.room.TileHeight), -1);
                }

                behaviorCounter--;
                break;

            case Behavior.Patrol:
                aneuron.runSpeed = Custom.LerpAndTick(aneuron.runSpeed, 1f, 0.01f, .1f);

                if (preyTracker.MostAttractivePrey != null)
                {
                    creature.abstractAI.SetDestination(preyTracker.MostAttractivePrey.BestGuessForPosition());
                }
                else
                {
                    behavior = Behavior.OnBreak;
                }
                tiredOfHuntingCounter++;
                if (tiredOfHuntingCounter > 100)
                {
                    tiredOfHuntingCreature = preyTracker.MostAttractivePrey?.representedCreature;
                    tiredOfHuntingCounter = 0;
                    preyTracker.ForgetPrey(tiredOfHuntingCreature);
                    tracker.ForgetCreature(tiredOfHuntingCreature);
                }
                break;
        }
    }

    private float IdleScore(WorldCoordinate coord)
    {
        if (coord.NodeDefined || coord.room != creature.pos.room || !pathFinder.CoordinateReachableAndGetbackable(coord) || aneuron.room.aimap.getAItile(coord).acc == AItile.Accessibility.Solid)
        {
            return float.MaxValue;
        }
        float result = 1f;
        if (aneuron.room.aimap.getAItile(coord).narrowSpace)
        {
            result += 100f;
        }
        result += threatTracker.ThreatOfTile(coord, true) * 1000f;
        result += threatTracker.ThreatOfTile(aneuron.room.GetWorldCoordinate((aneuron.room.MiddleOfTile(coord) + aneuron.room.MiddleOfTile(creature.pos)) / 2f), true) * 1000f;
        for (int i = 0; i < noiseTracker.sources.Count; i++)
        {
            result += Custom.LerpMap(Vector2.Distance(aneuron.room.MiddleOfTile(coord), noiseTracker.sources[i].pos), 40f, 400f, 100f, 0f);
        }
        return result;
    }

    public override Tracker.CreatureRepresentation CreateTrackerRepresentationForCreature(AbstractCreature otherCreature)
    {
        return otherCreature.creatureTemplate.smallCreature
            ? new Tracker.SimpleCreatureRepresentation(tracker, otherCreature, 0f, false)
            : new Tracker.ElaborateCreatureRepresentation(tracker, otherCreature, 1f, 3);
    }

    public override PathCost TravelPreference(MovementConnection coord, PathCost cost)
    {
        float val = Mathf.Max(0f, threatTracker.ThreatOfTile(coord.destinationCoord, false) - threatTracker.ThreatOfTile(creature.pos, false));
        return new PathCost(cost.resistance + Custom.LerpMap(val, 0f, 1.5f, 0f, 10000f, 5f), cost.legality);
    }
}