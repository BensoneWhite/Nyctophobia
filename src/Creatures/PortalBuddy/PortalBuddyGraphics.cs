namespace Nyctophobia;

public class PortalBuddyGraphics(PortalBuddy portalBuddy) : GraphicsModule(portalBuddy, false)
{
    public PortalBuddy PortalBuddy = portalBuddy;

    public override void InitiateSprites(SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Circle20");
        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        sLeaser.sprites[0].SetPosition(Vector2.Lerp(PortalBuddy.mainBodyChunk.lastPos, PortalBuddy.mainBodyChunk.pos, timeStacker) - camPos);
    }

    public override void ApplyPalette(SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette) => base.ApplyPalette(sLeaser, rCam, palette);

    public override void AddToContainer(SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        sLeaser.RemoveAllSpritesFromContainer();
        newContatiner ??= rCam.ReturnFContainer("Midground");

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }
}