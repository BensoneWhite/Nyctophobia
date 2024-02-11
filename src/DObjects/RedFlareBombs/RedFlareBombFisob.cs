using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace Witness
{
    public class RedFlareBombFisob : Fisob
    {
        public RedFlareBombFisob() : base(NTEnums.AbstractObjectType.RedFlareBomb)
        {
            Icon = new SimpleIcon("Symbol_FlashBomb", Color.red);

            RegisterUnlock(NTEnums.SandboxUnlock.RedFlareBomb);
        }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock)
        {
            var result = new RedFlareBombsAbstract(world, entitySaveData.Pos, entitySaveData.ID);

            return result;
        }

        private static readonly RedFlareBombsProperties redFlareBombsProperties = new();

        public override ItemProperties Properties(PhysicalObject forObject) => redFlareBombsProperties;
    }
}
