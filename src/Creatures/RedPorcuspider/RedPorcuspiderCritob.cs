namespace Nyctophobia;

public class RedPorcuspiderCritob : Critob
{
    public RedPorcuspiderCritob() : base(NTEnums.CreatureType.RedPorcuspider)
    {
        Icon = IsPrideDay
            ? new SimpleIcon("Kill_BigSpider", new Color(Random.value, Random.value, Random.value))
            : new SimpleIcon("Kill_BigSpider", new Color(1f, 0f, 0f));
        ShelterDanger = ShelterDanger.Hostile;
        LoadedPerformanceCost = 20f;
        SandboxPerformanceCost = new SandboxPerformanceCost(1f, 1f);
        CreatureName = nameof(NTEnums.CreatureType.RedPorcuspider);
        RegisterUnlock(KillScore.Configurable(5), NTEnums.SandboxUnlock.RedPorcuspider);
    }

    public override int ExpeditionScore() => 6;

    public override Color DevtoolsMapColor(AbstractCreature acrit)
    {
        return IsPrideDay ? new Color(Random.value, Random.value, Random.value) : new(1f, 0f, 0f);
    }

    public override string DevtoolsMapName(AbstractCreature acrit) => nameof(NTEnums.CreatureType.RedPorcuspider);

    public override IEnumerable<string> WorldFileAliases() => [nameof(NTEnums.CreatureType.RedPorcuspider)];

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => [RoomAttractivenessPanel.Category.Dark, RoomAttractivenessPanel.Category.LikesInside];

    public override CreatureType ArenaFallback() => CreatureType.BigSpider;

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new BigSpiderAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new BigSpider(acrit, acrit.world);

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = new CreatureFormula(CreatureType.BigSpider, Type, nameof(NTEnums.CreatureType.RedPorcuspider))
        {
            TileResistances = new()
            {
                Air = new(1f, Allowed)
            },
            ConnectionResistances = new()
            {
                Standard = new(1f, Allowed),
                ShortCut = new(1f, Unallowed),
                BigCreatureShortCutSqueeze = new(10f, Unwanted),
                OffScreenMovement = new(1f, Allowed),
                BetweenRooms = new(10f, Allowed)
            },
            DefaultRelationship = new(Eats, 1f),
            DamageResistances = new() { Base = 1f, Explosion = .5f },
            StunResistances = new() { Base = 2f, Explosion = .2f },
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.BigSpider),
        }.IntoTemplate();
        return template;
    }

    public override void EstablishRelationships()
    {
        Relationships spider = new(Type);
        spider.Ignores(Type);
    }
}