namespace Nyctophobia;

public class FlashWigCritob : Critob
{
    public FlashWigCritob() : base(NTEnums.CreatureType.FlashWig)
    {
        Icon = new SimpleIcon("Kill_DropBug", Color.cyan);
        LoadedPerformanceCost = 30;
        ShelterDanger = ShelterDanger.Hostile;
        CreatureName = nameof(NTEnums.CreatureType.FlashWig);
        RegisterUnlock(KillScore.Configurable(15), NTEnums.SandboxUnlock.FlashWig);
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new DropBugAI(acrit, acrit.world);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new FlashWig(acrit);

    public override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureFormula(CreatureType.DropBug, NTEnums.CreatureType.FlashWig, nameof(NTEnums.CreatureType.FlashWig))
	    {
		    HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.DropBug),
            DefaultRelationship = new Relationship(Eats, 1)
	    }.IntoTemplate();

	    return template;
    }

    public override string DevtoolsMapName(AbstractCreature acrit) => "FsWg";

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.cyan;

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => new[] { RoomAttractivenessPanel.Category.Dark, RoomAttractivenessPanel.Category.LikesInside };

    public override IEnumerable<string> WorldFileAliases() => [nameof(NTEnums.CreatureType.FlashWig)];

    public override CreatureType ArenaFallback() => CreatureType.DropBug;

    public override int ExpeditionScore() => 15;

    public override void EstablishRelationships()
    {
	    var self = new Relationships(Type);

	    self.Fears(CreatureType.RedLizard, 0.4f);
	    self.Fears(CreatureType.Vulture, 1f);
	    self.Fears(CreatureType.TentaclePlant, 0.6f);
	    self.Fears(CreatureType.PoleMimic, 0.3f);
	    self.Fears(CreatureType.BigEel, 1f);
	    self.Fears(CreatureType.DaddyLongLegs, 1f);
	    self.Eats(CreatureType.SmallNeedleWorm, 0.1f);
	    self.Eats(CreatureType.SmallCentipede, 0.1f);
	    self.Ignores(CreatureType.DropBug);
	    self.Fears(CreatureType.MirosBird, 0.9f);
	    self.Fears(CreatureType.RedCentipede, 0.8f);

	    self.EatenBy(CreatureType.LizardTemplate, 0.2f);
	    self.FearedBy(CreatureType.LanternMouse, 0.7f);
	    self.EatenBy(CreatureType.Vulture, 0.4f);
	    self.FearedBy(CreatureType.CicadaA, 0.5f);
	    self.EatenBy(CreatureType.Leech, 0.4f);
	    self.FearedBy(CreatureType.JetFish, 0.2f);
	    self.EatenBy(CreatureType.DaddyLongLegs, 0.1f);
	    self.FearedBy(CreatureType.Slugcat, 0.5f);
	    self.FearedBy(CreatureType.Scavenger, 0.55f);
	    self.EatenBy(CreatureType.BigSpider, 0.25f);
	    self.IgnoredBy(CreatureType.DropBug);

	    if (ModManager.MSC)
	    {
		    self.EatenBy(MoreSlugcatsEnums.CreatureTemplateType.MirosVulture, 0.4f);
		    self.FearedBy(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC, 0.5f);
	    }
	    
	    self.Ignores(Type);
    }
}