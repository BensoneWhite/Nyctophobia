namespace Nyctophobia;

public class ItemData
{
    public float cacaoSpeed;

    public WorldCoordinate playerPos;

    public Vector2 playerMainPos;

    public float distanceToPlayer;

    public float DangerNum;

    public float afraid;

    public float power;

    public ItemData(Player player)
    {
        cacaoSpeed = 0f;
        playerPos = default;
        playerMainPos = default;
        distanceToPlayer = 0f;
        DangerNum = 0f;
        afraid = 0f;
        power = 0f;
    }
}
