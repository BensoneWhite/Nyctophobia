namespace Nyctophobia;

public interface ICacaoFruit
{
    public int FoodPoints { get; }

    public void Init(DangleFruit fruit);

    public void Update(DangleFruit fruit, bool eu);

    void NewRoom(DangleFruit dangleFruit, Room newRoom);

    public void ThrowByPlayer(DangleFruit fruit);

    public void BitByPlayer(DangleFruit fruit, Creature.Grasp grasp, bool eu);

    public void InitiateSprites(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

    public void AddToContainer(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner);

    public void ApplyPalette(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

    public void DrawSprites(DangleFruit fruit, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);
}