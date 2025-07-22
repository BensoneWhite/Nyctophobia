namespace Nyctophobia;

[BepInDependency("slime-cubed.slugbase")]
[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "nyctophobia";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "Nyctophobia";
    public const string VERSION = "0.5.0";

    private bool isInit;

    public static new ManualLogSource Logger;


    public static void DebugWarning(object message) => Logger.LogWarning(message);

    public static void DebugError(object message) => Logger.LogError(message);


    public NTOptionsMenu nTOptionsMenu;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;

            DebugWarning($"{MOD_NAME} is loading.... {VERSION}");

            NTEnums.Init();
            GeneralHooks.Apply();

            DevToolsInit.Apply();
            ApplyCreatures();
            ApplyItems();
            RegisterPomObjects();

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (isInit) return;
            isInit = true;

            DebugWarning($"Initializing OnModsInit {MOD_NAME}");

            LoadAtlases();

            EXHooks.Init();
            WSHooks.Init();

            ESPHooks.Apply();

            HueRemixMenu.Apply();
            SelectMenuHooks.Apply();

            RegisterCustomPassages();

            MachineConnector.SetRegisteredOI(MOD_ID, nTOptionsMenu = new NTOptionsMenu());
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        try
        {
            foreach (var mod in newlyDisabledMods)
            {
                if (mod.id == MOD_ID || mod.id == "moreslugcats")
                {
                    NTEnums.Unregister();
                }
            }
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void ApplyItems()
    {
        BlueLanternHooks.Apply();
        BlueSpearHooks.Apply();
        BlueSpearPlacer.ItemPlacer();
        BlueBombaHooks.Apply();
        CacaoFruitHooks.Apply();
        RedFlareBombHooks.Apply();

        Content.Register(
            new CacaoFruitFisob(),
            new BlueLanternFisob(),
            new BlueSpearFisob(),
            new ImpalerFisob(),
            new BlueBombaFisob(),
            new RedFlareBombFisob()
        );
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
            new AncientNeuronCritob(),
            new SLLCritob()
        );
    }

    private void LoadAtlases()
    {
        var sprites = AssetManager.ListDirectory("nt_atlases")
            .Where(file => Path.GetExtension(file)
            .Equals(".png", StringComparison.OrdinalIgnoreCase));

        foreach (var file in sprites)
        {
            string fileWithoutExtension = Path.ChangeExtension(file, null);
            if (File.Exists(Path.ChangeExtension(file, ".txt")))
                Futile.atlasManager.LoadAtlas(fileWithoutExtension);
            else
                Futile.atlasManager.LoadImage(fileWithoutExtension);
        }
    }

    private void RegisterPomObjects()
    {
        RegisterManagedObject(new ImpalerObj());
        RegisterManagedObject(new LanternStickObj());
        RegisterManagedObject<BlueLanternPlacer, BlueLanternData, ManagedRepresentation>("BlueLantern", MOD_NAME);
        RegisterManagedObject<CacaoFruitPlacer, CacaoFruitData, ManagedRepresentation>("CacaoFruit", MOD_NAME);
        RegisterManagedObject<RedFlareBombPlacer, RedFlareBombData, ManagedRepresentation>("RedFlareBomb", MOD_NAME);
        RegisterManagedObject<BlueSpearPlacer, BlueSpearData, ManagedRepresentation>("BlueSpear", MOD_NAME);
        RegisterManagedObject<BlueBombaPlacer, BlueBombaData, ManagedRepresentation>("BlueBomba", MOD_NAME);
    }

    private void RegisterCustomPassages()
    {
        CustomPassages.Register(
            new EggHatcher()
        );
    }
}