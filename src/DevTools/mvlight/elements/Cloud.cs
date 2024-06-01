﻿namespace Nyctophobia;

public abstract class Cloud(CustomBgScene scene, Vector2 pos, float depth, int index) : CustomBgElement(scene, pos, depth, BgElementType.END)
{
    public float RandomOffset { get; private set; } = Random.value;
    public Color SkyColor { get; private set; }
    public int Index { get; private set; } = index;

    public override void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (IsPrideDay)
            SkyColor = new Color(Random.value, Random.value, Random.value);
        else
            SkyColor = palette.skyColor;
    }
}