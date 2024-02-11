﻿namespace Witness;

public class SLLCritob : Critob
{
    public SLLCritob() : base(NTEnums.CreatureType.ScarletLongLegs)
    {
        Icon = new SimpleIcon("Kill_Daddy", Color.red);
        LoadedPerformanceCost = 60f;
        SandboxPerformanceCost = new SandboxPerformanceCost(0.6f, 0.6f);
        ShelterDanger = ShelterDanger.TooLarge;
        CreatureName = nameof(NTEnums.CreatureType.ScarletLongLegs);
        RegisterUnlock(KillScore.Configurable(20), NTEnums.SandboxUnlock.ScarletLongLegs);
    }

    public override void ConnectionIsAllowed(AImap map, MovementConnection connection, ref bool? allow)
    {
        if (connection.type == MovementConnection.MovementType.ShortCut)
        {
            if (connection.startCoord.TileDefined && map.room.shortcutData(connection.StartTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
            if (connection.destinationCoord.TileDefined && map.room.shortcutData(connection.DestTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
        }
        else if (connection.type == MovementConnection.MovementType.BigCreatureShortCutSqueeze)
        {
            if (map.room.GetTile(connection.startCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.StartTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
            if (map.room.GetTile(connection.destinationCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.DestTile).shortCutType == ShortcutData.Type.Normal)
                allow = true;
        }
    }

    public override void TileIsAllowed(AImap map, IntVector2 tilePos, ref bool? allow) => allow = map.getAItile(tilePos).terrainProximity > 1;

    public override int ExpeditionScore() => 40;

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.red;

    public override string DevtoolsMapName(AbstractCreature acrit) => "SLL";

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => new[] { RoomAttractivenessPanel.Category.LikesInside };

    public override IEnumerable<string> WorldFileAliases() => new[] { "ScarletLongLegs" };

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new DaddyAI(acrit, acrit.world);

    public override CreatureTemplate CreateTemplate()
    {
        var template = new CreatureFormula(CreatureType.DaddyLongLegs, Type, "ScarletLongLEgs")
        {
            TileResistances = new()
            {
                Air = new(1f, Allowed)
            },
            ConnectionResistances = new()
            {
                Standard = new(1f, Allowed),
                ShortCut = new(1f, Allowed),
                BigCreatureShortCutSqueeze = new(10f, Allowed),
                OffScreenMovement = new(1f, Allowed),
                BetweenRooms = new(10f, Allowed)
            },
            DefaultRelationship = new(CreatureTemplate.Relationship.Type.Eats, 1f),
            DamageResistances = new() { Base = 100f, Explosion = .5f },
            StunResistances = new() { Base = 200f, Explosion = .2f },
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(CreatureType.DaddyLongLegs),
        }.IntoTemplate();
        return template;
    }

    public override void EstablishRelationships()
    {
        var daddy = new Relationships(Type);
        daddy.Ignores(Type);
    }

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new DaddyLongLegs(acrit, acrit.world);

    public override CreatureState CreateState(AbstractCreature acrit) => new DaddyLongLegs.DaddyState(acrit);

    public override void LoadResources(RainWorld rainWorld) { }

    public override CreatureType ArenaFallback() => CreatureType.DaddyLongLegs;
}
