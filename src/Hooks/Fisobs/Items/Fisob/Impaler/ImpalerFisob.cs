namespace Nyctophobia;

public class ImpalerFisob : Fisob
{
    public static readonly ImpalerFisob Instance = new();
    public static readonly ImpalerProperties ImpalerProperties = new();

    public ImpalerFisob() : base(NTEnums.AbstractObjectTypes.Impaler)
    {
        SetIcon();
        RegisterUnlock(NTEnums.SandboxUnlock.Impaler);
    }

    private void SetIcon()
    {
        string iconName = "Symbol_Spear";
        Color iconColor = Color.red;
        Icon = new SimpleIcon(iconName, iconColor);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock unlock) =>
        new ImpalerAbstract(world, null, entitySaveData.Pos, entitySaveData.ID);

    public override ItemProperties Properties(PhysicalObject forObject) => ImpalerProperties;
}