using Fisobs.Core;
using Fisobs.Creatures;
using Fisobs.Sandbox;
using static MultiplayerUnlocks;
using static CreatureTemplate;
using static CreatureCommunities;
using Nyctophobia;

namespace Nyctophobia;

public class BoyKisserCritob : Critob
{

    public BoyKisserCritob(): base(NTEnums.CreatureType.BoyKisser)
    {
        LoadedPerformanceCost = 50;
        RegisterUnlock(KillScore.Configurable(666), NTEnums.SandboxUnlock.BoyKisser, SandboxUnlockID.Slugcat);
        Icon = new SimpleIcon("Futile_White", Color.white);
        CreatureName = nameof(NTEnums.CreatureType.BoyKisser);
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new BoyKisserAI(acrit, (acrit.realizedCreature as Boykisser));

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new Boykisser(acrit);

    public override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureFormula(this)
        {
            DefaultRelationship = new Relationship(Relationship.Type.Ignores, 0f),
            HasAI = true,
            InstantDeathDamage = 10000f,
            Pathing = PreBakedPathing.Ancestral(CreatureTemplate.Type.DropBug),
            TileResistances = new TileResist
            {
                OffScreen = new PathCost(1f, 0),
                Floor = new PathCost(1f, 0),
                Corridor = new PathCost(1f, 0),
                Climb = new PathCost(1f, 0),
                Wall = new PathCost(3f, 0),
                Ceiling = new PathCost(3f, 0)
            },
            ConnectionResistances = new ConnectionResist
            {
                Standard = new PathCost(1f, 0),
                ReachOverGap = new PathCost(1f, 0),
                ReachUp = new PathCost(1f, 0),
                ReachDown = new PathCost(1f, 0),
                DropToFloor = new PathCost(1f, 0),
                DropToClimb = new PathCost(1f, 0),
                DropToWater = new PathCost(1f, 0),
                ShortCut = new PathCost(1f, 0),
                NPCTransportation = new PathCost(1f, 0),
                OffScreenMovement = new PathCost(1f, 0),
                BetweenRooms = new PathCost(1f, 0),
                Slope = new PathCost(1f, 0),
                CeilingSlope = new PathCost(1f, 0)
            },
            DamageResistances = new AttackResist
            {
                Base = 10000f
            },
            StunResistances = new AttackResist
            {
                Base = 10000f
            }
        }.IntoTemplate();
        template.offScreenSpeed = 2f;
        template.abstractedLaziness = 0;
        template.roamBetweenRoomsChance = 1f;
        template.requireAImap = true;
        template.bodySize = 5f;
        template.stowFoodInDen = false;
        template.shortcutSegments = 4;
        template.grasps = 0;
        template.visualRadius = 2000f;
        template.communityID = CommunityID.None;
        template.waterRelationship = WaterRelationship.Amphibious;
        template.waterPathingResistance = 1f;
        template.canSwim = true;
        template.dangerousToPlayer = 1f;
        template.wormGrassImmune = true;
        return template;
    }

    public override void EstablishRelationships()
    {
        var s = new Relationships(Type);
        s.Eats(CreatureTemplate.Type.Slugcat, 1f);
        s.FearedBy(CreatureTemplate.Type.Slugcat, 1f);
    }

    public override string DevtoolsMapName(AbstractCreature acrit) => "BoyKisser";

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.white;
}
