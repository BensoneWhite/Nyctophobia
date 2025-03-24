namespace Nyctophobia;

public class ImpalerAbstract : AbstractSpear
{
    public ImpalerAbstract(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID) : base(world, realizedObject, pos, ID, false)
    {
        type = ImpalerFisob.Instance.Type;
    }

    public bool StuckInWall => stuckInWallCycles != 0;

    public override void Realize()
    {
        base.Realize();
        if (type == ImpalerFisob.Instance.Type)
        {
            realizedObject ??= new ImpalerRealizer(this, world);
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