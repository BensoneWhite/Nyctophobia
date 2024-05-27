namespace Nyctophobia;

public class MiroAlbinoCritob : Critob
{
    public MiroAlbinoCritob() : base(NTEnums.CreatureType.MiroAlbino)
    {
        Icon = new SimpleIcon("Kill_MirosBird", Color.white);
        LoadedPerformanceCost = 30;
        SandboxPerformanceCost = new SandboxPerformanceCost(0.3f, 0.3f);
        ShelterDanger = ShelterDanger.TooLarge;
        CreatureName = "MiroAlbino";
        RegisterUnlock(KillScore.Configurable(25), NTEnums.SandboxUnlock.MiroAlbino);
    }

    public override CreatureType ArenaFallback()
    {
        return NTEnums.CreatureType.MiroAlbino;
    }

    public override string DevtoolsMapName(AbstractCreature acrit)
    {
        return "MirosAlbino";
    }

    public override Color DevtoolsMapColor(AbstractCreature acrit)
    {
        return Color.white;
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit)
    {
        return new MirosBirdAI(acrit, acrit.world);
    }

    public override Creature CreateRealizedCreature(AbstractCreature acrit)
    {
        return new MiroAlbino(acrit, acrit.world);
    }

    public override IEnumerable<string> WorldFileAliases()
    {
        return ["MiroAlbino"];
    }

    public override int ExpeditionScore()
    {
        return 25;
    }

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