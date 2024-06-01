namespace Nyctophobia;

public class MiroAlbinoCritob : Critob
{
    public MiroAlbinoCritob() : base(NTEnums.CreatureType.MiroAlbino)
    {
        Icon = IsPrideDay
            ? new SimpleIcon("Kill_MirosBird", new Color(Random.value, Random.value, Random.value))
            : new SimpleIcon("Kill_MirosBird", Color.white);
        LoadedPerformanceCost = 30;
        SandboxPerformanceCost = new SandboxPerformanceCost(0.3f, 0.3f);
        ShelterDanger = ShelterDanger.TooLarge;
        CreatureName = nameof(NTEnums.SandboxUnlock.MiroAlbino);
        RegisterUnlock(KillScore.Configurable(25), NTEnums.SandboxUnlock.MiroAlbino);
    }

    public override CreatureType ArenaFallback() => NTEnums.CreatureType.MiroAlbino;

    public override string DevtoolsMapName(AbstractCreature acrit) => nameof(NTEnums.SandboxUnlock.MiroAlbino);

    public override Color DevtoolsMapColor(AbstractCreature acrit)
    {
        if (IsPrideDay)
            return new Color(Random.value, Random.value, Random.value);
        else
            return Color.white;
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new MirosBirdAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new MiroAlbino(acrit, acrit.world);

    public override IEnumerable<string> WorldFileAliases() => [nameof(NTEnums.SandboxUnlock.MiroAlbino)];

    public override int ExpeditionScore() => 25;

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate s = new CreatureFormula(CreatureType.MirosBird, NTEnums.CreatureType.MiroAlbino, nameof(NTEnums.CreatureType.MiroAlbino))
        {
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.MirosBird)
        }.IntoTemplate();
        return s;
    }

    public override void EstablishRelationships()
    {
        Relationships result = new(CreatureType.MirosBird);
        result.Ignores(Type);
    }
}