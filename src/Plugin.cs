#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Nyctophobia;

[BepInDependency("slime-cubed.slugbase")]
[BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(AUTHORS, MOD_NAME, VERSION)]
class Plugin : BaseUnityPlugin
{
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "Nyctophobia";
    public const string VERSION = "0.3.4.16";

    public bool IsInit;
    public bool IsPreInit;
    public bool IsPostInit;

    public static Texture2D TailTextureNW;
    public static Texture2D TailTextureEX;
    public static Texture2D TailTextureWS;

    public void OnEnable()
    {
        Debug.LogWarning($"{MOD_NAME} is loading....");

        try
        {
            On.RainWorld.PreModsInit += RainWorld_PreModsInit;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;

            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
        }
        catch(Exception ex) 
        {
            Logger.LogError(ex);
        }
    }

    private void RainWorld_PreModsInit(On.RainWorld.orig_PreModsInit orig, RainWorld self)
    {
        orig(self);

        try
        {
            if (IsPreInit) return;
            IsPreInit = true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            IsInit = true;

            NTEnums.Init();

            NWHooks.Init();
            EXHooks.Init();
            WSHooks.Init();

            LoadAtlases();

            ApplyCreatures();
            ApplyItems();

            TailTextureNW = new Texture2D(150, 75, TextureFormat.ARGB32, false);
            var tailTextureFile = AssetManager.ResolveFilePath("nt_atlases/nightwalkertail.png");
            if (File.Exists(tailTextureFile))
            {
                var rawData = File.ReadAllBytes(tailTextureFile);
                TailTextureNW.LoadImage(rawData);
            }

            TailTextureEX = new Texture2D(150, 75, TextureFormat.ARGB32, false);
            var ExitailTextureFile = AssetManager.ResolveFilePath("nt_atlases/exiletail.png");
            if (File.Exists(ExitailTextureFile))
            {
                var rawData = File.ReadAllBytes(ExitailTextureFile);
                TailTextureEX.LoadImage(rawData);
            }

            TailTextureWS = new Texture2D(150, 75, TextureFormat.ARGB32, false);
            var WStailTextureFile = AssetManager.ResolveFilePath("nt_atlases/witnesstail.png");
            if (File.Exists(WStailTextureFile))
            {
                var rawData = File.ReadAllBytes(WStailTextureFile);
                TailTextureWS.LoadImage(rawData);
            }
        }

        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);

        for (int i = 0; i < newlyDisabledMods.Length; i++) 
        {
            if (newlyDisabledMods[i].id == "Nyctophobia")
            {
                NTEnums.Unregister();
            }
            if (newlyDisabledMods[i].id == "moreslugcats")
            {
                NTEnums.Unregister();
            }
        }
    }

    private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsPostInit) return;
            IsPostInit = true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void ApplyItems()
    {
        RedFlareBombsHooks.Apply();
        AncientNeuronsHooks.Apply();
        CacaoFruitHooks.Apply();

        Content.Register(
            new CacaoFruitFisob(),
            new AncientNeuronsFisobs(),
            new RedFlareBombFisob());
    }

    private void ApplyCreatures()
    {
        BlackLighMouseHooks.Apply();
        SLLHooks.Apply();
        ScarletLizardHooks.Apply();
        CicadaDronHooks.Apply();
        MiroAlbinoHooks.Apply();
        WitnessPupHooks.Apply();

        Content.Register(
            new WitnessPupCritob(),
            new MiroAlbinoCritob(),
            new CicadaDronCritob(),
            new BlackLighMouseCritob(),
            new ScarletLizardCritob(),
            new SLLCritob());
    }

    private void LoadAtlases()
    {
        foreach (var file in AssetManager.ListDirectory("nt_atlases"))
        {
            if (".png".Equals(Path.GetExtension(file)))
            {
                if (File.Exists(Path.ChangeExtension(file, ".txt")))
                {
                    Futile.atlasManager.LoadAtlas(Path.ChangeExtension(file, null));
                }
                else
                {
                    Futile.atlasManager.LoadImage(Path.ChangeExtension(file, null));
                }
            }
        }
    }
}