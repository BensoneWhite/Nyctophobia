namespace Nyctophobia;

public class Whiskerdata(Player player)
{
    public bool ready = false;
    public int initialfacewhiskerloc;
    public string sprite = "LizardScaleA0";
    public string facesprite = "LizardScaleA0";
    public WeakReference<Player> playerref = new(player);
    public Vector2[] headpositions = new Vector2[6];
    public Scale[] headScales = new Scale[6];
    public Color headcolor = new(1f, 1f, 0f);

    public int Facewhiskersprite(int side, int pair)
    {
        return initialfacewhiskerloc + side + pair + pair;
    }

    public class Scale(GraphicsModule cosmetics) : BodyPart(cosmetics)
    {
        public float length = 5f;
        public float width = 2f;

        public override void Update()
        {
            base.Update();
            if (owner.owner.room.PointSubmerged(pos))
            {
                vel *= 0.5f;
            }
            else
            {
                vel *= 0.9f;
            }

            lastPos = pos;
            pos += vel;
        }
    }
}