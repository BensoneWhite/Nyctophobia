namespace Nyctophobia;

[BepInDependency("slime-cubed.slugbase")]
[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "nyctophobia";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "Nyctophobia";
    public const string VERSION = "0.4.6";

    // Initialization flags.
    private bool isInit;
    private bool isPreInit;
    private bool isPostInit;
    private bool expeditionPatched;

    //BepInEx Logger for easy console logs
    public new static ManualLogSource Logger;

    public static void DebugLog(object ex) => Logger.LogInfo(ex);
    public static void DebugWarning(object ex) => Logger.LogWarning(ex);
    public static void DebugError(object ex) => Logger.LogError(ex);
    public static void DebugFatal(object ex) => Logger.LogFatal(ex);

    public NTOptionsMenu nTOptionsMenu;

    // Called when the plugin is loaded.
    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;
            DebugWarning($"{MOD_NAME} is loading.... {VERSION}");

            //Enums goes first as priority
            NTEnums.Init();
            //General, Unstable changes goes first
            GeneralHooks.Apply();

            DevToolsInit.Apply();
            //Custom things made with Fisobs and POM
            ApplyCreatures();
            ApplyItems();
            RegisterPomObjects();

            // Register hooks.
            On.RainWorld.PreModsInit += RainWorld_PreModsInit;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
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
            if (IsChristmas)    DebugWarning($"{MOD_NAME} is in Christmas mode!");
            if (IsNewYear)      DebugWarning($"{MOD_NAME} is in New Year mode!");
            if (IsPrideDay)     DebugWarning($"{MOD_NAME} is in Pride Day mode!");
            if (IsAnniversary)  DebugWarning($"{MOD_NAME} is in Anniversary mode!");
            if (IsApril)        DebugWarning($"{MOD_NAME} is in April mode!");
        }
    }

    private void RainWorld_PreModsInit(On.RainWorld.orig_PreModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (isPreInit) return;

            isPreInit = true;

            DebugWarning($"Initializing PreModsInit {MOD_NAME}");
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
            if (isInit) return;
            isInit = true;

            DebugWarning($"Initializing OnModsInit {MOD_NAME}");

            //This should go first before all the Graphics hooks
            LoadAtlases();

            //Initialize the Slugcat Hooks
            NWHooks.Init();
            EXHooks.Init();
            WSHooks.Init();

            //Iterator Hooks
            ESPHooks.Apply();
            
            //Misc Hooks
            HueRemixMenu.Apply();
            SelectMenuHooks.Apply();
            BigAcronymFix.Apply();

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
            Debug.LogException(ex);
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
            DebugWarning($"Initializing OnModsDisable {MOD_NAME}");
            foreach (var mod in newlyDisabledMods)
            {
                if (mod.id == MOD_ID || mod.id == "moreslugcats")
                {
                    NTEnums.Unregister();
                }
            }
            DebugLog($"Unregistering.... {MOD_NAME} creatures and items");
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
            if (isPostInit) return;
            isPostInit = true;

            DebugWarning($"Initializing PostModsInit {MOD_NAME}");

            if (ModManager.Expedition && !expeditionPatched)
            {
                BigAcronymFix.ApplyExpedition();
                expeditionPatched = true;
                DebugLog($"{MOD_NAME}, Patching Expedition");
            }
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
            CacaoFruitHooks.Apply();
            BloodyFlowerHooks.Apply();
            RedFlareBombHooks.Apply();
            BoomerangHooks.Apply();

            Content.Register(
                new BloodyFlowerFisob(),
                new CacaoFruitFisob(),
                new BlueLanternFisob(),
                new BlueSpearFisob(),
                new ImpalerFisob(),
                new BlueBombaFisob(),
                new BoomerangFisob(),
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
            //AncientNeuronsHooks.Apply();
            FlashWigHooks.Apply();
            RedPorcuspiderHooks.Apply();

            Content.Register(
                new BoyKisserCritob(),
                new WitnessPupCritob(),
                new MiroAlbinoCritob(),
                new CicadaDronCritob(),
                new BlackLighMouseCritob(),
                new FlashWigCritob(),
                new PortalBuddyCritob(),
                new ScarletLizardCritob(),
                new ProtoVultureCritob(),
                new RedPorcuspiderCritob(),
                new AncientNeuronCritob(),
                //new MosquitoCritob(),
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