namespace Nyctophobia;

public static class CacaoFruitHooks
{
    private const string AttributePrefix = Plugin.MOD_ID + "_CacaoFruitType_";
    private static readonly ConditionalWeakTable<AbstractPhysicalObject, ICacaoFruit> _cfcwt = new();

    public static ICacaoFruit CacaoFruit(this DangleFruit dangleFruit)
    {
        return _cfcwt.TryGetValue(dangleFruit.abstractPhysicalObject, out ICacaoFruit fruit) ? fruit : null;
    }

    public static bool IsCacao(AbstractConsumable abstractConsumable)
    {
        return abstractConsumable.unrecognizedAttributes?.Any(x => x.StartsWith(AttributePrefix)) ?? false;
    }

    public static void MakeCacao(AbstractPhysicalObject dangleFruit, NTEnums.SpecialItemType type)
    {
        dangleFruit.unrecognizedAttributes ??= [];
        string attribute = AttributePrefix + type.value;

        if (!dangleFruit.unrecognizedAttributes.Contains(attribute))
        {
            Array.Resize(ref dangleFruit.unrecognizedAttributes, dangleFruit.unrecognizedAttributes.Length + 1);
            dangleFruit.unrecognizedAttributes[dangleFruit.unrecognizedAttributes.Length - 1] = attribute;
        }

        if (_cfcwt.TryGetValue(dangleFruit, out _))
        {
            _ = _cfcwt.Remove(dangleFruit);
        }

        ICacaoFruit cacaoFruit = GenerateCacaoFruit(type);
        if (cacaoFruit != null)
        {
            _cfcwt.Add(dangleFruit, cacaoFruit);
        }
    }

    public static ICacaoFruit GenerateCacaoFruit(NTEnums.SpecialItemType type)
    {
        return type == NTEnums.SpecialItemType.CacaoFruit ? new CacaoFruit() : (ICacaoFruit)null;
    }

    public static void Apply()
    {
        On.DangleFruit.ctor += DangleFruit_ctor;
        On.DangleFruit.Update += DangleFruit_Update;
        On.PhysicalObject.NewRoom += PhysicalObject_NewRoom;
        On.DangleFruit.BitByPlayer += DangleFruit_BitByPlayer;
        On.DangleFruit.ThrowByPlayer += DangleFruit_ThrowByPlayer;
        On.DangleFruit.InitiateSprites += DangleFruit_InitiateSprites;
        On.DangleFruit.AddToContainer += DangleFruit_AddToContainer;
        On.DangleFruit.ApplyPalette += DangleFruit_ApplyPalette;
        On.DangleFruit.DrawSprites += DangleFruit_DrawSprites;
        _ = new Hook(typeof(DangleFruit).GetProperty(nameof(DangleFruit.FoodPoints))!.GetGetMethod(), DangleFruit_FoodPoints_get);

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
                if (entity is AbstractConsumable obj && obj.type == AbstractPhysicalObject.AbstractObjectType.DangleFruit && Random.value <= 1f / 750 && !IsCacao(obj))
                {
                    MakeCacao(obj, NTEnums.SpecialItemType.CacaoFruit);
                }
            }
        }
    }

    private static void DangleFruit_ctor(On.DangleFruit.orig_ctor orig, DangleFruit self, AbstractPhysicalObject abstractPhysicalObject)
    {
        orig(self, abstractPhysicalObject);
        self.CacaoFruit()?.Init(self);
    }

    private static void DangleFruit_Update(On.DangleFruit.orig_Update orig, DangleFruit self, bool eu)
    {
        orig(self, eu);
        if (self.room != null)
        {
            self.CacaoFruit()?.Update(self, eu);
        }
    }

    private static void PhysicalObject_NewRoom(On.PhysicalObject.orig_NewRoom orig, PhysicalObject self, Room newRoom)
    {
        orig(self, newRoom);

        if (self is DangleFruit dangleFruit)
        {
            dangleFruit.CacaoFruit()?.NewRoom(dangleFruit, newRoom);
        }
    }

    private static void DangleFruit_BitByPlayer(On.DangleFruit.orig_BitByPlayer orig, DangleFruit self, Creature.Grasp grasp, bool eu)
    {
        orig(self, grasp, eu);
        self.CacaoFruit()?.BitByPlayer(self, grasp, eu);
    }

    private static void DangleFruit_ThrowByPlayer(On.DangleFruit.orig_ThrowByPlayer orig, DangleFruit self)
    {
        orig(self);
        self.CacaoFruit()?.ThrowByPlayer(self);
    }

    private static void DangleFruit_InitiateSprites(On.DangleFruit.orig_InitiateSprites orig, DangleFruit self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        self.CacaoFruit()?.InitiateSprites(self, sLeaser, rCam);
    }

    private static void DangleFruit_AddToContainer(On.DangleFruit.orig_AddToContainer orig, DangleFruit self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        orig(self, sLeaser, rCam, newContatiner);
        self.CacaoFruit()?.AddToContainer(self, sLeaser, rCam, newContatiner);
    }

    private static void DangleFruit_ApplyPalette(On.DangleFruit.orig_ApplyPalette orig, DangleFruit self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);
        self.CacaoFruit()?.ApplyPalette(self, sLeaser, rCam, palette);
    }

    private static void DangleFruit_DrawSprites(On.DangleFruit.orig_DrawSprites orig, DangleFruit self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        self.CacaoFruit()?.DrawSprites(self, sLeaser, rCam, timeStacker, camPos);
    }

    private static int DangleFruit_FoodPoints_get(Func<DangleFruit, int> orig, DangleFruit self)
    {
        return self.CacaoFruit()?.FoodPoints ?? orig(self);
    }
}