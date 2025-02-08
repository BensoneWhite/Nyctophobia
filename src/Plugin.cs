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
    public bool ExpeditionPatched;

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
            DebugWarning($"{MOD_NAME} is loading.... {VERSION}");

            NTEnums.Init();

            ApplyCreatures();
            ApplyItems();
            PomObjects();
            DevToolsInit.Apply();

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

        IsChristmas = today == christmas;
        IsNewYear = today == newYear;
        if (!NTOptionsMenu.PrideDay.Value)
            IsPrideDay = today == prideDay;
        IsAnniversary = today == anniversaryDay;
        IsApril = today == AprilDay;
        if (!NTOptionsMenu.FestiveDays.Value)
        {
            IsFestive = IsChristmas ||
            IsNewYear ||
            IsPrideDay ||
            IsAnniversary ||
            IsApril;
        }

        if (IsFestive)
        {
            if (IsChristmas)
                DebugWarning($"{MOD_NAME} is in Christmas mode!");
            if (IsNewYear)
                DebugWarning($"{MOD_NAME} is in New Year mode!");
            if (IsPrideDay)
                DebugWarning($"{MOD_NAME} is in Pride Day mode!");
            if (IsAnniversary)
                DebugWarning($"{MOD_NAME} is in Anniversary mode!");
            if (IsApril)
                DebugWarning($"{MOD_NAME} is in April mode!");
        }
    }

    private void RainWorld_PreModsInit(On.RainWorld.orig_PreModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsPreInit) return;

            IsPreInit = true;

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
        Futile.atlasManager.LogAllElementNames();

        orig(self);
        try
        {
            if (IsInit) return;
            IsInit = true;

            DebugWarning($"Initializing OnModsInit {MOD_NAME}");

            NWHooks.Init();
            EXHooks.Init();
            WSHooks.Init();

            LoadAtlases();

            ESPHooks.Apply();
            GeneralHooks.Apply();
            HueRemixMenu.Apply();
            SelectMenuHooks.Apply();
            BigAcronymFix.Apply();

            CustomPassagesNT();

            _ = MachineConnector.SetRegisteredOI(MOD_ID, nTOptionsMenu = new NTOptionsMenu());

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

        if (!NTOptionsMenu.PrideDay.Value)
            IsPrideDay = true;
        else
            IsPrideDay = false;
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        try
        {
            DebugWarning($"Initializing OnModsDisable {MOD_NAME}");
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
            if (IsPostInit) return;
            IsPostInit = true;

            DebugWarning($"Initializing PostModsDisable {MOD_NAME}");

            //_ = new Hook(typeof(Player).GetProperty(nameof(Player.isSlugpup))!.GetGetMethod(), NWHooks.IsSlugpupOverride_NightWalker_get);

            if (ModManager.Expedition && !ExpeditionPatched)
            {
                BigAcronymFix.ApplyExpedition();
                ExpeditionPatched = true;
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
            Debug.LogException(ex);
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
            DebugLog(ex);
            throw new Exception($"{MOD_NAME} Creatures failed to load!!");
        }
    }

    private void LoadAtlases()
    {
        DebugLog($"Loading atlas from: {MOD_NAME}");
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
            ImpalerObj impalerObj = new();
            LanternStickObj lanternStickObj = new();

            RegisterManagedObject(impalerObj);
            RegisterManagedObject(lanternStickObj);
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
            Debug.LogError(ex);
        }
    }

    private void CustomPassagesNT()
    {
        CustomPassages.Register(
            new EggHatcher(),
            new TheGreatMother());
    }
}