using LogUtils;
using LogUtils.Enums;

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

    public static new LogUtils.Logger Logger;

    public static void DebugLog(object message) => Logger.LogInfo(message);

    public static void DebugWarning(object message) => Logger.LogWarning(message);

    public static void DebugError(object message) => Logger.LogError(message);

    public static void DebugFatal(object message) => Logger.LogFatal(message);

    public NTOptionsMenu nTOptionsMenu;

    public void OnEnable()
    {
        try
        {
            Logger = new LogUtils.Logger(LogEnums.LogEnum.Nyctophobia)
            {
                ManagedLogSource = base.Logger
            };

            UnsafeUtilityLoggerLoad();

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

        IsFestive = !NTOptionsMenu.DisableFestiveDays.Value;
        if (!IsFestive) return;

        IsChristmas = today == christmas;
        IsNewYear = today == newYear;
        IsPrideDay = today == prideDay;
        IsAnniversary = today == anniversaryDay;
        IsApril = today == AprilDay;

        if (IsFestive)
        {
            if (IsChristmas) DebugWarning($"{MOD_NAME} is in Christmas mode!");
            if (IsNewYear) DebugWarning($"{MOD_NAME} is in New Year mode!");
            if (IsPrideDay) DebugWarning($"{MOD_NAME} is in Pride Day mode!");
            if (IsAnniversary) DebugWarning($"{MOD_NAME} is in Anniversary mode!");
            if (IsApril) DebugWarning($"{MOD_NAME} is in April mode!");
        }
    }

    public static void UnsafeUtilityLoggerLoad()
    {
        UtilityCore.EnsureInitializedState();
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

            MachineConnector.SetRegisteredOI(MOD_ID, nTOptionsMenu = new NTOptionsMenu());

            CheckFestiveDates();

            if (!WeakTables.NyctoShaders.TryGetValue(self, out var _)) WeakTables.NyctoShaders.Add(self, _ = new WeakTables.Shaders());

            if (WeakTables.NyctoShaders.TryGetValue(self, out var shaders))
            {
                shaders.ShaderPack = NTUtils.LoadFromEmbeddedResource("Nyctophobia.shaderpack");

                if (shaders.ShaderPack != null)
                {
                    InPlaceTryCatch(ref shaders.Desaturation, FShader.CreateShader("Desaturation", shaders.ShaderPack.LoadAsset<Shader>("Assets/DesaturateShader.shader")), $"{MOD_NAME} Shader: Desaturation, Failed to set!");
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
        BloodyFlowerHooks.Apply();
        RedFlareBombHooks.Apply();

        Content.Register(
            new BloodyFlowerFisob(),
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
        RegisterManagedObject<BloodyFlowerPlacer, BloodyFlowerData, ManagedRepresentation>("BloodyKarmaFlower", MOD_NAME);
        RegisterManagedObject<RedFlareBombPlacer, RedFlareBombData, ManagedRepresentation>("RedFlareBomb", MOD_NAME);
        RegisterManagedObject<BlueSpearPlacer, BlueSpearData, ManagedRepresentation>("BlueSpear", MOD_NAME);
        RegisterManagedObject<BlueBombaPlacer, BlueBombaData, ManagedRepresentation>("BlueBomba", MOD_NAME);
    }

    private void RegisterCustomPassages()
    {
        CustomPassages.Register(
            new EggHatcher(),
            new TheGreatMother()
        );
    }

    public void InPlaceTryCatch<T>(ref T variableToSet, T defaultValue, string errorMessage, [CallerLineNumber] int lineNumber = 0)
    {
        try
        {
            variableToSet = defaultValue;
        }
        catch (Exception ex)
        {
            DebugError(errorMessage.Replace("%ln", $"{lineNumber}, {ex}"));
        }
    }
}

public static class  LogEnums
{
    public class LogEnum : LogID
    {
        public static readonly LogID Nyctophobia = new LogID("Nyctophobia", UtilityConsts.PathKeywords.ROOT, LogAccess.FullAccess, true);
        public LogEnum(string filename, LogAccess access, bool register = false) : base(filename, access, register)
        {
        }
    }
}