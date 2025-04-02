namespace Nyctophobia;

public class BlueSpearData(PlacedObject owner) : ManagedData(owner, [])
{
    [BooleanField(nameof(RandomPlacement), false, ManagedFieldWithPanel.ControlType.button, "Random Placement")]
    public bool RandomPlacement;

    [FloatField(nameof(RandomChange), 1f, 1f, 1f, 0.01f, ManagedFieldWithPanel.ControlType.slider, "Random Spawn Change")]
    public float RandomChange;
}

public class BlueSpearPlacer : UpdatableAndDeletable
{
    public BlueSpearPlacer(Room room, PlacedObject placedObject)
    {
        BlueSpearData data = (BlueSpearData)placedObject.data;

        if (room.abstractRoom.firstTimeRealized)
        {
            if (room.game != null && Random.value < data.RandomChange)
            {
                IntVector2 spawnTile = room.RandomTile();
                var randomTile = new WorldCoordinate(room.abstractRoom.index, spawnTile.x, spawnTile.y, -1);

                BlueSpearAbstract blueSpear = new(room.world, null, data.RandomPlacement ? randomTile : room.GetWorldCoordinate(placedObject.pos), room.game.GetNewID(), true, 0f);
                room.abstractRoom.AddEntity(blueSpear);
            }
        }
    }

    public static void ItemPlacer()
    {
        On.Room.Loaded += Room_Loaded;
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room room)
    {
        var itemSpawnChance = 0.1f;

        if (room.abstractRoom.firstTimeRealized)
        {
            if (!room.abstractRoom.shelter && !room.abstractRoom.gate && room.game != null)
            {
                for (var tile = (int)((float)room.TileHeight * room.TileWidth * Mathf.Pow(room.roomSettings.RandomItemDensity * room.roomSettings.RandomItemSpearChance * itemSpawnChance, 2f) / 5f); tile >= 0; tile--)
                {
                    IntVector2 spawnTile = room.RandomTile();

                    if (!room.GetTile(spawnTile).Solid)
                    {
                        bool canSpawnHere = true;

                        for (int j = -1; j < 2; j++)
                        {
                            if (!room.GetTile(spawnTile + new IntVector2(j, -1)).Solid)
                            {
                                canSpawnHere = false;
                                break;
                            }
                        }
                        if (canSpawnHere)
                        {
                            EntityID newID = room.game.GetNewID(-room.abstractRoom.index);
                            var entity = new BlueSpearAbstract(room.world, null, new WorldCoordinate(room.abstractRoom.index, spawnTile.x, spawnTile.y, -1), newID, true, 0f);
                            room.abstractRoom.AddEntity(entity);
                        }
                    }
                }
            }
        }

        orig(room);
    }
}