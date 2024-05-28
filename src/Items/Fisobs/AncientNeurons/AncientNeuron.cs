
using RWCustom;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Nyctophobia;

sealed class AncientNeuron : InsectoidCreature, IPlayerEdible
{
    enum Mode
    {
        Free,
        StuckInChunk
    }

    public AncientNeuronAI AI = null!;
    public float runSpeed;
    public Vector2 headdir;
    public Vector2 lastheaddir;


    // IntVector2 stuckTile;

    int stuckCounter;
    MovementConnection? lastFollowedConnection;
    Vector2 mvdir;
    Vector2 stuckPos;
    Vector2 stuckDir;
    Mode mode;

    public AncientNeuron(AbstractCreature acrit) : base(acrit, acrit.world)
    {
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 8f, .15f);
        bodyChunkConnections = new BodyChunkConnection[0];

        headdir = Custom.RNV();
        lastheaddir = headdir;

    }



    public override Color ShortCutColor()
    {
        return new Color(.7f, .4f, .4f);
    }

    public override void InitiateGraphicsModule()
    {
        graphicsModule ??= new AncientNeuronGraphics(this);
        graphicsModule.Reset();
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        if (room == null) {
            return;
        }

        lastheaddir = headdir;

        if (grasps[0] == null && mode == Mode.StuckInChunk) {
            ChangeMode(Mode.Free);
        }

        switch (mode) {
            case Mode.Free:
                headdir += mvdir * .4f;
                headdir.Normalize();
                break;
            case Mode.StuckInChunk:
                BodyChunk stuckInChunk = grasps[0].grabbedChunk;

                headdir = Custom.RotateAroundOrigo(stuckDir, Custom.VecToDeg(stuckInChunk.Rotation));
                firstChunk.pos = StuckInChunkPos(stuckInChunk) + Custom.RotateAroundOrigo(stuckPos, Custom.VecToDeg(stuckInChunk.Rotation));
                firstChunk.vel *= 0f;



                break;
        }

        if (stuckCounter > 0) {
            stuckCounter--;

            if (stuckCounter == 0) {
                Explode();
            }
        }

        if (Consious) {
            Act();
        } else {
            Explode();
        }
    }

    void Explode()
    {
        room.AddObject(new Explosion(room, this, firstChunk.pos, 7, 150f, 4.2f, 1.5f, 200f, 0.25f, this, 0.7f, 160f, 1f));
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos, 180f, 1f, 7, Color.red));
        room.AddObject(new Explosion.ExplosionLight(firstChunk.pos, 130f, 1f, 3, new Color(1f, 1f, 1f)));
        room.AddObject(new ExplosionSpikes(room, firstChunk.pos, 14, 30f, 9f, 7f, 170f, Color.red));
        room.AddObject(new ShockWave(firstChunk.pos, 230f, 0.045f, 5));
        room.PlaySound(SoundID.Bomb_Explode, firstChunk.pos, 0.7f, 1.1f);
        LoseAllGrasps();
        Destroy();
    }

    void Act()
    {
        AI.Update();

        Vector2 followingPos = bodyChunks[0].pos;
        GoThroughFloors = false;


        var pather = AI.pathFinder as StandardPather;
        var movementConnection = pather!.FollowPath(room.GetWorldCoordinate(followingPos), true);
        if (movementConnection == default) {movementConnection = pather.FollowPath(room.GetWorldCoordinate(followingPos), true);
        }
        if (movementConnection != default) {
            Run(movementConnection);
        } else {
            if (lastFollowedConnection != null) 
            {
                MoveTowards(room.MiddleOfTile(lastFollowedConnection.Value.DestTile));
            }

            GoThroughFloors = false;
        }
    }

    void MoveTowards(Vector2 moveTo)
    {
        Vector2 dir = Custom.DirVec(firstChunk.pos, moveTo);
        Vector2 yoing = new Vector2(moveTo[0] - moveTo[0], moveTo[1] - moveTo[1]);
        float magnitude = (float)(Math.Atan(yoing.magnitude)/Math.PI)+1;
        firstChunk.pos += 0.4f*dir;
        firstChunk.pos += 0.6f*dir*magnitude;
        //I'm trying to be generous here ok
        firstChunk.vel += 0.08f*dir;
        firstChunk.vel += 0.12f*dir*magnitude;
        firstChunk.vel *= .85f;


    }

    void Run(MovementConnection followingConnection)
    {
        if (followingConnection.type is MovementConnection.MovementType.ShortCut or MovementConnection.MovementType.NPCTransportation) {
            enteringShortCut = new IntVector2?(followingConnection.StartTile);
            if (followingConnection.type == MovementConnection.MovementType.NPCTransportation) {
                NPCTransportationDestination = followingConnection.destinationCoord;
            }
        } else {
            MoveTowards(room.MiddleOfTile(followingConnection.DestTile));
        }
        lastFollowedConnection = followingConnection;
    }

    Vector2 StuckInChunkPos(BodyChunk chunk)
    {
        return chunk.owner?.graphicsModule is PlayerGraphics g ? g.drawPositions[chunk.index, 0] : chunk.pos;
    }

    public override void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        base.Collide(otherObject, myChunk, otherChunk);

        if (Consious && grasps[0] == null && otherObject is Creature c && c.State.alive && AI.preyTracker.MostAttractivePrey?.representedCreature == c.abstractCreature) {
            StickIntoChunk(otherObject, otherChunk);
        }
    }

    void StickIntoChunk(PhysicalObject otherObject, int otherChunk)
    {
        stuckCounter = otherObject switch {
            Creature { dead: false } => Random.Range(50, 60),
            Creature => Random.Range(50, 100),
            _ => Random.Range(25, 50),
        };

        BodyChunk chunk = otherObject.bodyChunks[otherChunk];

        firstChunk.pos = chunk.pos + Custom.DirVec(chunk.pos, firstChunk.pos) * chunk.rad + Custom.DirVec(chunk.pos, firstChunk.pos) * 11f;
        stuckPos = Custom.RotateAroundOrigo(firstChunk.pos - StuckInChunkPos(chunk), -Custom.VecToDeg(chunk.Rotation));
        stuckDir = Custom.RotateAroundOrigo(Custom.DirVec(firstChunk.pos, Custom.DirVec(firstChunk.pos, chunk.pos)), -Custom.VecToDeg(chunk.Rotation));

        Grab(otherObject, 0, otherChunk, Grasp.Shareability.CanOnlyShareWithNonExclusive, .5f, false, false);

        if (grasps[0]?.grabbed is Creature grabbed) {
            grabbed.Violence(firstChunk, Custom.DirVec(firstChunk.pos, chunk.pos) * 3f, chunk, null, DamageType.Stab, 0.06f, 7f);
        } else {
            chunk.vel += Custom.DirVec(firstChunk.pos, chunk.pos) * 3f / chunk.mass;
        }

        new DartMaggot.DartMaggotStick(abstractPhysicalObject, chunk.owner.abstractPhysicalObject);

        ChangeMode(Mode.StuckInChunk);
    }

    void ChangeMode(Mode newMode)
    {
        if (mode != newMode) {
            mode = newMode;
            CollideWithTerrain = mode == Mode.Free;

            if (mode == Mode.Free) {
                abstractPhysicalObject.LoseAllStuckObjects();
                LoseAllGrasps();
                Stun(20);
                room.PlaySound(SoundID.Spear_Dislodged_From_Creature, firstChunk, false, 0.8f, 1.2f);
            } else {
                room.PlaySound(SoundID.Dart_Maggot_Stick_In_Creature, firstChunk, false, 0.8f, 1.2f);
            }
        }
    }

    public override void Violence(BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, Appendage.Pos hitAppendage, DamageType type, float damage, float stunBonus)
    {
        base.Violence(source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        if (source?.owner is Weapon && directionAndMomentum.HasValue) {
            hitChunk.vel = source.vel * source.mass / hitChunk.mass;
        }

        float speed = Mathf.Max(1, directionAndMomentum.GetValueOrDefault().magnitude);

    }

    int bites = 2;
//    private AncientNeuronsAbstract ancientNeuronsAbstract;

    public int BitesLeft => bites;
    public int FoodPoints => 1; //hope this is one whole thing and not a quarter
    bool IPlayerEdible.Edible => true;
    bool IPlayerEdible.AutomaticPickUp => false;

    void IPlayerEdible.ThrowByPlayer() { }

    void IPlayerEdible.BitByPlayer(Grasp grasp, bool eu)
    {
        bites --;

        
        firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);

        if (bites == 0 && grasp.grabber is Player p) {
            p.ObjectEaten(this);
            grasp.Release();
            Destroy();
        }
    }
}
