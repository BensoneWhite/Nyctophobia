namespace Nyctophobia;

public static class BlueBombaHooks
{
    private const string AttributePrefix = Plugin.MOD_ID + "_BlueBombaType_";
    private static readonly ConditionalWeakTable<AbstractPhysicalObject, IBlueBomba> _bbcwt = new();

    public static IBlueBomba BlueBomba(this ScavengerBomb bomba)
    {
        return _bbcwt.TryGetValue(bomba.abstractPhysicalObject, out IBlueBomba blueBomba) ? blueBomba : null;
    }

    public static bool IsBlueBomba(AbstractConsumable abstractConsumable)
    {
        return abstractConsumable.unrecognizedAttributes?.Any(x => x.StartsWith(AttributePrefix)) ?? false;
    }

    public static void MakeBomba(AbstractPhysicalObject bomba, NTEnums.SpecialItemType type, AbstractConsumable abstractConsumable, World world)
    {
        bomba.unrecognizedAttributes ??= [];
        string attribute = AttributePrefix + type.value;

        if (!bomba.unrecognizedAttributes.Contains(attribute))
        {
            Array.Resize(ref bomba.unrecognizedAttributes, bomba.unrecognizedAttributes.Length + 1);
            bomba.unrecognizedAttributes[bomba.unrecognizedAttributes.Length - 1] = attribute;
        }

        if (_bbcwt.TryGetValue(bomba, out _))
        {
            _ = _bbcwt.Remove(bomba);
        }

        IBlueBomba blueBomba = GenerateBlueBomba(type, abstractConsumable, world);
        if (blueBomba != null)
        {
            _bbcwt.Add(bomba, blueBomba);
        }
    }

    public static IBlueBomba GenerateBlueBomba(NTEnums.SpecialItemType type, AbstractConsumable abstractConsumable, World world)
    {
        return type == NTEnums.SpecialItemType.Bluebomba ? new BlueBomba(abstractConsumable, world) : null;
    }

    public static void Apply()
    {
        On.ScavengerBomb.InitiateBurn += ScavengerBomb_InitiateBurn;
        On.ScavengerBomb.ctor += ScavengerBomb_ctor;
        On.PhysicalObject.NewRoom += PhysicalObject_NewRoom;
        On.ScavengerBomb.Update += ScavengerBomb_Update;
        On.ScavengerBomb.TerrainImpact += ScavengerBomb_TerrainImpact;
        On.ScavengerBomb.HitSomething += ScavengerBomb_HitSomething;
        On.ScavengerBomb.Thrown += ScavengerBomb_Thrown;
        On.ScavengerBomb.PickedUp += ScavengerBomb_PickedUp;
        On.ScavengerBomb.HitByWeapon += ScavengerBomb_HitByWeapon;
        On.ScavengerBomb.WeaponDeflect += ScavengerBomb_WeaponDeflect;
        On.ScavengerBomb.HitByExplosion += ScavengerBomb_HitByExplosion;
        On.ScavengerBomb.Explode += ScavengerBomb_Explode;
        On.ScavengerBomb.InitiateSprites += ScavengerBomb_InitiateSprites;
        On.ScavengerBomb.DrawSprites += ScavengerBomb_DrawSprites;
        On.ScavengerBomb.ApplyPalette += ScavengerBomb_ApplyPalette;
        On.ScavengerBomb.UpdateColor += ScavengerBomb_UpdateColor;
        On.Room.Loaded += Room_Loaded;
    }

    private static void ScavengerBomb_InitiateBurn(On.ScavengerBomb.orig_InitiateBurn orig, ScavengerBomb self)
    {
        orig(self);
        self.BlueBomba()?.InitiateBurn(self);
    }

    private static void PhysicalObject_NewRoom(On.PhysicalObject.orig_NewRoom orig, PhysicalObject self, Room newRoom)
    {
        orig(self, newRoom);

        if(self is ScavengerBomb bomb)
        {
            bomb.BlueBomba()?.NewRoom(bomb, newRoom);
        }
    }

    private static void ScavengerBomb_ctor(On.ScavengerBomb.orig_ctor orig, ScavengerBomb self, AbstractPhysicalObject abstractPhysicalObject, World world)
    {
        orig(self, abstractPhysicalObject, world);
        self.BlueBomba()?.Init(self);
    }

