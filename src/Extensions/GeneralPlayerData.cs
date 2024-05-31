namespace Nyctophobia;

public class GeneralPlayerData
{
    public readonly bool IsAPlayer;

    public WorldCoordinate playerPos;

    public Vector2 playerMainPos;

    public float cacaoSpeed;
    public float distanceToPlayer;
    public float DangerNum;
    public float afraid;
    public float power;

    public readonly Player player;

    public int DelayedDeafen;
    public int DelayedDeafenDuration;

    public GeneralPlayerData(Player player)
    {
        this.player = player;

        IsAPlayer = player == this.player;

        if (!IsAPlayer)
        {
            return;
        }

        cacaoSpeed = 0f;
        playerPos = default;
        playerMainPos = default;
        distanceToPlayer = 0f;
        DangerNum = 0f;
        afraid = 0f;
        power = 0f;
    }
}