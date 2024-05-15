namespace Nyctophobia;

public interface IBlueSpear
{
    public void Init(ExplosiveSpear spear);

    public void PlaceInRoom(ExplosiveSpear spear, Room room);

    public void NewRoom(ExplosiveSpear spear, Room room);

    public void Update(ExplosiveSpear spear, bool eu);

    public void WeaponDeflect(ExplosiveSpear spear, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed);

    public float ConchunkWeight(ExplosiveSpear spear, Vector2 pushDir, BodyChunkConnection con);

    public void MiniExplode(ExplosiveSpear spear);

    public void Explode(ExplosiveSpear spear);

    public void HitByExplosion(ExplosiveSpear spear, float hitFac, Explosion explosion, int hitChunk);

    public void InitiateSprites(ExplosiveSpear spear, SpriteLeaser sLeaser, RoomCamera rCam);

    public void DrawSprites(ExplosiveSpear spear, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

    public void ApplyPallete(ExplosiveSpear spear, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);
}