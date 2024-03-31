namespace Nyctophobia;

[BepInDependency("slime-cubed.slugbase")]
[BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(AUTHORS, MOD_NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "Nyctophobia";
    public const string VERSION = "0.3.4.16";

    public bool IsInit;
    public bool IsPreInit;
    public bool IsPostInit;

    public void OnEnable()
    {
        Debug.LogWarning($"{MOD_NAME} is loading....");

        ApplyCreatures();
        ApplyItems();

        try
        {
            On.RainWorld.PreModsInit += RainWorld_PreModsInit;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;

            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogException(ex);
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
            Debug.LogError(ex);
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

            ESPHooks.Apply();
        }

        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError(ex);
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
        try
        {
            RedFlareBombsHooks.Apply();
            AncientNeuronsHooks.Apply();
            CacaoFruitHooks.Apply();

            Content.Register(
                new CacaoFruitFisob(),
                new AncientNeuronsFisobs(),
                new RedFlareBombFisob());
            Debug.LogWarning("Registering Items Nyctophobia");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogException(ex);
            throw new Exception("Nyctophobia creatures failed to load!!");
        }
    }

    private void ApplyCreatures()
    {
        try
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

            Debug.LogWarning("Registering Creatures Nyctophobia");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogWarning(ex);
            throw new Exception("Nyctophobia items failed to load!!");
        }
    }

    private void LoadAtlases()
    {
        foreach (var file in from file in AssetManager.ListDirectory("nt_atlases")
                             where ".png".Equals(Path.GetExtension(file))
                             select file)
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