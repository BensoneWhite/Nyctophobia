namespace Nyctophobia;

public class RedFlareBomb : FlareBomb
{
    public RedFlareBomb(AbstractConsumable abstractConsumable, World world) : base(abstractConsumable, world)
    {
        color = new Color(Random.Range(0.6f, 1f), 0f, Random.Range(0.2f, 0.3f));
    }
}