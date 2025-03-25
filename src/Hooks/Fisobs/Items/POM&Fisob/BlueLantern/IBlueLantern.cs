namespace Nyctophobia;

public interface IBlueLantern
{
    public void Init(Lantern lantern);

    public void NewRoom(Lantern lantern, Room room);

    public void Update(Lantern lantern, bool eu);

    public void PlaceInRoom(Lantern lantern, Room room);

    public void HitByWeapon(Lantern lantern, Weapon weapon);

    public void TerrainImpact(Lantern lantern, int chunk, IntVector2 direction, float speed, bool firstContact);

    public void InitiateSprites(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam);

    public void DrawSprites(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

    public void ApplyPalette(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

    public void AddToContainer(Lantern lantern, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner);
}