    private static void ScavengerBomb_Update(On.ScavengerBomb.orig_Update orig, ScavengerBomb self, bool eu)
    {
        orig(self, eu);
        self.BlueBomba()?.Update(self, eu);
    }

    private static void ScavengerBomb_TerrainImpact(On.ScavengerBomb.orig_TerrainImpact orig, ScavengerBomb self, int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        orig(self, chunk, direction, speed, firstContact);
        self.BlueBomba()?.TerrainImpact(self, chunk, direction, speed, firstContact);
    }

    private static bool ScavengerBomb_HitSomething(On.ScavengerBomb.orig_HitSomething orig, ScavengerBomb self, CollisionResult result, bool eu)
    {
        self.BlueBomba()?.HitSomething(self, result, eu);
        return orig(self, result, eu);

    }

    private static void ScavengerBomb_Thrown(On.ScavengerBomb.orig_Thrown orig, ScavengerBomb self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        self.BlueBomba()?.Thrown(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    private static void ScavengerBomb_PickedUp(On.ScavengerBomb.orig_PickedUp orig, ScavengerBomb self, Creature upPicker)
    {
        orig(self, upPicker);
        self.BlueBomba()?.PickedUp(self, upPicker);
    }

    private static void ScavengerBomb_HitByWeapon(On.ScavengerBomb.orig_HitByWeapon orig, ScavengerBomb self, Weapon weapon)
    {
        orig(self, weapon);
        self.BlueBomba()?.HitByWeapon(self, weapon);
    }

    private static void ScavengerBomb_WeaponDeflect(On.ScavengerBomb.orig_WeaponDeflect orig, ScavengerBomb self, Vector2 inbetweenPos, Vector2 deflectDir, float bounceSpeed)
    {
        orig(self, inbetweenPos, deflectDir, bounceSpeed);
        self.BlueBomba()?.WeaponDeflect(self, inbetweenPos, deflectDir, bounceSpeed);
    }

    private static void ScavengerBomb_HitByExplosion(On.ScavengerBomb.orig_HitByExplosion orig, ScavengerBomb self, float hitFac, Explosion explosion, int hitChunk)
    {
        orig(self, hitFac, explosion, hitChunk);
        self.BlueBomba()?.HitByExplosion(self, hitFac, explosion, hitChunk);
    }

    private static void ScavengerBomb_Explode(On.ScavengerBomb.orig_Explode orig, ScavengerBomb self, BodyChunk hitChunk)
    {
        orig(self, hitChunk);
        self.BlueBomba()?.Explode(self, hitChunk);
    }

    private static void ScavengerBomb_InitiateSprites(On.ScavengerBomb.orig_InitiateSprites orig, ScavengerBomb self, SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        self.BlueBomba()?.InitiateSprites(self, sLeaser, rCam);
    }

    private static void ScavengerBomb_DrawSprites(On.ScavengerBomb.orig_DrawSprites orig, ScavengerBomb self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        self.BlueBomba()?.DrawSprites(self, sLeaser, rCam, timeStacker, camPos);
    }

    private static void ScavengerBomb_ApplyPalette(On.ScavengerBomb.orig_ApplyPalette orig, ScavengerBomb self, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);
        self.BlueBomba()?.ApplyPalette(self, sLeaser, rCam, palette);
    }

    private static void ScavengerBomb_UpdateColor(On.ScavengerBomb.orig_UpdateColor orig, ScavengerBomb self, SpriteLeaser sLeaser, Color col)
    {
        orig(self, sLeaser, col);
        self.BlueBomba()?.UpdateColor(self, sLeaser, col);
    }

    private static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
    {
        bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
        orig(self);

        if (firstTimeRealized && self.game != null)
        {
            foreach (AbstractWorldEntity entity in self.abstractRoom.entities)
            {
                if (entity is AbstractConsumable obj && obj.type == AbstractObjectType.ScavengerBomb && Random.value <= 1f / 750 && !IsBlueBomba(obj))
                {
                    MakeBomba(obj, NTEnums.SpecialItemType.Bluebomba, obj, self.world);
                }
            }
        }
    }
}