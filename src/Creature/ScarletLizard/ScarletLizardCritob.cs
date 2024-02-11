using DevInterface;
using Fisobs.Core;
using Fisobs.Creatures;
using Fisobs.Sandbox;

namespace Witness
{
    public class ScarletLizardCritob : Critob
    {
        public ScarletLizardCritob() : base(NTEnums.CreatureType.ScarletLizard)
        {
            Icon = new SimpleIcon("Kill_Yellow_Lizard", Color.red);
            LoadedPerformanceCost = 100f;
            SandboxPerformanceCost = new SandboxPerformanceCost(1f, 1f);
            ShelterDanger = ShelterDanger.Hostile;
            CreatureName = nameof(NTEnums.CreatureType.ScarletLizard);
            RegisterUnlock(KillScore.Configurable(10), NTEnums.SandboxUnlock.ScarletLizards);
        }

        public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new ScarletLizardAI(acrit);

        public override Creature CreateRealizedCreature(AbstractCreature acrit) => new ScarletLizard(acrit, acrit.world);

        public override CreatureTemplate CreateTemplate() => LizardBreeds.BreedTemplate(Type, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.LizardTemplate), StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.PinkLizard), StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.BlueLizard), StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.GreenLizard));

        public override string DevtoolsMapName(AbstractCreature acrit) => "ScarletLizard";

        public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.red;

        public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => new[] { RoomAttractivenessPanel.Category.Lizards };

        public override IEnumerable<string> WorldFileAliases() => new[] { nameof(NTEnums.CreatureType.ScarletLizard) };

        public override CreatureTemplate.Type ArenaFallback() => CreatureTemplate.Type.YellowLizard;

        public override int ExpeditionScore() => 10;

        public override CreatureState CreateState(AbstractCreature acrit) => new LizardState(acrit);

        public override void EstablishRelationships()
        {
            var self = new Relationships(Type);

            self.IsInPack(Type, 0.2f);
            self.IsInPack(CreatureTemplate.Type.YellowLizard, 0.2f);
        }
    }
}