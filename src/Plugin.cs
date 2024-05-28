namespace Nyctophobia;

[BepInDependency("slime-cubed.slugbase")]
[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "nyctophobia";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "Nyctophobia";
    public const string VERSION = "0.4.6";

    public bool IsInit;
    public bool IsPreInit;
    public bool IsPostInit;

    public static void DebugLog(object ex) => Logger.LogInfo(ex);

    public static void DebugWarning(object ex) => Logger.LogWarning(ex);

    public static void DebugError(object ex) => Logger.LogError(ex);

    public static void DebugFatal(object ex) => Logger.LogFatal(ex);

    public new static ManualLogSource Logger;

    public NTOptionsMenu nTOptionsMenu;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;
            DebugLog($"{MOD_NAME} is loading.... {VERSION}");

            CheckFestiveDates();

            NTEnums.Init();

            ApplyCreatures();
            ApplyItems();

            On.RainWorld.PreModsInit += RainWorld_PreModsInit;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;

            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
        }
        catch (Exception ex)
        {
            DebugError(ex);
            Debug.LogException(ex);
        }
    }

    private void CheckFestiveDates()
    {
        DateTime today = DateTime.Today;

        Constants.IsChristmas = (today == Constants.christmas);
        Constants.IsNewYear = (today == Constants.newYear);

        Constants.IsFestive = Constants.IsChristmas || 
                              Constants.IsNewYear;

        if (Constants.IsFestive)
        {
            if (Constants.IsChristmas)
            {
                DebugLog($"{MOD_NAME} is in Christmas mode!");
            }
            if (Constants.IsNewYear)
            {
                DebugLog($"{MOD_NAME} is in New Year mode!");
            }
        }
    }

    private void RainWorld_PreModsInit(On.RainWorld.orig_PreModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsPreInit)
            {
                return;
            }

            IsPreInit = true;
        }
        catch (Exception ex)
        {
            DebugError(ex);
            Debug.LogException(ex);
        }
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;

            NWHooks.Init();
            EXHooks.Init();
            WSHooks.Init();

            LoadAtlases();
            PomObjects();

            ESPHooks.Apply();
            GeneralHooks.Apply();
            HueRemixMenu.Apply();
            SelectMenuHooks.Apply();

            DevToolsInit.Apply();

            _ = MachineConnector.SetRegisteredOI(MOD_ID, nTOptionsMenu = new NTOptionsMenu());

            if (!WeakTables.NyctoShaders.TryGetValue(self, out var _)) WeakTables.NyctoShaders.Add(self, _ = new WeakTables.Shaders());

            if(WeakTables.NyctoShaders.TryGetValue(self, out var shaders))
            {
                shaders.ShaderPack = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("assetsbundles/shaderpack"/*, false*/));

                if(shaders.ShaderPack != null )
                {
                    MethodHelpers.InPlaceTryCatch(ref shaders.desaturation, FShader.CreateShader("Desaturation", shaders.ShaderPack.LoadAsset<Shader>("Assets/")), $"{MOD_NAME} Shader: Desaturation, Failed to set!");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            DebugError(ex);
        }
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        try
        {
            for (int i = 0; i < newlyDisabledMods.Length; i++)
            {
                if (newlyDisabledMods[i].id == MOD_ID)
                {
                    NTEnums.Unregister();
                }
                if (newlyDisabledMods[i].id == "moreslugcats")
                {
                    NTEnums.Unregister();
                }
            }
            DebugLog("Unregister.... Nyctophobia creatures and items");
        }
        catch (Exception ex)
        {
            DebugError(ex);
            Debug.LogException(ex);
        }
    }

    private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsPostInit)
            {
                return;
            }
            IsPostInit = true;

            //_ = new Hook(typeof(Player).GetProperty(nameof(Player.isSlugpup))!.GetGetMethod(), NWHooks.IsSlugpupOverride_NightWalker_get);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            DebugError(ex);
        }
    }

    private void ApplyItems()
    {
        try
        {
            BlueLanternHooks.Apply();
            BlueSpearHooks.Apply();
            BlueBombaHooks.Apply();
            AncientNeuronsHooks.Apply();
            CacaoFruitHooks.Apply();
            BloodyFlowerHooks.Apply();
            RedFlareBombHooks.Apply();

            Content.Register(
                new BloodyFlowerFisob(),
                new CacaoFruitFisob(),
                new BlueLanternFisob(),
                new BlueSpearFisob(),
                new BlueBombaFisob(),
                new AncientNeuronsFisobs(),
                new RedFlareBombFisob());
            DebugLog("Registering Items Nyctophobia");
        }
        catch (Exception ex)
        {
            DebugError(ex);
            Debug.LogException(ex);
            throw new Exception("Nyctophobia Items failed to load!!");
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
                new BoyKisserCritob(),
                new WitnessPupCritob(),
                new MiroAlbinoCritob(),
                new CicadaDronCritob(),
                new BlackLighMouseCritob(),
                new ScarletLizardCritob(),
                new SLLCritob());

            DebugLog("Registering Creatures Nyctophobia");
        }
        catch (Exception ex)
        {
            DebugError(ex);
            DebugLog(ex);
            throw new Exception("Nyctophobia items failed to load!!");
        }
    }

    private void LoadAtlases()
    {
        try
        {
            foreach (string file in from file in AssetManager.ListDirectory("nt_atlases")
                                    where Path.GetExtension(file).Equals(".png")
                                    select file)
            {
                _ = File.Exists(Path.ChangeExtension(file, ".txt"))
                    ? Futile.atlasManager.LoadAtlas(Path.ChangeExtension(file, null))
                    : Futile.atlasManager.LoadImage(Path.ChangeExtension(file, null));
            }
        }
        catch (Exception ex)
        {
            DebugError(ex);
            throw new Exception($"Failed to load {MOD_NAME} atlases!");
        }
    }

    private void PomObjects()
    {
        try
        {
            LanternStickObj lanternStickObj = new();

            RegisterManagedObject(lanternStickObj);
            RegisterManagedObject<BlueLanternPlacer, BlueLanternData, ManagedRepresentation>("BlueLantern", MOD_NAME);
            RegisterManagedObject<CacaoFruitPlacer, CacaoFruitData, ManagedRepresentation>("CacaoFruit", MOD_NAME);
            RegisterManagedObject<BloodyFlowerPlacer, BloodyFlowerData, ManagedRepresentation>("BloodyKarmaFlower", MOD_NAME);
            RegisterManagedObject<RedFlareBombPlacer, RedFlareBombData, ManagedRepresentation>("RedFlareBomb", MOD_NAME);
            RegisterManagedObject<BlueSpearPlacer, BlueSpearData, ManagedRepresentation>("BlueSpear", MOD_NAME);
            RegisterManagedObject<BlueBombaPlacer, BlueBombaData, ManagedRepresentation>("BlueBomba", MOD_NAME);
        }
        catch (Exception ex)
        {
            DebugError(ex);
            Debug.LogError(ex);
        }
    }
}