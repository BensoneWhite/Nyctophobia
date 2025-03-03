namespace Nyctophobia;

[BepInDependency("slime-cubed.slugbase")]
[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "nyctophobia";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "Nyctophobia";
    public const string VERSION = "0.4.6";

    private bool isInit;

    // BepInEx requires this exact declaration.
    public static new ManualLogSource Logger;

    public static LogUtilsLoggerAdapter LogUtilsLogger;

    public static void DebugLog(object message)
    {
        if (NTUtils.IsLogUtilsEnabled)
            LogUtilsLogger?.LogInfo(message);
        else
            Logger.LogInfo(message);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    public static void DebugWarning(object message)
    {
        if (NTUtils.IsLogUtilsEnabled)
            LogUtilsLogger?.LogWarning(message);
        else
            Logger.LogWarning(message);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    public static void DebugError(object message)
    {
        if (NTUtils.IsLogUtilsEnabled)
            LogUtilsLogger?.LogError(message);
        else
            Logger.LogError(message);
    }

    /// <summary>
    /// Logs a fatal error message.
    /// </summary>
    public static void DebugFatal(object message)
    {
        if (NTUtils.IsLogUtilsEnabled)
            LogUtilsLogger?.LogFatal(message);
        else
            Logger.LogFatal(message);
    }

    public NTOptionsMenu nTOptionsMenu;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;

            DebugWarning($"Is Enabled? : {ModManager.ActiveMods.Any(x => x.id == "LogUtils")}");

            if (NTUtils.IsLogUtilsEnabled)
            {
                LogUtilsLogger = new LogUtilsLoggerAdapter();
            }
            else
            {
                DebugLog($"{MOD_NAME}, using BepInEx Logger");
            }

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

    private void CheckFestiveDates()
    {
        DateTime today = DateTime.Today;

        IsChristmas = today == christmas;
        IsNewYear = today == newYear;
        if (!NTOptionsMenu.DisablePrideDay.Value)
            IsPrideDay = today == prideDay;
        IsAnniversary = today == anniversaryDay;
        IsApril = today == AprilDay;
        IsFestive = !NTOptionsMenu.DisableFestiveDays.Value &&
                    (IsChristmas || IsNewYear || IsPrideDay || IsAnniversary || IsApril);

        if (IsFestive)
        {
            if (IsChristmas) DebugWarning($"{MOD_NAME} is in Christmas mode!");
            if (IsNewYear) DebugWarning($"{MOD_NAME} is in New Year mode!");
            if (IsPrideDay) DebugWarning($"{MOD_NAME} is in Pride Day mode!");
            if (IsAnniversary) DebugWarning($"{MOD_NAME} is in Anniversary mode!");
            if (IsApril) DebugWarning($"{MOD_NAME} is in April mode!");
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

            NWHooks.Init();
            EXHooks.Init();
            WSHooks.Init();

            ESPHooks.Apply();

            HueRemixMenu.Apply();
            SelectMenuHooks.Apply();

            RegisterCustomPassages();

            _ = MachineConnector.SetRegisteredOI(MOD_ID, nTOptionsMenu = new NTOptionsMenu());

            //Special days
            CheckFestiveDates();

            On.RainWorld.Update += RainWorld_Update;

            if (!WeakTables.NyctoShaders.TryGetValue(self, out var _)) WeakTables.NyctoShaders.Add(self, _ = new WeakTables.Shaders());

            if (WeakTables.NyctoShaders.TryGetValue(self, out var shaders))
            {
                shaders.ShaderPack = NTUtils.LoadFromEmbeddedResource("Nyctophobia.shaderpack");

                if (shaders.ShaderPack != null)
                {
                    MethodHelpers.InPlaceTryCatch(ref shaders.Desaturation, FShader.CreateShader("Desaturation", shaders.ShaderPack.LoadAsset<Shader>("Assets/DesaturateShader.shader")), $"{MOD_NAME} Shader: Desaturation, Failed to set!");
                }
                else
                {
                    DebugLog($"{MOD_NAME} failed to load shaders or assets");
                }
            }
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void RainWorld_Update(On.RainWorld.orig_Update orig, RainWorld self)
    {
        orig(self);

        IsPrideDay = !NTOptionsMenu.DisablePrideDay.Value;
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
                    DebugLog($"Unregistering.... {MOD_NAME} creatures and items");
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
        try
        {
            BlueLanternHooks.Apply();
            BlueSpearHooks.Apply();
            BlueBombaHooks.Apply();
            CacaoFruitHooks.Apply();
            BloodyFlowerHooks.Apply();
            RedFlareBombHooks.Apply();

            Content.Register(
                new BloodyFlowerFisob(),
                new CacaoFruitFisob(),
                new BlueLanternFisob(),
                new BlueSpearFisob(),
                new ImpalerFisob(),
                new BlueBombaFisob(),
                new RedFlareBombFisob());

            DebugLog($"Registering Items {MOD_NAME}");
        }
        catch (Exception ex)
        {
            DebugError(ex);
            throw new Exception($"{MOD_NAME} Items failed to load!!");
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
                new AncientNeuronCritob(),
                new SLLCritob());

            DebugLog($"Registering Creatures {MOD_NAME}");
        }
        catch (Exception ex)
        {
            DebugError(ex);
            throw new Exception($"{MOD_NAME} Creatures failed to load!!");
        }
    }

    private void LoadAtlases()
    {
        DebugWarning($"Loading atlas from: {MOD_NAME}");
        try
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
        catch (Exception ex)
        {
            DebugError(ex);
            throw new Exception($"Failed to load {MOD_NAME} atlases!", ex);
        }
    }

    private void RegisterPomObjects()
    {
        try
        {
            RegisterManagedObject(new ImpalerObj());
            RegisterManagedObject(new LanternStickObj());
            RegisterManagedObject<BlueLanternPlacer, BlueLanternData, ManagedRepresentation>("BlueLantern", MOD_NAME);
            RegisterManagedObject<CacaoFruitPlacer, CacaoFruitData, ManagedRepresentation>("CacaoFruit", MOD_NAME);
            RegisterManagedObject<BloodyFlowerPlacer, BloodyFlowerData, ManagedRepresentation>("BloodyKarmaFlower", MOD_NAME);
            RegisterManagedObject<RedFlareBombPlacer, RedFlareBombData, ManagedRepresentation>("RedFlareBomb", MOD_NAME);
            RegisterManagedObject<BlueSpearPlacer, BlueSpearData, ManagedRepresentation>("BlueSpear", MOD_NAME);
            RegisterManagedObject<BlueBombaPlacer, BlueBombaData, ManagedRepresentation>("BlueBomba", MOD_NAME);

            DebugLog($"Registering POM's objects from {MOD_NAME}");
        }
        catch (Exception ex)
        {
            DebugError(ex);
            throw new Exception($"Failed to load {MOD_NAME} POM's objects!", ex);
        }
    }

    private void RegisterCustomPassages()
    {
        CustomPassages.Register(
            new EggHatcher(),
            new TheGreatMother());

        DebugLog($"Registering custom passages, {MOD_NAME}");
    }
}