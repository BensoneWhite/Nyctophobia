namespace Nyctophobia;

//ROT BOORING
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
            {
                allow = true;
            }

            if (connection.destinationCoord.TileDefined && map.room.shortcutData(connection.DestTile).shortCutType == ShortcutData.Type.Normal)
            {
                allow = true;
            }
        }
        else if (connection.type == MovementConnection.MovementType.BigCreatureShortCutSqueeze)
        {
            if (map.room.GetTile(connection.startCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.StartTile).shortCutType == ShortcutData.Type.Normal)
            {
                allow = true;
            }

            if (map.room.GetTile(connection.destinationCoord).Terrain == Room.Tile.TerrainType.ShortcutEntrance && map.room.shortcutData(connection.DestTile).shortCutType == ShortcutData.Type.Normal)
            {
                allow = true;
            }
        }
    }

    public override void TileIsAllowed(AImap map, IntVector2 tilePos, ref bool? allow) => allow = map.getTerrainProximity(tilePos) > 1;

    public override int ExpeditionScore() => 20;

    public override Color DevtoolsMapColor(AbstractCreature acrit) => Color.red;

    public override string DevtoolsMapName(AbstractCreature acrit) => nameof(NTEnums.CreatureType.ScarletLongLegs);

    public override IEnumerable<RoomAttractivenessPanel.Category> DevtoolsRoomAttraction() => [RoomAttractivenessPanel.Category.LikesInside];

    public override IEnumerable<string> WorldFileAliases() => [nameof(NTEnums.CreatureType.ScarletLongLegs)];

    public override ArtificialIntelligence CreateRealizedAI(AbstractCreature acrit) => new DaddyAI(acrit, acrit.world);

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = new CreatureFormula(CreatureType.DaddyLongLegs, Type, nameof(NTEnums.CreatureType.ScarletLongLegs))
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
            DefaultRelationship = new(Eats, 1f),
            DamageResistances = new() { Base = 100f, Explosion = .5f },
            StunResistances = new() { Base = 200f, Explosion = .2f },
            HasAI = true,
            Pathing = PreBakedPathing.Ancestral(MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs),
        }.IntoTemplate();
        return template;
    }

    public override void EstablishRelationships()
    {
        Relationships daddy = new(Type);
        daddy.Ignores(Type);
    }

    public override Creature CreateRealizedCreature(AbstractCreature acrit) => new DaddyLongLegs(acrit, acrit.world);

    public override CreatureState CreateState(AbstractCreature acrit) => new DaddyLongLegs.DaddyState(acrit);

    public override CreatureType ArenaFallback() => NTEnums.CreatureType.ScarletLongLegs;
}