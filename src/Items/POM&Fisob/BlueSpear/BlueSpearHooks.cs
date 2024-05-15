namespace Nyctophobia;

public static class BlueSpearHooks
{
    private const string AttributePrefix = Plugin.MOD_ID + "_BlueSpearType_";
    private static readonly ConditionalWeakTable<AbstractPhysicalObject, IBlueSpear> _bscwt = new();

    public static IBlueSpear BlueSpear(this ExplosiveSpear spear)
    {
        return _bscwt.TryGetValue(spear.abstractPhysicalObject, out IBlueSpear blueSpear) ? blueSpear : null;
    }

    public static bool IsBlueSpear(AbstractConsumable abstractConsumable)
    {
        return abstractConsumable.unrecognizedAttributes?.Any(x => x.StartsWith(AttributePrefix)) ?? false;
    }

    public static void MakeSpear(AbstractPhysicalObject spear, NTEnums.SpecialItemType type, AbstractConsumable abstractConsumable, World world)
    {
        spear.unrecognizedAttributes ??= [];
        string attribute = AttributePrefix + type.value;

        if (!spear.unrecognizedAttributes.Contains(attribute))
        {
            Array.Resize(ref spear.unrecognizedAttributes, spear.unrecognizedAttributes.Length + 1);
            spear.unrecognizedAttributes[spear.unrecognizedAttributes.Length - 1] = attribute;
        }

        if (_bscwt.TryGetValue(spear, out _))
        {
            _ = _bscwt.Remove(spear);
        }

        IBlueSpear blueSpear = GenerateBlueSpear(type, abstractConsumable, world);
        if (blueSpear != null)
        {
            _bscwt.Add(spear, blueSpear);
        }
    }

    public static IBlueSpear GenerateBlueSpear(NTEnums.SpecialItemType type, AbstractConsumable abstractConsumable, World world)
    {
        return type == NTEnums.SpecialItemType.BlueSpear ? new BlueSpear(abstractConsumable, world) : null;
    }

    public static void Apply()
    {
        On.ExplosiveSpear.ctor += ExplosiveSpear_ctor;
        On.PhysicalObject.NewRoom += PhysicalObject_NewRoom;
        On.ExplosiveSpear.PlaceInRoom += ExplosiveSpear_PlaceInRoom;
        On.ExplosiveSpear.NewRoom += ExplosiveSpear_NewRoom;
        On.ExplosiveSpear.Update += ExplosiveSpear_Update;
        On.ExplosiveSpear.WeaponDeflect += ExplosiveSpear_WeaponDeflect;
        On.ExplosiveSpear.ConchunkWeight += ExplosiveSpear_ConchunkWeight;
        On.ExplosiveSpear.MiniExplode += ExplosiveSpear_MiniExplode;
        On.ExplosiveSpear.Explode += ExplosiveSpear_Explode;
        On.ExplosiveSpear.HitByExplosion += ExplosiveSpear_HitByExplosion;
        On.ExplosiveSpear.InitiateSprites += ExplosiveSpear_InitiateSprites;
        On.ExplosiveSpear.DrawSprites += ExplosiveSpear_DrawSprites;
        On.ExplosiveSpear.ApplyPalette += ExplosiveSpear_ApplyPalette;
        On.Room.Loaded += Room_Loaded;
    }

    private static void ExplosiveSpear_ctor(On.ExplosiveSpear.orig_ctor orig, ExplosiveSpear self, AbstractPhysicalObject abstractPhysicalObject, World world)
    {
        orig(self, abstractPhysicalObject, world);
        self.BlueSpear()?.Init(self);
    }

    private static void PhysicalObject_NewRoom(On.PhysicalObject.orig_NewRoom orig, PhysicalObject self, Room newRoom)
    {
        orig(self, newRoom);
        if (self is ExplosiveSpear spear)
        {
            spear.BlueSpear()?.NewRoom(spear, newRoom);
        }
    }

    private static void ExplosiveSpear_PlaceInRoom(On.ExplosiveSpear.orig_PlaceInRoom orig, ExplosiveSpear self, Room placeRoom)
    {
        orig(self, placeRoom);
        self.BlueSpear()?.PlaceInRoom(self, placeRoom);
    }

    private static void ExplosiveSpear_NewRoom(On.ExplosiveSpear.orig_NewRoom orig, ExplosiveSpear self, Room newRoom)
    {
        orig(self, newRoom);
        self.BlueSpear()?.NewRoom(self, newRoom);
    }

    private static void ExplosiveSpear_Update(On.ExplosiveSpear.orig_Update orig, ExplosiveSpear self, bool eu)
    {
        orig(self, eu);
        self.BlueSpear()?.Update(self, eu);
    }

    private static void ExplosiveSpear_WeaponDeflect(On.ExplosiveSpear.orig_WeaponDeflect orig, ExplosiveSpear self, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    {
        orig(self, inbetweenPos, deflectDir, bounceSpeed);
        self.BlueSpear()?.WeaponDeflect(self, inbetweenPos, deflectDir, bounceSpeed);
    }

    private static float ExplosiveSpear_ConchunkWeight(On.ExplosiveSpear.orig_ConchunkWeight orig, ExplosiveSpear self, Vector2 pushDir, BodyChunkConnection con)
    {
        self.BlueSpear()?.ConchunkWeight(self, pushDir, con);
        return orig(self, pushDir, con);
    }

    private static void ExplosiveSpear_MiniExplode(On.ExplosiveSpear.orig_MiniExplode orig, ExplosiveSpear self)
    {
        orig(self);
        self.BlueSpear()?.MiniExplode(self);
    }

    private static void ExplosiveSpear_Explode(On.ExplosiveSpear.orig_Explode orig, ExplosiveSpear self)
    {
        orig(self);
        self.BlueSpear()?.Explode(self);
    }

    private static void ExplosiveSpear_HitByExplosion(On.ExplosiveSpear.orig_HitByExplosion orig, ExplosiveSpear self, float hitFac, Explosion explosion, int hitChunk)
    {
        orig(self, hitFac, explosion, hitChunk);
        self.BlueSpear()?.HitByExplosion(self, hitFac, explosion, hitChunk);
    }

    private static void ExplosiveSpear_InitiateSprites(On.ExplosiveSpear.orig_InitiateSprites orig, ExplosiveSpear self, SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        self.BlueSpear()?.InitiateSprites(self, sLeaser, rCam);
    }

    private static void ExplosiveSpear_DrawSprites(On.ExplosiveSpear.orig_DrawSprites orig, ExplosiveSpear self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        self.BlueSpear()?.DrawSprites(self, sLeaser, rCam, timeStacker, camPos);
    }

    private static void ExplosiveSpear_ApplyPalette(On.ExplosiveSpear.orig_ApplyPalette orig, ExplosiveSpear self, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);
        self.BlueSpear()?.ApplyPallete(self, sLeaser, rCam, palette);
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
        orig(self);

        if (firstTimeRealized && self.game != null)
        {
            foreach (AbstractWorldEntity entity in self.abstractRoom.entities)
            {
                if (entity is AbstractConsumable obj && obj.type == AbstractPhysicalObject.AbstractObjectType.Spear && Random.value <= 1f / 750 && !IsBlueSpear(obj))
                {
                    MakeSpear(obj, NTEnums.SpecialItemType.BlueSpear, obj, self.world);
                }
            }
        }
    }
}