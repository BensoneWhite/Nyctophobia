namespace Nyctophobia;

public class PortalBuddyCritob : Critob
{
    public PortalBuddyCritob() : base(NTEnums.CreatureType.PortalBuddy)
    {
        LoadedPerformanceCost = 50;
        ShelterDanger = ShelterDanger.Hostile;
        CreatureName = nameof(NTEnums.CreatureType.PortalBuddy);

    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new PortalBuddyAI(acrit, acrit.realizedCreature as PortalBuddy);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new PortalBuddy(acrit);

    public override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureFormula(this)
        {
            DefaultRelationship = new Relationship(Ignores, 0f),
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.Scavenger),
            TileResistances = new()
            {
                Climb = new(1, Allowed),
                Floor = new(1, Allowed),
                Corridor = new(1, Allowed),
                OffScreen = new(1, Allowed),
            },
            ConnectionResistances = new()
            {
                Standard = new(1, Allowed),
                BetweenRooms = new(1, Allowed),
                Slope = new(1, Allowed),
                DropToClimb = new(1, Allowed),
                ReachUp = new(1, Allowed),
                OutsideRoom = new(1, Allowed),
                ShortCut = new(1, Allowed),
                ReachDown = new(1, Allowed),
                DropToFloor = new(1, Allowed),
                DropToWater = new(1, Unwanted),
                DoubleReachUp = new(1, Allowed),
                ReachOverGap = new(1, Allowed),
                OffScreenMovement = new(1, Allowed),
                SemiDiagonalReach = new(1, Allowed),
                OpenDiagonal = new(1, Allowed),
            },
            DamageResistances = new()
            {
                Base = 2
            },
            StunResistances = new()
            {
                Base = 0.5f
            }
        }.IntoTemplate();

        template.offScreenSpeed = 0.3f;
        template.abstractedLaziness = 50;
        template.roamBetweenRoomsChance = 0.1f;
        template.bodySize = 1;
        template.stowFoodInDen = true;
        template.shortcutSegments = 3;
        template.grasps = 1;
        template.visualRadius = 800f;
        template.movementBasedVision = 0.5f;
        template.waterRelationship = WaterRelationship.AirAndSurface;
        template.waterPathingResistance = 2;
        template.dangerousToPlayer = 0f;
        template.canFly = false;
        template.meatPoints = 3;
        template.SetDoubleReachUpConnectionParams(AItile.Accessibility.Floor, AItile.Accessibility.Air, AItile.Accessibility.Floor);

        return template;
    }

    public override void EstablishRelationships()
    {
        var self = new Relationships(Type);
        
        var slugcatTemplate = StaticWorld.GetCreatureTemplate(CreatureType.Slugcat);

        //-- Copying slugcat relationships
        for (var i = 0; i < slugcatTemplate.relationships.Length; i++)
        {
            var relationship = slugcatTemplate.relationships[i];
            if (relationship.type != null)
            {
                StaticWorld.EstablishRelationship(Type, new CreatureType(ExtEnum<CreatureType>.values.entries[i]), relationship.Duplicate());
            }
        }
        
        //-- Smells just like a slugcat!
        foreach (var template in StaticWorld.creatureTemplates)
        {
            if (template.type != NTEnums.CreatureType.PortalBuddy)
            {
                var relationship = template.relationships[CreatureType.Slugcat.Index];
                if (relationship.type != null)
                {
                    if (relationship.type == SocialDependent)
                    {
                        //-- Weird thingy spoopy
                        StaticWorld.EstablishRelationship(template.type, Type, new Relationship(Uncomfortable, 0.3f));
                    }
                    else
                    {
                        //-- Not as tasty as a slugcat, though
                        StaticWorld.EstablishRelationship(template.type, Type, new Relationship(relationship.type, relationship.intensity * (relationship.type == Eats ? 0.3f : 1)));
                    }
                }
            }
        }

        //-- "Better than starving, I guess" - White Lizard
        self.EatenBy(CreatureType.LizardTemplate, 0.02f);
        
        //-- They're just pretending <3
        self.Ignores(CreatureType.Slugcat);
        self.IgnoredBy(CreatureType.Slugcat);
    }


    public override string DevtoolsMapName(AbstractCreature acrit) => "Portal";

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.magenta;

    public override AbstractCreatureAI CreateAbstractAI(AbstractCreature acrit) => new PortalBuddyAbstractAI(acrit.world, acrit);

    public override ItemProperties Properties(Creature crit)
    {
        if (crit is PortalBuddy buddy)
        {
            return new PortalBuddyProperties(buddy);
        }

        return null;
    }

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction()
    {
        yield return RoomAttractivenessPanel.Category.All;
    }

    public override void GraspParalyzesPlayer(Creature.Grasp grasp, ref bool paralyzing) => paralyzing = true;

    public override void CorpseIsEdible(Player player, Creature crit, ref bool canEatMeat) => canEatMeat = true;

    public override IEnumerable<string> WorldFileAliases()
    {
        yield return nameof(NTEnums.CreatureType.PortalBuddy);
    }
}