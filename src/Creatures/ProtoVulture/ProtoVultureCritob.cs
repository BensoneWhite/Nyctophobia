namespace Nyctophobia;

public class ProtoVultureCritob : Critob
{
    public ProtoVultureCritob() : base(NTEnums.CreatureType.ProtoVulture)
    {
        Icon = IsPrideDay
            ? new SimpleIcon("Kill_Vulture", new Color(Random.value, Random.value, Random.value))
            : new SimpleIcon("Kill_Vulture", new Color(.27f, .78f, .2f));
        ShelterDanger = ShelterDanger.Hostile;
        LoadedPerformanceCost = 20f;
        SandboxPerformanceCost = new SandboxPerformanceCost(0.5f, 0.5f);
        CreatureName = nameof(NTEnums.CreatureType.ProtoVulture);
        RegisterUnlock(KillScore.Configurable(10), NTEnums.SandboxUnlock.ProtoVulture);
    }

    public override int ExpeditionScore() => 6;

    public override Color DevtoolsMapColor(AbstractCreature acrit)
    {
        return IsPrideDay ? new Color(Random.value, Random.value, Random.value) : new(.27f, .78f, .2f);
    }

    public override string DevtoolsMapName(AbstractCreature acrit) => nameof(NTEnums.CreatureType.ProtoVulture);

    public override IEnumerable<string> WorldFileAliases() => [nameof(NTEnums.CreatureType.ProtoVulture)];

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => [RoomAttractivenessPanel.Category.Flying];

    public override CreatureType ArenaFallback() => CreatureType.Vulture;

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new VultureAI(acrit, acrit.world);

    public override AbstractCreatureAI CreateAbstractAI(AbstractCreature acrit) => new VultureAbstractAI(acrit.world, acrit);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new Vulture(acrit, acrit.world);

    public override CreatureState CreateState(AbstractCreature acrit) => new Vulture.VultureState(acrit);

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = new CreatureFormula(CreatureType.Vulture, Type, nameof(NTEnums.CreatureType.ProtoVulture))
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
            Pathing = PreBakedPathing.Ancestral(CreatureType.Vulture),
        }.IntoTemplate();
        return template;
    }

    public override void EstablishRelationships()
    {
        Relationships vulture = new(Type);
        vulture.Ignores(Type);
    }
}