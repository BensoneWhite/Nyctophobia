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

    public Player player;

    public WeakReference<Player> PlayerRef;

    public int DelayedDeafen;
    public int DelayedDeafenDuration;

    public bool Berserker;
    public int BerserkerDuration;

    public GeneralPlayerData(Player player)
    {
        PlayerRef = new WeakReference<Player>(player);
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
        DelayedDeafen = 0;
        DelayedDeafenDuration = 0;
        Berserker = default;
        BerserkerDuration = 0;
    }
}