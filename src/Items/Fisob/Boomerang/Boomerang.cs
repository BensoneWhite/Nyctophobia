namespace Nyctophobia;

public class Boomerang : Weapon
{
    private const float WingAngle = 68;
    private const float RotationPivotOffset = 20;
    private const float HandleOffset = -0.3f;
    private const float RotationSpeed = 16;
    private const float FlightSpeed = 12;

    public override bool HeavyWeapon => true;
    private float Damage => 0.75f;
    private float Stun => 20;

    private int FlightTime;
    private int StillCounter;

    private PhysicalObject stuckInObject;
    private int stuckInChunkIndex;
    private int stuckBodyPart;
    private float stuckRotation;
    private BodyChunk StuckInChunk => stuckInObject.bodyChunks[stuckInChunkIndex];
    private Appendage.Pos stuckInAppendage;

    public Boomerang(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject, abstractPhysicalObject.world)
    {
        bodyChunks = new BodyChunk[1];
        bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0, 0), 10f, 0.1f);
        bodyChunkConnections = [];
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.4f;
        surfaceFriction = 0.4f;
        collisionLayer = 2;
        waterFriction = 0.98f;
        buoyancy = 0.4f;
        firstChunk.loudness = 7f;
        tailPos = firstChunk.pos;
        soundLoop = new ChunkDynamicSoundLoop(firstChunk);
    }

    public override void ChangeMode(Mode newMode)
    {
        if (mode == Mode.StuckInCreature)
        {
            room?.PlaySound(SoundID.Spear_Dislodged_From_Creature, firstChunk);
            PulledOutOfStuckObject();
            ChangeOverlap(newOverlap: true);
        }
        else if (newMode == Mode.StuckInCreature)
        {
            ChangeOverlap(newOverlap: false);
        }

        base.ChangeMode(newMode);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        if (mode == Mode.Thrown)
        {
            FlightTime++;
            gravity = FlightTime switch
            {
                < 45 => Custom.LerpMap(FlightTime, 0, 45, 0.25f, -0.15f),
                < 85 => -0.1f,
                < 127 => 0.1f,
                _ => Mathf.Min(gravity + 0.05f, 0.9f)
            };

            if (FlightTime > 40)
            {
                if ((throwDir.x > 0 && firstChunk.vel.x > FlightSpeed * -1) || throwDir.x < 0 && firstChunk.vel.x < FlightSpeed)
                {
                    firstChunk.vel.x += FlightSpeed * throwDir.x * -0.025f;
                }

                forbiddenToPlayer = 0;
            }

            if (firstChunk.ContactPoint.y < 0)
            {
                rotationSpeed = 0;
                room.PlaySound(SoundID.Spear_Stick_In_Ground, firstChunk);
                ChangeMode(Mode.Free);
            }
        }
        else
        {
            FlightTime = 0;
            gravity = 0.9f;
        }

        if (mode == Mode.Free)
        {
            if (Custom.DistLess(firstChunk.pos, firstChunk.lastPos, 4f * room.gravity))
            {
                StillCounter++;
            }
            else
            {
                StillCounter = 0;
            }

            if ((firstChunk.ContactPoint.y < 0 || StillCounter > 20) && Mathf.Abs(rotationSpeed) > 0)
            {
                rotationSpeed = 0f;
                rotation = Custom.DegToVec(Mathf.Lerp(-50f, 50f, Random.value) + 130f);
                firstChunk.vel *= 0f;
                room.PlaySound(SoundID.Spear_Stick_In_Ground, firstChunk);
            }
        }

        if (mode == Mode.StuckInCreature)
        {
            if (stuckInAppendage != null)
            {
                setRotation = Custom.DegToVec(stuckRotation + Custom.VecToDeg(stuckInAppendage.appendage.OnAppendageDirection(stuckInAppendage)));
                firstChunk.pos = stuckInAppendage.appendage.OnAppendagePosition(stuckInAppendage);
            }
            else
            {
                if (StuckInChunk.owner is not Creature creature)
                {
                    ChangeMode(Mode.Free);
                }
                else
                {
                    firstChunk.vel = StuckInChunk.vel;

                    if (stuckBodyPart == -1 || !room.BeingViewed || creature.BodyPartByIndex(stuckBodyPart) == null)
                    {
                        setRotation = Custom.DegToVec(stuckRotation + Custom.VecToDeg(StuckInChunk.Rotation));
                        firstChunk.MoveWithOtherObject(eu, StuckInChunk, new Vector2(0f, 0f));
                    }
                    else
                    {
                        setRotation = Custom.DegToVec(stuckRotation + Custom.AimFromOneVectorToAnother(StuckInChunk.pos, creature.BodyPartByIndex(stuckBodyPart).pos));
                        firstChunk.MoveWithOtherObject(eu, StuckInChunk, Vector2.Lerp(StuckInChunk.pos, creature.BodyPartByIndex(stuckBodyPart).pos, 0.5f) - StuckInChunk.pos);
                    }
                }
            }

            if (StuckInChunk.owner.slatedForDeletetion)
            {
                ChangeMode(Mode.Free);
            }
        }
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        if (thrownBy is Player { standing: true })
        {
            thrownPos -= new Vector2(0, 23);
        }

        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc * 0.3f, eu);

        doNotTumbleAtLowSpeed = true;
        firstChunk.vel.x = FlightSpeed * throwDir.x;
        rotationSpeed = RotationSpeed;
    }

    public override bool HitSomething(CollisionResult result, bool eu)
    {
        if (result.obj == null) return false;

        doNotTumbleAtLowSpeed = false;

        if (result.obj is Creature creature)
        {
            if (!ModManager.MSC || creature is not Player || creature.SpearStick(this, Mathf.Lerp(0.55f, 0.62f, Random.value), result.chunk, result.onAppendagePos, firstChunk.vel))
            {
                creature.Violence(firstChunk, firstChunk.vel * firstChunk.mass * 2f, result.chunk, result.onAppendagePos, Creature.DamageType.Stab, Damage, Stun);
                if (ModManager.MSC && creature is Player player)
                {
                    player.playerState.permanentDamageTracking += Damage / player.Template.baseDamageResistance;
                    if (player.playerState.permanentDamageTracking >= 1.0)
                    {
                        player.Die();
                    }
                }
            }

            if (creature.SpearStick(this, Mathf.Lerp(0.55f, 0.62f, Random.value), result.chunk, result.onAppendagePos, firstChunk.vel))
            {
                vibrate = 20;
                ChangeMode(Mode.Free);
                firstChunk.vel = firstChunk.vel * -0.5f + Custom.DegToVec(Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, Random.value) * firstChunk.vel.magnitude;
                SetRandomSpin();
                room.PlaySound(SoundID.Spear_Stick_In_Creature, firstChunk);
                LodgeInCreature(result, eu);
                return true;
            }
        }
        else if (result.chunk != null)
        {
            result.chunk.vel += firstChunk.vel * firstChunk.mass / result.chunk.mass;
        }
        else if (result.onAppendagePos != null && result.obj is IHaveAppendages iHaveAppendages)
        {
            iHaveAppendages.ApplyForceOnAppendage(result.onAppendagePos, firstChunk.vel * firstChunk.mass);
        }

        room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, firstChunk);
        vibrate = 20;
        ChangeMode(Mode.Free);
        firstChunk.vel = firstChunk.vel * -0.5f + Custom.DegToVec(Random.value * 360f) * Mathf.Lerp(0.1f, 0.4f, Random.value) * firstChunk.vel.magnitude;
        SetRandomSpin();
        return false;
    }

    private void LodgeInCreature(CollisionResult result, bool eu)
    {
        try
        {
            if (result.obj is not Creature creature)
            {
                ChangeMode(Mode.Free);
                stuckInObject = null;
                Debug.Log("Boomerang tried to lodge in non-creature!");
                Debug.Log(abstractPhysicalObject.pos);
                Debug.Log(result.obj);
                Debug.Log(room.abstractRoom.name);
                return;
            }

            ChangeMode(Mode.StuckInCreature);

            stuckInObject = creature;
            if (result.chunk != null)
            {
                stuckInChunkIndex = result.chunk.index;
                if (stuckBodyPart == -1)
                {
                    stuckRotation = Custom.Angle(throwDir.ToVector2(), StuckInChunk.Rotation);
                }

                firstChunk.MoveWithOtherObject(eu, StuckInChunk, new Vector2(0f, 0f));
                Debug.Log("Add boomerang to creature chunk " + StuckInChunk.index);
                _ = new AbstractBoomerangStick(abstractPhysicalObject, creature.abstractCreature, stuckInChunkIndex, stuckBodyPart, stuckRotation);
            }
            else if (result.onAppendagePos != null)
            {
                stuckInChunkIndex = 0;
                stuckInAppendage = result.onAppendagePos;
                stuckRotation = Custom.VecToDeg(rotation) - Custom.VecToDeg(stuckInAppendage.appendage.OnAppendageDirection(stuckInAppendage));
                Debug.Log("Add boomerang to creature Appendage");
                _ = new AbstractBoomerangAppendageStick(abstractPhysicalObject, creature.abstractCreature, result.onAppendagePos.appendage.appIndex, result.onAppendagePos.prevSegment, result.onAppendagePos.distanceToNext, stuckRotation);
            }

            if (room.BeingViewed)
            {
                for (var i = 0; i < 8; i++)
                {
                    room.AddObject(new WaterDrip(result.collisionPoint, -firstChunk.vel * Random.value * 0.5f + Custom.DegToVec(360f * Random.value) * firstChunk.vel.magnitude * Random.value * 0.5f, waterColor: false));
                }
            }
        }
        catch (NullReferenceException message)
        {
            ChangeMode(Mode.Free);
            stuckInObject = null;
            Debug.Log("Boomerang lodge in creature failure.");
            Debug.Log(message);
            Debug.Log(abstractPhysicalObject.pos);
            Debug.Log(result.obj);
            Debug.Log(room.abstractRoom.name);
        }
    }

    private void PulledOutOfStuckObject()
    {
        foreach (var objectStick in abstractPhysicalObject.stuckObjects)
        {
            if (objectStick is AbstractBoomerangStick stick && stick.Boomerang == abstractPhysicalObject)
            {
                objectStick.Deactivate();
                break;
            }

            if (objectStick is AbstractBoomerangAppendageStick appendageStick && appendageStick.Boomerang == abstractPhysicalObject)
            {
                objectStick.Deactivate();
                break;
            }
        }

        stuckInObject = null;
        stuckInAppendage = null;
        stuckInChunkIndex = 0;
    }

    public override void HitWall()
    {
        base.HitWall();
        doNotTumbleAtLowSpeed = false;
        forbiddenToPlayer = 0;
    }

    public override void WeaponDeflect(Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    {
        base.WeaponDeflect(inbetweenPos, deflectDir, bounceSpeed);
        doNotTumbleAtLowSpeed = false;
    }

    public override void Grabbed(Creature.Grasp grasp)
    {
        base.Grabbed(grasp);
        doNotTumbleAtLowSpeed = false;
    }

    public override void PickedUp(Creature upPicker)
    {
        ChangeMode(Mode.Carried);
        room.PlaySound(SoundID.Slugcat_Pick_Up_Spear, firstChunk);
    }

    public override void RecreateSticksFromAbstract()
    {
        foreach (var objectStick in abstractPhysicalObject.stuckObjects)
        {
            if (objectStick is AbstractBoomerangStick stick && stick.Boomerang == abstractPhysicalObject && stick.LodgedIn.realizedObject != null)
            {
                stuckInObject = stick.LodgedIn.realizedObject;
                stuckInChunkIndex = stick.chunk;
                stuckBodyPart = stick.bodyPart;
                stuckRotation = stick.angle;
                ChangeMode(Mode.StuckInCreature);
            }
            else if (objectStick is AbstractBoomerangAppendageStick appendageStick && appendageStick.Boomerang == abstractPhysicalObject && appendageStick.LodgedIn.realizedObject != null)
            {
                stuckInObject = appendageStick.LodgedIn.realizedObject;
                stuckInAppendage = new Appendage.Pos(stuckInObject.appendages[appendageStick.appendage], appendageStick.prevSeg, appendageStick.distanceToNext);
                stuckRotation = appendageStick.angle;
                ChangeMode(Mode.StuckInCreature);
            }
        }
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);

        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("CentipedeLegA")
        {
            scaleX = 1.3f
        };
        sLeaser.sprites[1] = new FSprite("FlashWig_MandibleB")
        {
            scaleX = 1.3f
        };

        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

        var firstWing = sLeaser.sprites[0];
        var secondWing = sLeaser.sprites[1];

        var boomerangRotation = Vector2.Lerp(lastRotation, rotation, timeStacker);

        var firstWingRotation = boomerangRotation;
        var secondWingRotation = Custom.DegToVec(Custom.VecToDeg(firstWingRotation) + WingAngle);

        var secondWingStartDistance = firstWing.element.sourcePixelSize.y * 0.9f;

        var firstWingPos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        if (mode == Mode.Carried)
        {
            firstWingPos += boomerangRotation * firstWing.element.sourcePixelSize.y * HandleOffset;
        }

        var secondWingPos = Custom.RotateAroundVector(firstWingPos + new Vector2(0, secondWingStartDistance), firstWingPos, Custom.VecToDeg(firstWingRotation));

        if (mode == Mode.Thrown || mode == Mode.Free)
        {
            var secondWingEnd = secondWingPos + secondWingRotation * secondWingStartDistance;
            var pivotPoint = secondWingPos + Custom.DirVec(secondWingPos, (firstWingPos + secondWingEnd) / 2) * RotationPivotOffset;
            var pivotOffset = firstWingPos - pivotPoint;

            firstWingPos += pivotOffset;
            secondWingPos = Custom.RotateAroundVector(firstWingPos + new Vector2(0, secondWingStartDistance), firstWingPos, Custom.VecToDeg(firstWingRotation));
        }

        if (mode == Mode.StuckInCreature)
        {
            firstWing.anchorY = 0.9f;
            secondWing.anchorY = 1;

            secondWingPos = firstWingPos;
            secondWingRotation = firstWingRotation;

            firstWingPos = Custom.RotateAroundVector(secondWingPos + new Vector2(0, secondWing.element.sourcePixelSize.y), secondWingPos, Custom.VecToDeg(-secondWingRotation));
            firstWingRotation = Custom.DegToVec(Custom.VecToDeg(secondWingRotation) - WingAngle);
        }
        else
        {
            firstWing.anchorY = 0;
            secondWing.anchorY = 0;
        }

        if (blink > 0 && Random.value < 0.5f)
        {
            firstWing.color = blinkColor;
            secondWing.color = blinkColor;
        }
        else
        {
            if (IsPrideDay)
            {
                firstWing.color = new Color(Random.value, Random.value, Random.value);
                secondWing.color = new Color(Random.value, Random.value, Random.value);
            }
            else
            {
                firstWing.color = Color.blue;
                secondWing.color = Color.red;
            }
        }

        firstWing.SetPosition(firstWingPos - camPos);
        firstWing.rotation = Custom.VecToDeg(firstWingRotation);

        secondWing.SetPosition(secondWingPos - camPos);
        secondWing.rotation = Custom.VecToDeg(secondWingRotation);
    }

    public class AbstractBoomerangStick(AbstractPhysicalObject boomerang, AbstractPhysicalObject stuckIn, int chunk, int bodyPart, float angle) : AbstractObjectStick(boomerang, stuckIn)
    {
        public int chunk = chunk;
        public int bodyPart = bodyPart;
        public float angle = angle;

        public AbstractPhysicalObject Boomerang => A;
        public AbstractPhysicalObject LodgedIn => B;

        public override string SaveToString(int roomIndex)
        {
            return SaveUtils.AppendUnrecognizedStringAttrs(string.Format(CultureInfo.InvariantCulture, "{0}<stkA>boomerangLdgStk<stkA>{1}<stkA>{2}<stkA>{3}<stkA>{4}<stkA>{5}", roomIndex, A.ID.ToString(), B.ID.ToString(), chunk, bodyPart, angle), "<stkA>", unrecognizedAttributes);
        }
    }

    public class AbstractBoomerangAppendageStick(AbstractPhysicalObject boomerang, AbstractPhysicalObject stuckIn, int appendage, int prevSeg, float distanceToNext, float angle) : AbstractObjectStick(boomerang, stuckIn)
    {
        public int appendage = appendage;
        public int prevSeg = prevSeg;
        public float distanceToNext = distanceToNext;
        public float angle = angle;

        public AbstractPhysicalObject Boomerang => A;
        public AbstractPhysicalObject LodgedIn => B;

        public override string SaveToString(int roomIndex)
        {
            return SaveUtils.AppendUnrecognizedStringAttrs(string.Format(CultureInfo.InvariantCulture, "{0}<stkA>boomerangLdgAppStk<stkA>{1}<stkA>{2}<stkA>{3}<stkA>{4}<stkA>{5}<stkA>{6}", roomIndex, A.ID.ToString(), B.ID.ToString(), appendage, prevSeg, distanceToNext, angle), "<stkA>", unrecognizedAttributes);
        }
    }
}