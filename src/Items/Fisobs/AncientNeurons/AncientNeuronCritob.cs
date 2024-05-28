namespace Nyctophobia;

public class AncientNeuronCritob : Critob
{
    public AncientNeuronCritob() : base(NTEnums.CreatureType.AncientNeuron)
    {
        Icon = new SimpleIcon("Symbol_Neuron", Color.yellow);

        LoadedPerformanceCost = 20f;
        SandboxPerformanceCost = new(linear: 0.6f, exponential: 0.1f);
        ShelterDanger = ShelterDanger.Safe;
        CreatureName = "aneuron";

        RegisterUnlock(KillScore.Configurable(1), NTEnums.SandboxUnlock.AncientNeuron);
    }

    public override CreatureTemplate CreateTemplate()
    {
        // CreatureFormula does most of the ugly work for you when creating a new CreatureTemplate,
        // but you can construct a CreatureTemplate manually if you need to.

        CreatureTemplate t = new CreatureFormula(this)
        {
            DefaultRelationship = new(Eats, 0.5f),
            //            neuronrelationship = new(CreatureTemplate.Relationship.Type.Ignores.)
            //should ignore normal neurons
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
        t.abstractedLaziness = 200;
        t.roamBetweenRoomsChance = 0.07f;
        t.bodySize = 0.3f;
        t.stowFoodInDen = false;
        t.shortcutSegments = 2;
        t.grasps = 1;
        t.visualRadius = 800f;
        t.movementBasedVision = 1f;
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

        //are neurons not creatures??
        //self.Ignores(CreatureType.)
    }

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit)
    {
        return new AncientNeuronAI(acrit, acrit.realizedCreature as AncientNeuron);
    }

    public override Creature CreateRealizedCreature(AbstractCreature acrit)
    {
        return new AncientNeuron(acrit);
    }

    public override void ConnectionIsAllowed(AImap map, MovementConnection connection, ref bool? allowed)
    {
        // DLLs don't travel through shortcuts that start and end in the same room—they only travel through room exits.
        // To emulate this behavior, use something like:

        //ShortcutData.Type n = ShortcutData.Type.Normal;
        //if (connection.type == MovementConnection.MovementType.ShortCut) {
        //    allowed &=
        //        connection.startCoord.TileDefined && map.room.shortcutData(connection.StartTile).shortCutType == n ||
        //        connection.destinationCoord.TileDefined && map.room.shortcutData(connection.DestTile).shortCutType == n
        //        ;
        //} else if (connection.type == MovementConnection.MovementType.BigCreatureShortCutSqueeze) {
        //    allowed &=
        //        map.room.GetTile(connection.startCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.StartTile).shortCutType == n ||
        //        map.room.GetTile(connection.destinationCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.DestTile).shortCutType == n
        //        ;
        //}
    }

    public override void TileIsAllowed(AImap map, IntVector2 tilePos, ref bool? allowed)
    {
        // Large creatures like vultures, miros birds, and DLLs need 2 tiles of free space to move around in. Leviathans need 4! None of them can fit in one-tile tunnels.
        // To emulate this behavior, use something like:

        //allowed &= map.IsFreeSpace(tilePos, tilesOfFreeSpace: 2);

        // DLLs can fit into shortcuts despite being fat.
        // To emulate this behavior, use something like:

        //allowed |= map.room.GetTile(tilePos).Terrain == Room.Tile.TerrainType.ShortcutEntrance;
    }

    public override IEnumerable<string> WorldFileAliases()
    {
        yield return "aneuron";
        yield return "aneuron";
    }

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction()
    {
        yield return RoomAttractivenessPanel.Category.Flying;
        yield return RoomAttractivenessPanel.Category.LikesWater;
        yield return RoomAttractivenessPanel.Category.LikesOutside;
    }

    public override string DevtoolsMapName(AbstractCreature acrit)
    {
        return "aneuron";
    }

    public override Color DevtoolsMapColor(AbstractCreature acrit)
    {
        // Default would return the mosquito's icon color (which is gray), which is fine, but red is better.
        return new Color(.7f, .4f, .4f);
    }

    public override ItemProperties Properties(Creature crit)
    //was     public override ItemProperties? Properties(Creature crit)
    {
        // If you don't need the `forObject` parameter, store one ItemProperties instance as a static object and return that.
        // The CentiShields example demonstrates this.
        if (crit is AncientNeuron)
        {
            //    return new AncientNeuronProperties(aneuron);
            return new AncientNeuronProperties();
        }

        return null;
    }
}