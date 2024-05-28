
using UnityEngine;

namespace Nyctophobia;

public class HorizonCloud(CustomBgScene scene, Vector2 pos, float depth, int index, float flatten, float alpha, float shaderColor) : Cloud(scene, pos, depth, index)
{
    public float Flatten { get; set; } = flatten;
    public float Alpha { get; set; } = alpha;
    public float ShaderColor { get; set; } = shaderColor;

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];

        sLeaser.sprites[0] = new FSprite("pearlcat_flyingclouds", true)
        {
            shader = Utils.Shaders["CloudDistant"],
            anchorY = 1f
        };

        AddToContainer(sLeaser, rCam, null!);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var firstSprite = sLeaser.sprites[0];

        float worldPosY = scene.RoomToWorldPos(rCam.room.cameraPositions[rCam.currentCameraPosition]).y;
     
        if (Mathf.InverseLerp(Scene.StartAltitude, Scene.EndAltitude, worldPosY) < 0.33f)
        {
            firstSprite.isVisible = false;
            return;
        }
     
        firstSprite.isVisible = true;
        
        float scaleX = 2f;
        float posY = DrawPos(camPos, rCam.hDisplace).y;
        
        firstSprite.scaleY = Flatten * scaleX;
        firstSprite.scaleX = scaleX;
        firstSprite.color = new(ShaderColor, RandomOffset, Mathf.Lerp(Flatten, 1f, 0.5f), Alpha);
        firstSprite.x = 683f;
        firstSprite.y = posY;

        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }
}