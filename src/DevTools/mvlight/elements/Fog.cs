namespace Nyctophobia;

public class Fog(BackgroundScene bgScene) : BackgroundScene.FullScreenSingleColor(bgScene, default, 0.0f, true, float.MaxValue)
{
    public float Alpha { get; set; } = 1.0f;
    public float Depth { get; set; } = 0.0f;

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        alpha = Alpha;
        depth = Depth;

        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }

    public override void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : palette.skyColor;
    }
}