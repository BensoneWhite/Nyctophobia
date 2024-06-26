﻿namespace Nyctophobia;

public class CloseCloud : Cloud
{
    public float CloudDepth { get; private set; }

    public CloseCloud(CustomBgScene scene, Vector2 pos, float depth, int index) : base(scene, pos, scene.DepthFromCloud(depth), index)
    {
        CloudDepth = depth;
        InitialDepth = depth;
    }

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];

        sLeaser.sprites[0] = new FSprite("pixel", true)
        {
            shader = Utils.Shaders["Background"],
            anchorY = 0f,
            scaleX = 1400f,
            x = 683f,
            y = 0f,
        };

        sLeaser.sprites[1] = new FSprite("pearlcat_clouds" + (Index % 3).ToString(), true)
        {
            shader = Utils.Shaders["Cloud"],
            anchorY = 1f
        };

        AddToContainer(sLeaser, rCam, null!);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var firstSprite = sLeaser.sprites[0];
        var cloudSprite = sLeaser.sprites[1];

        float y = scene.RoomToWorldPos(rCam.room.cameraPositions[rCam.currentCameraPosition]).y;
        float alt = Mathf.InverseLerp(Scene.StartAltitude, Scene.EndAltitude, y);
        float cloudDepth = CloudDepth;

        if (alt > 0.5f)
            cloudDepth = Mathf.Lerp(cloudDepth, 1f, Mathf.InverseLerp(0.5f, 1f, alt) * 0.5f);

        depth = Mathf.Lerp(Scene.CloudsStartDepth, Scene.CloudsEndDepth, cloudDepth);

        float scaleX = Mathf.Lerp(10f, 2f, cloudDepth);
        float posY = DrawPos(new(camPos.x, camPos.y + Scene.YShift), rCam.hDisplace).y;

        posY += Mathf.Lerp(Mathf.Pow(CloudDepth, 0.75f), Mathf.Sin(CloudDepth * Mathf.PI), 0.5f) * Mathf.InverseLerp(0.5f, 0f, alt) * 600f;
        posY -= Mathf.InverseLerp(0.18f, 0.1f, alt) * Mathf.Pow(1f - CloudDepth, 3f) * 100f;

        float scaleY = Mathf.Lerp(1f, Mathf.Lerp(0.75f, 0.25f, alt), cloudDepth);

        cloudSprite.scaleY = scaleY * scaleX;
        cloudSprite.scaleX = scaleX;
        cloudSprite.x = 683f;
        cloudSprite.y = posY;

        firstSprite.scaleY = posY - 150f * scaleX * scaleY;

        if (IsPrideDay)
        {
            cloudSprite.color = new Color(Random.value, Random.value, Random.value);
            firstSprite.color = new Color(Random.value, Random.value, Random.value);
        }
        else
        {
            cloudSprite.color = new(cloudDepth * 0.75f, RandomOffset, Mathf.Lerp(scaleY, 1f, 0.5f), 1f);
            firstSprite.color = Color.Lerp(SkyColor, Scene.AtmosphereColor, cloudDepth * 0.75f);
        }

        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }
}