namespace Nyctophobia;

public class AncientNeuronCritob : Critob
{
    public AncientNeuronCritob() : base(NTEnums.CreatureType.AncientNeuron)
    {
        Icon = new SimpleIcon("Symbol_Neuron", Color.yellow);

        LoadedPerformanceCost = 20f;
        SandboxPerformanceCost = new(linear: 0.6f, exponential: 0.1f);
        ShelterDanger = ShelterDanger.Safe;
        CreatureName = nameof(NTEnums.CreatureType.AncientNeuron);

        RegisterUnlock(KillScore.Configurable(1), NTEnums.SandboxUnlock.AncientNeuron);
    }

    public override CreatureTemplate CreateTemplate()
    {
        // CreatureFormula does most of the ugly work for you when creating a new CreatureTemplate,
        // but you can construct a CreatureTemplate manually if you need to.

        CreatureTemplate t = new CreatureFormula(this)
        {
            DefaultRelationship = new(Eats, 0.5f),
            //fuckfuckfuckfuck why dont you have ai has ai is set to true what the fuck is wrong with you I swear to god
            HasAI = true,
            InstantDeathDamage = 1,
            Pathing = PreBakedPathing.Ancestral(CreatureType.Fly),
            TileResistances = new()
            {
                Air = new(1, Allowed),
            },
            ConnectionResistances = new()
            {
                Standard = new(1, Allowed),
                OpenDiagonal = new(1, Allowed),
                ShortCut = new(1, Allowed),
                NPCTransportation = new(10, Allowed),
                OffScreenMovement = new(1, Allowed),
                BetweenRooms = new(1, Allowed),
            },
            DamageResistances = new()
            {
                Base = .1f,
            },
            StunResistances = new()
            {
                Base = 0.3f,
            }
        }.IntoTemplate();

        // The below properties are derived from vanilla creatures, so you should have your copy of the decompiled source code handy.

        // Some notes on the fields of CreatureTemplate:

        // offScreenSpeed       how fast the creature moves between abstract rooms
        // abstractLaziness     how long it takes the creature to start migrating
        // smallCreature        determines if rocks instakill, if large predators ignore it, etc
        // dangerToPlayer       DLLs are 0.85, spiders are 0.1, pole plants are 0.5
        // waterVision          0..1 how well the creature can see through water
        // throughSurfaceVision 0..1 how well the creature can see through water surfaces
        // movementBasedVision  0..1 bonus to vision for moving creatures
        // lungCapacity         ticks until the creature falls unconscious from drowning
        // quickDeath           determines if the creature should die as determined by Creature.Violence(). if false, you must define custom death logic
        // saveCreature         determines if the creature is saved after a cycle ends. false for overseers and garbage worms
        // hibernateOffScreen   true for deer, miros birds, leviathans, vultures, and scavengers
        // bodySize             batflies are 0.1, eggbugs are 0.4, DLLs are 5.5, slugcats are 1

        t.offScreenSpeed = 0.1f;
        t.abstractedLaziness = 0;
        t.roamBetweenRoomsChance = 0.03f;
        t.bodySize = 0.3f;
        t.stowFoodInDen = false;
        t.shortcutSegments = 2;
        t.grasps = 1;
        t.movementBasedVision = 5f;
        t.visualRadius = 1300f;
        //        t.communityInfluence = 0.1f;
        t.canFly = true;
        t.meatPoints = 4;
        t.dangerousToPlayer = 0.5f;

        return t;
    }

    public override void EstablishRelationships()
    {
        // You can use StaticWorld.EstablishRelationship, but the Relationships class exists to make this process more ergonomic.

        Relationships self = new(NTEnums.CreatureType.AncientNeuron);

        foreach (var template in StaticWorld.creatureTemplates)
        {
            if (template.quantified)
            {
                self.Ignores(template.type);
                self.IgnoredBy(template.type);
            }
        }
        self.Eats(CreatureType.BrotherLongLegs,1f);
        self.Eats(CreatureType.DaddyLongLegs,1f);
        self.Eats(CreatureType.Slugcat,1f);

        //are neurons not creatures??
        //self.Ignores(CreatureType.)
    }
//why the fuck is there no ai in this thing aaaa it literally overrides ai with that thingy but like just f-ing gimme the shite ya did
    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new AncientNeuronAI(acrit, acrit.realizedCreature as AncientNeuron);

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new AncientNeuron(acrit);


    public override IEnumerable<string> WorldFileAliases() => [nameof(NTEnums.CreatureType.AncientNeuron)];


    public override string DevtoolsMapName(AbstractCreature acrit) => nameof(NTEnums.CreatureType.AncientNeuron);

    // Default would return the mosquito's icon color (which is gray), which is fine, but red is better.
    public override Color DevtoolsMapColor(AbstractCreature acrit) => new(1f, 1f, 1f);

    public override ItemProperties Properties(Creature crit)
    //was     public override ItemProperties? Properties(Creature crit)
    {
        // If you don't need the `forObject` parameter, store one ItemProperties instance as a static object and return that.
        // The CentiShields example demonstrates this.
        if (crit is AncientNeuron) return new AncientNeuronProperties();

        return null;
    }
}