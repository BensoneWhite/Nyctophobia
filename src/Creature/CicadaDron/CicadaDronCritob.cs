namespace Nyctophobia;

public class CicadaDronCritob : Critob
{
    public CicadaDronCritob() : base(NTEnums.CreatureType.CicadaDron) 
    {
        Icon = new SimpleIcon("Kill_Cicada", Color.red);
        LoadedPerformanceCost = 120f;
        SandboxPerformanceCost = new SandboxPerformanceCost(1.2f, 1.2f);
        ShelterDanger = ShelterDanger.Hostile;
        CreatureName = nameof(NTEnums.CreatureType.CicadaDron);
        RegisterUnlock(KillScore.Configurable(20), NTEnums.SandboxUnlock.CicadaDron);
    }

    public override CreatureType ArenaFallback() => CreatureType.CicadaA;

    public override string DevtoolsMapName(AbstractCreature acrit) => "CicadaDron";

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.red;

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new CicadaAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new Cicada(acrit, acrit.world, true);

    public override IEnumerable<string> WorldFileAliases() => new[] { nameof(NTEnums.CreatureType.CicadaDron) };

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => new[] { RoomAttractivenessPanel.Category.Lizards };

    public override int ExpeditionScore() => 20;

    public override CreatureTemplate CreateTemplate()
    {
        var s = new CreatureFormula(CreatureType.CicadaA, NTEnums.CreatureType.CicadaDron, nameof(NTEnums.CreatureType.CicadaDron))
        {
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.CicadaB)
        }.IntoTemplate();
        return s;
    }

    public override void EstablishRelationships()
    {
        var result = new Relationships(Type);
        result.Ignores(CreatureType.CicadaA);
        result.Ignores(CreatureType.CicadaB);
        result.Ignores(Type);
    }
}
