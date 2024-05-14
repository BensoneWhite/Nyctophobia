namespace Nyctophobia;

public class BlueSpearAbstract : AbstractSpear
{
    public BlueSpearAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID, bool explosive, float hue) : base(world, realizedObject, pos, ID, explosive, hue)
    {
        type = BlueSpearFisob.Instance.Type;
    }

    public override void Realize()
    {
        base.Realize();
        if (type == BlueSpearFisob.Instance.Type)
        {
            realizedObject ??= new BlueSpear(this, world);
        }
        for (int i = 0; i < stuckObjects.Count; i++)
        {
            if (stuckObjects[i].A.realizedObject == null && stuckObjects[i].A != this)
            {
                stuckObjects[i].A.Realize();
            }
            if (stuckObjects[i].B.realizedObject == null && stuckObjects[i].B != this)
            {
                stuckObjects[i].B.Realize();
            }
        }
    }
}