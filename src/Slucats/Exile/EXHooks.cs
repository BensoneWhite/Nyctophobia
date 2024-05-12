namespace Nyctophobia;

public static class EXHooks
{
    public static void Init()
    {
        On.PlayerGraphics.ctor += PlayerGraphics_ctor;
        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        On.Player.Update += Player_Update;
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!self.IsExile(out EXPlayerData EX))
        {
            return;
        }

        if (self is null || self.room is null)
        {
            return;
        }

        //InputPackage inputPackage = self.input[0];

        float inputValx = self.input[0].x;
        float inputValy = self.input[0].y;

        Vector2 combinedDirection = new Vector2(inputValx, inputValy).normalized;

        EX.dashDirectionX = combinedDirection;
        EX.dashDirectionY = combinedDirection;

        Vector2 newPosition = self.bodyChunks[0].pos + (combinedDirection * EX.dashDistance);

        Vector2 currentVelocity = newPosition - self.bodyChunks[0].pos;
        Vector2 newVelocity = currentVelocity * 0.9f;

        if ((EX.dashCooldown <= 0 && EX.currentDashes == 3) || EX.DoesThatPlayerDashedOrNoBOZO < 0)
        {
            EX.currentDashes = 0;
            EX.dashCooldown = 0;
        }

        if (Input.GetKey(NTOptionsMenu.Dash.Value) && (inputValx != 0 || inputValy != 0) && EX.dashCooldown <= 0 && EX.currentDashes <= 3)
        {
            float DashDelay = 0.35f;
            EX.dashCooldown = (int)(DashDelay * 40f);

            float MaxDistance = 0.04f;
            EX.maxDashDistance = (int)(MaxDistance * 40f);

            float TIMERISTIMEHOHOHO = 20f;
            EX.DoesThatPlayerDashedOrNoBOZO = (int)(TIMERISTIMEHOHOHO * 40f);

            EX.currentDashes++;

            _ = self.room.PlaySound(SoundID.Slugcat_Flip_Jump, self.mainBodyChunk, false, 1f, 1f);
            _ = self.room.PlaySound(SoundID.Leaves, self.mainBodyChunk, false, 1f, 1f);

            EX.Dashed = true;
        }

        if (EX.Dashed)
        {
            newPosition = self.bodyChunks[0].pos + newVelocity;

            self.bodyChunks[0].pos = newPosition;
            self.bodyChunks[1].pos = newPosition;

            if (inputValx == 0 || EX.dashCooldown <= 0 || EX.maxDashDistance <= 0)
            {
                EX.Dashed = false;
            }
        }

        if (EX.currentDashes >= 3)
        {
            EX.currentDashes = 0;

            float FlashDelay = 5f;
            EX.dashCooldown = (int)(FlashDelay * 40f);
        }

        if (EX.DoesThatPlayerDashedOrNoBOZO > 0)
        {
            EX.DoesThatPlayerDashedOrNoBOZO--;
        }

        if (EX.dashCooldown > 0)
        {
            EX.dashCooldown--;
        }

        if (EX.maxDashDistance > 0)
        {
            EX.maxDashDistance--;
        }
    }

    private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        if (!self.player.IsExile(out _))
        {
            return;
        }

        sLeaser.sprites[2].MoveBehindOtherNode(sLeaser.sprites[1]);
    }

    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        if (!self.player.IsExile(out EXPlayerData ex))
        {
            return;
        }

        if (sLeaser.sprites[2] is TriangleMesh tail && ex.TailAtlas.elements != null && ex.TailAtlas.elements.Count > 0)
        {
            tail.element = ex.TailAtlas.elements[0];

            for (int i = tail.vertices.Length - 1; i >= 0; i--)
            {
                float perc = i / 2 / (float)(tail.vertices.Length / 2);

                Vector2 uv = i % 2 == 0 ? new Vector2(perc, 0f) : i < tail.vertices.Length - 1 ? new Vector2(perc, 1f) : new Vector2(1f, 0f);
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

        if (!self.player.IsExile(out EXPlayerData ex))
        {
            return;
        }

        ex.EXTail(self);
        ex.SetupTailTextureEX();
        ex.SetupColorsEX(self);
    }
}