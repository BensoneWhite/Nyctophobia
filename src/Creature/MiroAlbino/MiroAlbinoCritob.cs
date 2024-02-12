namespace Nyctophobia;

public class MiroAlbinoCritob : Critob
{
    public MiroAlbinoCritob() : base(NTEnums.CreatureType.MiroAlbino) 
    {
        Icon = new SimpleIcon("Kill_Mouse", Color.white);
        LoadedPerformanceCost = 30;
        SandboxPerformanceCost = new SandboxPerformanceCost(0.3f, 0.3f);
        ShelterDanger = ShelterDanger.TooLarge;
        CreatureName = nameof(NTEnums.CreatureType.MiroAlbino);
        RegisterUnlock(KillScore.Configurable(25), NTEnums.SandboxUnlock.MiroAlbino);
    }
    public override CreatureType ArenaFallback() => CreatureType.MirosBird;

    public override string DevtoolsMapName(AbstractCreature acrit) => "MirosAlbino";

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.white;

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new MirosBirdAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new MirosBird(acrit, acrit.world);

    public override IEnumerable<string> WorldFileAliases() => new[] { nameof(NTEnums.CreatureType.MiroAlbino) };

    public override int ExpeditionScore() => 25;

    public override CreatureTemplate CreateTemplate()
    {
        var s = new CreatureFormula(CreatureType.MirosBird, NTEnums.CreatureType.MiroAlbino, nameof(NTEnums.CreatureType.MiroAlbino))
        {
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.MirosBird)
        }.IntoTemplate();
        return s;
    }

    public override void EstablishRelationships()
    {
        var result = new Relationships(CreatureType.MirosBird);
        result.Ignores(Type);
    }
}
