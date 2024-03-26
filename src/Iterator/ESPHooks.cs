namespace Nyctophobia;

public static class ESPHooks
{
    public static void Apply()
    {
        On.SuperStructureFuses.InitiateSprites += SuperStructureFuses_InitiateSprites;
        On.RainWorldGame.IsMoonActive += RainWorldGame_IsMoonActive;
        On.OracleGraphics.DrawSprites += OracleGraphics_DrawSprites;
        On.OracleGraphics.Gown.Color += Gown_Color;
        On.OracleGraphics.ApplyPalette += OracleGraphics_ApplyPalette;
        On.OracleGraphics.InitiateSprites += OracleGraphics_InitiateSprites;
        On.Oracle.ctor += Oracle_ctor;
        On.Room.ReadyForAI += Room_ReadyForAI;
        //On.Music.MusicPlayer.RequestSSSong += MusicPlayer_RequestSSSong;

        Debug.LogWarning("Applying Iterator Hooks Nyctophobia");
    }

    //private static void MusicPlayer_RequestSSSong(On.Music.MusicPlayer.orig_RequestSSSong orig, Music.MusicPlayer self)
    //{
    //    Song song;
    //    if (self.manager.currentMainLoop is RainWorldGame && (self.manager.currentMainLoop as RainWorldGame).IsStorySession && (self.manager.currentMainLoop as RainWorldGame).world.region.name == "DN")
    //    {
    //        song = new SSSong(self, "MyIteratorSong");
    //        self.nextSong = song;
    //        return;
    //    }
    //    else
    //    {
    //        orig(self);
    //    }
    //}

    private static void Room_ReadyForAI(On.Room.orig_ReadyForAI orig, Room self)
    {
        orig(self);
        if (self.game != null && self.abstractRoom.name == "DD_AI" && ModManager.MSC && self.game.IsStorySession)
        {
            Oracle esp = new(new(self.world, AbstractPhysicalObject.AbstractObjectType.Oracle, null, new WorldCoordinate(self.abstractRoom.index, 15, 15, -1), self.game.GetNewID()), self);
            self.AddObject(esp);
        }
    }

    private static void Oracle_ctor(On.Oracle.orig_ctor orig, Oracle self, AbstractPhysicalObject abstractPhysicalObject, Room room)
    {
        if (room.abstractRoom.name == "DD_AI")
        {
            room.abstractRoom.name = "SS_AI";
            orig(self, abstractPhysicalObject, room);
            self.ID = NTEnums.Iterator.ESP;
            room.abstractRoom.name = "DD_AI";
            self.oracleBehavior = new ESPBehavior(self);
        }
        else orig(self, abstractPhysicalObject, room);
    }

