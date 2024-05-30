namespace Nyctophobia;

public class PortalBuddy : AirBreatherCreature
{
    public PortalBuddyAI AI = null;
    public MovementConnection lastFollowedConnection;
    public Vector2 travelDir;

    public PortalBuddy(AbstractCreature abstrCrit) : base(abstrCrit, abstrCrit.world)
    {
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0, 0), 9f, 1);
        bodyChunkConnections = [];

        airFriction = 0.98f;
        gravity = 0.9f;
        bounce = 0.1f;
        surfaceFriction = 0.5f;
        collisionLayer = 1;
        waterFriction = 0.9f;
        buoyancy = 0.94f;
        CollideWithTerrain = true;
    }

    public override Color ShortCutColor()
    {
        return Color.magenta;
    }

    public override void InitiateGraphicsModule()
    {
        graphicsModule ??= new PortalBuddyGraphics(this);
        graphicsModule.Reset();
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        if (room == null)
        {
            return;
        }

        if (Consious)
        {
            Act();
        }
    }
    
    public void MoveTowards(Vector2 moveTo)
    {
        var dir = Custom.DirVec(firstChunk.pos, moveTo);
        travelDir = dir;
        bodyChunks[0].vel.y += Mathf.Lerp(gravity, gravity - buoyancy, bodyChunks[0].submersion);
        firstChunk.pos += dir;
        firstChunk.vel += dir * 2f;
        firstChunk.vel *= .85f;

        GoThroughFloors = moveTo.y < bodyChunks[0].pos.y - 5f;
    }

    void Run(MovementConnection followingConnection)
    {
        if (followingConnection.type is MovementConnection.MovementType.ShortCut) {
            enteringShortCut = followingConnection.StartTile;
        } else {
            MoveTowards(room.MiddleOfTile(followingConnection.DestTile));
        }
        lastFollowedConnection = followingConnection;
    }

    public void Act()
    {
        AI.Update();

        var followingPos = bodyChunks[0].pos;

        var pather = AI.pathFinder as StandardPather;
        var movementConnection = pather!.FollowPath(room.GetWorldCoordinate(followingPos), true);
        if (movementConnection != null) {
            Run(movementConnection);
        } else {
            if (lastFollowedConnection != null) {
                MoveTowards(room.MiddleOfTile(lastFollowedConnection.DestTile));
            }
            if (Submersion > 0.5f) {
                firstChunk.vel += new Vector2((Random.value - 0.5f) * 0.5f, Random.value * 0.5f);
                if (Random.value < 0.1f) {
                    bodyChunks[0].vel += new Vector2((Random.value - 0.5f) * 2f, Random.value * 1.5f);
                }
            }
            GoThroughFloors = false;
        }
    }
}