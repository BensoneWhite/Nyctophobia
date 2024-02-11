namespace Witness
{
    public static class EXRelativeHooks
    {
        
        public static void Init()
        {
            if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
            {
                On.PlayerGraphics.ctor += PlayerGraphics_ctor;
                On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
                On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            }
        }

        private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig(self, sLeaser, rCam, newContatiner);
            if(!self.player.IsExile(out _))
            {
                return;
            }
            sLeaser.sprites[2].MoveBehindOtherNode(sLeaser.sprites[1]);
        }

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (!self.player.IsExile(out var ex)) return;

            if (sLeaser.sprites[2] is TriangleMesh tail && ex.TailAtlas.elements != null && ex.TailAtlas.elements.Count > 0)
            {
                tail.element = ex.TailAtlas.elements[0];

                for (var i = tail.vertices.Length - 1; i >= 0; i--)
                {
                    var perc = i / 2 / (float)(tail.vertices.Length / 2);

                    Vector2 uv;
                    if (i % 2 == 0)
                        uv = new Vector2(perc, 0f);
                    else if (i < tail.vertices.Length - 1)
                        uv = new Vector2(perc, 1f);
                    else
                        uv = new Vector2(1f, 0f);

                    uv.x = Mathf.Lerp(tail.element.uvBottomLeft.x, tail.element.uvTopRight.x, uv.x);
                    uv.y = Mathf.Lerp(tail.element.uvBottomLeft.y, tail.element.uvTopRight.y, uv.y);

                    tail.UVvertices[i] = uv;
                }
            }
            self.AddToContainer(sLeaser, rCam, null);
        }

        private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);

            if (!self.player.IsExile(out var ex))
            {
                return;
            }

            ex.EXTail(self);
            ex.SetupTailTextureEX();
            ex.SetupColorsEX(self);
        }
    }
}
