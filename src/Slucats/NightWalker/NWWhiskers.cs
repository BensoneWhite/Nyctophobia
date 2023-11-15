using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RWCustom;

namespace Witness;

internal class NWwhiskers
{

    public static ConditionalWeakTable<Player, Whiskerdata> whiskerstorage = new ConditionalWeakTable<Player, Whiskerdata>();

    public class Whiskerdata
    {
        public bool ready = false;
        public int initialfacewhiskerloc;
        public string sprite = "LizardScaleA0";
        public string facesprite = "LizardScaleA0";
        public WeakReference<Player> playerref;
        public Whiskerdata(Player player) => playerref = new WeakReference<Player>(player);
        public Vector2[] headpositions = new Vector2[6];
        public Scale[] headScales = new Scale[6];
        public class Scale : BodyPart
        {
            public Scale(GraphicsModule cosmetics) : base(cosmetics)
            {
            }

            public override void Update()
            {
                base.Update();
                if (this.owner.owner.room.PointSubmerged(this.pos))
                {
                    this.vel *= 0.5f;
                }
                else
                {
                    this.vel *= 0.9f;
                }
                this.lastPos = this.pos;
                this.pos += this.vel;
            }
            public float length = 7f;
            public float width = 2f;
        }
        public Color headcolor = new Color(1f, 1f, 0f);

        public int facewhiskersprite(int side, int pair) => initialfacewhiskerloc + side + pair + pair;
    }

    public static void Init()
    {
        On.PlayerGraphics.ctor += PlayerGraphics_ctor;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.Update += PlayerGraphics_Update;
    }

    public static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
    {
        orig(self, ow);

        if ((self.player).slugcatStats.name.value == "NightWalker")
        {
            whiskerstorage.Add(self.player, new Whiskerdata(self.player));
            whiskerstorage.TryGetValue(self.player, out Whiskerdata data);
            for (int i = 0; i < data.headScales.Length; i++)
            {
                data.headScales[i] = new Whiskerdata.Scale(self);
                data.headpositions[i] = new Vector2((i < data.headScales.Length / 2 ? 0.7f : -0.7f), i == 1 ? 0.035f : 0.026f);

            }
        }
    }

    public static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        if ((self.owner as Player).slugcatStats.name.value == "NightWalker")
        {
            whiskerstorage.TryGetValue(self.player, out var thedata);
            thedata.initialfacewhiskerloc = sLeaser.sprites.Length;
            Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 6);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    sLeaser.sprites[thedata.facewhiskersprite(i, j)] = new FSprite(thedata.facesprite);

                    sLeaser.sprites[thedata.facewhiskersprite(i, j)].scaleY = 17f / Futile.atlasManager.GetElementWithName(thedata.sprite).sourcePixelSize.y;
                    sLeaser.sprites[thedata.facewhiskersprite(i, j)].anchorY = 0.1f;
                }
            }
            thedata.ready = true;
            self.AddToContainer(sLeaser, rCam, null);
        }
    }

    public static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        if (((self.player).slugcatStats.name.value == "NightWalker") && whiskerstorage.TryGetValue(self.player, out Whiskerdata data) && data.ready)
        {
            FContainer container = rCam.ReturnFContainer("Midground");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    FSprite whisker = sLeaser.sprites[data.facewhiskersprite(i, j)];
                    container.AddChild(whisker);
                }
            }
            data.ready = false;
        }
    }

    public static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        if (((self.player).slugcatStats.name.value == "NightWalker") && whiskerstorage.TryGetValue(self.player, out Whiskerdata data))
        {
            int index = 0;
            index = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2 vector = new Vector2(sLeaser.sprites[9].x + camPos.x, sLeaser.sprites[9].y + camPos.y);
                    float f = 0f;
                    float num = 0f;
                    if (i == 0)
                    {
                        vector.x -= 5f;
                    }
                    else
                    {
                        num = 180f;
                        vector.x += 5f;
                    }
                    sLeaser.sprites[data.facewhiskersprite(i, j)].x = vector.x - camPos.x;
                    sLeaser.sprites[data.facewhiskersprite(i, j)].y = vector.y - camPos.y;
                    sLeaser.sprites[data.facewhiskersprite(i, j)].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(data.headScales[index].lastPos, data.headScales[index].pos, timeStacker)) + num;
                    sLeaser.sprites[data.facewhiskersprite(i, j)].scaleX = 0.4f * Mathf.Sign(f);
                    sLeaser.sprites[data.facewhiskersprite(i, j)].color = sLeaser.sprites[1].color;
                    index++;
                }
            }
        }
    }

    public static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
        if (((self.player).slugcatStats.name.value == "NightWalker") && whiskerstorage.TryGetValue(self.player, out Whiskerdata data))
        {
            int index = 0;
            index = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2 pos = self.owner.bodyChunks[0].pos;
                    Vector2 pos2 = self.owner.bodyChunks[1].pos;
                    float num = 0f;
                    float num2 = 90f;
                    int num3 = index % (data.headScales.Length / 2);
                    float num4 = num2 / (float)(data.headScales.Length / 2);
                    if (i == 1)
                    {
                        num = 0f;
                        pos.x += 5f;
                    }
                    else
                    {
                        pos.x -= 5f;
                    }
                    Vector2 a = Custom.rotateVectorDeg(Custom.DegToVec(0f), (float)num3 * num4 - num2 / 2f + num + 90f);
                    float f = Custom.VecToDeg(self.lookDirection);
                    Vector2 vector = Custom.rotateVectorDeg(Custom.DegToVec(0f), (float)num3 * num4 - num2 / 2f + num);
                    Vector2 a2 = Vector2.Lerp(vector, Custom.DirVec(pos2, pos), Mathf.Abs(f));
                    if (data.headpositions[index].y < 0.2f)
                    {
                        a2 -= a * Mathf.Pow(Mathf.InverseLerp(0.2f, 0f, data.headpositions[index].y), 2f) * 2f;
                    }
                    a2 = Vector2.Lerp(a2, vector, Mathf.Pow(0.0875f, 1f)).normalized;
                    Vector2 vector2 = pos + a2 * data.headScales.Length;
                    if (!Custom.DistLess(data.headScales[index].pos, vector2, data.headScales[index].length / 2f))
                    {
                        Vector2 a3 = Custom.DirVec(data.headScales[index].pos, vector2);
                        float num5 = Vector2.Distance(data.headScales[index].pos, vector2);
                        float num6 = data.headScales[index].length / 2f;
                        data.headScales[index].pos += a3 * (num5 - num6);
                        data.headScales[index].vel += a3 * (num5 - num6);
                    }
                    data.headScales[index].vel += Vector2.ClampMagnitude(vector2 - data.headScales[index].pos, 10f) / Mathf.Lerp(5f, 1.5f, 0.5873646f);
                    data.headScales[index].vel *= Mathf.Lerp(1f, 0.8f, 0.5873646f);
                    data.headScales[index].ConnectToPoint(pos, data.headScales[index].length, true, 0f, new Vector2(0f, 0f), 0f, 0f);
                    data.headScales[index].Update();
                    index++;
                }
            }
        }
    }
}