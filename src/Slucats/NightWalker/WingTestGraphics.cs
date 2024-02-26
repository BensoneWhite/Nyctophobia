using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nyctophobia
{
    internal class WingTestGraphics : PlayerGraphics
    {
        public class WingPart
        {
            public TriangleMesh mesh;

            private FSprite blankSprite;

            public WingTestGraphics owner;

            public float length;

            public float width;

            public Vector2 targetFarPos;

            public Vector2 targetClosePos;

            public WingPart(WingTestGraphics owner, bool isLarge = false)
            {
                this.owner = owner;
                if (isLarge)
                {
                    length = 25f;
                    width = 3f;
                }
                else
                {
                    length = 15f;
                    width = 2f;
                }
            }

            public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                if (mesh != null)
                {
                    mesh.RemoveFromContainer();
                }
                TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[6]
                {
                new TriangleMesh.Triangle(0, 1, 2),
                new TriangleMesh.Triangle(1, 2, 3),
                new TriangleMesh.Triangle(2, 3, 4),
                new TriangleMesh.Triangle(3, 4, 5),
                new TriangleMesh.Triangle(4, 5, 6),
                new TriangleMesh.Triangle(5, 6, 7)
                };
                if (blankSprite != null)
                {
                    blankSprite.RemoveFromContainer();
                }
                blankSprite = new FSprite("Futile_White");
                mesh = new TriangleMesh("Futile_White", tris, customColor: true);
            }

            public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
            {
                if (newContatiner == null)
                {
                    newContatiner = rCam.ReturnFContainer("Midground");
                }
                mesh.RemoveFromContainer();
                newContatiner.AddChild(mesh);
                blankSprite.RemoveFromContainer();
                newContatiner.AddChild(blankSprite);
            }

            public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, int index)
            {
                float num = ((index % 2 == 0) ? 1 : (-1));
                float num2 = (float)(index / 2) / (float)(owner.wingPartCount / 2);
                BodyChunk bodyChunk = owner.owner.bodyChunks[0];
                BodyChunk bodyChunk2 = owner.owner.bodyChunks[1];
                Vector2 val = Vector2.Lerp(Vector2.Lerp(bodyChunk.lastPos, bodyChunk.pos, timeStacker), Vector2.Lerp(bodyChunk2.lastPos, bodyChunk2.pos, timeStacker), 0.5f - Mathf.Lerp(0f, 0.3f, num2));
                float num3 = Custom.VecToDeg(Vector2.Lerp(bodyChunk.lastPos, bodyChunk.pos, timeStacker) - val);
                num3 += (float)(index / 2) * 25f * num;
                Vector2 val2 = Vector2.Lerp(owner.head.lastPos, owner.head.pos, timeStacker) + Custom.DegToVec(num3 + 80f * num) * 10f;
                Vector2 val3 = Vector2.Lerp(owner.head.lastPos, owner.head.pos, timeStacker) + Custom.DegToVec(num3 + 80f * num) * (10f + length);
                targetClosePos = Vector2.Lerp(targetClosePos, val2, Mathf.Lerp(0.7f, 0.4f, num2));
                targetFarPos = Vector2.Lerp(targetFarPos, val3, Mathf.Lerp(0.7f, 0.4f, num2));
                Vector2 val4 = Custom.PerpendicularVector(targetClosePos, targetFarPos);
                for (int i = 0; i < 3; i++)
                {
                    mesh.MoveVertice(i * 2 + 1, Vector2.Lerp(targetClosePos, targetFarPos, ((float)i + 1f) / 4f) + val4 * width - camPos);
                    mesh.MoveVertice(i * 2 + 2, Vector2.Lerp(targetClosePos, targetFarPos, ((float)i + 1f) / 4f) - val4 * width - camPos);
                }
                mesh.MoveVertice(0, targetClosePos - camPos);
                mesh.MoveVertice(7, targetFarPos - camPos);
            }
        }

        public int wingPartCount = 3;

        public List<WingPart> wingParts;

        public WingTestGraphics(PhysicalObject ow)
            : base(ow)
        {
            wingParts = new List<WingPart>();
            for (int i = 0; i < wingPartCount; i++)
            {
                wingParts.Add(new WingPart(this, i == 0));
                wingParts.Add(new WingPart(this, i == 0));
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            foreach (WingPart wingPart in wingParts)
            {
                wingPart.InitiateSprites(sLeaser, rCam);
            }
            base.InitiateSprites(sLeaser, rCam);
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            foreach (WingPart wingPart in wingParts)
            {
                wingPart.AddToContainer(sLeaser, rCam, newContatiner);
            }
            base.AddToContainer(sLeaser, rCam, newContatiner);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            //IL_0029: Unknown result type (might be due to invalid IL or missing references)
            //IL_0043: Unknown result type (might be due to invalid IL or missing references)
            SlugcatHand[] array = hands;
            foreach (SlugcatHand obj in array)
            {
                obj.mode = Limb.Mode.Retracted;
                obj.retractCounter = 0;
            }
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            for (int j = 0; j < wingParts.Count; j++)
            {
                wingParts[j].DrawSprites(sLeaser, rCam, timeStacker, camPos, j);
            }
            for (int k = 0; k < 2; k++)
            {
                sLeaser.sprites[7 + k].isVisible = false;
            }
        }
    }
}