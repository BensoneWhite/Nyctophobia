namespace Nyctophobia;

public interface IBlueBomba
{
    public void Init(ScavengerBomb bomb);

    public void NewRoom(ScavengerBomb bomb, Room room);

    public void InitiateBurn(ScavengerBomb bomb);

    public void Update(ScavengerBomb bomb, bool eu);

    public void TerrainImpact(ScavengerBomb bomb, int chunk, IntVector2 direction, float speed, bool firstContact);

    public void HitSomething(ScavengerBomb bomb, CollisionResult result, bool eu);

    public void Thrown(ScavengerBomb bomb, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu);

    public void PickedUp(ScavengerBomb bomb, Creature upPicker);

    public void HitByWeapon(ScavengerBomb bomb, Weapon weapon);

    public void WeaponDeflect(ScavengerBomb bomb, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed);

    public void HitByExplosion(ScavengerBomb bomb, float hitFac, Explosion explosion, int hitChunk);

    public void Explode(ScavengerBomb bomb, BodyChunk hitChunk);

    public void InitiateSprites(ScavengerBomb bomb, SpriteLeaser sLeaser, RoomCamera rCam);

    public void DrawSprites(ScavengerBomb bomb, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

    public void ApplyPalette(ScavengerBomb bomb, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette);

    public void UpdateColor(ScavengerBomb bomb, SpriteLeaser sLeaser, Color col);
}