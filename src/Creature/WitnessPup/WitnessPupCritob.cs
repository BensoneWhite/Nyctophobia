namespace Nyctophobia;

public class WitnessPupCritob : Critob
{
    public WitnessPupCritob() : base(NTEnums.CreatureType.WitnessPup)
    {
        Icon = new SimpleIcon("Kill_Slugcat", new Color(0.125f, 0.125f, 1));
        ShelterDanger = ShelterDanger.Safe;
        LoadedPerformanceCost = 100f;
        SandboxPerformanceCost = new SandboxPerformanceCost(0.5f, 0.5f);
        RegisterUnlock(KillScore.Configurable(6), NTEnums.SandboxUnlock.WitnessPup);
    }

    public override int ExpeditionScore() => 6;

    public override Color DevtoolsMapColor(AbstractCreature acrit) => new(1, 0.8117647058823529f, 0.050980392156862744f);

    public override string DevtoolsMapName(AbstractCreature acrit) => "WitnessPup";

    public override IEnumerable<string> WorldFileAliases() => new[] { "WitnessPup" };

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => new[] { RoomAttractivenessPanel.Category.Lizards, RoomAttractivenessPanel.Category.Swimming, RoomAttractivenessPanel.Category.LikesInside, RoomAttractivenessPanel.Category.LikesWater };

    public override CreatureType ArenaFallback() => MoreSlugcatsEnums.CreatureTemplateType.SlugNPC;

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new SlugNPCAI(acrit, acrit.world);

    public override AbstractCreatureAI CreateAbstractAI(AbstractCreature acrit) => new SlugNPCAbstractAI(acrit.world, acrit);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new Player(acrit, acrit.world);

    public override CreatureState CreateState(AbstractCreature acrit) => new PlayerNPCState(acrit, 0);

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate cf = new CreatureFormula(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC, Type, nameof(NTEnums.CreatureType.WitnessPup))
        {
            DefaultRelationship = new(CreatureTemplate.Relationship.Type.Eats, 1),
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.Slugcat),
        }.IntoTemplate();
        cf.meatPoints = 6;
        cf.visualRadius = 1500;
        cf.baseStunResistance = -25;
        cf.waterVision = 0.3f;
        cf.stowFoodInDen = true;
        cf.throughSurfaceVision = 0.5f;
        cf.movementBasedVision = 0.5f;
        cf.communityInfluence = 0.1f;
        cf.bodySize = 1;
        cf.usesCreatureHoles = false;
        cf.usesNPCTransportation = true;
        cf.BlizzardAdapted = true;
        cf.BlizzardWanderer = true;
        cf.waterRelationship = CreatureTemplate.WaterRelationship.AirOnly;
        cf.lungCapacity = 1600;
        cf.jumpAction = "Swap Heads";
        cf.pickupAction = "Grab/Freeze";
        cf.shortcutSegments = 2;
        return cf;
    }

    public override void EstablishRelationships()
    {
        var s = new Relationships(Type);
        s.Eats(CreatureType.Fly, .5f);
        s.Eats(CreatureType.EggBug, 1f);
        s.Fears(CreatureType.Vulture, 1f);
        s.Fears(CreatureType.BigEel, 1f);
        s.Fears(CreatureType.DaddyLongLegs, 1f);
        s.Fears(CreatureType.TentaclePlant, 1f);
        s.Fears(CreatureType.MirosBird, 1f);
        s.Fears(CreatureType.Centipede, 0.5f);
        s.Fears(CreatureType.Centiwing, 0.4f);
        s.Fears(CreatureType.LizardTemplate, 0.6f);
        s.Eats(CreatureType.SmallCentipede, 0.6f);
        s.Eats(CreatureType.SmallNeedleWorm, 0.4f);
        s.Fears(CreatureType.BigSpider, 0.5f);
        s.Fears(CreatureType.SpitterSpider, 0.8f);
        s.Fears(CreatureType.DropBug, 0.5f);
        s.Fears(CreatureType.RedCentipede, 1f);
        s.IsInPack(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC, 0.5f);
        s.Eats(CreatureType.VultureGrub, 0.4f);
        s.EatenBy(CreatureType.LizardTemplate, 0.5f);
        s.FearedBy(CreatureType.Fly, 0.5f);
        s.FearedBy(CreatureType.LanternMouse, 0.3f);
        s.EatenBy(CreatureType.Vulture, 0.3f);
        s.HasDynamicRelationship(CreatureType.CicadaA, 1f);
        s.HasDynamicRelationship(CreatureType.CicadaB, 1f);
        s.HasDynamicRelationship(CreatureType.JetFish, 1f);
        s.EatenBy(CreatureType.DaddyLongLegs, 1f);
        s.EatenBy(CreatureType.MirosBird, 0.6f);
        s.HasDynamicRelationship(CreatureType.Scavenger, 1f);
        s.EatenBy(CreatureType.BigSpider, 0.6f);
        s.Fears(MoreSlugcatsEnums.CreatureTemplateType.MirosVulture, 1f);
        s.EatenBy(MoreSlugcatsEnums.CreatureTemplateType.MirosVulture, 0.6f);
        s.Eats(MoreSlugcatsEnums.CreatureTemplateType.FireBug, 0.5f);
    }
}
