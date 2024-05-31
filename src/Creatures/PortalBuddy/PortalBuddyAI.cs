namespace Nyctophobia;

public class PortalBuddyAI : ArtificialIntelligence, IUseARelationshipTracker
{
    private const float DesiredCloseness = 30;
    private const float StandStillDistance = 3;
    private const float ThreatThreshold = 0.3f;
    
    public enum Behavior
    {
        Idle,
        FollowFriend,
        AvoidThreat,
        StandStill
    }

    public PortalBuddy portalBuddy;
    public Behavior behavior;
    public int behaviorCounter;
    public WorldCoordinate tempIdlePos;
    public AbstractCreature friend;
    public float friendDistance;

    public PortalBuddyAI(AbstractCreature acrit, PortalBuddy portalBuddy) : base(acrit, acrit.world)
    {
        this.portalBuddy = portalBuddy;
        portalBuddy.AI = this;
        
        AddModule(new StandardPather(this, acrit.world, acrit));
        pathFinder.stepsPerFrame = 20;
        AddModule(new Tracker(this, 10, 10, 600, 0.5f, 5, 5, 10));
        AddModule(new ThreatTracker(this, 6));
        AddModule(new NoiseTracker(this, tracker));
        AddModule(new RelationshipTracker(this, tracker));
        AddModule(new UtilityComparer(this));
        utilityComparer.AddComparedModule(threatTracker, null, 1f, 1.1f);
        noiseTracker.hearingSkill = 1f;
        behavior = Behavior.FollowFriend;
    }
    
    public override void Update()
    {
        base.Update();

        if (portalBuddy.room == null) return;

        if (friend == null || friend.state.dead || friend.world != portalBuddy.abstractCreature.world)
        {
            friend = portalBuddy.room.game.FirstAlivePlayer;
        }

        //-- Life is not worth it without friends
        if (friend == null) return;

        friendDistance = creature.pos.Tile.FloatDist(friend.pos.Tile);
        
        //-- No fren in sight, must find fren!
        if (friend.Room != portalBuddy.room.abstractRoom || friendDistance > DesiredCloseness * 1.5f)
        {
            behavior = Behavior.FollowFriend;
        }
        //-- Big creature scary! Hide behind fren
        else if (threatTracker.currentThreat > ThreatThreshold)
        {
            behavior = Behavior.AvoidThreat;
            behaviorCounter = 0;
        }
        //-- Fren wants to pet! yay
        else if (friendDistance <= StandStillDistance)
        {
            behavior = Behavior.StandStill;
        }
        
        Debug.LogWarning(behavior);

        switch (behavior)
        {
            case Behavior.Idle:
                WorldCoordinate coord = new(portalBuddy.room.abstractRoom.index, (int)Random.Range(friend.pos.x - DesiredCloseness / 2, friend.pos.x + DesiredCloseness / 2), (int)Random.Range(friend.pos.y - DesiredCloseness / 2, friend.pos.y + DesiredCloseness / 2), -1);
                if (IdleScore(coord) < IdleScore(tempIdlePos))
                {
                    tempIdlePos = coord;
                }

                if (tempIdlePos.Tile.FloatDist(friend.pos.Tile) < DesiredCloseness * 1.2f && IdleScore(tempIdlePos) < IdleScore(pathFinder.GetDestination) + Custom.LerpMap(behaviorCounter, 0f, 300f, 100f, -300f))
                {
                    SetDestination(tempIdlePos);
                    behaviorCounter = Random.Range(100, 400);
                    tempIdlePos = new WorldCoordinate(portalBuddy.room.abstractRoom.index, Random.Range(0, portalBuddy.room.TileWidth), Random.Range(0, portalBuddy.room.TileHeight), -1);
                }

                behaviorCounter--;
                break;
            case Behavior.FollowFriend:
                creature.abstractAI.SetDestination(friend.pos);

                //-- Found fren, am happy
                if (friendDistance < DesiredCloseness && VisualContact(friend.pos, 1))
                {
                    behavior = Behavior.Idle;
                }

                break;
            case Behavior.AvoidThreat:
                if (threatTracker.currentThreat < ThreatThreshold * 0.6f)
                {
                    behavior = Behavior.FollowFriend;
                    break;
                }

                //-- Will check if scary still there
                if (behaviorCounter < Mathf.Lerp(0, 150, threatTracker.currentThreat))
                {
                    //-- Far away from scary
                    var threatPos = threatTracker.mostThreateningCreature.BestGuessForPosition();
                    var threatVector = new Vector2(threatPos.x, threatPos.y);
                    var friendVector = new Vector2(friend.pos.x, friend.pos.y);

                    //-- But still close to fren
                    var safeVector = friendVector + Custom.DirVec(threatVector, friendVector) * DesiredCloseness * 0.5f;
                    var safePos = new WorldCoordinate(friend.Room.index, (int)safeVector.x, (int)safeVector.y, -1);
                    var goToPos = new WorldCoordinate(friend.Room.index, (int)(safePos.x + Random.Range(DesiredCloseness * -0.2f, DesiredCloseness * 0.2f)), (int)(safePos.y + Random.Range(DesiredCloseness * -0.1f, DesiredCloseness * 0.1f)), -1);

                    SetDestination(goToPos);

                    behaviorCounter = Random.Range(200, 400);
                }

                behaviorCounter--;
                break;
            case Behavior.StandStill:
                SetDestination(portalBuddy.abstractCreature.pos);
                break;
        }
    }

    private float IdleScore(WorldCoordinate coord)
    {
        if (coord.NodeDefined || coord.room != creature.pos.room || !pathFinder.CoordinateReachableAndGetbackable(coord) || portalBuddy.room.aimap.getAItile(coord).acc == AItile.Accessibility.Solid) {
            return float.MaxValue;
        }

        var result = 1f;
        
        //-- Don't want to get into narrow spaces
        if (portalBuddy.room.aimap.getAItile(coord).narrowSpace) {
            result += 100f;
        }
        
        //-- Scared of loud noises
        foreach (var source in noiseTracker.sources)
        {
            result += Custom.LerpMap(Vector2.Distance(portalBuddy.room.MiddleOfTile(coord), source.pos), 40f, 400f, 100f, 0f);
        }

        return result;
    }

    public AIModule ModuleToTrackRelationship(Relationship relationship)
    {
        if (relationship.type == Afraid)
        {
            return threatTracker;
        }

        return null;
    }

    public Relationship UpdateDynamicRelationship(RelationshipTracker.DynamicRelationship dRelation)
    {
        return StaticRelationship(dRelation.trackerRep.representedCreature);
    }

    public RelationshipTracker.TrackedCreatureState CreateTrackedCreatureState(RelationshipTracker.DynamicRelationship rel)
    {
        return new RelationshipTracker.TrackedCreatureState();
    }

    public override Tracker.CreatureRepresentation CreateTrackerRepresentationForCreature(AbstractCreature otherCreature)
    {
        if (otherCreature.creatureTemplate.smallCreature)
        {
            return new Tracker.SimpleCreatureRepresentation(tracker, otherCreature, 0f, forgetWhenNotVisible: false);
        }

        return new Tracker.ElaborateCreatureRepresentation(tracker, otherCreature, 1f, 3);
    }
}