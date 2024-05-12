using static SharedPhysics;

namespace Nyctophobia;

public class Boykisser : Creature
{
    public BoyKisserAI AI;

    public Boykisser boykisser;

    public Vector2 travelDir;

    public int specialMoveCounter;

    public IntVector2 specialMoveDestination;

    public MovementConnection lastFollowedConnection;

    public static Vector2 boykisserPos;

    public ChunkDynamicSoundLoop soundLoop;

    public ChunkDynamicSoundLoop soundLoop2;

    public float distanceToPlayer;

    public Boykisser(AbstractCreature acrit) : base(acrit, acrit.world)
    {
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 5f, 1f);
        bodyChunkConnections = [];
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.1f;
        surfaceFriction = 0.4f;
        collisionLayer = 1;
        waterFriction = 0.95f;
        buoyancy = 0.95f;
    }

    public override Color ShortCutColor()
    {
        return Custom.HSL2RGB(1f, 1f, 1f);
    }

    public override void InitiateGraphicsModule()
    {
        graphicsModule ??= new BoyKisserGraphics(this);
        graphicsModule.Reset();
    }

    public override void SpitOutOfShortCut(IntVector2 pos, Room newRoom, bool spitOutAllSticks)
    {
        base.SpitOutOfShortCut(pos, newRoom, spitOutAllSticks);
        Vector2 val = Custom.IntVector2ToVector2(newRoom.ShorcutEntranceHoleDirection(pos));
        for (int i = 0; i < bodyChunks.Length; i++)
        {
            bodyChunks[i].pos = newRoom.MiddleOfTile(pos) - (val * (-1.5f + i) * 15f);
            bodyChunks[i].lastPos = newRoom.MiddleOfTile(pos);
            bodyChunks[i].vel = val * 2f;
        }
        graphicsModule?.Reset();
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (room != null)
        {
            float minVolume = 0.1f;
            float maxVolume = 0.3f;
            float lerpFactor = Mathf.InverseLerp(0.0f, 100.0f, distanceToPlayer);

            distanceToPlayer = Vector2.Distance(GeneralHooks.generalPlayerMainPos, mainBodyChunk.pos);

            soundLoop ??= new ChunkDynamicSoundLoop(mainBodyChunk)
            {
                sound = NTEnums.Sound.BoyKisserSilly
            };

            soundLoop2 ??= new ChunkDynamicSoundLoop(mainBodyChunk)
            {
                sound = NTEnums.Sound.BoyKisserChase
            };

            if (soundLoop != null)
            {
                soundLoop.Volume = distanceToPlayer is < 2000.0f and >= 200.0f
                    ? Custom.LerpAndTick(minVolume, maxVolume, lerpFactor, 0.01f)
                    : Custom.LerpAndTick(maxVolume, 0f, 0.1f, 1f);

                soundLoop.Update();
            }
            if (soundLoop2 != null)
            {
                soundLoop2.Volume = distanceToPlayer is < 200f and >= 0.0f
                    ? Custom.LerpAndTick(minVolume, maxVolume, lerpFactor, 0.01f)
                    : Custom.LerpAndTick(maxVolume, 0f, 0.1f, 1f);

                soundLoop2.Update();
            }
            BoyKisserAct();
        }
    }

    public void BoyKisserAct()
    {
        AI.Update();

        if ((room is not null && room.game.world.rainCycle.timer > 1000f) || room.game.IsArenaSession)
        {
            CollisionResult val = TraceProjectileAgainstBodyChunks(null, room, mainBodyChunk.lastPos, ref mainBodyChunk.pos, 40f, 1, this, false);
            if (val.obj != null)
            {
                PhysicalObject obj = val.obj;
                Creature val2 = (Creature)((obj is Creature) ? obj : null);
                if (val2 is not null and not Boykisser)
                {
                    val2.Die();
                    Vector2 val3 = Custom.RNV() * 30f;
                    BodyChunk[] bodyChunks = val2.bodyChunks;
                    foreach (BodyChunk val4 in bodyChunks)
                    {
                        val4.vel += val3;
                    }
                    _ = room.PlaySound(NTEnums.Sound.BoyKisserKiss, mainBodyChunk, false, 0.5f, 0.75f);
                }
            }
        }
        if (specialMoveCounter > 0)
        {
            specialMoveCounter--;
            BoyKisserMoveTowards(room.MiddleOfTile(specialMoveDestination), climb: false);
            travelDir = Vector2.Lerp(travelDir, Custom.DirVec(mainBodyChunk.pos, room.MiddleOfTile(specialMoveDestination)), 0.4f);
            if (Custom.DistLess(mainBodyChunk.pos, room.MiddleOfTile(specialMoveDestination), 5f))
            {
                specialMoveCounter = 0;
            }
        }
        else if (room.GetWorldCoordinate(mainBodyChunk.pos) == AI.pathFinder.GetDestination)
        {
            GoThroughFloors = false;
        }
        else
        {
            PathFinder pathFinder = AI.pathFinder;
            MovementConnection val5 = ((StandardPather)((pathFinder is StandardPather) ? pathFinder : null)).FollowPath(room.GetWorldCoordinate(mainBodyChunk.pos), true);
            if (val5 != null)
            {
                BoyKisserRuns(val5);
                travelDir = Vector2.Lerp(travelDir, Custom.DirVec(mainBodyChunk.pos, room.MiddleOfTile(val5.destinationCoord)), 0.4f);
            }
            else
            {
                GoThroughFloors = false;
            }
        }
    }

    public void BoyKisserRuns(MovementConnection followingConnection)
    {
        if ((int)followingConnection.type == 2)
        {
            PathFinder pathFinder = AI.pathFinder;
            ((StandardPather)((pathFinder is StandardPather) ? pathFinder : null)).pastConnections.Clear();
        }
        if ((int)followingConnection.type is 13 or 14)
        {
            enteringShortCut = followingConnection.StartTile;
            if ((int)followingConnection.type == 14)
            {
                NPCTransportationDestination = followingConnection.destinationCoord;
            }
        }
        else if ((int)followingConnection.type is 1 or 10 or 2 or 4 or 5 or 12 or 13 or 14)
        {
            specialMoveCounter = 30;
            specialMoveDestination = followingConnection.DestTile;
        }
        else
        {
            Vector2 val = room.MiddleOfTile(followingConnection.DestTile);
            if (lastFollowedConnection != null && (int)lastFollowedConnection.type == 2)
            {
                BodyChunk mainBodyChunk = this.mainBodyChunk;
                mainBodyChunk.vel += Custom.DirVec(mainBodyChunk.pos, val) * 4f;
            }
            if (lastFollowedConnection != null && room.aimap.TileAccessibleToCreature(mainBodyChunk.pos, Template) && ((followingConnection.startCoord.x != followingConnection.destinationCoord.x && lastFollowedConnection.startCoord.x == lastFollowedConnection.destinationCoord.x) || (followingConnection.startCoord.y != followingConnection.destinationCoord.y && lastFollowedConnection.startCoord.y == lastFollowedConnection.destinationCoord.y)))
            {
                BodyChunk mainBodyChunk2 = mainBodyChunk;
                mainBodyChunk2.vel *= 0.7f;
            }
            if ((int)room.aimap.getAItile(followingConnection.DestTile).acc == 3)
            {
                BoyKisserMoveTowards(val, climb: true);
            }
            else
            {
                BoyKisserMoveTowards(val, climb: false);
            }
        }
        lastFollowedConnection = followingConnection;
    }

    public void BoyKisserMoveTowards(Vector2 destination, bool climb)
    {
        Vector2 val = Custom.DirVec(this.mainBodyChunk.pos, destination);
        float num = 0.6f;
        num = Mathf.Pow(num, 0.6f);
        BodyChunk mainBodyChunk = this.mainBodyChunk;
        mainBodyChunk.vel += val * ((!room.BeingViewed) ? 3f : 1.8f) * num;
        GoThroughFloors = destination.y < mainBodyChunk.pos.y - 5f;
        if (climb)
        {
            BodyChunk mainBodyChunk2 = mainBodyChunk;
            mainBodyChunk2.pos += val * ((!room.BeingViewed) ? 15f : 8f);
        }
    }
}