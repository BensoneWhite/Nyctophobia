namespace Nyctophobia;

public class Whiskerdata
{
    public bool ready = false;
    public int initialfacewhiskerloc;
    public string sprite = "LizardScaleA0";
    public string facesprite = "LizardScaleA0";
    public WeakReference<Player> playerref;
    public Whiskerdata(Player player) => playerref = new WeakReference<Player>(player);
    public Vector2[] headpositions = new Vector2[6];
    public Scale[] headScales = new Scale[6];
    public class Scale : BodyPart
    {
        public Scale(GraphicsModule cosmetics) : base(cosmetics)
        {
        }

        public override void Update()
        {
            base.Update();
            if (owner.owner.room.PointSubmerged(pos)) vel *= 0.5f;

            else vel *= 0.9f;

            lastPos = pos;
            pos += vel;
        }
        public float length = 7f;
        public float width = 2f;
    }
    public Color headcolor = new(1f, 1f, 0f);

    public int Facewhiskersprite(int side, int pair) => initialfacewhiskerloc + side + pair + pair;
}
