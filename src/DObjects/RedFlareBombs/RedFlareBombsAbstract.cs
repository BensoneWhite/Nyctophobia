namespace Witness
{
    public class RedFlareBombsAbstract : AbstractPhysicalObject
    {
        public RedFlareBombsAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, NTEnums.AbstractObjectType.RedFlareBomb, null, pos, ID)
        {
        }

        public override void Realize()
        {
            base.Realize();
            realizedObject ??= new RedFlareBomb(this, world);
        }
    }
}
