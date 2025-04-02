namespace Nyctophobia;

//Booring creature, needs a rework
public class BlackLighMouseCritob : Critob
{
    public BlackLighMouseCritob() : base(NTEnums.CreatureType.BlackLightMouse)
    {
        Icon = new SimpleIcon("Kill_Mouse", Color.black);
        LoadedPerformanceCost = 20;
        SandboxPerformanceCost = new SandboxPerformanceCost(0.2f, 0.2f);
        ShelterDanger = ShelterDanger.Safe;
        CreatureName = nameof(NTEnums.CreatureType.BlackLightMouse);
        RegisterUnlock(KillScore.Configurable(5), NTEnums.SandboxUnlock.BlackLightMouse);
    }

    public override CreatureState CreateState(AbstractCreature acrit) => new MouseState(acrit);

    public override CreatureType ArenaFallback() => CreatureType.LanternMouse;

    public override string DevtoolsMapName(AbstractCreature acrit) => nameof(NTEnums.CreatureType.BlackLightMouse);

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.black;

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new MouseAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new LanternMouse(acrit, acrit.world);

    public override IEnumerable<string> WorldFileAliases() => [nameof(NTEnums.CreatureType.BlackLightMouse)];

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => [RoomAttractivenessPanel.Category.Lizards];

    public override int ExpeditionScore() => 5;

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate s = new CreatureFormula(CreatureType.LanternMouse, NTEnums.CreatureType.BlackLightMouse, nameof(NTEnums.CreatureType.BlackLightMouse))
        {
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.LanternMouse)
        }.IntoTemplate();
        return s;
    }

    public override void EstablishRelationships()
    {
        Relationships self = new(Type);
        self.Ignores(Type);
        self.Attacks(CreatureType.LanternMouse, 1f);
        self.Attacks(CreatureType.Slugcat, 1f);
    }
}