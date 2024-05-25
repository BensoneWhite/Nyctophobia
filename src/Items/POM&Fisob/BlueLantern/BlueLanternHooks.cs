namespace Nyctophobia;

public static class BlueLanternHooks
{
    private const string AttributePrefix = Plugin.MOD_ID + "_BlueLanternType_";
    private static readonly ConditionalWeakTable<AbstractPhysicalObject, IBlueLantern> _blcwt = new();

    public static IBlueLantern BlueLantern(this Lantern lantern)
    {
        return _blcwt.TryGetValue(lantern.abstractPhysicalObject, out IBlueLantern blueLantern) ? blueLantern : null;
    }

    public static bool IsBlueLantern(AbstractPhysicalObject abstractConsumable)
    {
        return abstractConsumable.unrecognizedAttributes?.Any(x => x.StartsWith(AttributePrefix)) ?? false;
    }

    public static void MakeLantern(AbstractPhysicalObject lantern, NTEnums.SpecialItemType type, AbstractPhysicalObject abstractConsumable)
    {
        lantern.unrecognizedAttributes ??= [];
        string attribute = AttributePrefix + type.value;

        if (!lantern.unrecognizedAttributes.Contains(attribute))
        {
            Array.Resize(ref lantern.unrecognizedAttributes, lantern.unrecognizedAttributes.Length + 1);
            lantern.unrecognizedAttributes[lantern.unrecognizedAttributes.Length - 1] = attribute;
        }

        if (_blcwt.TryGetValue(lantern, out _))
        {
            _ = _blcwt.Remove(lantern);
        }

        IBlueLantern blueBomba = GenerateBlueLantern(type, abstractConsumable);
        if (blueBomba != null)
        {
            _blcwt.Add(lantern, blueBomba);
        }
    }

    public static IBlueLantern GenerateBlueLantern(NTEnums.SpecialItemType type, AbstractPhysicalObject abstractConsumable)
    {
        return type == NTEnums.SpecialItemType.BlueLantern ? new BlueLantern(abstractConsumable) : null;
    }
    public static void Apply()
    {
        On.PhysicalObject.NewRoom += PhysicalObject_NewRoom;
        On.Lantern.ctor += Lantern_ctor;
        On.Lantern.Update += Lantern_Update;
        On.Lantern.PlaceInRoom += Lantern_PlaceInRoom;
        On.Lantern.HitByWeapon += Lantern_HitByWeapon;
        On.Lantern.TerrainImpact += Lantern_TerrainImpact;
        On.Lantern.InitiateSprites += Lantern_InitiateSprites;
        On.Lantern.DrawSprites += Lantern_DrawSprites;
        On.Lantern.ApplyPalette += Lantern_ApplyPalette;
        On.Lantern.AddToContainer += Lantern_AddToContainer;
        On.Room.Loaded += Room_Loaded;
    }

    private static void PhysicalObject_NewRoom(On.PhysicalObject.orig_NewRoom orig, PhysicalObject self, Room newRoom)
    {
        orig(self, newRoom);
        if(self is Lantern lantern)
        {
            lantern.BlueLantern()?.NewRoom(lantern, newRoom);
        }
    }

    private static void Lantern_ctor(On.Lantern.orig_ctor orig, Lantern self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);
        self.BlueLantern()?.Init(self);
    }

    private static void Lantern_Update(On.Lantern.orig_Update orig, Lantern self, bool eu)
    {
        orig(self, eu);
        self.BlueLantern()?.Update(self, eu);
    }

    private static void Lantern_PlaceInRoom(On.Lantern.orig_PlaceInRoom orig, Lantern self, Room placeRoom)
    {
        orig(self, placeRoom);
        self.BlueLantern()?.PlaceInRoom(self, placeRoom);
    }

    private static void Lantern_HitByWeapon(On.Lantern.orig_HitByWeapon orig, Lantern self, Weapon weapon)
    {
        orig(self, weapon);
        self.BlueLantern()?.HitByWeapon(self, weapon);
    }

    private static void Lantern_TerrainImpact(On.Lantern.orig_TerrainImpact orig, Lantern self, int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        orig(self, chunk, direction, speed, firstContact);
        self.BlueLantern()?.TerrainImpact(self, chunk, direction, speed, firstContact);
    }

    private static void Lantern_InitiateSprites(On.Lantern.orig_InitiateSprites orig, Lantern self, SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        self.BlueLantern()?.InitiateSprites(self, sLeaser, rCam);
    }

    private static void Lantern_DrawSprites(On.Lantern.orig_DrawSprites orig, Lantern self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        self.BlueLantern()?.DrawSprites(self, sLeaser, rCam, timeStacker, camPos);
    }

    private static void Lantern_ApplyPalette(On.Lantern.orig_ApplyPalette orig, Lantern self, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        self.BlueLantern()?.ApplyPalette(self, sLeaser, rCam, palette);

        if (self is BlueLantern)
        {
            sLeaser.sprites[0].color = new Color(0.196f, 0.596f, 0.965f);
            sLeaser.sprites[1].color = new Color(1f, 1f, 1f);
            sLeaser.sprites[2].color = Color.Lerp(new Color(0.196f, 0.596f, 0.965f), new Color(1f, 1f, 1f), 0.3f);
            sLeaser.sprites[3].color = new Color(0.4f, 0.596f, 0.965f);
            if (self.stick != null)
            {
                sLeaser.sprites[4].color = palette.blackColor;
            }
        }
        else
        {
            orig(self, sLeaser, rCam, palette);
        }
    }

    private static void Lantern_AddToContainer(On.Lantern.orig_AddToContainer orig, Lantern self, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        self.BlueLantern()?.AddToContainer(self, sLeaser, rCam, newContatiner);
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
        orig(self);

        if (firstTimeRealized && self.game != null)
        {
            foreach (AbstractWorldEntity entity in self.abstractRoom.entities)
            {
                if (entity is AbstractConsumable obj && obj.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb && Random.value <= 1f / 750 && !IsBlueLantern(obj))
                {
                    MakeLantern(obj, NTEnums.SpecialItemType.Bluebomba, obj);
                }
            }
        }
    }
}