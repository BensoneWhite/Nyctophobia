namespace Nyctophobia;

public interface IBloodyFlower
{
    public int FoodPoints { get; }

    public void Init(KarmaFlower flower);

    public void Update(KarmaFlower flower, bool eu);

    void NewRoom(KarmaFlower flower, Room newRoom);

    public void ThrowByPlayer(KarmaFlower flower);

    public void BitByPlayer(KarmaFlower flower, Creature.Grasp grasp, bool eu);

    public void InitiateSprites(KarmaFlower flower, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

    public void AddToContainer(KarmaFlower flower, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner);

    public void ApplyPalette(KarmaFlower flower, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

    public void DrawSprites(KarmaFlower flower, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);
}