namespace Nyctophobia;

public class RedPorcuspider : BigSpider
{
    public RedPorcuspider(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
    {
        yellowCol = Color.Lerp(new Color(1f, 0f, 0f), Custom.HSL2RGB(Random.Range(0f, 1f), Random.Range(0f, 0.349f), Random.Range(0f, 0.349f)), Random.value * 0.2f);
    }
}