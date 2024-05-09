namespace Nyctophobia;

public class ItemData
{
    public readonly bool IsAPlayer;

    public float cacaoSpeed;

    public WorldCoordinate playerPos;

    public Vector2 playerMainPos;

    public float distanceToPlayer;

    public float DangerNum;

    public float afraid;

    public float power;

    public readonly Player player;

    public ItemData(Player player)
    {
        this.player = player;

        IsAPlayer = player == this.player;

        if (!IsAPlayer) return;

        cacaoSpeed = 0f;
        playerPos = default;
        playerMainPos = default;
        distanceToPlayer = 0f;
        DangerNum = 0f;
        afraid = 0f;
        power = 0f;
    }
}