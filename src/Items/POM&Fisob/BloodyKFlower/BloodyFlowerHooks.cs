namespace Nyctophobia;

public static class BloodyFlowerHooks
{
    private const string AttributePrefix = Plugin.MOD_ID + "_BloodyKarmaFlowerType_";
    private static readonly ConditionalWeakTable<AbstractPhysicalObject, IBloodyFlower> _bfcwt = new();

    public static IBloodyFlower BloodyFlower(this KarmaFlower flower)
    {
        return _bfcwt.TryGetValue(flower.abstractPhysicalObject, out IBloodyFlower bflower) ? bflower : null;
    }

    public static bool IsBFlower(AbstractConsumable abstractConsumable)
    {
        return abstractConsumable.unrecognizedAttributes?.Any(x => x.StartsWith(AttributePrefix)) ?? false;
    }

    public static void MakeBFlower(AbstractPhysicalObject blodyflower, NTEnums.SpecialItemType type)
    {
        blodyflower.unrecognizedAttributes ??= [];
        string attribute = AttributePrefix + type.value;

        if (!blodyflower.unrecognizedAttributes.Contains(attribute))
        {
            Array.Resize(ref blodyflower.unrecognizedAttributes, blodyflower.unrecognizedAttributes.Length + 1);
            blodyflower.unrecognizedAttributes[blodyflower.unrecognizedAttributes.Length - 1] = attribute;
        }

        if (_bfcwt.TryGetValue(blodyflower, out _))
        {
            _ = _bfcwt.Remove(blodyflower);
        }

        IBloodyFlower bflower = GenerateBFlower(type, blodyflower);
        if (bflower != null)
        {
            _bfcwt.Add(blodyflower, bflower);
        }
    }

    public static IBloodyFlower GenerateBFlower(NTEnums.SpecialItemType type, AbstractPhysicalObject abstractPhysicalObject)
    {
        return type == NTEnums.SpecialItemType.BoodyKarmaFlower ? new BloodyFlower(abstractPhysicalObject) : (IBloodyFlower)null;
    }

    public static void Apply()
    {
        On.KarmaFlower.ctor += KarmaFlower_ctor;
        On.KarmaFlower.Update += KarmaFlower_Update;
        On.PhysicalObject.NewRoom += PhysicalObject_NewRoom;
        On.KarmaFlower.BitByPlayer += KarmaFlower_BitByPlayer;
        On.KarmaFlower.ThrowByPlayer += KarmaFlower_ThrowByPlayer;
        On.KarmaFlower.InitiateSprites += KarmaFlower_InitiateSprites;
        On.KarmaFlower.AddToContainer += KarmaFlower_AddToContainer;
        On.KarmaFlower.ApplyPalette += KarmaFlower_ApplyPalette;
        On.KarmaFlower.DrawSprites += KarmaFlower_DrawSprites;

        _ = new Hook(typeof(KarmaFlower).GetProperty(nameof(KarmaFlower.FoodPoints))!.GetGetMethod(), KarmaFlower_FoodPoints_get);

        On.Room.Loaded += Room_Loaded;
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
        orig(self);

        if (firstTimeRealized && self.game != null)
        {
            foreach (AbstractWorldEntity entity in self.abstractRoom.entities)
            {
                if (entity is AbstractConsumable obj && obj.type == AbstractPhysicalObject.AbstractObjectType.KarmaFlower && Random.value <= 1f / 750 && !IsBFlower(obj))
                {
                    MakeBFlower(obj, NTEnums.SpecialItemType.BoodyKarmaFlower);
                }
            }
        }
    }

    private static int KarmaFlower_FoodPoints_get(Func<KarmaFlower, int> orig, KarmaFlower self)
    {
        return self.BloodyFlower()?.FoodPoints ?? orig(self);
    }

    private static void KarmaFlower_DrawSprites(On.KarmaFlower.orig_DrawSprites orig, KarmaFlower self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        self.BloodyFlower()?.DrawSprites(self, sLeaser, rCam, timeStacker, camPos);
    }

    private static void KarmaFlower_AddToContainer(On.KarmaFlower.orig_AddToContainer orig, KarmaFlower self, SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        self.BloodyFlower()?.AddToContainer(self, sLeaser, rCam, newContatiner);
    }

    private static void KarmaFlower_InitiateSprites(On.KarmaFlower.orig_InitiateSprites orig, KarmaFlower self, SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        self.BloodyFlower()?.InitiateSprites(self, sLeaser, rCam);
    }

    private static void KarmaFlower_ThrowByPlayer(On.KarmaFlower.orig_ThrowByPlayer orig, KarmaFlower self)
    {
        orig(self);
        self.BloodyFlower()?.ThrowByPlayer(self);
    }

    private static void KarmaFlower_BitByPlayer(On.KarmaFlower.orig_BitByPlayer orig, KarmaFlower self, Creature.Grasp grasp, bool eu)
    {
        orig(self, grasp, eu);
        self.BloodyFlower()?.BitByPlayer(self, grasp, eu);
    }

    private static void PhysicalObject_NewRoom(On.PhysicalObject.orig_NewRoom orig, PhysicalObject self, Room newRoom)
    {
        orig(self, newRoom);
        if(self is KarmaFlower flower)
        {
            flower.BloodyFlower()?.NewRoom(flower, newRoom);
        }    
    }

    private static void KarmaFlower_Update(On.KarmaFlower.orig_Update orig, KarmaFlower self, bool eu)
    {
        orig(self, eu);
        self.BloodyFlower()?.Update(self, eu);
    }

    private static void KarmaFlower_ctor(On.KarmaFlower.orig_ctor orig, KarmaFlower self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);
        self.BloodyFlower()?.Init(self);
    }

    private static void KarmaFlower_ApplyPalette(On.KarmaFlower.orig_ApplyPalette orig, KarmaFlower self, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);
        self.BloodyFlower()?.ApplyPalette(self, sLeaser, rCam, palette);
    }
}