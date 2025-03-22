namespace Nyctophobia;

public class ESPBehavior : OracleBehavior, Conversation.IOwnAConversation
{
    public class ESPSleepoverBehavior : ConversationBehavior
    {
        public bool holdPlayer;

        public bool gravOn;

        public bool firstMetOnThisCycle;

        public float lowGravity;

        public float lastGetToWork;

        public float tagTimer;

        public OraclePanicDisplay panicObject;

        public int timeUntilNextPanic;

        public int panicTimer;

        private Vector2 HoldPlayerPos => new(668f, 268f + (Mathf.Sin(InActionCounter / 70f * (float)Math.PI * 2f) * 4f));

        public override bool Gravity => gravOn;

        public override float LowGravity => lowGravity;

        public ESPSleepoverBehavior(ESPBehavior owner) : base(owner, NTEnums.ESPBehaviorSubBehavID.SlumberParty, MoreSlugcatsEnums.ConversationID.MoonGiveMark)
        {
            PickNextPanicTime();
            lowGravity = -1f;
            if (!Oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
            {
                owner.getToWorking = 0f;
                gravOn = false;
                firstMetOnThisCycle = true;
                owner.SlugcatEnterRoomReaction();
                base.owner.voice = Oracle.room.PlaySound(SoundID.SL_AI_Talk_4, Oracle.firstChunk);
                base.owner.voice.requireActiveUpkeep = true;
                owner.LockShortcuts();
                return;
            }
            if (base.owner.conversation != null)
            {
                base.owner.conversation.Destroy();
                base.owner.conversation = null;
                return;
            }
            base.owner.TurnOffSSMusic(abruptEnd: true);
            owner.getToWorking = 1f;
            gravOn = true;
            if (Oracle.ID == Oracle.OracleID.SS)
            {
                if (Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts < 100)
                {
                    Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts = 0;
                    if (Oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.altEnding)
                    {
                        Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts = 100;
                        Debug.Log("Condition met for artificer pity dialog");
                        DialogBox.NewMessage(Translate("Ah, you've returned. You know that I care very little for the creatures that wander through my facility."), 0);
                        DialogBox.NewMessage(Translate("In your current state. I can only assume that you have found what you were looking for."), 0);
                        DialogBox.NewMessage(Translate("For your own sake, I hope it was worth your struggle."), 0);
                        if (Random.value < 0.4f)
                        {
                            DialogBox.NewMessage(Translate("Now, please leave. I would prefer to be alone."), 0);
                        }
                        else
                        {
                            DialogBox.NewMessage(Translate("Now, I hope you have reason to visit me. I am very busy."), 0);
                        }
                        return;
                    }
                }
                Debug.Log($"artificer SSAI convos had: {Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad}");
                if (Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad <= 2)
                {
                    DialogBox.NewMessage(Translate("If you are going to make your visits a habit, the least you can do is bring me something new to read."), 0);
                }
                else if (Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad <= 3)
                {
                    DialogBox.NewMessage(Translate("Oh. It's you, why have you come back...? Again."), 0);
                }
                else if (Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 8)
                {
                    DialogBox.NewMessage(Translate("For what reason do you visit so often? There is nothing more I can do for you."), 0);
                    DialogBox.NewMessage(Translate(".  .  ."), 40);
                    DialogBox.NewMessage(Translate("I can only think that you somehow find me pleasant to listen to."), 0);
                }
                else if (Random.value < 0.1f)
                {
                    DialogBox.NewMessage(Translate("Little creature, please leave. I would prefer to be alone."), 0);
                }
                else if (Random.value < 0.3f)
                {
                    DialogBox.NewMessage(Translate("Have you brought something new this time?"), 0);
                }
                else if (Random.value < 0.3f)
                {
                    DialogBox.NewMessage(Translate("Do you have something new, or have you come to just stare at me?"), 0);
                }
                else if (Random.value < 0.3f)
                {
                    DialogBox.NewMessage(Translate("Hello again. I hope you have a reason to visit me."), 0);
                }
                else
                {
                    DialogBox.NewMessage(Translate(".  .  ."), 0);
                }
                Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad++;
            }
            else if (ModManager.Expedition && Oracle.room.game.rainWorld.ExpeditionMode)
            {
                if (Random.value < 0.3f)
                {
                    DialogBox.NewMessage(Translate("It is nice to see you again little messenger!"), 0);
                }
                else if (Random.value < 0.5f)
                {
                    DialogBox.NewMessage(Translate("Thank you for visiting me, but I'm afraid there is nothing here for you."), 0);
                }
                else
                {
                    DialogBox.NewMessage(Translate("Welcome back little messenger."), 0);
                }
            }
            else if (Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.smPearlTagged)
            {
                if (Random.value < 0.3f)
                {
                    DialogBox.NewMessage(Translate("Little messenger, we do not have much time. You must hurry!"), 0);
                }
                else if (Random.value < 0.3f)
                {
                    DialogBox.NewMessage(Translate("It is nice to see you again, but we do not have much time left. Please hurry, messenger!"), 0);
                }
                else if (Random.value < 0.3f)
                {
                    DialogBox.NewMessage(Translate("This is no time for games little messenger. Hurry now, before it is too late!"), 0);
                }
                else
                {
                    DialogBox.NewMessage(Translate("I have nothing for you here little messenger. Please, I have little time left."), 0);
                }
            }
            else if (Random.value < 0.5f)
            {
                DialogBox.NewMessage(Translate("Thank you for visiting me, but I'm afraid there is nothing here for you."), 0);
            }
            else
            {
                DialogBox.NewMessage(Translate("Welcome back little messenger."), 0);
            }
        }

        public void PickNextPanicTime()
        {
            timeUntilNextPanic = Random.Range(800, 2400);
        }

        public override void Activate(Action oldAction, Action newAction)
        {
            base.Activate(oldAction, newAction);
        }

        public override void NewAction(Action oldAction, Action newAction)
        {
            base.NewAction(oldAction, newAction);
            if (newAction == Action.ThrowOut_KillOnSight && owner.conversation != null)
            {
                owner.conversation.Destroy();
                owner.conversation = null;
            }
        }

        public override void Update()
        {
            base.Update();
            if (Player == null)
            {
                return;
            }
            if (owner.conversation != null && owner.conversation.slatedForDeletion && owner.conversation.id == MoreSlugcatsEnums.ConversationID.Moon_Spearmaster_Pearl)
            {
                owner.UnlockShortcuts();
                owner.conversation = null;
                if (owner.inspectPearl != null)
                {
                    owner.inspectPearl.firstChunk.vel = Custom.DirVec(owner.inspectPearl.firstChunk.pos, owner.player.mainBodyChunk.pos) * 3f;
                    owner.inspectPearl = null;
                    owner.getToWorking = 1f;
                }
            }
            if (tagTimer > 0f && owner.inspectPearl != null)
            {
                owner.killFac = Mathf.Clamp(tagTimer / 120f, 0f, 1f);
                tagTimer -= 1f;
                if (tagTimer <= 0f)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (IsPrideDay)
                            Oracle.room.AddObject(new Spark(owner.inspectPearl.firstChunk.pos, Custom.RNV() * Random.value * 40f, new Color(Random.value, Random.value, Random.value), null, 30, 120));
                        else
                            Oracle.room.AddObject(new Spark(owner.inspectPearl.firstChunk.pos, Custom.RNV() * Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                    }
                    Oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, owner.inspectPearl.firstChunk.pos, 1f, 0.5f + (Random.value * 0.5f));
                    if (owner.inspectPearl is SpearMasterPearl)
                    {
                        (owner.inspectPearl.AbstractPearl as SpearMasterPearl.AbstractSpearMasterPearl).broadcastTagged = true;
                        (owner.inspectPearl as SpearMasterPearl).holoVisible = true;
                        Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.smPearlTagged = true;
                    }
                    owner.killFac = 0f;
                }
            }
            if (!Oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
            {
                owner.NewAction(Action.MeetWhite_Texting);
                return;
            }
            if (holdPlayer && Player.room == Oracle.room)
            {
                Player.mainBodyChunk.vel *= Custom.LerpMap(InActionCounter, 0f, 30f, 1f, 0.95f);
                Player.bodyChunks[1].vel *= Custom.LerpMap(InActionCounter, 0f, 30f, 1f, 0.95f);
                Player.mainBodyChunk.vel += Custom.DirVec(Player.mainBodyChunk.pos, HoldPlayerPos) * Mathf.Lerp(0.5f, Custom.LerpMap(Vector2.Distance(Player.mainBodyChunk.pos, HoldPlayerPos), 30f, 150f, 2.5f, 7f), Oracle.room.gravity) * Mathf.InverseLerp(0f, 10f, InActionCounter) * Mathf.InverseLerp(0f, 30f, Vector2.Distance(Player.mainBodyChunk.pos, HoldPlayerPos));
            }
            if (panicObject == null || panicObject.slatedForDeletetion)
            {
                if (panicObject != null)
                {
                    owner.getToWorking = lastGetToWork;
                }
                panicObject = null;
                lastGetToWork = owner.getToWorking;
            }
            else
            {
                owner.getToWorking = 1f;
                if (lowGravity < 0f)
                {
                    lowGravity = 0f;
                }
                if (panicObject.gravOn)
                {
                    lowGravity = Mathf.Lerp(lowGravity, 0.5f, 0.01f);
                }
                gravOn = panicObject.gravOn;
                owner.SetNewDestination(Oracle.firstChunk.pos);
            }
            if (Action == Action.General_GiveMark)
            {
                return;
            }
            if (Action == NTEnums.ESPBehaviorAction.Moon_SlumberParty)
            {
                if (!Oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                {
                    owner.NewAction(NTEnums.ESPBehaviorAction.Moon_BeforeGiveMark);
                }
                if (panicObject == null)
                {
                    lowGravity = -1f;
                    if (owner.inspectPearl == null)
                    {
                        panicTimer++;
                    }
                    if (panicTimer > timeUntilNextPanic)
                    {
                        panicTimer = 0;
                        PickNextPanicTime();
                        panicObject = new OraclePanicDisplay(Oracle);
                        Oracle.room.AddObject(panicObject);
                    }
                }
            }
            else if (Action == NTEnums.ESPBehaviorAction.Moon_BeforeGiveMark)
            {
                MovementBehavior = MovementBehavior.KeepDistance;
                holdPlayer = false;
                gravOn = true;
                if (InActionCounter == 120)
                {
                    owner.voice = Oracle.room.PlaySound(SoundID.SL_AI_Talk_1, Oracle.firstChunk);
                }
                if (InActionCounter == 320)
                {
                    owner.voice = Oracle.room.PlaySound(SoundID.SL_AI_Talk_2, Oracle.firstChunk);
                }
                if (InActionCounter > 480)
                {
                    owner.NewAction(Action.General_GiveMark);
                }
            }
            else
            {
                if (!(Action == NTEnums.ESPBehaviorAction.Moon_AfterGiveMark))
                {
                    return;
                }
                owner.LockShortcuts();
                MovementBehavior = MovementBehavior.KeepDistance;
                gravOn = true;
                if (InActionCounter == 80 && (owner.conversation == null || owner.conversation.id != MoreSlugcatsEnums.ConversationID.MoonGiveMarkAfter))
                {
                    owner.InitateConversation(MoreSlugcatsEnums.ConversationID.MoonGiveMarkAfter, this);
                }
                if (InActionCounter <= 80 || (owner.conversation != null && (owner.conversation == null || !(owner.conversation.id == MoreSlugcatsEnums.ConversationID.MoonGiveMarkAfter) || !owner.conversation.slatedForDeletion)))
                {
                    return;
                }
                owner.UnlockShortcuts();
                owner.conversation = null;
                owner.getToWorking = 1f;
                owner.NewAction(NTEnums.ESPBehaviorAction.Moon_SlumberParty);
                for (int j = 0; j < owner.player.grasps.Length; j++)
                {
                    if (owner.player.grasps[j] != null && owner.player.grasps[j].grabbed is DataPearl && (owner.player.grasps[j].grabbed as DataPearl).AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.Spearmasterpearl)
                    {
                        owner.player.ReleaseGrasp(j);
                        break;
                    }
                }
            }
        }
    }

    public class ESPConversation : Conversation
    {
        public class PauseAndWaitForStillEvent : DialogueEvent
        {
            private readonly ConversationBehavior convBehav;

            public int pauseFrames;

            public PauseAndWaitForStillEvent(Conversation owner, ConversationBehavior _convBehav, int pauseFrames) : base(owner, 0)
            {
                convBehav = _convBehav;
                if (convBehav == null && owner is ESPConversation)
                {
                    convBehav = (owner as ESPConversation).convBehav;
                }
                this.pauseFrames = pauseFrames;
            }

            public override void Activate()
            {
                base.Activate();
                convBehav.communicationPause = pauseFrames;
                (owner as ESPConversation).waitForStill = true;
            }
        }

        private readonly ESPBehavior owner;

        private readonly ConversationBehavior convBehav;

        public bool waitForStill;

        public int age;

        public ESPConversation(ESPBehavior owner, ConversationBehavior convBehav, ID id, DialogBox dialogBox) : base(owner, id, dialogBox)
        {
            this.owner = owner;
            this.convBehav = convBehav;
            AddEvents();
        }

        public override void Update()
        {
            age++;
            if (waitForStill)
            {
                if (!convBehav.CurrentlyCommunicating && convBehav.communicationPause > 0)
                {
                    convBehav.communicationPause--;
                }
                if (!convBehav.CurrentlyCommunicating && convBehav.communicationPause < 1 && owner.allStillCounter > 20)
                {
                    waitForStill = false;
                }
            }
            else
            {
                base.Update();
            }
        }

        public string Translate(string s)
        {
            return owner.Translate(s);
        }

        public override void AddEvents()
        {
            if (id == ID.Pebbles_White)
            {
                if (!owner.playerEnteredWithMark)
                {
                    events.Add(new TextEvent(this, 0, ".  .  .", 0));
                    events.Add(new TextEvent(this, 0, Translate("...is this reaching you?"), 0));
                    events.Add(new PauseAndWaitForStillEvent(this, convBehav, 4));
                }
                else
                {
                    events.Add(new PauseAndWaitForStillEvent(this, convBehav, 210));
                }
                events.Add(new TextEvent(this, 0, Translate("A little animal, on the floor of my chamber. I think I know what you are looking for."), 0));
                events.Add(new TextEvent(this, 0, Translate("You're stuck in a cycle, a repeating pattern. You want a way out."), 0));
                events.Add(new TextEvent(this, 0, Translate("Know that this does not make you special - every living thing shares that same frustration.<LINE>From the microbes in the processing strata to me, who am, if you excuse me, godlike in comparison."), 0));
                events.Add(new TextEvent(this, 0, Translate("The good news first. In a way, I am what you are searching for. Me and my kind have as our<LINE>purpose to solve that very oscillating claustrophobia in the chests of you and countless others.<LINE>A strange charity - you the unknowing recipient, I the reluctant gift. The noble benefactors?<LINE>Gone."), 0));
                events.Add(new TextEvent(this, 0, Translate("The bad news is that no definitive solution has been found. And every moment the equipment erodes to a new state of decay.<LINE>I can't help you collectively, or individually. I can't even help myself."), 0));
                events.Add(new PauseAndWaitForStillEvent(this, convBehav, 10));
                if (owner.playerEnteredWithMark)
                {
                    events.Add(new TextEvent(this, 0, Translate("For you though, there is another way. The old path. Go to the west past the Farm Arrays, and then down into the earth<LINE>where the land fissures, as deep as you can reach, where the ancients built their temples and danced their silly rituals."), 0));
                }
                else
                {
                    events.Add(new TextEvent(this, 0, Translate("For you though, there is another way. The old path. Go to the west past the Farm Arrays, and then down into the earth<LINE>where the land fissures, as deep as you can reach, where the ancients built their temples and danced their silly rituals.<LINE>The mark I gave you will let you through."), 0));
                }
                events.Add(new TextEvent(this, 0, Translate("Not that it solves anyone's problem but yours."), 0));
                events.Add(new PauseAndWaitForStillEvent(this, convBehav, 20));
                if (owner.oracle.room.game.IsStorySession && owner.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.memoryArraysFrolicked)
                {
                    events.Add(new TextEvent(this, 0, Translate("At the end of time none of this will matter I suppose, but it would be nice if you took another way out.<LINE>One free of... frolicking in my memory arrays. There is a perfectly good access shaft right here."), 0));
                }
                else
                {
                    events.Add(new TextEvent(this, 0, Translate("Best of luck to you, little creature. I must resume my work."), 0));
                }
            }
        }
    }

    public class Action(string value, bool register = false) : ExtEnum<Action>(value, register)
    {
        public static readonly Action General_Idle = new("General_Idle", register: true);

        public static readonly Action General_MarkTalk = new("General_MarkTalk", register: true);

        public static readonly Action General_GiveMark = new("General_GiveMark", register: true);

        public static readonly Action MeetWhite_Shocked = new("MeetWhite_Shocked", register: true);

        public static readonly Action MeetWhite_Curious = new("MeetWhite_Curious", register: true);

        public static readonly Action MeetWhite_Talking = new("MeetWhite_Talking", register: true);

        public static readonly Action MeetWhite_Texting = new("MeetWhite_Texting", register: true);

        public static readonly Action MeetWhite_Images = new("MeetWhite_Images", register: true);

        public static readonly Action MeetWhite_SecondCurious = new("MeetWhite_SecondCurious", register: true);

        public static readonly Action MeetYellow_Init = new("MeetYellow_Init", register: true);

        public static readonly Action MeetRed_Init = new("MeetRed_Init", register: true);

        public static readonly Action GetNeuron_Init = new("GetNeuron_Init", register: true);

        public static readonly Action GetNeuron_TakeNeuron = new("GetNeuron_TakeNeuron", register: true);

        public static readonly Action GetNeuron_GetOutOfStomach = new("GetNeuron_GetOutOfStomach", register: true);

        public static readonly Action GetNeuron_InspectNeuron = new("GetNeuron_InspectNeuron", register: true);

        public static readonly Action ThrowOut_ThrowOut = new("ThrowOut_ThrowOut", register: true);

        public static readonly Action ThrowOut_SecondThrowOut = new("ThrowOut_SecondThrowOut", register: true);

        public static readonly Action ThrowOut_KillOnSight = new("ThrowOut_KillOnSight", register: true);

        public static readonly Action ThrowOut_Polite_ThrowOut = new("ThrowOut_Polite_ThrowOut", register: true);
    }

    public class MovementBehavior(string value, bool register = false) : ExtEnum<MovementBehavior>(value, register)
    {
        public static readonly MovementBehavior Idle = new("Idle", register: true);

        public static readonly MovementBehavior Meditate = new("Meditate", register: true);

        public static readonly MovementBehavior KeepDistance = new("KeepDistance", register: true);

        public static readonly MovementBehavior Investigate = new("Investigate", register: true);

        public static readonly MovementBehavior Talk = new("Talk", register: true);

        public static readonly MovementBehavior ShowMedia = new("ShowMedia", register: true);
    }

    public abstract class SubBehavior(ESPBehavior owner, SubBehavior.SubBehavID ID)
    {
        public class SubBehavID(string value, bool register = false) : ExtEnum<SubBehavID>(value, register)
        {
            public static readonly SubBehavID General = new("General", register: true);

            public static readonly SubBehavID MeetWhite = new("MeetWhite", register: true);

            public static readonly SubBehavID MeetRed = new("MeetRed", register: true);

            public static readonly SubBehavID MeetYellow = new("MeetYellow", register: true);

            public static readonly SubBehavID ThrowOut = new("ThrowOut", register: true);

            public static readonly SubBehavID GetNeuron = new("GetNeuron", register: true);
        }

        public SubBehavID ID = ID;

        public ESPBehavior owner = owner;

        public Action Action => owner.action;

        public Oracle Oracle => owner.oracle;

        public int InActionCounter => owner.inActionCounter;

        public MovementBehavior MovementBehavior
        {
            get => owner.movementBehavior;
            set => owner.movementBehavior = value;
        }

        public Player Player => owner.player;

        public virtual Vector2? LookPoint => null;

        public virtual float LowGravity => -1f;

        public virtual bool CurrentlyCommunicating => false;

        public virtual bool Gravity => true;

        public virtual void Update()
        {
        }

        public virtual void NewAction(Action oldAction, Action newAction)
        {
        }

        public virtual void Activate(Action oldAction, Action newAction)
        {
            NewAction(oldAction, newAction);
        }

        public virtual void Deactivate()
        {
            owner.UnlockShortcuts();
        }
    }

    public class NoSubBehavior(ESPBehavior owner) : SubBehavior(owner, SubBehavID.General)
    {
    }

    public abstract class TalkBehavior(ESPBehavior owner, SubBehavior.SubBehavID ID) : SubBehavior(owner, ID)
    {
        public int communicationIndex;

        public int communicationPause;

        public DialogBox DialogBox => owner.dialogBox;

        public override bool CurrentlyCommunicating => DialogBox.ShowingAMessage;

        public string Translate(string s)
        {
            return owner.Translate(s);
        }

        public override void NewAction(Action oldAction, Action newAction)
        {
            base.NewAction(oldAction, newAction);
            communicationIndex = 0;
        }
    }

    public abstract class ConversationBehavior(ESPBehavior owner, SubBehavior.SubBehavID ID, Conversation.ID convoID) : TalkBehavior(owner, ID)
    {
        public Conversation.ID convoID = convoID;
    }

    public class ESPOracleMeetWhite : ConversationBehavior
    {
        public ProjectedImage showImage;

        public Vector2 idealShowMediaPos;

        public Vector2 showMediaPos;

        public int consistentShowMediaPosCounter;

        public OracleChatLabel chatLabel;

        private ProjectionCircle myProjectionCircle;

        public ChunkSoundEmitter Voice
        {
            get => owner.voice;
            set => owner.voice = value;
        }

        public override bool CurrentlyCommunicating => base.CurrentlyCommunicating || Voice != null || (Action == Action.MeetWhite_Texting && !chatLabel.finishedShowingMessage) || showImage != null;

        public ESPOracleMeetWhite(ESPBehavior owner)
            : base(owner, SubBehavID.MeetWhite, Conversation.ID.Pebbles_White)
        {
            chatLabel = new OracleChatLabel(owner);
            Oracle.room.AddObject(chatLabel);
            chatLabel.Hide();
            if (ModManager.MMF && owner.oracle.room.game.IsStorySession && owner.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.memoryArraysFrolicked && Oracle.room.world.rainCycle.timer > Oracle.room.world.rainCycle.cycleLength / 4)
            {
                Oracle.room.world.rainCycle.timer = Oracle.room.world.rainCycle.cycleLength / 4;
                Oracle.room.world.rainCycle.dayNightCounter = 0;
            }
        }

        public override void Update()
        {
            if (Player == null)
            {
                return;
            }
            owner.LockShortcuts();
            if (ModManager.MSC && (Action == NTEnums.ESPBehaviorAction.MeetWhite_ThirdCurious || Action == NTEnums.ESPBehaviorAction.MeetWhite_SecondImages))
            {
                Vector2 off = Oracle.room.MiddleOfTile(24, 14) - Player.mainBodyChunk.pos;
                float dist = Custom.Dist(Oracle.room.MiddleOfTile(24, 14), Player.mainBodyChunk.pos);
                Player.mainBodyChunk.vel += Vector2.ClampMagnitude(off, 40f) / 40f * Mathf.Clamp(16f - (dist / 100f * 16f), 4f, 16f);
                if (Player.mainBodyChunk.vel.magnitude < 1f || dist < 8f)
                {
                    Player.mainBodyChunk.vel = Vector2.zero;
                    Player.mainBodyChunk.HardSetPosition(Oracle.room.MiddleOfTile(24, 14));
                }
            }
            if (Action == Action.MeetWhite_Shocked)
            {
                owner.movementBehavior = MovementBehavior.KeepDistance;
                if (owner.oracle.room.game.manager.rainWorld.progression.miscProgressionData.redHasVisitedPebbles || owner.oracle.room.game.manager.rainWorld.options.validation)
                {
                    if (InActionCounter > 40)
                    {
                        owner.NewAction(Action.General_GiveMark);
                        owner.afterGiveMarkAction = Action.General_MarkTalk;
                    }
                }
                else if (owner.oracle.room.game.IsStorySession && owner.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                {
                    if (InActionCounter > 40)
                    {
                        owner.NewAction(Action.General_MarkTalk);
                    }
                }
                else if (InActionCounter > 120)
                {
                    owner.NewAction(Action.MeetWhite_Curious);
                }
            }
            else if (Action == Action.MeetWhite_Curious)
            {
                owner.movementBehavior = MovementBehavior.Investigate;
                if (InActionCounter > 360)
                {
                    owner.NewAction(Action.MeetWhite_Talking);
                }
            }
            else if (Action == Action.MeetWhite_Talking)
            {
                owner.movementBehavior = MovementBehavior.Talk;
                if (!CurrentlyCommunicating && communicationPause > 0)
                {
                    communicationPause--;
                }
                if (!CurrentlyCommunicating && communicationPause < 1)
                {
                    if (communicationIndex >= 4)
                    {
                        owner.NewAction(Action.MeetWhite_Texting);
                    }
                    else if (owner.allStillCounter > 20)
                    {
                        NextCommunication();
                    }
                }
                if (!CurrentlyCommunicating)
                {
                    owner.nextPos += Custom.RNV();
                }
            }
            else if (Action == Action.MeetWhite_Texting)
            {
                MovementBehavior = MovementBehavior.ShowMedia;
                if (Oracle.graphicsModule != null)
                {
                    (Oracle.graphicsModule as OracleGraphics).halo.connectionsFireChance = 0f;
                }
                if (!CurrentlyCommunicating && communicationPause > 0)
                {
                    communicationPause--;
                }
                if (!CurrentlyCommunicating && communicationPause < 1)
                {
                    if (communicationIndex >= 6 || (ModManager.MSC && owner.oracle.ID == MoreSlugcatsEnums.OracleID.DM && communicationIndex >= 4))
                    {
                        owner.NewAction(Action.MeetWhite_Images);
                    }
                    else if (owner.allStillCounter > 20)
                    {
                        NextCommunication();
                    }
                }
                chatLabel.setPos = showMediaPos;
            }
            else if (Action == Action.MeetWhite_Images || (ModManager.MSC && Action == NTEnums.ESPBehaviorAction.MeetWhite_SecondImages))
            {
                MovementBehavior = MovementBehavior.ShowMedia;
                if (communicationPause > 0)
                {
                    communicationPause--;
                }
                if (ModManager.MSC && Action == NTEnums.ESPBehaviorAction.MeetWhite_SecondImages)
                {
                    myProjectionCircle.pos = new Vector2(Player.mainBodyChunk.pos.x - 10f, Player.mainBodyChunk.pos.y);
                }
                if (InActionCounter > 150 && communicationPause < 1)
                {
                    if (Action == Action.MeetWhite_Images && (communicationIndex >= 3 || (ModManager.MSC && owner.oracle.ID == MoreSlugcatsEnums.OracleID.DM && communicationIndex >= 1)))
                    {
                        owner.NewAction(Action.MeetWhite_SecondCurious);
                    }
                    else if (ModManager.MSC && Action == NTEnums.ESPBehaviorAction.MeetWhite_SecondImages && communicationIndex >= 2)
                    {
                        owner.NewAction(NTEnums.ESPBehaviorAction.MeetWhite_StartDialog);
                    }
                    else
                    {
                        NextCommunication();
                    }
                }
                if (showImage != null)
                {
                    showImage.setPos = showMediaPos;
                }
                if (Random.value < 1f / 30f)
                {
                    idealShowMediaPos += Custom.RNV() * Random.value * 30f;
                    showMediaPos += Custom.RNV() * Random.value * 30f;
                }
            }
            else if (Action == Action.MeetWhite_SecondCurious)
            {
                MovementBehavior = MovementBehavior.Investigate;
                if (InActionCounter == 80)
                {
                    Debug.Log("extra talk");
                    Voice = ModManager.MSC && owner.oracle.ID == MoreSlugcatsEnums.OracleID.DM
                        ? Oracle.room.PlaySound(SoundID.SL_AI_Talk_5, Oracle.firstChunk)
                        : Oracle.room.PlaySound(SoundID.SS_AI_Talk_5, Oracle.firstChunk);
                    Voice.requireActiveUpkeep = true;
                }
                if (InActionCounter > 240)
                {
                    owner.NewAction(Action.General_GiveMark);
                    owner.afterGiveMarkAction = Action.General_MarkTalk;
                }
            }
            else if (Action == Action.General_MarkTalk)
            {
                MovementBehavior = MovementBehavior.Talk;
                if (owner.conversation != null && owner.conversation.id == convoID && owner.conversation.slatedForDeletion)
                {
                    owner.conversation = null;
                    owner.NewAction(Action.ThrowOut_ThrowOut);
                }
            }
            else if (ModManager.MSC && Action == NTEnums.ESPBehaviorAction.MeetWhite_ThirdCurious)
            {
                owner.movementBehavior = MovementBehavior.Investigate;
                if (InActionCounter % 180 == 1)
                {
                    owner.investigateAngle = Random.value * 360f;
                }
                if (InActionCounter == 180)
                {
                    DialogBox.NewMessage(Translate("Hello there."), 0);
                    DialogBox.NewMessage(Translate("Are my words reaching you?"), 0);
                }
                if (InActionCounter == 460)
                {
                    myProjectionCircle = new ProjectionCircle(Player.mainBodyChunk.pos, 0f, 3f);
                    Oracle.room.AddObject(myProjectionCircle);
                }
                if (InActionCounter > 460)
                {
                    float prog2 = Mathf.Lerp(0f, 1f, (InActionCounter - 460f) / 150f);
                    myProjectionCircle.radius = 18f * Mathf.Clamp(prog2 * 2f, 0f, 1f);
                    myProjectionCircle.pos = new Vector2(Player.mainBodyChunk.pos.x - 10f, Player.mainBodyChunk.pos.y);
                    if (Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0)
                    {
                        (Player.graphicsModule as PlayerGraphics).bodyPearl.visible = true;
                        (Player.graphicsModule as PlayerGraphics).bodyPearl.globalAlpha = prog2;
                    }
                }
                if (InActionCounter > 770)
                {
                    owner.NewAction(NTEnums.ESPBehaviorAction.MeetWhite_SecondImages);
                }
            }
            else
            {
                if (!ModManager.MSC || !(Action == NTEnums.ESPBehaviorAction.MeetWhite_StartDialog))
                {
                    return;
                }
                if (InActionCounter < 48)
                {
                    Player.mainBodyChunk.vel += Vector2.ClampMagnitude(Oracle.room.MiddleOfTile(24, 14) - Player.mainBodyChunk.pos, 40f) / 40f * (6f - (InActionCounter / 8f));
                    float prog = 1f - (InActionCounter / 48f);
                    if (Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0)
                    {
                        (Player.graphicsModule as PlayerGraphics).bodyPearl.globalAlpha = prog;
                    }
                    myProjectionCircle.radius = 18f * Mathf.Clamp(prog * 2f, 0f, 3f);
                    myProjectionCircle.pos = new Vector2(Player.mainBodyChunk.pos.x - 10f, Player.mainBodyChunk.pos.y);
                }
                if (InActionCounter == 48)
                {
                    myProjectionCircle.Destroy();
                    (Player.graphicsModule as PlayerGraphics).bodyPearl.visible = false;
                }
                if (InActionCounter == 180)
                {
                    Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SLOracleState.playerEncounters++;
                    owner.NewAction(NTEnums.ESPBehaviorAction.Moon_AfterGiveMark);
                }
            }
        }

        public override void NewAction(Action oldAction, Action newAction)
        {
            base.NewAction(oldAction, newAction);
            if (oldAction == Action.MeetWhite_Texting)
            {
                chatLabel.Hide();
            }
            if ((oldAction == Action.MeetWhite_Images || (ModManager.MSC && oldAction == NTEnums.ESPBehaviorAction.MeetWhite_SecondImages)) && showImage != null)
            {
                showImage.Destroy();
                showImage = null;
            }
            if (newAction == Action.MeetWhite_Curious)
            {
                owner.investigateAngle = Mathf.Lerp(-70f, 70f, Random.value);
                owner.invstAngSpeed = Mathf.Lerp(0.4f, 0.8f, Random.value) * ((Random.value < 0.5f) ? (-1f) : 1f);
            }
            else if (newAction == Action.MeetWhite_Texting)
            {
                communicationPause = 170;
            }
            else if (newAction == Action.General_MarkTalk)
            {
                owner.InitateConversation(Conversation.ID.Pebbles_White, this);
            }
        }

        public override void Deactivate()
        {
            chatLabel.Hide();
            showImage?.Destroy();
            Voice = null;
            base.Deactivate();
        }

        private void NextCommunication()
        {
            Debug.Log($"New com att: {Action}, {communicationIndex}");
            if (Action == Action.MeetWhite_Talking)
            {
                switch (communicationIndex)
                {
                    case 0:
                        Voice = Oracle.room.PlaySound(SoundID.SS_AI_Talk_1, Oracle.firstChunk);
                        Voice.requireActiveUpkeep = true;
                        communicationPause = 10;
                        break;

                    case 1:
                        Voice = Oracle.room.PlaySound(SoundID.SS_AI_Talk_2, Oracle.firstChunk);
                        Voice.requireActiveUpkeep = true;
                        communicationPause = 70;
                        break;

                    case 2:
                        Voice = Oracle.room.PlaySound(SoundID.SS_AI_Talk_3, Oracle.firstChunk);
                        Voice.requireActiveUpkeep = true;
                        break;

                    case 3:
                        Voice = Oracle.room.PlaySound(SoundID.SS_AI_Talk_4, Oracle.firstChunk);
                        Voice.requireActiveUpkeep = true;
                        communicationPause = 170;
                        break;
                }
            }
            else if (Action == Action.MeetWhite_Texting)
            {
                chatLabel.NewPhrase(communicationIndex + ((ModManager.MSC && Oracle.ID == MoreSlugcatsEnums.OracleID.DM) ? 10 : 0));
            }
            else if (Action == Action.MeetWhite_Images)
            {
                showImage?.Destroy();
                if (ModManager.MSC && Oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                {
                    if (communicationIndex == 0)
                    {
                        showImage = Oracle.myScreen.AddImage("AIimg2_DM");
                        communicationPause = 380;
                    }
                }
                else
                {
                    switch (communicationIndex)
                    {
                        case 0:
                            showImage = Oracle.myScreen.AddImage("AIimg1");
                            communicationPause = 380;
                            break;

                        case 1:
                            showImage = Oracle.myScreen.AddImage("AIimg2");
                            communicationPause = 290;
                            break;

                        case 2:
                            showImage = Oracle.myScreen.AddImage(["AIimg3a", "AIimg3b"], 15);
                            communicationPause = 330;
                            break;
                    }
                }
                if (showImage != null)
                {
                    Oracle.room.PlaySound(SoundID.SS_AI_Image, 0f, 1f, 1f);
                    showImage.lastPos = showMediaPos;
                    showImage.pos = showMediaPos;
                    showImage.lastAlpha = 0f;
                    showImage.alpha = 0f;
                    showImage.setAlpha = 1f;
                }
            }
            else if (ModManager.MSC && Action == NTEnums.ESPBehaviorAction.MeetWhite_SecondImages)
            {
                showImage?.Destroy();
                if (communicationIndex == 0)
                {
                    showImage = Oracle.myScreen.AddImage("AIimg1_DM");
                    communicationPause = 380;
                }
                else if (communicationIndex == 1)
                {
                    showImage = Oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0
                        ? Oracle.myScreen.AddImage(["AIimg3a", "AIimg3c_DM"], 15)
                        : Oracle.myScreen.AddImage(["AIimg3a", "AIimg3b_DM"], 15);
                    communicationPause = 330;
                }
                if (showImage != null)
                {
                    Oracle.room.PlaySound(SoundID.SS_AI_Image, 0f, 1f, 1f);
                    showImage.lastPos = showMediaPos;
                    showImage.pos = showMediaPos;
                    showImage.lastAlpha = 0f;
                    showImage.alpha = 0f;
                    showImage.setAlpha = 1f;
                }
            }
            communicationIndex++;
        }

        public void ShowMediaMovementBehavior()
        {
            if (Player != null)
            {
                owner.lookPoint = Player.DangerPos;
            }
            Vector2 tryPos = new(Random.value * Oracle.room.PixelWidth, Random.value * Oracle.room.PixelHeight);
            if (owner.CommunicatePosScore(tryPos) + 40f < owner.CommunicatePosScore(owner.nextPos) && !Custom.DistLess(tryPos, owner.nextPos, 30f))
            {
                owner.SetNewDestination(tryPos);
            }
            consistentShowMediaPosCounter += (int)Custom.LerpMap(Vector2.Distance(showMediaPos, idealShowMediaPos), 0f, 200f, 1f, 10f);
            tryPos = new Vector2(Random.value * Oracle.room.PixelWidth, Random.value * Oracle.room.PixelHeight);
            if (ShowMediaScore(tryPos) + 40f < ShowMediaScore(idealShowMediaPos))
            {
                idealShowMediaPos = tryPos;
                consistentShowMediaPosCounter = 0;
            }
            tryPos = idealShowMediaPos + (Custom.RNV() * Random.value * 40f);
            if (ShowMediaScore(tryPos) + 20f < ShowMediaScore(idealShowMediaPos))
            {
                idealShowMediaPos = tryPos;
                consistentShowMediaPosCounter = 0;
            }
            if (consistentShowMediaPosCounter > 300)
            {
                showMediaPos = Vector2.Lerp(showMediaPos, idealShowMediaPos, 0.1f);
                showMediaPos = Custom.MoveTowards(showMediaPos, idealShowMediaPos, 10f);
            }
        }

        private float ShowMediaScore(Vector2 tryPos)
        {
            if (Oracle.room.GetTile(tryPos).Solid || Player == null)
            {
                return float.MaxValue;
            }
            float f = Mathf.Abs(Vector2.Distance(tryPos, Player.DangerPos) - 250f);
            f -= Math.Min(Oracle.room.aimap.getTerrainProximity(tryPos), 9f) * 30f;
            f -= Vector2.Distance(tryPos, owner.nextPos) * 0.5f;
            for (int j = 0; j < Oracle.arm.joints.Length; j++)
            {
                f -= Mathf.Min(Vector2.Distance(tryPos, Oracle.arm.joints[j].pos), 100f) * 10f;
            }
            if (Oracle.graphicsModule != null)
            {
                for (int i = 0; i < (Oracle.graphicsModule as OracleGraphics).umbCord.coord.GetLength(0); i += 3)
                {
                    f -= Mathf.Min(Vector2.Distance(tryPos, (Oracle.graphicsModule as OracleGraphics).umbCord.coord[i, 0]), 100f);
                }
            }
            return f;
        }
    }

    public class ThrowOutBehavior(ESPBehavior owner) : TalkBehavior(owner, SubBehavID.ThrowOut)
    {
        public bool telekinThrowOut;

        public override bool Gravity => Action != Action.ThrowOut_ThrowOut && Action != Action.ThrowOut_SecondThrowOut;

        public override void Activate(Action oldAction, Action newAction)
        {
            base.Activate(oldAction, newAction);
            if (newAction != Action.ThrowOut_Polite_ThrowOut)
            {
                owner.pearlPickupReaction = false;
            }
        }

        public override void NewAction(Action oldAction, Action newAction)
        {
            base.NewAction(oldAction, newAction);
            if (newAction == Action.ThrowOut_KillOnSight)
            {
                if (owner.conversation != null)
                {
                    owner.conversation.Interrupt("...", 0);
                    owner.conversation.Destroy();
                    owner.conversation = null;
                }
                else if (DialogBox.ShowingAMessage)
                {
                    DialogBox.Interrupt("...", 0);
                }
            }
        }

        public override void Update()
        {
            base.Update();
            owner.UnlockShortcuts();
            if (Player == null)
            {
                return;
            }
            if (Player.room == Oracle.room || (ModManager.MSC && Oracle.room.abstractRoom.creatures.Count > 0))
            {
                if (ModManager.MMF && !MMF.cfgVanillaExploits.Value && owner.greenNeuron != null && owner.greenNeuron.room == null)
                {
                    Debug.Log("Player destroyed neuron, kicking safety in.");
                    owner.greenNeuron = null;
                }
                if (owner.greenNeuron == null && owner.action != Action.ThrowOut_KillOnSight && owner.throwOutCounter < 900)
                {
                    Vector2 p2 = Oracle.room.MiddleOfTile(28, 33);
                    if (ModManager.MSC && Oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                    {
                        p2 = Oracle.room.MiddleOfTile(24, 33);
                    }
                    foreach (AbstractCreature crit2 in Oracle.room.abstractRoom.creatures)
                    {
                        if (crit2.realizedCreature == null)
                        {
                            continue;
                        }
                        if (!Oracle.room.aimap.getAItile(crit2.realizedCreature.mainBodyChunk.pos).narrowSpace || crit2.realizedCreature != Player)
                        {
                            crit2.realizedCreature.mainBodyChunk.vel += Custom.DirVec(crit2.realizedCreature.mainBodyChunk.pos, p2) * 0.2f * (1f - Oracle.room.gravity) * Mathf.InverseLerp(220f, 280f, InActionCounter);
                        }
                        else if (crit2.realizedCreature != null && crit2.realizedCreature != Player && !crit2.realizedCreature.enteringShortCut.HasValue && crit2.pos == owner.oracle.room.ToWorldCoordinate(p2))
                        {
                            crit2.realizedCreature.enteringShortCut = owner.oracle.room.ToWorldCoordinate(p2).Tile;
                            crit2.abstractAI.RealAI?.SetDestination(owner.oracle.room.ToWorldCoordinate(p2));
                        }
                    }
                }
                else if (owner.greenNeuron != null && owner.action != Action.ThrowOut_KillOnSight && owner.greenNeuron.grabbedBy.Count < 1 && owner.throwOutCounter < 900)
                {
                    Player.mainBodyChunk.vel *= Mathf.Lerp(0.9f, 1f, Oracle.room.gravity);
                    Player.bodyChunks[1].vel *= Mathf.Lerp(0.9f, 1f, Oracle.room.gravity);
                    Player.mainBodyChunk.vel += Custom.DirVec(Player.mainBodyChunk.pos, new Vector2(Oracle.room.PixelWidth / 2f, Oracle.room.PixelHeight / 2f)) * 0.5f * (1f - Oracle.room.gravity);
                    if (Random.value < 1f / 30f)
                    {
                        owner.greenNeuron.storyFly = true;
                    }
                    if (owner.greenNeuron.storyFly)
                    {
                        owner.greenNeuron.storyFlyTarget = Player.firstChunk.pos;
                        if (Custom.DistLess(owner.greenNeuron.firstChunk.pos, Player.firstChunk.pos, 40f))
                        {
                            Player.ReleaseGrasp(1);
                            Player.SlugcatGrab(owner.greenNeuron, 1);
                            owner.greenNeuron.storyFly = false;
                        }
                    }
                }
                else if (telekinThrowOut)
                {
                    Vector2 p = Oracle.room.MiddleOfTile(28, 33);
                    if (ModManager.MSC && Oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                    {
                        p = Oracle.room.MiddleOfTile(24, 33);
                    }
                    foreach (AbstractCreature crit in Oracle.room.abstractRoom.creatures)
                    {
                        if (crit.realizedCreature == null)
                        {
                            continue;
                        }
                        if (!Oracle.room.aimap.getAItile(crit.realizedCreature.mainBodyChunk.pos).narrowSpace || crit.realizedCreature != Player)
                        {
                            crit.realizedCreature.mainBodyChunk.vel += Custom.DirVec(Player.mainBodyChunk.pos, Oracle.room.MiddleOfTile(28, 32)) * 0.2f * (1f - Oracle.room.gravity) * Mathf.InverseLerp(220f, 280f, InActionCounter);
                        }
                        else if (crit.realizedCreature != Player && !crit.realizedCreature.enteringShortCut.HasValue && crit.pos == owner.oracle.room.ToWorldCoordinate(p))
                        {
                            crit.realizedCreature.enteringShortCut = owner.oracle.room.ToWorldCoordinate(p).Tile;
                            crit.abstractAI.RealAI?.SetDestination(owner.oracle.room.ToWorldCoordinate(p));
                        }
                    }
                }
            }
            if (Action == Action.ThrowOut_ThrowOut)
            {
                if (Player.room == Oracle.room)
                {
                    owner.throwOutCounter++;
                }
                MovementBehavior = MovementBehavior.KeepDistance;
                telekinThrowOut = InActionCounter > 220;
                if (ModManager.MSC && Oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
                {
                    if (owner.inspectPearl != null)
                    {
                        owner.NewAction(NTEnums.ESPBehaviorAction.Pebbles_SlumberParty);
                        owner.getToWorking = 1f;
                        return;
                    }
                    if (owner.throwOutCounter == 700)
                    {
                        DialogBox.Interrupt(Translate("Please leave."), 60);
                        DialogBox.NewMessage(Translate("This is not a request. I have important work to do."), 0);
                    }
                    else if (owner.throwOutCounter == 1300)
                    {
                        DialogBox.Interrupt(Translate("Unfortunately, my operations are encoded with a restriction that prevents<LINE>me from carrying out violent actions against my own citizens."), 0);
                        DialogBox.NewMessage(Translate("Please do not take advantage of this. I do not have the patience for your continued presence here."), 0);
                    }
                    else if (owner.throwOutCounter == 2100)
                    {
                        DialogBox.Interrupt(Translate("Did you not register what I've said to you?"), 60);
                        DialogBox.NewMessage(Translate("LEAVE."), 0);
                    }
                    else if (owner.throwOutCounter == 2900)
                    {
                        DialogBox.Interrupt(Translate("I'm returning to my work. Unless you have anything productive<LINE>for me, I have nothing further to say to you."), 0);
                    }
                }
                else if (owner.throwOutCounter == 700)
                {
                    DialogBox.Interrupt(Translate("That's all. You'll have to go now."), 0);
                }
                else if (owner.throwOutCounter == 980)
                {
                    DialogBox.Interrupt(Translate("LEAVE."), 0);
                }
                else if (owner.throwOutCounter == 1530)
                {
                    DialogBox.Interrupt(Translate("Little creature. This is your last warning."), 0);
                }
                else if (owner.throwOutCounter > 1780)
                {
                    owner.NewAction(Action.ThrowOut_KillOnSight);
                }
                if ((owner.playerOutOfRoomCounter > 100 && owner.throwOutCounter > 400) || owner.throwOutCounter > 3200)
                {
                    owner.NewAction(Action.General_Idle);
                    owner.getToWorking = 1f;
                }
            }
            else if (Action == Action.ThrowOut_SecondThrowOut)
            {
                if (Player.room == Oracle.room)
                {
                    owner.throwOutCounter++;
                }
                MovementBehavior = MovementBehavior.KeepDistance;
                telekinThrowOut = InActionCounter > 220;
                if (owner.throwOutCounter == 50)
                {
                    if (Oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && Oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.altEnding)
                    {
                        DialogBox.Interrupt(Translate("Oh, it's you again? I had told you to leave and never return."), 0);
                    }
                    else
                    {
                        DialogBox.Interrupt(Translate("You again? I have nothing for you."), 0);
                    }
                }
                else if (owner.throwOutCounter == 250)
                {
                    int slugpupsInRoom = 0;
                    if (ModManager.MSC)
                    {
                        for (int j = 0; j < Oracle.room.physicalObjects.Length; j++)
                        {
                            for (int k = 0; k < Oracle.room.physicalObjects[j].Count; k++)
                            {
                                if (Oracle.room.physicalObjects[j][k] is Player && (Oracle.room.physicalObjects[j][k] as Player).isNPC)
                                {
                                    slugpupsInRoom++;
                                }
                            }
                        }
                    }
                    if (slugpupsInRoom > 0)
                    {
                        DialogBox.Interrupt(Translate("Leave immediately and don't come back. And take THEM with you!"), 0);
                    }
                    else
                    {
                        DialogBox.Interrupt(Translate("I won't tolerate this. Leave immediately and don't come back."), 0);
                    }
                }
                else if (owner.throwOutCounter == 700)
                {
                    DialogBox.Interrupt(Translate("You had your chances."), 0);
                }
                else if (owner.throwOutCounter > 770)
                {
                    owner.NewAction(Action.ThrowOut_KillOnSight);
                }
                if (owner.playerOutOfRoomCounter > 100 && owner.throwOutCounter > 400)
                {
                    owner.NewAction(Action.General_Idle);
                    owner.getToWorking = 1f;
                }
            }
            else if (Action == Action.ThrowOut_KillOnSight)
            {
                if (ModManager.MMF)
                {
                    if (Player.room == Oracle.room)
                    {
                        owner.throwOutCounter++;
                    }
                    if (owner.throwOutCounter == 10)
                    {
                        DialogBox.Interrupt(Translate("..."), 200);
                    }
                }
                if (ModManager.MMF && owner.throwOutCounter < 200)
                {
                    return;
                }
                if ((!Player.dead || owner.killFac > 0.5f) && Player.room == Oracle.room)
                {
                    owner.killFac += 0.025f;
                    if (owner.killFac >= 1f)
                    {
                        Player.mainBodyChunk.vel += Custom.RNV() * 12f;
                        for (int i = 0; i < 20; i++)
                        {
                            if (IsPrideDay)
                                Oracle.room.AddObject(new Spark(Player.mainBodyChunk.pos, Custom.RNV() * Random.value * 40f, new Color(Random.value, Random.value, Random.value), null, 30, 120));
                            else
                                Oracle.room.AddObject(new Spark(Player.mainBodyChunk.pos, Custom.RNV() * Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                        }
                        Player.Die();
                        owner.killFac = 0f;
                    }
                    return;
                }
                owner.killFac *= 0.8f;
                owner.getToWorking = 1f;
                MovementBehavior = MovementBehavior.KeepDistance;
                if (Player.room == Oracle.room || Oracle.oracleBehavior.PlayersInRoom.Count > 0)
                {
                    if (Oracle.ID == Oracle.OracleID.SS)
                    {
                        if (ModManager.CoopAvailable)
                        {
                            foreach (Player p3 in Oracle.oracleBehavior.PlayersInRoom)
                            {
                                p3.mainBodyChunk.vel += Custom.DirVec(p3.mainBodyChunk.pos, Oracle.room.MiddleOfTile(28, 32)) * 0.6f * (1f - Oracle.room.gravity);
                                if (Oracle.room.GetTilePosition(p3.mainBodyChunk.pos) == new IntVector2(28, 32) && !p3.enteringShortCut.HasValue)
                                {
                                    p3.enteringShortCut = Oracle.room.ShortcutLeadingToNode(1).StartTile;
                                }
                            }
                            return;
                        }
                        Player.mainBodyChunk.vel += Custom.DirVec(Player.mainBodyChunk.pos, Oracle.room.MiddleOfTile(28, 32)) * 0.6f * (1f - Oracle.room.gravity);
                        if (Oracle.room.GetTilePosition(Player.mainBodyChunk.pos) == new IntVector2(28, 32) && !Player.enteringShortCut.HasValue)
                        {
                            Player.enteringShortCut = Oracle.room.ShortcutLeadingToNode(1).StartTile;
                        }
                    }
                    else if (ModManager.MSC && Oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                    {
                        Player.mainBodyChunk.vel += Custom.DirVec(Player.mainBodyChunk.pos, Oracle.room.MiddleOfTile(24, 32)) * 0.6f * (1f - Oracle.room.gravity);
                        if (Oracle.room.GetTilePosition(Player.mainBodyChunk.pos) == new IntVector2(24, 32) && !Player.enteringShortCut.HasValue)
                        {
                            Player.enteringShortCut = Oracle.room.ShortcutLeadingToNode(1).StartTile;
                        }
                    }
                }
                else
                {
                    owner.NewAction(Action.General_Idle);
                }
            }
            else if (Action == Action.ThrowOut_Polite_ThrowOut)
            {
                owner.getToWorking = 1f;
                MovementBehavior = InActionCounter < 200
                    ? MovementBehavior.Idle
                    : InActionCounter < 530 ? MovementBehavior.Talk : InActionCounter < 1050 ? MovementBehavior.Idle : MovementBehavior.KeepDistance;
                if (owner.playerOutOfRoomCounter > 100 && InActionCounter > 400)
                {
                    owner.NewAction(Action.General_Idle);
                }
                else if (InActionCounter == 500)
                {
                    DialogBox.Interrupt(Translate("Thank you little creature. I must resume my work."), 0);
                }
                else if (InActionCounter == 1100)
                {
                    DialogBox.NewMessage(Translate("I appreciate what you have done but it is time for you to leave."), 0);
                    if (owner.oracle.room.game.StoryCharacter == SlugcatStats.Name.Red)
                    {
                        DialogBox.NewMessage(Translate("As I mentioned you do not have unlimited time."), 0);
                    }
                }
                else if (InActionCounter > 1400)
                {
                    owner.NewAction(Action.ThrowOut_ThrowOut);
                    owner.getToWorking = 0f;
                }
            }
            else
            {
                if (!ModManager.MSC || !(Action == NTEnums.ESPBehaviorAction.ThrowOut_Singularity))
                {
                    return;
                }
                if (InActionCounter == 10)
                {
                    if ((Oracle.oracleBehavior as SSOracleBehavior).conversation != null)
                    {
                        (Oracle.oracleBehavior as SSOracleBehavior).conversation.Destroy();
                        (Oracle.oracleBehavior as SSOracleBehavior).conversation = null;
                    }
                    DialogBox.Interrupt(Translate(". . . !"), 0);
                }
                owner.getToWorking = 1f;
                if (Player.room != Oracle.room && !Player.inShortcut)
                {
                    if (Player.grasps[0] != null && Player.grasps[0].grabbed is SingularityBomb)
                    {
                        (Player.grasps[0].grabbed as SingularityBomb).Thrown(Player, Player.firstChunk.pos, null, new IntVector2(0, -1), 1f, eu: true);
                        (Player.grasps[0].grabbed as SingularityBomb).ignited = true;
                        (Player.grasps[0].grabbed as SingularityBomb).activateSucktion = true;
                        (Player.grasps[0].grabbed as SingularityBomb).counter = 50f;
                        (Player.grasps[0].grabbed as SingularityBomb).floatLocation = Player.firstChunk.pos;
                        (Player.grasps[0].grabbed as SingularityBomb).firstChunk.pos = Player.firstChunk.pos;
                    }
                    if (Player.grasps[1] != null && Player.grasps[1].grabbed is SingularityBomb)
                    {
                        (Player.grasps[1].grabbed as SingularityBomb).Thrown(Player, Player.firstChunk.pos, null, new IntVector2(0, -1), 1f, eu: true);
                        (Player.grasps[1].grabbed as SingularityBomb).ignited = true;
                        (Player.grasps[1].grabbed as SingularityBomb).activateSucktion = true;
                        (Player.grasps[1].grabbed as SingularityBomb).counter = 50f;
                        (Player.grasps[1].grabbed as SingularityBomb).floatLocation = Player.firstChunk.pos;
                        (Player.grasps[1].grabbed as SingularityBomb).firstChunk.pos = Player.firstChunk.pos;
                    }
                    Player.Stun(200);
                    owner.NewAction(Action.General_Idle);
                    return;
                }
                MovementBehavior = MovementBehavior.KeepDistance;
                if (Oracle.ID == Oracle.OracleID.SS)
                {
                    Player.mainBodyChunk.vel += Custom.DirVec(Player.mainBodyChunk.pos, Oracle.room.MiddleOfTile(28, 32)) * 1.3f;
                    Player.mainBodyChunk.pos = Vector2.Lerp(Player.mainBodyChunk.pos, Oracle.room.MiddleOfTile(28, 32), 0.08f);
                    if (!Player.enteringShortCut.HasValue && Player.mainBodyChunk.pos.x < 560f && Player.mainBodyChunk.pos.y > 630f)
                    {
                        Player.mainBodyChunk.pos.y = 630f;
                    }
                    if ((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity != null)
                    {
                        (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.activateSucktion = false;
                        (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.vel += Custom.DirVec((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, Player.mainBodyChunk.pos) * 1.3f;
                        (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos = Vector2.Lerp((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, Player.mainBodyChunk.pos, 0.1f);
                        if (Vector2.Distance((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, Player.mainBodyChunk.pos) < 10f)
                        {
                            if (Player.grasps[0] == null)
                            {
                                Player.SlugcatGrab((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 0);
                            }
                            if (Player.grasps[1] == null)
                            {
                                Player.SlugcatGrab((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 1);
                            }
                        }
                    }
                    if (Oracle.room.GetTilePosition(Player.mainBodyChunk.pos) == new IntVector2(28, 32) && !Player.enteringShortCut.HasValue)
                    {
                        bool playerGrabbedBomb = false;
                        if (Player.grasps[0] != null && Player.grasps[0].grabbed == (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                        {
                            playerGrabbedBomb = true;
                        }
                        if (Player.grasps[1] != null && Player.grasps[1].grabbed == (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                        {
                            playerGrabbedBomb = true;
                        }
                        if (playerGrabbedBomb)
                        {
                            Player.enteringShortCut = Oracle.room.ShortcutLeadingToNode(1).StartTile;
                            return;
                        }
                        Player.ReleaseGrasp(0);
                        Player.ReleaseGrasp(1);
                    }
                }
                if (!(Oracle.ID == MoreSlugcatsEnums.OracleID.DM))
                {
                    return;
                }
                Player.mainBodyChunk.vel += Custom.DirVec(Player.mainBodyChunk.pos, Oracle.room.MiddleOfTile(24, 32)) * 1.3f;
                Player.mainBodyChunk.pos = Vector2.Lerp(Player.mainBodyChunk.pos, Oracle.room.MiddleOfTile(24, 32), 0.08f);
                if ((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity != null)
                {
                    (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.activateSucktion = false;
                    (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.vel += Custom.DirVec((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, Player.mainBodyChunk.pos) * 1.3f;
                    (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos = Vector2.Lerp((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, Player.mainBodyChunk.pos, 0.1f);
                    if (Vector2.Distance((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, Player.mainBodyChunk.pos) < 10f)
                    {
                        if (Player.grasps[0] == null)
                        {
                            Player.SlugcatGrab((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 0);
                        }
                        if (Player.grasps[1] == null)
                        {
                            Player.SlugcatGrab((Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 1);
                        }
                    }
                }
                if (Oracle.room.GetTilePosition(Player.mainBodyChunk.pos) == new IntVector2(28, 32) && !Player.enteringShortCut.HasValue)
                {
                    bool playerGrabbedBomb2 = false;
                    if (Player.grasps[0] != null && Player.grasps[0].grabbed == (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                    {
                        playerGrabbedBomb2 = true;
                    }
                    if (Player.grasps[1] != null && Player.grasps[1].grabbed == (Oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                    {
                        playerGrabbedBomb2 = true;
                    }
                    if (playerGrabbedBomb2)
                    {
                        Player.enteringShortCut = Oracle.room.ShortcutLeadingToNode(1).StartTile;
                        return;
                    }
                    Player.ReleaseGrasp(0);
                    Player.ReleaseGrasp(1);
                }
            }
        }
    }

    private Vector2 lastPos;

    private Vector2 nextPos;

    private Vector2 lastPosHandle;

    private Vector2 nextPosHandle;

    private Vector2 currentGetTo;

    private float pathProgression;

    private float investigateAngle;

    private float invstAngSpeed;

    public PebblesPearl investigateMarble;

    private Vector2 baseIdeal;

    public float working;

    public float getToWorking;

    public bool floatyMovement;

    public int discoverCounter;

    public float killFac;

    public float lastKillFac;

    public int throwOutCounter;

    public int playerOutOfRoomCounter;

    public SubBehavior currSubBehavior;

    public List<SubBehavior> allSubBehaviors;

    public NSHSwarmer greenNeuron;

    public ESPConversation conversation;

    public List<EntityID> talkedAboutThisSession;

    private bool pearlPickupReaction = true;

    private bool lastPearlPickedUp = true;

    private bool restartConversationAfterCurrentDialoge;

    private readonly bool playerEnteredWithMark;

    public int timeSinceSeenPlayer = -1;

    public Action action = Action.General_Idle;

    public Action afterGiveMarkAction;

    public MovementBehavior movementBehavior;

    public float unconciousTick;

    public float killFacOverseer;

    public float lastKillFacOverseer;

    public SpearMasterPearl SMCorePearl;

    public SLOracleBehaviorHasMark.MoonConversation pearlConversation;

    public DataPearl inspectPearl;

    public SingularityBomb dangerousSingularity;

    public int killOnSightCounter;

    public List<DataPearl.AbstractDataPearl> readDataPearlOrbits;

    public Dictionary<DataPearl.AbstractDataPearl, GlyphLabel> readPearlGlyphs;

    public override DialogBox dialogBox
    {
        get
        {
            if (oracle.room.game.cameras[0].hud.dialogBox == null)
            {
                _ = oracle.room.game.cameras[0].hud.InitDialogBox();
                oracle.room.game.cameras[0].hud.dialogBox.defaultYPos = -10f;
            }
            return oracle.room.game.cameras[0].hud.dialogBox;
        }
    }

    private bool HasSeenGreenNeuron => oracle.room.game.IsStorySession && oracle.room.game.GetStorySession.saveState.miscWorldSaveData.pebblesSeenGreenNeuron;

    public override Vector2 OracleGetToPos
    {
        get
        {
            Vector2 rtrn = currentGetTo;
            if (floatyMovement && Custom.DistLess(oracle.firstChunk.pos, nextPos, 50f))
            {
                rtrn = nextPos;
            }
            return ClampVectorInRoom(rtrn);
        }
    }

    public override Vector2 BaseGetToPos => baseIdeal;

    public override Vector2 GetToDir => movementBehavior == MovementBehavior.Idle
                ? Custom.DegToVec(investigateAngle)
                : movementBehavior == MovementBehavior.Investigate ? -Custom.DegToVec(investigateAngle) : new Vector2(0f, 1f);

    public override bool EyesClosed => movementBehavior == MovementBehavior.Meditate;

    public new RainWorld rainWorld => oracle.room.game.rainWorld;

    public ESPBehavior(Oracle oracle) : base(oracle)
    {
        currentGetTo = oracle.firstChunk.pos;
        lastPos = oracle.firstChunk.pos;
        nextPos = oracle.firstChunk.pos;
        pathProgression = 1f;
        investigateAngle = Random.value * 360f;
        allSubBehaviors = [];
        currSubBehavior = new NoSubBehavior(this);
        allSubBehaviors.Add(currSubBehavior);
        working = 1f;
        getToWorking = 1f;
        movementBehavior = (Random.value < 0.5f) ? MovementBehavior.Meditate : MovementBehavior.Idle;
        playerEnteredWithMark = oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark;
        talkedAboutThisSession = [];
        if (ModManager.MSC)
        {
            InitStoryPearlCollection();
        }
    }

    private void InitateConversation(Conversation.ID convoId, ConversationBehavior convBehav)
    {
        if (conversation != null)
        {
            conversation.Interrupt("...", 0);
            conversation.Destroy();
        }
        conversation = new ESPConversation(this, convBehav, convoId, dialogBox);
        if (ModManager.MSC && oracle.room.world.name == "HR")
        {
            conversation.colorMode = true;
        }
    }

    private void LockShortcuts()
    {
        if (oracle.room.lockedShortcuts.Count == 0)
        {
            for (int i = 0; i < oracle.room.shortcutsIndex.Length; i++)
            {
                oracle.room.lockedShortcuts.Add(oracle.room.shortcutsIndex[i]);
            }
        }
    }

    private void UnlockShortcuts()
    {
        oracle.room.lockedShortcuts.Clear();
    }

    public void SeePlayer()
    {
        if (timeSinceSeenPlayer < 0)
        {
            timeSinceSeenPlayer = 0;
        }
        if (ModManager.CoopAvailable && timeSinceSeenPlayer < 5)
        {
            Player futurePlayer = null;
            int counter = 0;
            int node = 0;
            IEnumerable<Player> listPlayers = from x in oracle.room.game.NonPermaDeadPlayers
                                              where x.Room != oracle.room.abstractRoom
                                              select x.realizedCreature as Player;
            foreach (Player p in listPlayers.OrderBy((Player x) => x.slugOnBack != null))
            {
                p.slugOnBack?.DropSlug();
                Debug.Log($"Warping player to 5P room, {p} - back occupied?{p.slugOnBack}");
                try
                {
                    node = (p.room.abstractRoom.name == "SS_D07") ? 1 : 0;
                    WorldCoordinate pos = oracle.room.LocalCoordinateOfNode(node);
                    JollyCustom.MovePlayerWithItems(p, p.room, oracle.room.abstractRoom.name, pos);
                    Vector2 holeDir = Vector2.down;
                    for (int i = 0; i < p.bodyChunks.Length; i++)
                    {
                        p.bodyChunks[i].HardSetPosition(oracle.room.MiddleOfTile(pos) - (holeDir * (-0.5f + i) * 5f));
                        p.bodyChunks[i].vel = holeDir * 2f;
                    }
                    counter++;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to move player, {ex}, throwException: true");
                }
                if (futurePlayer == null && p?.objectInStomach?.type == AbstractObjectType.NSHSwarmer)
                {
                    futurePlayer = p;
                    Debug.Log($"Found player with neuron in stomach, focusing ...{p}");
                }
            }
            if (futurePlayer != null)
            {
                player = futurePlayer;
            }
        }
        greenNeuron = null;
        for (int j = 0; j < oracle.room.updateList.Count; j++)
        {
            if (oracle.room.updateList[j] is NSHSwarmer)
            {
                greenNeuron = oracle.room.updateList[j] as NSHSwarmer;
                break;
            }
        }

        Debug.Log(message: $"See player, {oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad}, gn?: {greenNeuron != null}");
        if (ModManager.MSC && oracle.room.world.name == "HR")
        {
            NewAction(NTEnums.ESPBehaviorAction.Rubicon);
        }
        else if ((greenNeuron != null || (player.objectInStomach != null && player.objectInStomach.type == AbstractObjectType.NSHSwarmer)) && !HasSeenGreenNeuron)
        {
            oracle.room.game.GetStorySession.saveState.miscWorldSaveData.pebblesSeenGreenNeuron = true;
            NewAction(Action.GetNeuron_Init);
        }
        else if (oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0 || (ModManager.MSC && oracle.ID == MoreSlugcatsEnums.OracleID.DM))
        {
            if (oracle.room.game.StoryCharacter == SlugcatStats.Name.Red)
            {
                NewAction(Action.MeetRed_Init);
            }
            else if (oracle.room.game.StoryCharacter == SlugcatStats.Name.Yellow)
            {
                NewAction(Action.MeetYellow_Init);
            }
            else if (ModManager.MSC && oracle.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            {
                if (oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                {
                    NewAction(NTEnums.ESPBehaviorAction.Moon_SlumberParty);
                }
                else
                {
                    NewAction(NTEnums.ESPBehaviorAction.MeetPurple_Init);
                }
            }
            else if (ModManager.MSC && oracle.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            {
                NewAction(NTEnums.ESPBehaviorAction.MeetArty_Init);
            }
            else if (ModManager.MSC && oracle.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel)
            {
                NewAction(NTEnums.ESPBehaviorAction.MeetInv_Init);
            }
            else if (ModManager.MSC && oracle.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
            {
                NewAction(NTEnums.ESPBehaviorAction.MeetGourmand_Init);
            }
            else
            {
                NewAction(Action.MeetWhite_Shocked);
            }
            if (oracle.room.game.StoryCharacter != SlugcatStats.Name.Red && (!ModManager.MSC || (oracle.room.game.StoryCharacter != MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel && oracle.room.game.StoryCharacter != MoreSlugcatsEnums.SlugcatStatsName.Artificer && oracle.ID != MoreSlugcatsEnums.OracleID.DM)))
            {
                SlugcatEnterRoomReaction();
            }
        }
        else
        {
            if (ModManager.MSC && oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            {
                Debug.Log($"Artificer visit, {oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts}");
                NewAction(NTEnums.ESPBehaviorAction.Pebbles_SlumberParty);
            }
            else if (ModManager.MSC && oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            {
                Debug.Log("Spearmaster kill on sight");
                NewAction(Action.ThrowOut_KillOnSight);
            }
            else
            {
                Debug.Log($"Throw out player, {oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts}");
                if (oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts > 0)
                {
                    NewAction(Action.ThrowOut_KillOnSight);
                }
                else
                {
                    NewAction(Action.ThrowOut_SecondThrowOut);
                    SlugcatEnterRoomReaction();
                }
            }
            oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts++;
        }
        if (!ModManager.MSC || oracle.room.game.GetStorySession.saveStateNumber != MoreSlugcatsEnums.SlugcatStatsName.Spear || oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad != 0)
        {
            oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad++;
        }
    }

    public override void Update(bool eu)
    {
        if (ModManager.MMF && player != null && player.dead && currSubBehavior.ID != SubBehavior.SubBehavID.ThrowOut)
        {
            NewAction(Action.ThrowOut_KillOnSight);
        }
        if (ModManager.MSC)
        {
            if (inspectPearl != null)
            {
                movementBehavior = inspectPearl.AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.Spearmasterpearl
                    ? MovementBehavior.Talk
                    : MovementBehavior.Meditate;
                if (inspectPearl.grabbedBy.Count > 0)
                {
                    for (int i = 0; i < inspectPearl.grabbedBy.Count; i++)
                    {
                        Creature grabber = inspectPearl.grabbedBy[i].grabber;
                        if (grabber == null)
                        {
                            continue;
                        }
                        for (int j2 = 0; j2 < grabber.grasps.Length; j2++)
                        {
                            if (grabber.grasps[j2].grabbed != null && grabber.grasps[j2].grabbed == inspectPearl)
                            {
                                grabber.ReleaseGrasp(j2);
                            }
                        }
                    }
                }
                Vector2 vector = oracle.firstChunk.pos - inspectPearl.firstChunk.pos;
                float num = Custom.Dist(oracle.firstChunk.pos, inspectPearl.firstChunk.pos);
                if (inspectPearl.AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.Spearmasterpearl && num < 64f)
                {
                    inspectPearl.firstChunk.vel += Vector2.ClampMagnitude(vector, 2f) / 20f * Mathf.Clamp(16f - (num / 100f * 16f), 4f, 16f);
                    if (inspectPearl.firstChunk.vel.magnitude < 1f || num < 8f)
                    {
                        inspectPearl.firstChunk.vel = Vector2.zero;
                        inspectPearl.firstChunk.HardSetPosition(oracle.firstChunk.pos);
                    }
                }
                else
                {
                    inspectPearl.firstChunk.vel += Vector2.ClampMagnitude(vector, 40f) / 40f * Mathf.Clamp(2f - (num / 200f * 2f), 0.5f, 2f);
                    if (inspectPearl.firstChunk.vel.magnitude < 1f && num < 16f)
                    {
                        inspectPearl.firstChunk.vel = Custom.RNV() * 8f;
                    }
                    if (inspectPearl.firstChunk.vel.magnitude > 8f)
                    {
                        inspectPearl.firstChunk.vel /= 2f;
                    }
                }
                if (num < 100f && pearlConversation == null && conversation == null)
                {
                    if (inspectPearl.AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.Spearmasterpearl && currSubBehavior is ESPSleepoverBehavior)
                    {
                        InitateConversation(MoreSlugcatsEnums.ConversationID.Moon_Spearmaster_Pearl, currSubBehavior as ESPSleepoverBehavior);
                    }
                    else
                    {
                        StartItemConversation(inspectPearl);
                    }
                }
            }
            UpdateStoryPearlCollection();
        }
        if (timeSinceSeenPlayer >= 0)
        {
            timeSinceSeenPlayer++;
        }
        if (pearlPickupReaction && timeSinceSeenPlayer > 300 && oracle.room.game.IsStorySession && oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark && (currSubBehavior is not ThrowOutBehavior || action == Action.ThrowOut_Polite_ThrowOut))
        {
            bool p = false;
            if (player != null)
            {
                for (int j = 0; j < player.grasps.Length; j++)
                {
                    if (player.grasps[j] != null && player.grasps[j].grabbed is PebblesPearl)
                    {
                        p = true;
                        break;
                    }
                }
            }
            if (ModManager.MSC && oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Spear)
            {
                p = false;
            }
            if (p && !lastPearlPickedUp && (conversation == null || (conversation.age > 300 && !conversation.paused)))
            {
                if (conversation != null)
                {
                    conversation.paused = true;
                    restartConversationAfterCurrentDialoge = true;
                }
                dialogBox.Interrupt(Translate("Yes, help yourself. They are not edible."), 10);
                pearlPickupReaction = false;
            }
            lastPearlPickedUp = p;
        }
        if (conversation != null)
        {
            if (restartConversationAfterCurrentDialoge && conversation.paused && action != Action.General_GiveMark && dialogBox.messages.Count == 0 && (!ModManager.MSC || player.room == oracle.room))
            {
                conversation.paused = false;
                restartConversationAfterCurrentDialoge = false;
                conversation.RestartCurrent();
            }
        }
        else if (ModManager.MSC && pearlConversation != null)
        {
            if (pearlConversation.slatedForDeletion)
            {
                pearlConversation = null;
                if (inspectPearl != null)
                {
                    inspectPearl.firstChunk.vel = Custom.DirVec(inspectPearl.firstChunk.pos, player.mainBodyChunk.pos) * 3f;
                    readDataPearlOrbits.Add(inspectPearl.AbstractPearl);
                    inspectPearl = null;
                }
            }
            else
            {
                pearlConversation.Update();
                if (player.room != oracle.room)
                {
                    if (player.room != null && !pearlConversation.paused)
                    {
                        pearlConversation.paused = true;
                        InterruptPearlMessagePlayerLeaving();
                    }
                }
                else if (pearlConversation.paused && !restartConversationAfterCurrentDialoge)
                {
                    ResumePausedPearlConversation();
                }
                if (pearlConversation.paused && restartConversationAfterCurrentDialoge && dialogBox.messages.Count == 0)
                {
                    pearlConversation.paused = false;
                    restartConversationAfterCurrentDialoge = false;
                    pearlConversation.RestartCurrent();
                }
            }
        }
        else
        {
            restartConversationAfterCurrentDialoge = false;
        }
        base.Update(eu);
        for (int k = 0; k < oracle.room.game.cameras.Length; k++)
        {
            oracle.room.game.cameras[k].virtualMicrophone.volumeGroups[2] = oracle.room.game.cameras[k].room == oracle.room ? 1f - oracle.room.gravity : 1f;
        }
        if (!oracle.Consious)
        {
            return;
        }
        unconciousTick = 0f;
        currSubBehavior.Update();
        if (oracle.slatedForDeletetion)
        {
            return;
        }
        conversation?.Update();
        if (!currSubBehavior.CurrentlyCommunicating && (!ModManager.MSC || pearlConversation == null))
        {
            pathProgression = Mathf.Min(1f, pathProgression + (1f / Mathf.Lerp(40f + (pathProgression * 80f), Vector2.Distance(lastPos, nextPos) / 5f, 0.5f)));
        }
        if (ModManager.MSC && inspectPearl != null && inspectPearl is SpearMasterPearl)
        {
            pathProgression = Mathf.Min(1f, pathProgression + (1f / Mathf.Lerp(40f + (pathProgression * 80f), Vector2.Distance(lastPos, nextPos) / 5f, 0.5f)));
        }
        currentGetTo = Custom.Bezier(lastPos, ClampVectorInRoom(lastPos + lastPosHandle), nextPos, ClampVectorInRoom(nextPos + nextPosHandle), pathProgression);
        floatyMovement = false;
        investigateAngle += invstAngSpeed;
        inActionCounter++;
        if (player != null && player.room == oracle.room)
        {
            if (ModManager.MSC && playerOutOfRoomCounter > 0 && currSubBehavior != null && currSubBehavior is ESPSleepoverBehavior && pearlConversation == null && (currSubBehavior as ESPSleepoverBehavior).firstMetOnThisCycle && oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0)
            {
                UrgeAlong();
            }
            if (oracle.room.game.StoryCharacter == SlugcatStats.Name.Red && !HasSeenGreenNeuron)
            {
                for (int l = 0; l < player.grasps.Length; l++)
                {
                    if (player.grasps[l] != null && player.grasps[l].grabbed is NSHSwarmer)
                    {
                        Debug.Log("PEBBLES SEE GREEN NEURON");
                        SeePlayer();
                        break;
                    }
                }
            }
            playerOutOfRoomCounter = 0;
        }
        else
        {
            killFac = 0f;
            playerOutOfRoomCounter++;
        }
        if (pathProgression >= 1f && consistentBasePosCounter > 100 && !oracle.arm.baseMoving)
        {
            allStillCounter++;
        }
        else
        {
            allStillCounter = 0;
        }
        lastKillFac = killFac;
        lastKillFacOverseer = killFacOverseer;
        if (action == Action.General_Idle)
        {
            if (movementBehavior != MovementBehavior.Idle && movementBehavior != MovementBehavior.Meditate)
            {
                movementBehavior = MovementBehavior.Idle;
            }
            throwOutCounter = 0;
            if (player != null && player.room == oracle.room)
            {
                discoverCounter++;
                if (ModManager.MSC && oracle.room.world.name == "HR")
                {
                    SeePlayer();
                }
                else if (oracle.room.GetTilePosition(player.mainBodyChunk.pos).y < 32 && (discoverCounter > 220 || Custom.DistLess(player.mainBodyChunk.pos, oracle.firstChunk.pos, 150f) || !Custom.DistLess(player.mainBodyChunk.pos, oracle.room.MiddleOfTile(oracle.room.ShortcutLeadingToNode(1).StartTile), 150f)))
                {
                    SeePlayer();
                }
            }
        }
        else if (action == Action.General_GiveMark)
        {
            bool spearPebbles = ModManager.MSC && oracle.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Spear && oracle.ID == Oracle.OracleID.SS;
            movementBehavior = MovementBehavior.KeepDistance;
            if ((inActionCounter > 30 && inActionCounter < 300) || (ModManager.MSC && oracle.ID == MoreSlugcatsEnums.OracleID.DM))
            {
                if (inActionCounter < 300)
                {
                    if (ModManager.CoopAvailable)
                    {
                        StunCoopPlayers(20);
                    }
                    else
                    {
                        player.Stun(20);
                    }
                }
                Vector2 addVelocity = Vector2.ClampMagnitude(oracle.room.MiddleOfTile(24, 14) - player.mainBodyChunk.pos, 40f) / 40f * 2.8f * Mathf.InverseLerp(30f, 160f, inActionCounter);
                if (ModManager.CoopAvailable)
                {
                    foreach (Player p2 in PlayersInRoom)
                    {
                        p2.mainBodyChunk.vel += addVelocity;
                    }
                }
                else
                {
                    player.mainBodyChunk.vel += addVelocity;
                }
            }
            if (inActionCounter == 30)
            {
                oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Telekenisis, 0f, 1f, 1f);
            }
            if (spearPebbles && inActionCounter > 30 && inActionCounter < 300)
            {
                (player.graphicsModule as PlayerGraphics).bodyPearl.visible = true;
                (player.graphicsModule as PlayerGraphics).bodyPearl.globalAlpha = Mathf.Lerp(0f, 1f, inActionCounter / 300f);
            }
            if (inActionCounter == 300)
            {
                if (!ModManager.MSC || oracle.ID != MoreSlugcatsEnums.OracleID.DM)
                {
                    player.mainBodyChunk.vel += Custom.RNV() * 10f;
                    player.bodyChunks[1].vel += Custom.RNV() * 10f;
                }
                if (spearPebbles)
                {
                    (player.graphicsModule as PlayerGraphics).bodyPearl.visible = false;
                    (player.graphicsModule as PlayerGraphics).bodyPearl.scarVisible = true;
                    player.Regurgitate();
                    player.aerobicLevel = 1.1f;
                    player.exhausted = true;
                    player.SetMalnourished(m: true);
                    if (SMCorePearl == null)
                    {
                        for (int l2 = 0; l2 < oracle.room.updateList.Count; l2++)
                        {
                            if (oracle.room.updateList[l2] is SpearMasterPearl)
                            {
                                SMCorePearl = oracle.room.updateList[l2] as SpearMasterPearl;
                                if (UsesAPersistantTracker(SMCorePearl.abstractPhysicalObject))
                                {
                                    (oracle.room.game.session as StoryGameSession).AddNewPersistentTracker(SMCorePearl.abstractPhysicalObject);
                                }
                                break;
                            }
                        }
                    }
                    if (SMCorePearl != null)
                    {
                        SMCorePearl.firstChunk.vel *= 0f;
                        SMCorePearl.DisableGravity();
                        afterGiveMarkAction = NTEnums.ESPBehaviorAction.MeetPurple_GetPearl;
                    }
                    else
                    {
                        afterGiveMarkAction = Action.General_Idle;
                    }
                    if (ModManager.CoopAvailable)
                    {
                        StunCoopPlayers(60);
                    }
                    else
                    {
                        player.Stun(60);
                    }
                }
                else
                {
                    if (ModManager.MSC && oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                    {
                        afterGiveMarkAction = NTEnums.ESPBehaviorAction.MeetWhite_ThirdCurious;
                        player.AddFood(10);
                    }
                    if (ModManager.CoopAvailable)
                    {
                        StunCoopPlayers(40);
                    }
                    else
                    {
                        player.Stun(40);
                    }
                    (oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.theMark = true;
                }
                if (oracle.room.game.StoryCharacter == SlugcatStats.Name.Red)
                {
                    oracle.room.game.GetStorySession.saveState.redExtraCycles = true;
                    if (oracle.room.game.cameras[0].hud != null)
                    {
                        if (oracle.room.game.cameras[0].hud.textPrompt != null)
                        {
                            oracle.room.game.cameras[0].hud.textPrompt.cycleTick = 0;
                        }
                        if (oracle.room.game.cameras[0].hud.map != null && oracle.room.game.cameras[0].hud.map.cycleLabel != null)
                        {
                            oracle.room.game.cameras[0].hud.map.cycleLabel.UpdateCycleText();
                        }
                    }
                    player.redsIllness?.GetBetter();
                    if (ModManager.CoopAvailable)
                    {
                        foreach (AbstractCreature p3 in oracle.room.game.AlivePlayers)
                        {
                            if (p3.Room == oracle.room.abstractRoom)
                            {
                                (p3.realizedCreature as Player)?.redsIllness?.GetBetter();
                            }
                        }
                    }
                    if (!oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.pebblesHasIncreasedRedsKarmaCap)
                    {
                        oracle.room.game.GetStorySession.saveState.IncreaseKarmaCapOneStep();
                        oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.pebblesHasIncreasedRedsKarmaCap = true;
                    }
                    else
                    {
                        Debug.Log("PEBBLES HAS ALREADY GIVEN RED ONE KARMA CAP STEP");
                    }
                }
                else if (ModManager.MSC && oracle.room.game.StoryCharacter == MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
                {
                    if (!oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.pebblesHasIncreasedRedsKarmaCap)
                    {
                        oracle.room.game.GetStorySession.saveState.IncreaseKarmaCapOneStep();
                        oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.pebblesHasIncreasedRedsKarmaCap = true;
                    }
                    else
                    {
                        Debug.Log("PEBBLES HAS ALREADY GIVEN GOURMAND ONE KARMA CAP STEP");
                    }
                }
                else if (!ModManager.MSC || (oracle.ID == Oracle.OracleID.SS && oracle.room.game.StoryCharacter != MoreSlugcatsEnums.SlugcatStatsName.Artificer && !spearPebbles))
                {
                    (oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap = 9;
                }
                if (!spearPebbles)
                {
                    (oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma = (oracle.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap;
                    for (int n = 0; n < oracle.room.game.cameras.Length; n++)
                    {
                        oracle.room.game.cameras[n].hud.karmaMeter?.UpdateGraphic();
                    }
                }
                if (ModManager.CoopAvailable)
                {
                    foreach (Player p4 in PlayersInRoom)
                    {
                        for (int i3 = 0; i3 < 20; i3++)
                        {
                            if (IsPrideDay)
                                oracle.room.AddObject(new Spark(p4.mainBodyChunk.pos, Custom.RNV() * Random.value * 40f, new Color(Random.value, Random.value, Random.value), null, 30, 120));
                            else
                                oracle.room.AddObject(new Spark(p4.mainBodyChunk.pos, Custom.RNV() * Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                        }
                    }
                }
                else
                {
                    for (int i2 = 0; i2 < 20; i2++)
                    {
                        if (IsPrideDay)
                            oracle.room.AddObject(new Spark(player.mainBodyChunk.pos, Custom.RNV() * Random.value * 40f, new Color(Random.value, Random.value, Random.value), null, 30, 120));
                        else
                            oracle.room.AddObject(new Spark(player.mainBodyChunk.pos, Custom.RNV() * Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                    }
                }
                if (!spearPebbles)
                {
                    oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, 0f, 1f, 1f);
                }
            }
            if (inActionCounter > 300 && player.graphicsModule != null && !spearPebbles)
            {
                (player.graphicsModule as PlayerGraphics).markAlpha = Mathf.Max((player.graphicsModule as PlayerGraphics).markAlpha, Mathf.InverseLerp(500f, 300f, inActionCounter));
            }
            if (inActionCounter >= 500 || (spearPebbles && inActionCounter > 310))
            {
                NewAction(afterGiveMarkAction);
                if (conversation != null)
                {
                    conversation.paused = false;
                }
            }
        }
        Move();
        if (working != getToWorking)
        {
            working = Custom.LerpAndTick(working, getToWorking, 0.05f, 1f / 30f);
        }
        if (!ModManager.MSC || oracle.room.world.name != "HR")
        {
            for (int m = 0; m < oracle.room.game.cameras.Length; m++)
            {
                if (oracle.room.game.cameras[m].room == oracle.room && !oracle.room.game.cameras[m].AboutToSwitchRoom && oracle.room.game.cameras[m].paletteBlend != working)
                {
                    oracle.room.game.cameras[m].ChangeBothPalettes(25, 26, working);
                }
            }
        }
        if (ModManager.MSC)
        {
            if ((oracle.ID == MoreSlugcatsEnums.OracleID.DM || (oracle.ID == Oracle.OracleID.SS && oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer)) && player != null && player.room == oracle.room)
            {
                List<PhysicalObject>[] physicalObjects = oracle.room.physicalObjects;
                for (int num2 = 0; num2 < physicalObjects.Length; num2++)
                {
                    for (int num3 = 0; num3 < physicalObjects[num2].Count; num3++)
                    {
                        PhysicalObject item = physicalObjects[num2][num3];
                        if (item is Weapon && oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                        {
                            Weapon weapon = item as Weapon;
                            if (weapon.mode == Mode.Thrown && Custom.Dist(weapon.firstChunk.pos, oracle.firstChunk.pos) < 100f)
                            {
                                weapon.ChangeMode(Mode.Free);
                                weapon.SetRandomSpin();
                                weapon.firstChunk.vel *= -0.2f;
                                for (int num4 = 0; num4 < 5; num4++)
                                {
                                    if (IsPrideDay)
                                        oracle.room.AddObject(new Spark(weapon.firstChunk.pos, Custom.RNV(), new Color(Random.value, Random.value, Random.value), null, 16, 24));
                                    else
                                        oracle.room.AddObject(new Spark(weapon.firstChunk.pos, Custom.RNV(), Color.white, null, 16, 24));
                                }
                                if (IsPrideDay)
                                    oracle.room.AddObject(new Explosion.ExplosionLight(weapon.firstChunk.pos, 150f, 1f, 8, new Color(Random.value, Random.value, Random.value)));
                                else
                                    oracle.room.AddObject(new Explosion.ExplosionLight(weapon.firstChunk.pos, 150f, 1f, 8, Color.white));
                                oracle.room.AddObject(new ShockWave(weapon.firstChunk.pos, 60f, 0.1f, 8));
                                _ = oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, weapon.firstChunk, loop: false, 1f, 1.5f + (Random.value * 0.5f));
                            }
                        }
                        bool artificerCheck = false;
                        bool behaviorValid = (action == NTEnums.ESPBehaviorAction.Pebbles_SlumberParty || action == NTEnums.ESPBehaviorAction.Moon_SlumberParty || action == Action.General_Idle) && currSubBehavior is ESPSleepoverBehavior && (currSubBehavior as ESPSleepoverBehavior).panicObject == null;
                        if (oracle.ID == Oracle.OracleID.SS && oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer && currSubBehavior is ThrowOutBehavior)
                        {
                            behaviorValid = true;
                            artificerCheck = true;
                        }
                        if (!(inspectPearl == null && (conversation == null || artificerCheck) && item is DataPearl && (item as DataPearl).grabbedBy.Count == 0 && ((item as DataPearl).AbstractPearl.dataPearlType != DataPearl.AbstractDataPearl.DataPearlType.PebblesPearl || (oracle.ID == MoreSlugcatsEnums.OracleID.DM && ((item as DataPearl).AbstractPearl as PebblesPearl.AbstractPebblesPearl).color >= 0)) && !readDataPearlOrbits.Contains((item as DataPearl).AbstractPearl) && behaviorValid) || !oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark || talkedAboutThisSession.Contains(item.abstractPhysicalObject.ID))
                        {
                            continue;
                        }
                        inspectPearl = item as DataPearl;
                        if (inspectPearl is not SpearMasterPearl || !(inspectPearl.AbstractPearl as SpearMasterPearl.AbstractSpearMasterPearl).broadcastTagged)
                        {
                            Debug.Log($"---------- INSPECT PEARL TRIGGERED: {inspectPearl.AbstractPearl.dataPearlType}");
                            if (inspectPearl is SpearMasterPearl)
                            {
                                LockShortcuts();
                                if (oracle.room.game.cameras[0].followAbstractCreature.realizedCreature.firstChunk.pos.y > 600f)
                                {
                                    oracle.room.game.cameras[0].followAbstractCreature.realizedCreature.Stun(40);
                                    oracle.room.game.cameras[0].followAbstractCreature.realizedCreature.firstChunk.vel = new Vector2(0f, -4f);
                                }
                                getToWorking = 0.5f;
                                SetNewDestination(new Vector2(600f, 450f));
                            }
                            break;
                        }
                        inspectPearl = null;
                    }
                }
            }
            if (oracle.room.world.name == "HR")
            {
                int cornerID = 0;
                if (oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                {
                    cornerID = 2;
                }
                float midDist = Custom.Dist(oracle.arm.cornerPositions[0], oracle.arm.cornerPositions[2]) * 0.4f;
                if (Custom.Dist(baseIdeal, oracle.arm.cornerPositions[cornerID]) >= midDist)
                {
                    baseIdeal = oracle.arm.cornerPositions[cornerID] + ((baseIdeal - oracle.arm.cornerPositions[cornerID]).normalized * midDist);
                }
            }
            if (currSubBehavior.LowGravity >= 0f)
            {
                oracle.room.gravity = currSubBehavior.LowGravity;
                return;
            }
        }
        if (!currSubBehavior.Gravity)
        {
            oracle.room.gravity = Custom.LerpAndTick(oracle.room.gravity, 0f, 0.05f, 0.02f);
        }
        else if (!ModManager.MSC || oracle.room.world.name != "HR" || !oracle.room.game.IsStorySession || !oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon || oracle.ID != Oracle.OracleID.SS)
        {
            oracle.room.gravity = 1f - working;
        }
    }

    public void SlugcatEnterRoomReaction()
    {
        getToWorking = 0f;
        oracle.room.PlaySound(SoundID.SS_AI_Exit_Work_Mode, 0f, 1f, 1f);
        if (oracle.graphicsModule != null)
        {
            (oracle.graphicsModule as OracleGraphics).halo.ChangeAllRadi();
            (oracle.graphicsModule as OracleGraphics).halo.connectionsFireChance = 1f;
        }
        TurnOffSSMusic(abruptEnd: true);
    }

    public void TurnOffSSMusic(bool abruptEnd)
    {
        Debug.Log($"Fading out SS music, {abruptEnd}");
        for (int i = 0; i < oracle.room.updateList.Count; i++)
        {
            if (oracle.room.updateList[i] is SSMusicTrigger)
            {
                oracle.room.updateList[i].Destroy();
                break;
            }
        }
        if (abruptEnd && oracle.room.game.manager.musicPlayer != null && oracle.room.game.manager.musicPlayer.song != null && oracle.room.game.manager.musicPlayer.song is SSSong)
        {
            oracle.room.game.manager.musicPlayer.song.FadeOut(2f);
        }
    }

    public void NewAction(Action nextAction)
    {
        Debug.Log($"new action: {nextAction}, (from, {action}, )");
        if (nextAction == action)
        {
            return;
        }
        if (ModManager.MSC && oracle.room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && nextAction == Action.MeetWhite_Images)
        {
            inActionCounter = 0;
            action = nextAction;
            return;
        }
        if (ModManager.Expedition && oracle.room.game.rainWorld.ExpeditionMode && nextAction == Action.ThrowOut_KillOnSight)
        {
            inActionCounter = 0;
            nextAction = Action.ThrowOut_ThrowOut;
        }
        SubBehavior.SubBehavID switchToBehaviorID = !(nextAction == Action.MeetWhite_Curious) && !(nextAction == Action.MeetWhite_Images) && !(nextAction == Action.MeetWhite_SecondCurious) && !(nextAction == Action.MeetWhite_Shocked) && !(nextAction == Action.MeetWhite_Talking) && !(nextAction == Action.MeetWhite_Texting) ? nextAction == Action.MeetYellow_Init ? SubBehavior.SubBehavID.MeetYellow : nextAction == Action.MeetRed_Init ? SubBehavior.SubBehavID.MeetRed : !(nextAction == Action.ThrowOut_KillOnSight) && !(nextAction == Action.ThrowOut_SecondThrowOut) && !(nextAction == Action.ThrowOut_ThrowOut) && !(nextAction == Action.ThrowOut_Polite_ThrowOut) ? nextAction == Action.GetNeuron_Init || nextAction == Action.GetNeuron_TakeNeuron || nextAction == Action.GetNeuron_GetOutOfStomach || nextAction == Action.GetNeuron_InspectNeuron ? SubBehavior.SubBehavID.GetNeuron : ModManager.MSC && (nextAction == NTEnums.ESPBehaviorAction.MeetWhite_SecondImages || nextAction == NTEnums.ESPBehaviorAction.MeetWhite_ThirdCurious || nextAction == NTEnums.ESPBehaviorAction.MeetWhite_StartDialog) ? SubBehavior.SubBehavID.MeetWhite : ModManager.MSC && nextAction == NTEnums.ESPBehaviorAction.ThrowOut_Singularity ? SubBehavior.SubBehavID.ThrowOut : ModManager.MSC && (nextAction == NTEnums.ESPBehaviorAction.MeetPurple_Init || nextAction == NTEnums.ESPBehaviorAction.MeetPurple_GetPearl || nextAction == NTEnums.ESPBehaviorAction.MeetPurple_InspectPearl || nextAction == NTEnums.ESPBehaviorAction.MeetPurple_markeddialog || nextAction == NTEnums.ESPBehaviorAction.MeetPurple_anger || nextAction == NTEnums.ESPBehaviorAction.MeetPurple_killoverseer || nextAction == NTEnums.ESPBehaviorAction.MeetPurple_getout) ? NTEnums.ESPBehaviorSubBehavID.MeetPurple : ModManager.MSC && (nextAction == NTEnums.ESPBehaviorAction.Moon_SlumberParty || nextAction == NTEnums.ESPBehaviorAction.Moon_BeforeGiveMark || nextAction == NTEnums.ESPBehaviorAction.Moon_AfterGiveMark || nextAction == NTEnums.ESPBehaviorAction.Pebbles_SlumberParty) ? NTEnums.ESPBehaviorSubBehavID.SlumberParty : ModManager.MSC && nextAction == NTEnums.ESPBehaviorAction.MeetInv_Init ? NTEnums.ESPBehaviorSubBehavID.Commercial : ModManager.MSC && nextAction == NTEnums.ESPBehaviorAction.MeetGourmand_Init ? NTEnums.ESPBehaviorSubBehavID.MeetGourmand : ModManager.MSC && (nextAction == NTEnums.ESPBehaviorAction.MeetArty_Init || nextAction == NTEnums.ESPBehaviorAction.MeetArty_Talking) ? NTEnums.ESPBehaviorSubBehavID.MeetArty : !ModManager.MSC || !(nextAction == NTEnums.ESPBehaviorAction.Rubicon) ? SubBehavior.SubBehavID.General : NTEnums.ESPBehaviorSubBehavID.Rubicon : SubBehavior.SubBehavID.ThrowOut : SubBehavior.SubBehavID.MeetWhite;
        currSubBehavior.NewAction(action, nextAction);
        if (switchToBehaviorID != SubBehavior.SubBehavID.General && switchToBehaviorID != currSubBehavior.ID)
        {
            SubBehavior newBehav = null;
            for (int i = 0; i < allSubBehaviors.Count; i++)
            {
                if (allSubBehaviors[i].ID == switchToBehaviorID)
                {
                    newBehav = allSubBehaviors[i];
                    break;
                }
            }
            if (newBehav == null)
            {
                LockShortcuts();
                if (switchToBehaviorID == SubBehavior.SubBehavID.MeetWhite)
                {
                    newBehav = new ESPOracleMeetWhite(this);
                }
                allSubBehaviors.Add(newBehav);
            }
            newBehav.Activate(action, nextAction);
            currSubBehavior.Deactivate();
            Debug.Log($"Switching subbehavior to: {newBehav.ID}, from: {currSubBehavior.ID}");
            currSubBehavior = newBehav;
        }
        inActionCounter = 0;
        action = nextAction;
    }

    private void Move()
    {
        if (movementBehavior == MovementBehavior.Idle)
        {
            invstAngSpeed = 1f;
            if (investigateMarble == null && oracle.marbles.Count > 0)
            {
                investigateMarble = oracle.marbles[Random.Range(0, oracle.marbles.Count)];
            }
            if (investigateMarble != null && (investigateMarble.orbitObj == oracle || Custom.DistLess(new Vector2(250f, 150f), investigateMarble.firstChunk.pos, 100f)))
            {
                investigateMarble = null;
            }
            if (investigateMarble != null)
            {
                lookPoint = investigateMarble.firstChunk.pos;
                if (Custom.DistLess(nextPos, investigateMarble.firstChunk.pos, 100f))
                {
                    floatyMovement = true;
                    nextPos = investigateMarble.firstChunk.pos - (Custom.DegToVec(investigateAngle) * 50f);
                }
                else
                {
                    SetNewDestination(investigateMarble.firstChunk.pos - (Custom.DegToVec(investigateAngle) * 50f));
                }
                if (pathProgression == 1f && Random.value < 0.005f)
                {
                    investigateMarble = null;
                }
            }
            if (ModManager.MSC && oracle.ID == MoreSlugcatsEnums.OracleID.DM && Random.value < 0.001f)
            {
                movementBehavior = MovementBehavior.Meditate;
            }
        }
        else if (movementBehavior == MovementBehavior.Meditate)
        {
            if (nextPos != oracle.room.MiddleOfTile(24, 17))
            {
                SetNewDestination(oracle.room.MiddleOfTile(24, 17));
            }
            investigateAngle = 0f;
            lookPoint = oracle.firstChunk.pos + new Vector2(0f, -40f);
            if (ModManager.MMF && Random.value < 0.001f)
            {
                movementBehavior = MovementBehavior.Idle;
            }
        }
        else if (movementBehavior == MovementBehavior.KeepDistance)
        {
            if (player == null)
            {
                movementBehavior = MovementBehavior.Idle;
            }
            else
            {
                lookPoint = player.DangerPos;
                Vector2 tryPos = new(Random.value * oracle.room.PixelWidth, Random.value * oracle.room.PixelHeight);
                if (!oracle.room.GetTile(tryPos).Solid && oracle.room.aimap.getTerrainProximity(tryPos) > 2 && Vector2.Distance(tryPos, player.DangerPos) > Vector2.Distance(nextPos, player.DangerPos) + 100f)
                {
                    SetNewDestination(tryPos);
                }
            }
        }
        else if (movementBehavior == MovementBehavior.Investigate)
        {
            if (player == null)
            {
                movementBehavior = MovementBehavior.Idle;
            }
            else
            {
                lookPoint = player.DangerPos;
                if (investigateAngle < -90f || investigateAngle > 90f || oracle.room.aimap.getTerrainProximity(nextPos) < 2f)
                {
                    investigateAngle = Mathf.Lerp(-70f, 70f, Random.value);
                    invstAngSpeed = Mathf.Lerp(0.4f, 0.8f, Random.value) * ((Random.value < 0.5f) ? (-1f) : 1f);
                }
                Vector2 tryPos = player.DangerPos + (Custom.DegToVec(investigateAngle) * 150f);
                if (oracle.room.aimap.getTerrainProximity(tryPos) >= 2f)
                {
                    if (pathProgression > 0.9f)
                    {
                        if (Custom.DistLess(oracle.firstChunk.pos, tryPos, 30f))
                        {
                            floatyMovement = true;
                        }
                        else if (!Custom.DistLess(nextPos, tryPos, 30f))
                        {
                            SetNewDestination(tryPos);
                        }
                    }
                    nextPos = tryPos;
                }
            }
        }
        else if (movementBehavior == MovementBehavior.Talk)
        {
            if (player == null)
            {
                movementBehavior = MovementBehavior.Idle;
            }
            else
            {
                lookPoint = player.DangerPos;
                Vector2 tryPos = new(Random.value * oracle.room.PixelWidth, Random.value * oracle.room.PixelHeight);
                if (CommunicatePosScore(tryPos) + 40f < CommunicatePosScore(nextPos) && !Custom.DistLess(tryPos, nextPos, 30f))
                {
                    SetNewDestination(tryPos);
                }
            }
        }
        else if (movementBehavior == MovementBehavior.ShowMedia)
        {
            if (currSubBehavior is ESPOracleMeetWhite)
            {
                (currSubBehavior as ESPOracleMeetWhite).ShowMediaMovementBehavior();
            }
        }
        if (currSubBehavior != null && currSubBehavior.LookPoint.HasValue)
        {
            lookPoint = currSubBehavior.LookPoint.Value;
        }
        consistentBasePosCounter++;
        if (oracle.room.readyForAI)
        {
            Vector2 tryPos = new(Random.value * oracle.room.PixelWidth, Random.value * oracle.room.PixelHeight);
            if (!oracle.room.GetTile(tryPos).Solid && BasePosScore(tryPos) + 40f < BasePosScore(baseIdeal))
            {
                baseIdeal = tryPos;
                consistentBasePosCounter = 0;
            }
        }
        else
        {
            baseIdeal = nextPos;
        }
    }

    private float BasePosScore(Vector2 tryPos)
    {
        if (movementBehavior == MovementBehavior.Meditate || player == null)
        {
            return Vector2.Distance(tryPos, oracle.room.MiddleOfTile(24, 5));
        }
        if (movementBehavior == MovementBehavior.ShowMedia)
        {
            return 0f - Vector2.Distance(player.DangerPos, tryPos);
        }
        float f = Mathf.Abs(Vector2.Distance(nextPos, tryPos) - 200f);
        return f + Custom.LerpMap(Vector2.Distance(player.DangerPos, tryPos), 40f, 300f, 800f, 0f);
    }

    private float CommunicatePosScore(Vector2 tryPos)
    {
        if (oracle.room.GetTile(tryPos).Solid || player == null)
        {
            return float.MaxValue;
        }
        float f = Mathf.Abs(Vector2.Distance(tryPos, player.DangerPos) - ((movementBehavior == MovementBehavior.Talk) ? 250f : 400f));
        f -= Custom.IntClamp(oracle.room.aimap.getTerrainProximity(tryPos), 0, 8) * 10f;
        if (movementBehavior == MovementBehavior.ShowMedia)
        {
            f += (Custom.IntClamp(oracle.room.aimap.getTerrainProximity(tryPos), 8, 16) - 8) * 10f;
        }
        return f;
    }

    private void SetNewDestination(Vector2 dst)
    {
        lastPos = currentGetTo;
        nextPos = dst;
        lastPosHandle = Custom.RNV() * Mathf.Lerp(0.3f, 0.65f, Random.value) * Vector2.Distance(lastPos, nextPos);
        nextPosHandle = -GetToDir * Mathf.Lerp(0.3f, 0.65f, Random.value) * Vector2.Distance(lastPos, nextPos);
        pathProgression = 0f;
    }

    private Vector2 ClampVectorInRoom(Vector2 v)
    {
        Vector2 rtrn = v;
        rtrn.x = Mathf.Clamp(rtrn.x, oracle.arm.cornerPositions[0].x + 10f, oracle.arm.cornerPositions[1].x - 10f);
        rtrn.y = Mathf.Clamp(rtrn.y, oracle.arm.cornerPositions[2].y + 10f, oracle.arm.cornerPositions[1].y - 10f);
        return rtrn;
    }

    public new string ReplaceParts(string s)
    {
        s = Regex.Replace(s, "<PLAYERNAME>", NameForPlayer(capitalized: false));
        s = Regex.Replace(s, "<CAPPLAYERNAME>", NameForPlayer(capitalized: true));
        s = Regex.Replace(s, "<PlayerName>", NameForPlayer(capitalized: false));
        s = Regex.Replace(s, "<CapPlayerName>", NameForPlayer(capitalized: true));
        return s;
    }

    protected string NameForPlayer(bool capitalized)
    {
        string useName = Translate("creature");
        string little = Translate("little");
        if (capitalized && InGameTranslator.LanguageID.UsesCapitals(oracle.room.game.rainWorld.inGameTranslator.currentLanguage))
        {
            little = char.ToUpper(little[0]) + little.Substring(1);
        }
        return little + " " + useName;
    }

    public new void SpecialEvent(string eventName)
    {
        Debug.Log($"SPECEVENT : {eventName}");
        if (eventName == "karma")
        {
            if (conversation != null)
            {
                conversation.paused = true;
            }
            afterGiveMarkAction = action;
            NewAction(Action.General_GiveMark);
        }
        if (!ModManager.MSC)
        {
            return;
        }
        if (eventName == "panic")
        {
            OraclePanicDisplay panicObj = new(oracle);
            oracle.room.AddObject(panicObj);
            if (currSubBehavior is ESPSleepoverBehavior)
            {
                (currSubBehavior as ESPSleepoverBehavior).panicObject = panicObj;
            }
        }
        if (eventName == "tag" && currSubBehavior is ESPSleepoverBehavior)
        {
            (currSubBehavior as ESPSleepoverBehavior).tagTimer = 120f;
        }
        if (eventName == "unlock")
        {
            if (conversation != null)
            {
                conversation.paused = true;
            }
            NewAction(Action.MeetWhite_Images);
        }
    }

    public void InitStoryPearlCollection()
    {
        readDataPearlOrbits = [];
        readPearlGlyphs = [];
        foreach (AbstractWorldEntity ent in oracle.room.abstractRoom.entities)
        {
            if (ent is DataPearl.AbstractDataPearl)
            {
                DataPearl.AbstractDataPearl getPearl = ent as DataPearl.AbstractDataPearl;
                if (getPearl.type != AbstractObjectType.PebblesPearl)
                {
                    readDataPearlOrbits.Add(getPearl);
                }
            }
        }
        int index = 0;
        foreach (DataPearl.AbstractDataPearl pearl in readDataPearlOrbits)
        {
            Vector2 goalPos = StoredPearlOrbitLocation(index);
            if (pearl.realizedObject != null)
            {
                pearl.realizedObject.firstChunk.pos = goalPos;
            }
            else
            {
                pearl.pos.Tile = oracle.room.GetTilePosition(goalPos);
            }
            index++;
        }
        inspectPearl = null;
    }

    public Vector2 StoredPearlOrbitLocation(int index)
    {
        if (!ModManager.MSC || ((oracle.room.game.StoryCharacter != MoreSlugcatsEnums.SlugcatStatsName.Spear || oracle.ID != MoreSlugcatsEnums.OracleID.DM) && (oracle.room.game.StoryCharacter != MoreSlugcatsEnums.SlugcatStatsName.Artificer || oracle.ID != Oracle.OracleID.SS)))
        {
            return new Vector2(570f, 630f) + (Custom.DegToVec(index * 3) * 5f);
        }
        float gridSize = 5f;
        float row = index % gridSize;
        float col = Mathf.Floor(index / gridSize);
        float yalign = row * 0.5f;
        return new Vector2(615f, 100f) + new Vector2(row * 26f, (col + yalign) * 18f);
    }

    public void UpdateStoryPearlCollection()
    {
        List<DataPearl.AbstractDataPearl> reorderList = [];
        int pearlOrbitCounter = 0;
        foreach (DataPearl.AbstractDataPearl pearl in readDataPearlOrbits)
        {
            if (pearl.realizedObject == null)
            {
                continue;
            }
            if (pearl.realizedObject.grabbedBy.Count > 0)
            {
                reorderList.Add(pearl);
                continue;
            }
            if (!readPearlGlyphs.ContainsKey(pearl))
            {
                readPearlGlyphs.Add(pearl, new GlyphLabel(pearl.realizedObject.firstChunk.pos, GlyphLabel.RandomString(1, 1, 12842 + pearl.dataPearlType.Index, cyrillic: false)));
                oracle.room.AddObject(readPearlGlyphs[pearl]);
            }
            else
            {
                readPearlGlyphs[pearl].setPos = pearl.realizedObject.firstChunk.pos;
            }
            pearl.realizedObject.firstChunk.pos = Custom.MoveTowards(pearl.realizedObject.firstChunk.pos, StoredPearlOrbitLocation(pearlOrbitCounter), 2.5f);
            pearl.realizedObject.firstChunk.vel *= 0.99f;
            pearlOrbitCounter++;
        }
        foreach (DataPearl.AbstractDataPearl releasePearl in reorderList)
        {
            Debug.Log($"stored pearl grabbed, releasing from storage, {releasePearl}");
            readPearlGlyphs[releasePearl].Destroy();
            _ = readPearlGlyphs.Remove(releasePearl);
            _ = readDataPearlOrbits.Remove(releasePearl);
        }
    }

    public override void UnconciousUpdate()
    {
        base.UnconciousUpdate();
        oracle.room.gravity = 1f;
        oracle.setGravity(0.9f);
        if (oracle.ID == Oracle.OracleID.SS)
        {
            for (int i = 0; i < oracle.room.game.cameras.Length; i++)
            {
                if (oracle.room.game.cameras[i].room == oracle.room && !oracle.room.game.cameras[i].AboutToSwitchRoom)
                {
                    oracle.room.game.cameras[i].ChangeBothPalettes(10, 26, 0.51f + (Mathf.Sin(unconciousTick * 0.25707963f) * 0.35f));
                }
            }
            unconciousTick += 1f;
        }
        if (!ModManager.MSC || !(oracle.ID == MoreSlugcatsEnums.OracleID.DM))
        {
            return;
        }
        oracle.dazed = 320f;
        float flashProgress = Mathf.Min(1f, unconciousTick / 320f);
        float flashIntensity = flashProgress * 2f;
        if (flashProgress > 0.5f)
        {
            flashIntensity = 1f - ((flashProgress - 0.5f) / 0.5f);
        }
        if (Random.value < 0.5f)
        {
            for (int j = 0; j < oracle.room.game.cameras.Length; j++)
            {
                if (oracle.room.game.cameras[j].room == oracle.room && !oracle.room.game.cameras[j].AboutToSwitchRoom)
                {
                    oracle.room.game.cameras[j].ChangeBothPalettes(10, 26, 1f - (Mathf.Abs(Mathf.Sin(Random.value * (float)Math.PI * 2f)) * flashIntensity * 0.75f));
                }
            }
        }
        unconciousTick += 1f;
    }

    public void StartItemConversation(DataPearl item)
    {
        SLOrcacleState slState = oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SLOracleState;
        Debug.Log(item.AbstractPearl.dataPearlType.ToString());
        isRepeatedDiscussion = false;
        if (item.AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc || item.AbstractPearl.dataPearlType.Index == -1)
        {
            pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(Conversation.ID.Moon_Pearl_Misc, this, SLOracleBehaviorHasMark.MiscItemType.NA);
        }
        else if (item.AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc2)
        {
            pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(Conversation.ID.Moon_Pearl_Misc2, this, SLOracleBehaviorHasMark.MiscItemType.NA);
        }
        else if (ModManager.MSC && item.AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.BroadcastMisc)
        {
            pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(MoreSlugcatsEnums.ConversationID.Moon_Pearl_BroadcastMisc, this, SLOracleBehaviorHasMark.MiscItemType.NA);
        }
        else if (ModManager.MSC && oracle.ID == MoreSlugcatsEnums.OracleID.DM && item.AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.PebblesPearl && (item.AbstractPearl as PebblesPearl.AbstractPebblesPearl).color >= 0)
        {
            pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(Conversation.ID.Moon_Pebbles_Pearl, this, SLOracleBehaviorHasMark.MiscItemType.NA);
        }
        else
        {
            if (pearlConversation != null)
            {
                pearlConversation.Interrupt("...", 0);
                pearlConversation.Destroy();
                pearlConversation = null;
            }
            Conversation.ID id = Conversation.DataPearlToConversation(item.AbstractPearl.dataPearlType);
            if (!slState.significantPearls.Contains(item.AbstractPearl.dataPearlType))
            {
                slState.significantPearls.Add(item.AbstractPearl.dataPearlType);
            }
            if (ModManager.MSC && oracle.ID == MoreSlugcatsEnums.OracleID.DM)
            {
                isRepeatedDiscussion = rainWorld.progression.miscProgressionData.GetDMPearlDeciphered(item.AbstractPearl.dataPearlType);
                _ = rainWorld.progression.miscProgressionData.SetDMPearlDeciphered(item.AbstractPearl.dataPearlType);
            }
            else
            {
                isRepeatedDiscussion = rainWorld.progression.miscProgressionData.GetPebblesPearlDeciphered(item.AbstractPearl.dataPearlType);
                _ = rainWorld.progression.miscProgressionData.SetPebblesPearlDeciphered(item.AbstractPearl.dataPearlType);
            }
            pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(id, this, SLOracleBehaviorHasMark.MiscItemType.NA);
            slState.totalPearlsBrought++;
            Debug.Log($"pearls brought up:, {slState.totalPearlsBrought}");
        }
        if (!isRepeatedDiscussion)
        {
            slState.totalItemsBrought++;
            slState.AddItemToAlreadyTalkedAbout(item.abstractPhysicalObject.ID);
        }
        talkedAboutThisSession.Add(item.abstractPhysicalObject.ID);
    }

    public void InterruptPearlMessagePlayerLeaving()
    {
        int num = Random.Range(0, 5);
        string s = (!ModManager.MSC || !(oracle.ID == MoreSlugcatsEnums.OracleID.DM)) ? (num switch
        {
            0 => "Oh... Good bye.",
            1 => "Where are you going?",
            2 => "Are you not interested?",
            3 => "Please do not return.",
            _ => "What are you doing?",
        }) : (num switch
        {
            0 => "Oh... Good bye, little creature.",
            1 => "Oh, leaving already?",
            2 => "Oh... Good luck, little creature.",
            3 => "Oh... Please be careful on your journey.",
            _ => "Ah, you must be in a rush.",
        });
        pearlConversation.Interrupt(Translate(s), 10);
    }

    public void ResumePausedPearlConversation()
    {
        int num = Random.Range(0, 5);
        string s = (!ModManager.MSC || !(oracle.ID == MoreSlugcatsEnums.OracleID.DM)) ? (num switch
        {
            0 => "Let me continue...",
            1 => "Please do not do that...",
            2 => "Let me finish.",
            3 => "Please, a moment of your time to finish?",
            _ => "Shall we finish?",
        }) : (num switch
        {
            0 => "You've returned! I thought you were leaving. Allow me to continue...",
            1 => "Ah, welcome back. As I was saying...",
            2 => "Ah, nevermind, let me finish our conversation.",
            3 => "You're not leaving? I suppose I should continue then...",
            _ => "Hello again. Shall I keep reading?",
        });
        pearlConversation.Interrupt(Translate(s), 10);
        restartConversationAfterCurrentDialoge = true;
    }

    public void UrgeAlong()
    {
        string resumeMessage = "";
        switch (Random.Range(0, 3))
        {
            case 0:
                resumeMessage = "You'd better hurry along, little creature!";
                break;

            case 1:
                resumeMessage = "Ah, don't worry about me. You should get going!";
                break;

            case 2:
                resumeMessage = "There's not a lot of time. You should go!";
                break;
        }
        dialogBox.Interrupt(Translate(resumeMessage), 10);
    }

    public void WitnessIsNotReal()
    {
        dialogBox.NewMessage("Whissty? Did you rest well?", 0);
        dialogBox.NewMessage("You can stay as long as you want, but I have something for you.", 0);
        dialogBox.NewMessage("This pearl, I need you to take it to some friends.", 0);
        dialogBox.NewMessage("Pearl", 0);
        dialogBox.currentColor = Color.red;
        dialogBox.NewMessage("First, take it to my friend, she's to the west.", 0);
        dialogBox.currentColor = Color.white;
        dialogBox.NewMessage("Then she'll give you instructions.", 0);
        dialogBox.NewMessage("Thank you for supporting me.", 0);
    }
}