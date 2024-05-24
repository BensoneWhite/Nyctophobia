/*
using System;
using System.Collections.Generic;
using System.Linq;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace SomethingSea;

public class SomethingScene : BackgroundScene
{
    

    public abstract class SomethingSceneElement : BackgroundSceneElement
    {
        public bool visible = true;

        private SomethingScene somethingScene => scene as SomethingScene;

        public SomethingSceneElement(SomethingScene somethingScene, Vector2 pos, float depth)
            : base(somethingScene, pos, depth)
        {
        }


        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("Background");
            }
            base.AddToContainer(sLeaser, rCam, newContatiner);
        }
    }

    public class SomethingCeiling : SomethingSceneElement
    {
        public float scale = 50f;

        private string assetName;

        public int index;

        private SomethingScene somethingScene => scene as SomethingScene;

        public SomethingCeiling(SomethingScene somethingScene, string assetName, Vector2 pos, float depth, int index)
            : base(somethingScene, pos, depth)
        {
            base.depth = depth;
            this.assetName = assetName;
            this.index = index;
            somethingScene.LoadGraphic(assetName, crispPixels: false, clampWrapMode: false);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite(assetName);
            if (somethingScene.Inverted)
            {
                sLeaser.sprites[0].scaleY = scale / depth;
            }
            else
            {
                sLeaser.sprites[0].scaleY = (0f - scale) / depth;
            }
            sLeaser.sprites[0].scaleX = scale;
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["VoidCeiling"];
            sLeaser.sprites[0].anchorY = 0f;
            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = DrawPos(camPos, rCam.hDisplace);
            visible = vector.y - 150f * (scale / depth) < 768f && camPos.y > -12500f;
            if (somethingScene.Inverted)
            {
                visible = camPos.y < somethingScene.room.PixelHeight + 12800f;
            }
            sLeaser.sprites[0].x = rCam.game.rainWorld.screenSize.x / 2f;
            sLeaser.sprites[0].y = vector.y;
            sLeaser.sprites[0].color = new Color(1f / depth, 1f / scale, 1f);
            if (index == 0)
            {
                Shader.SetGlobalVector(RainWorld.ShadPropWorldCamPos, camPos - scene.sceneOrigo + somethingScene.cameraOffset);
            }
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }
    }

    public class VoidSprite : SomethingSceneElement
    {
        public Vector2[,] positions;

        private SomethingScene somethingScene => scene as SomethingScene;

        public float Rad => 1f;

        public VoidSprite(SomethingScene somethingScene, float depth, int index)
            : base(somethingScene, new Vector2(0f, 0f), depth)
        {
            base.depth = depth;
            positions = new Vector2[somethingScene.room.game.cameras.Length, 3];
        }


        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("pixel");
            sLeaser.sprites[0].anchorY = 0f;
            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            positions[rCam.cameraNumber, 2] = positions[rCam.cameraNumber, 0];
            positions[rCam.cameraNumber, 0] += (positions[rCam.cameraNumber, 1] - camPos) / depth;
            positions[rCam.cameraNumber, 1] = camPos;
            if (positions[rCam.cameraNumber, 0].x < 0f - Rad)
            {
                positions[rCam.cameraNumber, 0].x = 1400f + Rad;
                positions[rCam.cameraNumber, 0].y = UnityEngine.Random.value * 800f;
                positions[rCam.cameraNumber, 2] = positions[rCam.cameraNumber, 0];
            }
            else if (positions[rCam.cameraNumber, 0].x > 1400f + Rad)
            {
                positions[rCam.cameraNumber, 0].x = 0f - Rad;
                positions[rCam.cameraNumber, 0].y = UnityEngine.Random.value * 800f;
                positions[rCam.cameraNumber, 2] = positions[rCam.cameraNumber, 0];
            }
            if (positions[rCam.cameraNumber, 0].y < 0f - Rad)
            {
                positions[rCam.cameraNumber, 0].y = 800f + Rad;
                positions[rCam.cameraNumber, 0].x = UnityEngine.Random.value * 1400f;
                positions[rCam.cameraNumber, 2] = positions[rCam.cameraNumber, 0];
            }
            else if (positions[rCam.cameraNumber, 0].y > 800f + Rad)
            {
                positions[rCam.cameraNumber, 0].y = 0f - Rad;
                positions[rCam.cameraNumber, 0].x = UnityEngine.Random.value * 1400f;
                positions[rCam.cameraNumber, 2] = positions[rCam.cameraNumber, 0];
            }
            Vector2 vector = positions[rCam.cameraNumber, 0];
            sLeaser.sprites[0].x = vector.x;
            sLeaser.sprites[0].y = vector.y;
            if (Custom.DistLess(positions[rCam.cameraNumber, 2], positions[rCam.cameraNumber, 0], 1f))
            {
                sLeaser.sprites[0].rotation = 0f;
                sLeaser.sprites[0].scaleY = 1f;
            }
            else
            {
                sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(positions[rCam.cameraNumber, 0], positions[rCam.cameraNumber, 2]);
                sLeaser.sprites[0].scaleY = Vector2.Distance(positions[rCam.cameraNumber, 0], positions[rCam.cameraNumber, 2]);
                sLeaser.sprites[0].alpha *= Custom.LerpMap(Vector2.Distance(positions[rCam.cameraNumber, 0], positions[rCam.cameraNumber, 2]), 1f, 16f, 0.75f, 0.35f);
            }
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }
    }

    public class SomethingBkg : FullScreenSingleColor
    {
        public SomethingScene something;

        public SomethingBkg(SomethingScene something)
            : base(something, new Color(0f, 0f, 0.003921569f), 1f, singlePixelTexture: true, float.MaxValue)
        {
            this.something = something;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["Basic"];
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {

            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            newContatiner = rCam.ReturnFContainer("Background");
            base.AddToContainer(sLeaser, rCam, newContatiner);
        }
    }

    public class SomethingFade : BackgroundSceneElement
    {
        public SomethingScene something;

        public SomethingFade(SomethingScene something)
            : base(something, default(Vector2), 0.1f)
        {
            this.something = something;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new CustomFSprite("Futile_White");
            for (int i = 0; i < 4; i++)
            {
                (sLeaser.sprites[0] as CustomFSprite).verticeColors[i] = new Color(69f / 85f, 0.5568628f, 24f / 85f, (i < 2) ? 0f : 0.75f);
            }
            base.InitiateSprites(sLeaser, rCam);
            AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("GrabShaders"));
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (something.Inverted)
            {
                (sLeaser.sprites[0] as CustomFSprite).vertices[0] = new Vector2(-100f, something.room.PixelHeight - 560f - camPos.y);
                (sLeaser.sprites[0] as CustomFSprite).vertices[1] = new Vector2(1400f, something.room.PixelHeight - 560f - camPos.y);
                (sLeaser.sprites[0] as CustomFSprite).vertices[2] = new Vector2(1400f, something.room.PixelHeight + 140f - camPos.y);
                (sLeaser.sprites[0] as CustomFSprite).vertices[3] = new Vector2(-100f, something.room.PixelHeight + 140f - camPos.y);
            }
            else
            {
                (sLeaser.sprites[0] as CustomFSprite).vertices[0] = new Vector2(-100f, 800f - camPos.y);
                (sLeaser.sprites[0] as CustomFSprite).vertices[1] = new Vector2(1400f, 800f - camPos.y);
                (sLeaser.sprites[0] as CustomFSprite).vertices[2] = new Vector2(1400f, 100f - camPos.y);
                (sLeaser.sprites[0] as CustomFSprite).vertices[3] = new Vector2(-100f, 100f - camPos.y);
            }
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }
    }

    public class WormLightFade : FullScreenSingleColor
    {
        public SomethingScene something;

        public WormLightFade(SomethingScene something)
            : base(something, new Color(0.5294118f, 31f / 85f, 0.18431373f), 1f, singlePixelTexture: false, 0f)
        {
            this.something = something;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["Basic"];
        }



        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            base.AddToContainer(sLeaser, rCam, newContatiner);
        }
    }

    public class TheEgg : SomethingSceneElement
    {
        public float whiteFade;

        public float lastWhiteFade;

        public float greyFade;

        public float lastGreyFade;

        public int fadeWait;

        public float musicVolumeDirectionBoost;

        public float musicVolume;

        public int counter;

        private bool exitCommand;

        public List<float> playerDists;

        private SomethingScene somethingScene => scene as SomethingScene;

        public float maxAllowedDist => Custom.LerpMap(counter, 400f, 9600f, 11000f, 5000f);

        public TheEgg(SomethingScene somethingScene, Vector2 pos)
            : base(somethingScene, pos, 1f)
        {
            playerDists = new List<float>();
        }

        



        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[23];
            sLeaser.sprites[0] = new FSprite("Futile_White");
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["FlatLightNoisy"];
            sLeaser.sprites[sLeaser.sprites.Length - 3] = new FSprite("Futile_White");
            sLeaser.sprites[sLeaser.sprites.Length - 3].shader = rCam.game.rainWorld.Shaders["FlatLightNoisy"];
            sLeaser.sprites[sLeaser.sprites.Length - 2] = new FSprite("Futile_White");
            sLeaser.sprites[sLeaser.sprites.Length - 2].shader = rCam.game.rainWorld.Shaders["FlatLightNoisy"];
            sLeaser.sprites[sLeaser.sprites.Length - 1] = new FSprite("Futile_White");
            sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = 93.75f;
            sLeaser.sprites[sLeaser.sprites.Length - 1].scaleY = 56.25f;
            sLeaser.sprites[sLeaser.sprites.Length - 1].x = 700f;
            sLeaser.sprites[sLeaser.sprites.Length - 1].y = 400f;
            for (int i = 1; i < sLeaser.sprites.Length - 3; i++)
            {
                sLeaser.sprites[i] = new FSprite("Futile_White");
                sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["FlatLightNoisy"];
                sLeaser.sprites[i].color = new Color(0f, 0f, 0f);
                sLeaser.sprites[i].anchorY = 0.2f;
            }
            for (int j = 0; j < sLeaser.sprites.Length; j++)
            {
                sLeaser.sprites[j].isVisible = false;
            }
            AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Foreground"));
        }

        
    }

    public class SaintEndingPhase : ExtEnum<SaintEndingPhase>
    {
        public static readonly SaintEndingPhase Inactive = new SaintEndingPhase("Inactive", register: true);

        public static readonly SaintEndingPhase WormDeath = new SaintEndingPhase("WormDeath", register: true);

        public static readonly SaintEndingPhase EchoTransform = new SaintEndingPhase("EchoTransform", register: true);

        public static readonly SaintEndingPhase Drowned = new SaintEndingPhase("Drowned", register: true);

        public SaintEndingPhase(string value, bool register = false)
            : base(value, register)
        {
        }
    }

    public class MeltingItem
    {
        public PhysicalObject meltingObject;

        private int meltTimer;

        private int maxMeltTime;

        private SomethingScene voidScene;

        public MeltingItem(PhysicalObject obj, SomethingScene voidScene)
        {
            meltingObject = obj;
            this.voidScene = voidScene;
            maxMeltTime = UnityEngine.Random.Range(100, 200);
        }

        public void Update()
        {
            if (meltingObject == null)
            {
                return;
            }
            if (voidScene.Inverted)
            {
                if (meltingObject.firstChunk.pos.y > 6000f || meltTimer > 0)
                {
                    meltTimer++;
                }
            }
            else
            {
                meltingObject.firstChunk.vel += new Vector2(0f, 0.5f);
                if (meltingObject.firstChunk.pos.y < 500f || meltTimer > 0)
                {
                    meltTimer++;
                }
            }
            if (meltTimer > maxMeltTime)
            {
                Custom.Log($"Object melted by void sea {meltingObject.abstractPhysicalObject}");
                while (meltingObject.grabbedBy.Count > 0)
                {
                    meltingObject.grabbedBy[0].Release();
                }
                for (int i = 0; i < 12; i++)
                {
                    voidScene.room.AddObject(new VoidParticle(meltingObject.firstChunk.pos + Custom.RNV() * 12f, Custom.DegToVec(Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), new Vector2(0f, 1f)) + (float)UnityEngine.Random.Range(-5, 5)) * UnityEngine.Random.Range(0.25f, 3f), UnityEngine.Random.Range(20f, 80f)));
                }
                voidScene.room.RemoveObject(meltingObject);
                meltingObject.abstractPhysicalObject.Destroy();
                meltingObject.Destroy();
                meltingObject = null;
            }
            else
            {
                if (UnityEngine.Random.value < dissolved() / 10f && meltingObject.grabbedBy.Count > 0)
                {
                    meltingObject.grabbedBy[0].Release();
                }
                if (dissolved() > 0f && UnityEngine.Random.value < dissolved() * 2f)
                {
                    voidScene.room.AddObject(new VoidParticle(meltingObject.firstChunk.pos + Custom.RNV() * 12f, Custom.DegToVec(Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), new Vector2(0f, 1f)) + (float)UnityEngine.Random.Range(-5, 5)) * UnityEngine.Random.Range(0.25f, 3f), UnityEngine.Random.Range(20f, 80f)));
                }
            }
        }

        public static bool Valid(PhysicalObject obj, Room room)
        {
            if (obj != null && obj.Submersion > 0.5f && !obj.slatedForDeletetion)
            {
                return obj.room == room;
            }
            return false;
        }

        public float dissolved()
        {
            return (float)meltTimer / (float)maxMeltTime;
        }
    }

    public Vector2 cameraOffset;

    public bool[] lastSomethingModes;

    public PlayerGhosts playerGhosts;

    public List<VoidWorm> worms;


    public bool secondSpace;

    public bool ridingWorm;

    public int eggScenarioTimer;

    public float eggProximity;

    public TheEgg theEgg;

    public bool playerInRoom;

    public DisembodiedDynamicSoundLoop wooshLoop;

    public DisembodiedDynamicSoundLoop wormsLoop;

    public DisembodiedDynamicSoundLoop eggLoop;

    public DisembodiedDynamicSoundLoop swimLoop;

    public float playerY;

    public bool playerDipped;

    public SomethingBkg somethingBackground;

    public SaintEndingPhase saintEndPhase;

    public float timeInSaintPhase;

    public int editObject;

    public SaintsJourneyIllustration storedJourneyIllustration;

    private float fadeOutSaint;

    public bool fadeOutLights;

    public int fadeOutLightsTimer;

    public float musicFadeFac;

    public FadeOut blackFade;

    public bool endingSavedFlag;

    public List<MeltingItem> meltingObjects;



    public bool Inverted
    {
        get
        {
            if (ModManager.MSC)
            {
                return room.waterInverted;
            }
            return false;
        }
    }

    public SomethingScene(Room room)
        : base(room)
    {
        base.room = room;
        if (ModManager.MMF)
        {
            meltingObjects = new List<MeltingItem>();
            foreach (AbstractWorldEntity entity in base.room.abstractRoom.entities)
            {
                if (entity is AbstractPhysicalObject && (entity as AbstractPhysicalObject).type != AbstractPhysicalObject.AbstractObjectType.Creature && (entity as AbstractPhysicalObject).realizedObject != null && MeltingItem.Valid((entity as AbstractPhysicalObject).realizedObject, base.room))
                {
                    Custom.Log($"Add premelt object {entity as AbstractPhysicalObject}");
                    AddMeltObject((entity as AbstractPhysicalObject).realizedObject);
                }
            }
        }

        somethingBackground = new SomethingBkg(this);
        AddElement(somethingBackground);
        AddElement(new SomethingFade(this));
        theEgg = new TheEgg(this, new Vector2(-200000f, -200000f));
        AddElement(theEgg);
        for (int i = 0; i < 20; i++)
        {
            float f = (float)i / 19f;
            float y = 580f;
            if (Inverted)
            {
                y = 22000f;
            }
            AddElement(new SomethingCeiling(this, "clouds" + (i % 3 + 1), new Vector2(0f, y), Mathf.Lerp(1.5f, 25f, Mathf.Pow(f, 1.5f)), i));
        }
        for (int j = 0; j < 150; j++)
        {
            AddElement(new VoidSprite(this, Mathf.Lerp(0.5f, 15f, Mathf.Pow(UnityEngine.Random.value, 2f)), j));
        }
        lastSomethingModes = new bool[room.game.cameras.Length];
        worms = new List<VoidWorm>();
        int num = 5;
        for (int k = 0; k < num; k++)
        {
            float f2 = (float)k / (float)(num - 1);
            worms.Add(new VoidWorm(this, default(Vector2), Mathf.Lerp(1f, 12f, Mathf.Pow(f2, 2f)), k == 0));
            AddElement(worms[worms.Count - 1]);
        }
        num = 10;
        for (int l = 0; l < num; l++)
        {
            float f3 = (float)l / (float)(num - 1);
            worms.Add(new VoidWorm(this, default(Vector2), Mathf.Lerp(8f, 50f, Mathf.Pow(f3, 0.75f)), mainWorm: false));
            AddElement(worms[worms.Count - 1]);
        }
        int num2 = 150;
        for (int m = 0; m < num2; m++)
        {
            float t = (float)m / (float)(num2 - 1);
            float num3 = Mathf.Lerp(1f / 33f, 0.0010204081f, t);
            float depth = Mathf.Lerp(1f / num3, Mathf.Lerp(33f, 980f, t), 0.85f);
            AddElement(new DistantWormLight(this, depth, m));
        }
        if (Inverted)
        {
            storedJourneyIllustration = new SaintsJourneyIllustration(9, this, new Vector2(683f, 384f));
            AddElement(storedJourneyIllustration);
        }
        room.game.cameras[0].somethingGoldFilter = 1f;
        wooshLoop = new DisembodiedDynamicSoundLoop(this);
        wooshLoop.sound = SoundID.Void_Sea_Worm_Swimby_Woosh_LOOP;
        wooshLoop.VolumeGroup = 1;
        wooshLoop.Volume = 0f;
        wormsLoop = new DisembodiedDynamicSoundLoop(this);
        wormsLoop.sound = SoundID.Void_Sea_Worms_Bkg_LOOP;
        wormsLoop.VolumeGroup = 1;
        wormsLoop.Volume = 0f;
        swimLoop = new DisembodiedDynamicSoundLoop(this);
        swimLoop.sound = SoundID.Void_Sea_Swim_LOOP;
        swimLoop.VolumeGroup = 1;
        swimLoop.Volume = 0f;
        Custom.Log("void sea spawning");
    }

    public override void Update(bool eu)
    {
        if (!playerInRoom && room.game.cameras[0].room == room && room.game.cameras[0].currentCameraPosition != 0)
        {
            Custom.Log("INIT VOID SEA");
            playerInRoom = true;
            for (int i = 0; i < room.world.NumberOfRooms; i++)
            {
                if (room.world.firstRoomIndex + i != room.abstractRoom.index)
                {
                    for (int num = room.world.GetAbstractRoom(room.world.firstRoomIndex + i).entities.Count - 1; num >= 0; num--)
                    {
                        room.world.GetAbstractRoom(room.world.firstRoomIndex + i).entities[num].Destroy();
                    }
                    for (int num2 = room.world.GetAbstractRoom(room.world.firstRoomIndex + i).entitiesInDens.Count - 1; num2 >= 0; num2--)
                    {
                        room.world.GetAbstractRoom(room.world.firstRoomIndex + i).entitiesInDens[num2].Destroy();
                    }
                }
            }
        }
        if (ModManager.MSC && fadeOutLights)
        {
            for (int j = 0; j < worms.Count; j++)
            {
                worms[j].lightAlpha = Mathf.Max(0f, worms[j].lightAlpha - 0.001f);
            }
            for (int k = 0; k < elements.Count; k++)
            {
                if (elements[k] is DistantWormLight)
                {
                    (elements[k] as DistantWormLight).alpha = Mathf.Max(0f, (elements[k] as DistantWormLight).alpha - 0.001f);
                }
            }
            Player player = room.game.FirstRealizedPlayer;
            if (ModManager.CoopAvailable)
            {
                player = room.game.RealizedPlayerFollowedByCamera;
            }
            if (player != null)
            {
                fadeOutLightsTimer++;
                if (fadeOutLightsTimer == 1)
                {
                    DestroyCeiling();
                }
                if (fadeOutLightsTimer == 1000)
                {
                    DestroyAllWormsExceptMainWorm();
                }
                if (fadeOutLightsTimer >= 1000)
                {
                    ArtificerEndUpdate(player, fadeOutLightsTimer - 1000);
                }
                else
                {
                    musicFadeFac = (float)fadeOutLightsTimer / 1000f;
                }
            }
        }
        bool flag = false;
        if (!ModManager.CoopAvailable)
        {
            for (int l = 0; l < room.game.Players.Count; l++)
            {
                if (room.game.Players[l].realizedCreature == null || room.game.Players[l].realizedCreature.room != room)
                {
                    continue;
                }
                flag = true;
                playerY = room.game.Players[l].realizedCreature.mainBodyChunk.pos.y;
                (room.game.Players[l].realizedCreature as Player).inSomething = (playerY < sceneOrigo.y && !Inverted) || (playerY > sceneOrigo.y && Inverted);
                if (!playerDipped && room.game.Players[l].realizedCreature.Submersion > 0.5f)
                {
                    if (room.game.manager.musicPlayer != null)
                    {
                        room.game.manager.musicPlayer.RequestSomethingMusic(this);
                    }
                    if ((room.game.Players[0].realizedCreature as Player).redsIllness != null)
                    {
                        (room.game.Players[0].realizedCreature as Player).redsIllness.GetBetter();
                    }
                    if ((room.game.Players[0].realizedCreature as Player).Malnourished)
                    {
                        (room.game.Players[0].realizedCreature as Player).SetMalnourished(m: false);
                    }
                    playerDipped = true;
                }
                break;
            }
        }
        else
        {
            bool dead = (room.game.Players[0].state as PlayerState).dead;
            foreach (AbstractCreature alivePlayer in room.game.AlivePlayers)
            {
                if (alivePlayer.realizedCreature == null)
                {
                    continue;
                }
                Player player2 = alivePlayer.realizedCreature as Player;
                if (player2.room != room)
                {
                    continue;
                }
                flag = true;
                if (player2 != room.game.FirstRealizedPlayer && !dead)
                {
                    continue;
                }
                playerY = player2.mainBodyChunk.pos.y;
                player2.inSomething = (playerY < sceneOrigo.y && !Inverted) || (playerY > sceneOrigo.y && Inverted);
                if (!playerDipped && !(player2.Submersion <= 0.5f))
                {
                    if (room.game.manager.musicPlayer != null)
                    {
                        room.game.manager.musicPlayer.RequestSomethingMusic(this);
                    }
                    playerDipped = true;
                    room.game.cameras[0].EnterCutsceneMode(player2.abstractCreature, RoomCamera.CameraCutsceneType.Something);
                    break;
                }
            }
            if (playerDipped)
            {
                foreach (Player item in (from x in room.game.NonPermaDeadPlayers
                    select (Player)x.realizedCreature into y
                    where y != null
                    select y).ToList())
                {
                    if (item.redsIllness != null)
                    {
                        item.redsIllness.GetBetter();
                    }
                    if (item.Malnourished)
                    {
                        item.SetMalnourished(m: false);
                    }
                }
            }
        }
        if (!flag || !room.game.cameras[0].InCutscene)
        {
            playerDipped = false;
        }
        if (room.game.rainWorld.skipSomething && playerDipped && playerY < 1045f && room.game.manager.upcomingProcess == null)
        {
            room.game.ExitToSomethingSlideShow();
            Custom.Log("SKIP VOID");
        }
        if (!secondSpace)
        {
            float a = 0f;
            float num3 = float.MaxValue;
            if (Inverted)
            {
                room.game.cameras[0].virtualMicrophone.volumeGroups[2] = Mathf.InverseLerp(16000f, 40000f, room.game.cameras[0].pos.y) * (1f - musicFadeFac);
            }
            else
            {
                room.game.cameras[0].virtualMicrophone.volumeGroups[2] = Mathf.InverseLerp(-40000f, -16000f, room.game.cameras[0].pos.y) * (1f - musicFadeFac);
            }
            a = Mathf.Max(a, Mathf.InverseLerp(0f, 0.9f, room.game.cameras[0].screenShake));
            if (!secondSpace)
            {
                num3 = Mathf.Min(num3, Mathf.Abs(room.game.cameras[0].pos.y - voidWormsAltitude));
            }
            if (wooshLoop.Volume < a)
            {
                wooshLoop.Volume = Custom.LerpAndTick(wooshLoop.Volume * (1f - musicFadeFac), a, 0.07f, 0.05f);
            }
            else
            {
                wooshLoop.Volume = Custom.LerpAndTick(wooshLoop.Volume * (1f - musicFadeFac), a, 0.006f, 1f / 60f);
            }
            wooshLoop.Update();
            wormsLoop.Volume = Mathf.Pow(Custom.SCurve(Mathf.InverseLerp(8000f, 2000f, num3), 0.6f), 0.75f) * (1f - musicFadeFac);
            wormsLoop.Update();
        }
        else
        {
            room.game.cameras[0].virtualMicrophone.volumeGroups[2] = 0f;
            if (eggLoop != null && theEgg != null)
            {
                eggLoop.Volume = Mathf.Pow(Custom.SCurve(Mathf.InverseLerp(0.78f, 0.98f, eggProximity), 0.5f), 1.7f) * (1f - theEgg.whiteFade);
                eggLoop.Update();
            }
        }
        if (playerGhosts != null)
        {
            playerGhosts.Update();
        }
        base.Update(eu);
        if (ModManager.MSC)
        {
            for (int m = 0; m < room.abstractRoom.creatures.Count; m++)
            {
                if (room.abstractRoom.creatures[m].realizedCreature != null && room.abstractRoom.creatures[m].creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
                {
                    SomethingTreatment(room.abstractRoom.creatures[m].realizedCreature as Player, 0.95f);
                }
            }
        }
        for (int n = 0; n < room.game.Players.Count; n++)
        {
            AbstractCreature abstractCreature = room.game.Players[n];
            if (abstractCreature.Room.index == room.abstractRoom.index && abstractCreature.realizedCreature != null)
            {
                if (!ModManager.CoopAvailable)
                {
                    UpdatePlayerInSomething(abstractCreature.realizedCreature as Player);
                }
                else if (abstractCreature == room.game.cameras[0].followAbstractCreature)
                {
                    UpdatePlayerInSomething(abstractCreature.realizedCreature as Player);
                }
                else
                {
                    SomethingTreatment(abstractCreature.realizedCreature as Player, 0.95f);
                }
            }
        }
        Player player3 = room.game.FirstRealizedPlayer;
        if (ModManager.CoopAvailable)
        {
            player3 = room.game.RealizedPlayerFollowedByCamera;
        }
        if (player3 != null)
        {
            swimLoop.Update();
            float val = Vector2.Distance(player3.bodyChunks[1].lastPos, player3.bodyChunks[1].pos) + Vector2.Distance(player3.bodyChunks[0].lastPos, player3.bodyChunks[0].pos);
            swimLoop.Volume = Custom.LerpMap(val, 0f, 8f, 0.3f, 1f);
            swimLoop.Pitch = Custom.LerpMap(val, 0f, 7f, 0.95f, 1.05f);
        }
        if (ModManager.MSC && room.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Saint)
        {
            if (storedJourneyIllustration != null)
            {
                storedJourneyIllustration.fadeCounter *= 0.9f;
            }
            SaintEndUpdate();
        }
        if (!ModManager.MMF)
        {
            return;
        }
        for (int num4 = 0; num4 < meltingObjects.Count; num4++)
        {
            if (!MeltingItem.Valid(meltingObjects[num4].meltingObject, room))
            {
                Custom.Log("remove invalidated melt");
                meltingObjects.RemoveAt(num4);
            }
            else
            {
                meltingObjects[num4].Update();
            }
        }
    }

    public void UpdatePlayerInSomething(Player somethingPlayer)
    {
        SomethingTreatment(somethingPlayer, 0.95f);
        bool flag = somethingPlayer.mainBodyChunk.pos.y < 240f && somethingPlayer.mainBodyChunk.pos.y > -40f;
        if (Inverted)
        {
            flag = somethingPlayer.mainBodyChunk.pos.y > room.PixelHeight + 100f && somethingPlayer.mainBodyChunk.pos.y < room.PixelHeight + 380f;
        }
        if (!secondSpace && flag)
        {
            float num = 2200f;
            float num2 = 2900f;
            if (Inverted)
            {
                num = 0f;
                num2 = room.PixelWidth;
            }
            if (somethingPlayer.mainBodyChunk.pos.x < num)
            {
                Move(somethingPlayer, new Vector2((num + num2) * 0.5f - somethingPlayer.mainBodyChunk.pos.x, 0f), moveCamera: true);
            }
            else if (somethingPlayer.mainBodyChunk.pos.x > num2)
            {
                Move(somethingPlayer, new Vector2((num + num2) * 0.5f - somethingPlayer.mainBodyChunk.pos.x, 0f), moveCamera: true);
            }
        }
        if ((int)deepDivePhase >= (int)DeepDivePhase.EggScenario && somethingPlayer.mainBodyChunk.pos.y > -10000f)
        {
            Custom.Log("second space mov");
            Vector2 vector = theEgg.pos - somethingPlayer.mainBodyChunk.pos;
            Move(somethingPlayer, new Vector2(0f, -11000f - somethingPlayer.mainBodyChunk.pos.x), moveCamera: true);
            theEgg.pos = somethingPlayer.mainBodyChunk.pos + vector;
        }
        for (int i = 0; i < room.game.cameras.Length; i++)
        {
            if (room.game.cameras[i].followAbstractCreature == somethingPlayer.abstractCreature)
            {
                room.game.cameras[i].somethingMode = (somethingPlayer.mainBodyChunk.pos.y < 240f && !Inverted) || (Inverted && somethingPlayer.mainBodyChunk.pos.y > room.PixelHeight + 100f);
                if (room.game.cameras[i].somethingMode && !lastSomethingModes[i])
                {
                    cameraOffset *= 0f;
                    room.game.cameras[i].pos = somethingPlayer.mainBodyChunk.pos - new Vector2(700f, 400f);
                    room.game.cameras[i].lastPos = room.game.cameras[i].pos;
                }
                lastSomethingModes[i] = room.game.cameras[i].somethingMode;
            }
        }
        if (deepDivePhase == DeepDivePhase.EggScenario && eggScenarioTimer < 2800 && (somethingPlayer.input[0].x != 0 || somethingPlayer.input[0].y != 0))
        {
            eggScenarioTimer++;
        }
    }

    public void UpdatePlayersJolly()
    {
        for (int i = 0; i < room.game.Players.Count; i++)
        {
            if (room.game.Players[i].Room.index != room.abstractRoom.index || room.game.Players[i].realizedCreature == null)
            {
                continue;
            }
            SomethingTreatment(room.game.Players[i].realizedCreature as Player, 0.95f);
            bool flag = room.game.Players[i].realizedCreature.mainBodyChunk.pos.y < 240f && room.game.Players[i].realizedCreature.mainBodyChunk.pos.y > -40f;
            if (Inverted)
            {
                flag = room.game.Players[i].realizedCreature.mainBodyChunk.pos.y > room.PixelHeight + 100f && room.game.Players[i].realizedCreature.mainBodyChunk.pos.y < room.PixelHeight + 380f;
            }
            if (!secondSpace && flag)
            {
                float num = 2200f;
                float num2 = 2900f;
                if (Inverted)
                {
                    num = 0f;
                    num2 = room.PixelWidth;
                }
                if (room.game.Players[i].realizedCreature.mainBodyChunk.pos.x < num)
                {
                    Move(room.game.Players[i].realizedCreature as Player, new Vector2((num + num2) * 0.5f - room.game.Players[i].realizedCreature.mainBodyChunk.pos.x, 0f), moveCamera: true);
                }
                else if (room.game.Players[i].realizedCreature.mainBodyChunk.pos.x > num2)
                {
                    Move(room.game.Players[i].realizedCreature as Player, new Vector2((num + num2) * 0.5f - room.game.Players[i].realizedCreature.mainBodyChunk.pos.x, 0f), moveCamera: true);
                }
            }
            if ((int)deepDivePhase >= (int)DeepDivePhase.EggScenario && room.game.Players[i].realizedCreature.mainBodyChunk.pos.y > -10000f)
            {
                Custom.Log("second space mov");
                Vector2 vector = theEgg.pos - room.game.Players[i].realizedCreature.mainBodyChunk.pos;
                Move(room.game.Players[i].realizedCreature as Player, new Vector2(0f, -11000f - room.game.Players[i].realizedCreature.mainBodyChunk.pos.x), moveCamera: true);
                theEgg.pos = room.game.Players[i].realizedCreature.mainBodyChunk.pos + vector;
            }
            for (int j = 0; j < room.game.cameras.Length; j++)
            {
                if (room.game.cameras[j].followAbstractCreature == room.game.Players[i])
                {
                    room.game.cameras[j].somethingMode = (room.game.Players[i].realizedCreature.mainBodyChunk.pos.y < 240f && !Inverted) || (Inverted && room.game.Players[i].realizedCreature.mainBodyChunk.pos.y > room.PixelHeight + 100f);
                    if (room.game.cameras[j].somethingMode && !lastSomethingModes[j])
                    {
                        cameraOffset *= 0f;
                        room.game.cameras[j].pos = room.game.Players[i].realizedCreature.mainBodyChunk.pos - new Vector2(700f, 400f);
                        room.game.cameras[j].lastPos = room.game.cameras[j].pos;
                    }
                    lastSomethingModes[j] = room.game.cameras[j].somethingMode;
                }
            }
            AbstractCreature firstAlivePlayer = room.game.FirstAlivePlayer;
            if (deepDivePhase == DeepDivePhase.EggScenario && firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null && eggScenarioTimer < 2800 && ((firstAlivePlayer.realizedCreature as Player).input[0].x != 0 || (firstAlivePlayer.realizedCreature as Player).input[0].y != 0))
            {
                eggScenarioTimer++;
            }
            break;
        }
    }

    public void SomethingTreatment(Player player, float swimSpeed)
    {
        if (player.room != room)
        {
            return;
        }
        for (int i = 0; i < player.bodyChunks.Length; i++)
        {
            player.bodyChunks[i].restrictInRoomRange = float.MaxValue;
            player.bodyChunks[i].vel *= Mathf.Lerp(swimSpeed, 1f, room.game.cameras[0].somethingGoldFilter);
            if (Inverted)
            {
                player.bodyChunks[i].vel.y += player.buoyancy;
                player.bodyChunks[i].vel.y -= player.gravity;
            }
            else
            {
                player.bodyChunks[i].vel.y -= player.buoyancy;
                player.bodyChunks[i].vel.y += player.gravity;
            }
        }
        if (!ModManager.MSC || saintEndPhase != SaintEndingPhase.EchoTransform)
        {
            player.airInLungs = 1f;
            player.lungsExhausted = false;
        }
        if (player.graphicsModule != null && (player.graphicsModule as PlayerGraphics).lightSource != null)
        {
            if (Inverted)
            {
                (player.graphicsModule as PlayerGraphics).lightSource.setAlpha = Custom.LerpMap(player.mainBodyChunk.pos.y, 2000f, 8000f, 1f, 0.2f) * (1f - eggProximity);
                (player.graphicsModule as PlayerGraphics).lightSource.setRad = Custom.LerpMap(player.mainBodyChunk.pos.y, 2000f, 8000f, 300f, 200f) * (0.5f + 0.5f * (1f - eggProximity));
            }
            else
            {
                (player.graphicsModule as PlayerGraphics).lightSource.setAlpha = Custom.LerpMap(player.mainBodyChunk.pos.y, -2000f, -8000f, 1f, 0.2f) * (1f - eggProximity);
                (player.graphicsModule as PlayerGraphics).lightSource.setRad = Custom.LerpMap(player.mainBodyChunk.pos.y, -2000f, -8000f, 300f, 200f) * (0.5f + 0.5f * (1f - eggProximity));
            }
        }
        if (deepDivePhase == DeepDivePhase.EggScenario && UnityEngine.Random.value < 0.1f)
        {
            player.mainBodyChunk.vel += Custom.DirVec(player.mainBodyChunk.pos, theEgg.pos) * 0.02f * UnityEngine.Random.value;
        }
        if (ModManager.MMF && player.Submersion > 0.5f)
        {
            if (player.grasps[0] != null && !(player.grasps[0].grabbed is Creature))
            {
                AddMeltObject(player.grasps[0].grabbed);
            }
            if (player.grasps[1] != null && !(player.grasps[1].grabbed is Creature))
            {
                AddMeltObject(player.grasps[1].grabbed);
            }
            if (player.spearOnBack != null && player.spearOnBack.HasASpear)
            {
                player.spearOnBack.DropSpear();
                AddMeltObject(player.spearOnBack.spear);
            }
        }
    }

    public void Move(Player player, Vector2 move, bool moveCamera)
    {
        if (moveCamera)
        {
            cameraOffset -= move;
        }
        for (int i = 0; i < player.bodyChunks.Length; i++)
        {
            player.bodyChunks[i].pos += move;
            player.bodyChunks[i].lastPos += move;
            player.bodyChunks[i].lastLastPos += move;
        }
        if (player.graphicsModule != null)
        {
            for (int j = 0; j < player.graphicsModule.bodyParts.Length; j++)
            {
                player.graphicsModule.bodyParts[j].pos += move;
                player.graphicsModule.bodyParts[j].lastPos += move;
            }
            if ((player.graphicsModule as PlayerGraphics).lightSource != null)
            {
                (player.graphicsModule as PlayerGraphics).lightSource.HardSetPos((player.graphicsModule as PlayerGraphics).lightSource.Pos + move);
            }
            for (int k = 0; k < (player.graphicsModule as PlayerGraphics).drawPositions.GetLength(0); k++)
            {
                (player.graphicsModule as PlayerGraphics).drawPositions[k, 0] += move;
                (player.graphicsModule as PlayerGraphics).drawPositions[k, 1] += move;
            }
        }
        if (!moveCamera)
        {
            return;
        }
        for (int l = 0; l < room.game.cameras.Length; l++)
        {
            if (room.game.cameras[l].followAbstractCreature == player.abstractCreature)
            {
                room.game.cameras[l].pos += move;
                room.game.cameras[l].lastPos += move;
            }
        }
    }

    public void DestroyAllWormsExceptMainWorm()
    {
        Custom.Log("DESTROY CLOSE WORMS");
        deepDivePhase = new DeepDivePhase(ExtEnum<DeepDivePhase>.values.GetEntry(Math.Max(deepDivePhase.Index, DeepDivePhase.CloseWormsDestroyed.Index)));
        for (int num = worms.Count - 1; num >= 0; num--)
        {
            if (!worms[num].mainWorm)
            {
                worms[num].Destroy();
                worms.RemoveAt(num);
            }
        }
        for (int i = 0; i < room.game.Players.Count; i++)
        {
            if (room.game.Players[i].realizedCreature != null && room.game.Players[i].realizedCreature.room == room)
            {
                if ((room.game.Players[i].realizedCreature as Player).spearOnBack != null)
                {
                    (room.game.Players[i].realizedCreature as Player).spearOnBack.DropSpear();
                }
                if (!ModManager.MSC || (room.game.Players[i].realizedCreature as Player).SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Saint)
                {
                    (room.game.Players[i].realizedCreature as Player).objectInStomach = null;
                }
            }
        }
        for (int num2 = elements.Count - 1; num2 >= 0; num2--)
        {
            if (elements[num2] is VoidWorm && elements[num2].slatedForDeletetion)
            {
                elements.RemoveAt(num2);
            }
        }
    }

    public void DestroyCeiling()
    {
        Custom.Log("DESTROY CEILING");
        deepDivePhase = new DeepDivePhase(ExtEnum<DeepDivePhase>.values.GetEntry(Math.Max(deepDivePhase.Index, DeepDivePhase.CeilingDestroyed.Index)));
        for (int num = elements.Count - 1; num >= 0; num--)
        {
            if (elements[num] is VoidCeiling)
            {
                elements[num].Destroy();
                elements.RemoveAt(num);
            }
        }
    }

    public void DestroyDistantWorms()
    {
        Custom.Log("DESTROY DISTANT WORMS");
        deepDivePhase = new DeepDivePhase(ExtEnum<DeepDivePhase>.values.GetEntry(Math.Max(deepDivePhase.Index, DeepDivePhase.DistantWormsDestroyed.Index)));
        for (int num = elements.Count - 1; num >= 0; num--)
        {
            if (elements[num] is DistantWormLight)
            {
                elements[num].Destroy();
                elements.RemoveAt(num);
            }
        }
    }

    public void MovedToSecondSpace()
    {
        Player player = room.game.FirstRealizedPlayer;
        if (ModManager.CoopAvailable)
        {
            player = room.game.RealizedPlayerFollowedByCamera;
        }
        if (player != null)
        {
            Custom.Log("SECOND SPACE");
            secondSpace = true;
            deepDivePhase = new DeepDivePhase(ExtEnum<DeepDivePhase>.values.GetEntry(Math.Max(deepDivePhase.Index, DeepDivePhase.MovedIntoSecondSpace.Index)));
            float y = -5000f - player.mainBodyChunk.pos.y;
            if (Inverted)
            {
                y = 5000f - player.mainBodyChunk.pos.y;
            }
            Move(player, new Vector2(0f, y), moveCamera: true);
            if (wooshLoop.emitter != null)
            {
                wooshLoop.emitter.Destroy();
            }
            wooshLoop = null;
            if (wormsLoop.emitter != null)
            {
                wormsLoop.emitter.Destroy();
            }
            wormsLoop = null;
            worms[0].Move(new Vector2(0f, y));
            eggLoop = new DisembodiedDynamicSoundLoop(this);
            eggLoop.sound = SoundID.Void_Sea_The_Core_LOOP;
            eggLoop.VolumeGroup = 1;
            eggLoop.Volume = 0f;
        }
    }

    public void DestroyMainWorm()
    {
        Custom.Log("DESTROY MAIN WORM");
        deepDivePhase = new DeepDivePhase(ExtEnum<DeepDivePhase>.values.GetEntry(Math.Max(deepDivePhase.Index, DeepDivePhase.EggScenario.Index)));
        DeleteMainWorm();
        Player player = room.game.FirstRealizedPlayer;
        if (ModManager.CoopAvailable)
        {
            player = room.game.RealizedPlayerFollowedByCamera;
        }
        theEgg.pos = player.mainBodyChunk.pos + Custom.DegToVec(115f) * 11000f;
        playerGhosts = new PlayerGhosts(player, this);
    }

    public void SaintEndUpdate()
    {
        if (room.game.Players.Count == 0)
        {
            return;
        }
        AbstractCreature firstAlivePlayer = room.game.FirstAlivePlayer;
        Player player = null;
        if (firstAlivePlayer != null)
        {
            player = firstAlivePlayer.realizedCreature as Player;
        }
        if (player == null || player.graphicsModule == null)
        {
            return;
        }
        if (saintEndPhase == SaintEndingPhase.Drowned)
        {
            player.airInLungs = 0.04f;
            if (fadeOutSaint < 0f)
            {
                if (!endingSavedFlag)
                {
                    if (room.game.Players.Count > 0 && firstAlivePlayer.realizedCreature != null)
                    {
                        room.game.rainWorld.progression.miscProgressionData.UpdateSaintStomach(firstAlivePlayer.realizedCreature as Player);
                    }
                    RainWorldGame.BeatGameMode(room.game, standardSomething: false);
                    room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "SI_SAINTINTRO");
                }
                endingSavedFlag = true;
            }
            else
            {
                fadeOutSaint -= 1f;
            }
        }
        else
        {
            if (!(saintEndPhase != SaintEndingPhase.Inactive))
            {
                return;
            }
            if (saintEndPhase == SaintEndingPhase.EchoTransform && timeInSaintPhase > 480f)
            {
                (player.graphicsModule as PlayerGraphics).darkenFactor = Mathf.Lerp((player.graphicsModule as PlayerGraphics).darkenFactor, 1f, 0.002f);
            }
            if (saintEndPhase == SaintEndingPhase.EchoTransform && timeInSaintPhase == 120f && room.game.manager.musicPlayer != null && room.game.manager.musicPlayer.song != null)
            {
                room.game.manager.musicPlayer.song.ResetSongStream();
            }
            float num = 1500f;
            float num2 = 240f;
            player.airInLungs = 1f;
            if (timeInSaintPhase > num + num2 * 5f)
            {
                player.airInLungs = Mathf.Clamp(Mathf.InverseLerp(num + num2 * 7f, num + num2 * 5f, timeInSaintPhase), 0.01f, 1f);
                if (player.airInLungs < 0.04f)
                {
                    fadeOutSaint = 500f;
                    saintEndPhase = SaintEndingPhase.Drowned;
                    timeInSaintPhase = 0f;
                    room.AddObject(new FadeOut(room, Color.white, 400f, fadeIn: false));
                }
                if (player.airInLungs < 0.03f)
                {
                    player.airInLungs = 0.03f;
                }
            }
            else if (timeInSaintPhase > num)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (timeInSaintPhase > num + num2 * (float)i && (player.graphicsModule as PlayerGraphics).tentaclesVisible < i + 1)
                    {
                        (player.graphicsModule as PlayerGraphics).tentacles[(player.graphicsModule as PlayerGraphics).tentaclesVisible].SetPosition(player.firstChunk.pos);
                        (player.graphicsModule as PlayerGraphics).tentaclesVisible++;
                        player.mainBodyChunk.vel += Custom.RNV() * 50f;
                        for (int j = 0; j < 20; j++)
                        {
                            float value = UnityEngine.Random.value;
                            Spark spark = new Spark(player.mainBodyChunk.pos, Custom.RNV() * value * 5f, new Color(0.01f, 0.01f, 0.01f), null, 10 + (int)((1f - value) * 60f), 36 + (int)((1f - value) * 200f));
                            spark.gravity = 0f;
                            room.AddObject(spark);
                        }
                    }
                }
            }
            if ((player.graphicsModule as PlayerGraphics).darkenFactor > 0.35f && UnityEngine.Random.value < (player.graphicsModule as PlayerGraphics).darkenFactor * 0.25f)
            {
                for (int k = 0; k < 4; k++)
                {
                    float value2 = UnityEngine.Random.value;
                    Spark spark2 = new Spark(player.mainBodyChunk.pos, Custom.RNV() * value2, new Color(0.01f, 0.01f, 0.01f), null, 4 + (int)((1f - value2) * 20f), 18 + (int)((1f - value2) * 100f));
                    spark2.gravity = 0f;
                    room.AddObject(spark2);
                }
            }
            timeInSaintPhase += 1f;
        }
    }

    public void switchSaintEndPhase(SaintEndingPhase phase)
    {
        saintEndPhase = phase;
        timeInSaintPhase = 0f;
    }

    public void ArtificerEndUpdate(Player player, int timer)
    {
        if (timer == 720)
        {
            player.InitiateDissolve();
        }
        int num = 960;
        int num2 = 1400;
        if (timer > num && timer < num + num2)
        {
            if (eggLoop == null)
            {
                eggLoop = new DisembodiedDynamicSoundLoop(this);
                eggLoop.sound = SoundID.Void_Sea_The_Core_LOOP;
                eggLoop.VolumeGroup = 1;
                theEgg = null;
            }
            eggLoop.Volume = Mathf.Max(0f, (float)(timer - num) / (float)num2 * 0.9f);
            eggLoop.Update();
            player.dissolved = Mathf.Max(0.01f, (float)(timer - num) / (float)num2 * 0.4f);
        }
        else if (timer >= num + num2)
        {
            if (blackFade == null)
            {
                blackFade = new FadeOut(room, Color.black, 200f, fadeIn: false);
                room.AddObject(blackFade);
            }
            eggLoop.Volume = 0.9f - Mathf.Max(0f, (float)(timer - (num + num2)) / 200f * 0.9f);
            eggLoop.Update();
        }
        if (blackFade != null && blackFade.IsDoneFading())
        {
            if (!endingSavedFlag)
            {
                room.game.ExitToSomethingSlideShow();
            }
            endingSavedFlag = true;
        }
    }

    public void DeleteMainWorm()
    {
        worms[0].Destroy();
        worms.Clear();
        for (int num = elements.Count - 1; num >= 0; num--)
        {
            if (elements[num] is VoidWorm)
            {
                elements.RemoveAt(num);
            }
        }
    }

    public void AddMeltObject(PhysicalObject obj)
    {
        bool flag = true;
        foreach (MeltingItem meltingObject in meltingObjects)
        {
            if (meltingObject != null && MeltingItem.Valid(meltingObject.meltingObject, room) && meltingObject.meltingObject.abstractPhysicalObject == obj.abstractPhysicalObject)
            {
                flag = false;
                break;
            }
        }
        if (flag && MeltingItem.Valid(obj, room))
        {
            Custom.Log($"Add melt object {obj}");
            meltingObjects.Add(new MeltingItem(obj, this));
        }
    }
}
*/