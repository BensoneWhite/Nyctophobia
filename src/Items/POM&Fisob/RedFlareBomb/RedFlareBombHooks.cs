namespace Nyctophobia;

public static class RedFlareBombHooks
{
    private const string AttributePrefix = Plugin.MOD_ID + "_FlareBombType_";
    private static readonly ConditionalWeakTable<AbstractPhysicalObject, IRedFlareBomb> _rfcwt = new();

    public static IRedFlareBomb RedFlare(this FlareBomb flare)
    {
        return _rfcwt.TryGetValue(flare.abstractPhysicalObject, out IRedFlareBomb redFlare) ? redFlare : null;
    }

    public static bool IsFlare(AbstractConsumable abstractConsumable)
    {
        return abstractConsumable.unrecognizedAttributes?.Any(x => x.StartsWith(AttributePrefix)) ?? false;
    }

    public static void MakeFlare(AbstractPhysicalObject flareBomb, NTEnums.SpecialItemType type, AbstractConsumable abstractConsumable, World world)
    {
        flareBomb.unrecognizedAttributes ??= [];
        string attribute = AttributePrefix + type.value;

        if (!flareBomb.unrecognizedAttributes.Contains(attribute))
        {
            Array.Resize(ref flareBomb.unrecognizedAttributes, flareBomb.unrecognizedAttributes.Length + 1);
            flareBomb.unrecognizedAttributes[flareBomb.unrecognizedAttributes.Length - 1] = attribute;
        }

        if (_rfcwt.TryGetValue(flareBomb, out _))
        {
            _ = _rfcwt.Remove(flareBomb);
        }

        IRedFlareBomb redFlareBomb = GenerateRedFlare(type, abstractConsumable, world);
        if (redFlareBomb != null)
        {
            _rfcwt.Add(flareBomb, redFlareBomb);
        }
    }

    public static IRedFlareBomb GenerateRedFlare(NTEnums.SpecialItemType type, AbstractConsumable abstractConsumable, World world)
    {
        return type == NTEnums.SpecialItemType.RedFlareBomb ? new RedFlareBomb(abstractConsumable, world) : null;
    }

    public static void Apply()
    {
        On.FlareBomb.ctor += FlareBomb_ctor;
        On.FlareBomb.Update += FlareBomb_Update;
        On.PhysicalObject.NewRoom += PhysicalObject_NewRoom;
        On.FlareBomb.HitSomething += FlareBomb_HitSomething;
        On.FlareBomb.Thrown += FlareBomb_Thrown;
        On.FlareBomb.PickedUp += FlareBomb_PickedUp;
        On.FlareBomb.InitiateSprites += FlareBomb_InitiateSprites;
        On.FlareBomb.AddToContainer += FlareBomb_AddToContainer;
        On.FlareBomb.ApplyPalette += FlareBomb_ApplyPalette;
        On.FlareBomb.DrawSprites += FlareBomb_DrawSprites;
        On.Room.Loaded += Room_Loaded;
    }

    private static void FlareBomb_ctor(On.FlareBomb.orig_ctor orig, FlareBomb self, AbstractPhysicalObject abstractPhysicalObject, World world)
    {
        orig(self, abstractPhysicalObject, world);
        self.RedFlare()?.Init(self);
    }

    private static void FlareBomb_Update(On.FlareBomb.orig_Update orig, FlareBomb self, bool eu)
    {
        orig(self, eu);
        if (self.room != null)
        {
            self.RedFlare()?.Update(self, eu);
        }
    }

    private static void PhysicalObject_NewRoom(On.PhysicalObject.orig_NewRoom orig, PhysicalObject self, Room newRoom)
    {
        orig(self, newRoom);
        if(self is FlareBomb flareBomb)
        {
            flareBomb.RedFlare()?.NewRoom(flareBomb, newRoom);
        }
    }

    private static bool FlareBomb_HitSomething(On.FlareBomb.orig_HitSomething orig, FlareBomb self, CollisionResult result, bool eu)
    {
        self.RedFlare()?.HitSomething(self, result, eu);
        return orig(self, result, eu);
    }

    private static void FlareBomb_Thrown(On.FlareBomb.orig_Thrown orig, FlareBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        self.RedFlare()?.Thrown(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    private static void FlareBomb_PickedUp(On.FlareBomb.orig_PickedUp orig, FlareBomb self, Creature upPicker)
    {
        orig(self, upPicker);
        self.RedFlare()?.PickedUp(self, upPicker);
    }

    private static void FlareBomb_InitiateSprites(On.FlareBomb.orig_InitiateSprites orig, FlareBomb self, SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        self.RedFlare()?.InitiateSprites(self, sLeaser, rCam);
    }

    private static void FlareBomb_AddToContainer(On.FlareBomb.orig_AddToContainer orig, FlareBomb self, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        self.RedFlare()?.AddToContainer(self, sLeaser, rCam, newContatiner);
    }

    private static void FlareBomb_ApplyPalette(On.FlareBomb.orig_ApplyPalette orig, FlareBomb self, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);
        self.RedFlare()?.ApplyPalette(self, sLeaser, rCam, palette);
    }

    private static void FlareBomb_DrawSprites(On.FlareBomb.orig_DrawSprites orig, FlareBomb self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        self.RedFlare()?.DrawSprites(self, sLeaser, rCam, timeStacker, camPos);
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
        orig(self);

        if (firstTimeRealized && self.game != null)
        {
            foreach (AbstractWorldEntity entity in self.abstractRoom.entities)
            {
                if (entity is AbstractConsumable obj && obj.type == AbstractPhysicalObject.AbstractObjectType.FlareBomb && Random.value <= 1f / 750 && !IsFlare(obj))
                {
                    MakeFlare(obj, NTEnums.SpecialItemType.RedFlareBomb, obj, self.world);
                }
            }
        }
    }
}