    private static void OracleGraphics_InitiateSprites(On.OracleGraphics.orig_InitiateSprites orig, OracleGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (self.IsESP())
        {
            orig(self, sLeaser, rCam);
            Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 1);
            self.totalSprites++;
            sLeaser.sprites[sLeaser.sprites.Length - 1] = new("Futile_White");
            self.AddToContainer(sLeaser, rCam, null);

        }
        else orig(self, sLeaser, rCam);
    }

    private static void OracleGraphics_ApplyPalette(On.OracleGraphics.orig_ApplyPalette orig, OracleGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (self.IsESP())
        {
            orig(self, sLeaser, rCam, palette);
            Color color = Custom.hexToColor("A9FDAC");

            for (int j = 0; j < self.owner.bodyChunks.Length; j++)
            {
                sLeaser.sprites[self.firstBodyChunkSprite + j].color = color;
            }

            sLeaser.sprites[self.MoonThirdEyeSprite].color = UnityEngine.Color.Lerp(new UnityEngine.Color(0.72156862745f, 0.14509803921f, 0.80392156862f), color, 0.3f);
            sLeaser.sprites[self.MoonSigilSprite].color = new UnityEngine.Color(1f, 0.85098039215f, 0.00735294f);

            sLeaser.sprites[self.neckSprite].color = color;
            sLeaser.sprites[self.HeadSprite].color = color;
            sLeaser.sprites[self.ChinSprite].color = color;
            for (int k = 0; k < 2; k++)
            {
                sLeaser.sprites[self.EyeSprite(k)].color = new Color(0.02f, 0f, 0f);
                if (self.armJointGraphics.Length == 0)
                {
                    //Unknown

                    sLeaser.sprites[self.PhoneSprite(k, 0)].color = self.GenericJointBaseColor();
                    sLeaser.sprites[self.PhoneSprite(k, 1)].color = self.GenericJointHighLightColor();
                    sLeaser.sprites[self.PhoneSprite(k, 2)].color = self.GenericJointHighLightColor();

                }
                else
                {

                    //antenna color

                    sLeaser.sprites[self.PhoneSprite(k, 0)].color = self.armJointGraphics[0].BaseColor(default);
                    sLeaser.sprites[self.PhoneSprite(k, 1)].color = self.armJointGraphics[0].HighLightColor(default);
                    sLeaser.sprites[self.PhoneSprite(k, 2)].color = self.armJointGraphics[0].HighLightColor(default);

                }
                sLeaser.sprites[self.HandSprite(k, 0)].color = color;
                if (self.gown != null)
                {
                    for (int l = 0; l < 7; l++)
                    {
                        //sleeve color
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4] = new Color(0.23921568627f, 0.20784313725f, 0.72156862745f);
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 1] = new Color(0.23921568627f, 0.20784313725f, 0.72156862745f);
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 2] = new Color(0.23921568627f, 0.20784313725f, 0.72156862745f);
                        (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 3] = new Color(0.23921568627f, 0.20784313725f, 0.72156862745f);
                    }
                }
                else
                {
                    sLeaser.sprites[self.HandSprite(k, 1)].color = color;
                }
                sLeaser.sprites[self.FootSprite(k, 0)].color = color;
                sLeaser.sprites[self.FootSprite(k, 1)].color = color;
            }
        }
        else orig(self, sLeaser, rCam, palette);

    }

    private static Color Gown_Color(On.OracleGraphics.Gown.orig_Color orig, OracleGraphics.Gown self, float f)
    {
        Color color = Custom.HSL2RGB(Mathf.Lerp(0.08f, 0.02f, Mathf.Pow(f, 2f)), Mathf.Lerp(1f, 0.8f, f), 0.5f);

        if (self.owner.IsESP())
        {
            return Color.Lerp(Custom.hexToColor("F8C7CC"), Custom.hexToColor("550C18"), f);
        }
        else return orig(self, f);
    }

    private static void OracleGraphics_DrawSprites(On.OracleGraphics.orig_DrawSprites orig, OracleGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (self.IsESP())
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            Vector2 vector = Vector2.Lerp(self.owner.firstChunk.lastPos, self.owner.firstChunk.pos, timeStacker);
            Vector2 vector5 = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker);
            Vector2 vector6 = Custom.DirVec(vector5, vector);
            Vector2 vector7 = Custom.PerpendicularVector(vector6);
            Vector2 vector8 = self.RelativeLookDir(timeStacker);
            Vector2 vector24 = vector5 + vector7 * vector8.x * 2.5f + vector6 * (-2f - vector8.y * 1.5f);
            sLeaser.sprites[sLeaser.sprites.Length - 1].x = vector24.x - camPos.x;
            sLeaser.sprites[sLeaser.sprites.Length - 1].y = vector24.y - camPos.y;
            sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = Custom.AimFromOneVectorToAnother(vector24, vector5 - vector6 * 10f);
        }
        else orig(self, sLeaser, rCam, timeStacker, camPos);
    }

    private static bool RainWorldGame_IsMoonActive(On.RainWorldGame.orig_IsMoonActive orig, RainWorldGame self)
    {
        return self.StoryCharacter.value == "Witness" || self.StoryCharacter.value == "NightWalker" || orig(self);
    }

    private static void SuperStructureFuses_InitiateSprites(On.SuperStructureFuses.orig_InitiateSprites orig, SuperStructureFuses self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (!rCam.game.IsStorySession || (rCam.game.GetStorySession.characterStats.name != new SlugcatStats.Name("Witness") || rCam.game.GetStorySession.characterStats.name != new SlugcatStats.Name("NightWalker")))
        { 
            return; 
        } 

        float num;
        switch (self.depth)
        {
            case 0:
                num = 0.98333335f;
                goto DepthSet;
            case 1:
                num = 0.6333333f;
                goto DepthSet;
        }
        num = 0.3f;
        DepthSet:
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].alpha = num;
        }
        self.broken = 0; //TODO fix this later
    }
}