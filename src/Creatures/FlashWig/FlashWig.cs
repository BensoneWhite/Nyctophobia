namespace Nyctophobia;

public class FlashWig : DropBug
{
    public Color EffectColor;
    public Color FlashColor;
    public float LightIntensity;
    
    public FlashWig(AbstractCreature abstractCreature) : base(abstractCreature, abstractCreature.world)
    {
        var state = Random.state;
        Random.InitState(abstractCreature.ID.RandomSeed);

        var hue = Random.value;
        FlashColor = Random.ColorHSV(hue, hue, 1, 1, 1, 1);
        EffectColor = Random.ColorHSV(hue, hue, 0.4f, 1, 0.4f, 1);

        Random.state = state;
    }

    public override void InitiateGraphicsModule()
    {
        graphicsModule ??= new FlashWigGraphics(this);
        graphicsModule.Reset();
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        if (dead || (dropAnticipation == 0 && inCeilingMode > 0))
        {
            LightIntensity = Mathf.Max(0, LightIntensity - 0.008f);
        }
        else
        {
            LightIntensity = inCeilingMode == 0 ? 0.8f : dropAnticipation * 0.8f;
        }
    }
}