namespace Nyctophobia;

//TODO: Revert the changes made for this object to single object Fisobs and POM with procedural object placer
public interface IRedFlareBomb
{
    public void Init(FlareBomb flare);

    public void Update(FlareBomb flare, bool eu);

    void NewRoom(FlareBomb flare, Room newRoom);

    public void HitSomething(FlareBomb flare, CollisionResult result, bool eu);

    public void Thrown(FlareBomb flare, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu);

    public void PickedUp(FlareBomb flare, Creature upPicker);

    public void InitiateSprites(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam);

    public void AddToContainer(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner);

    public void ApplyPalette(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

    public void DrawSprites(FlareBomb flare, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);
}