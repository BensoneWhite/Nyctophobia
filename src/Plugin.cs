using BepInEx.Logging;
using Nyctophobia;

namespace Nyctophobia;

[BepInDependency("slime-cubed.slugbase")]
[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "nyctophobia";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "Nyctophobia";
    public const string VERSION = "0.4.1";

    public bool IsInit;
    public bool IsPreInit;
    public bool IsPostInit;

    public static void LogInfo(object ex) => Logger.LogWarning(ex);

    public static void LogError(object ex) => Logger.LogError(ex);

    public static new ManualLogSource Logger;

    private NTOptionsMenu nTOptionsMenu;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;
            LogInfo($"{MOD_NAME} is loading.... {VERSION}");

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
            LogError(ex);
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
            LogError(ex);
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

            NWHooks.Init();
            EXHooks.Init();
            WSHooks.Init();

            LoadAtlases();
            PomObjects();

            ESPHooks.Apply();

            GeneralHooks.Apply();

            MachineConnector.SetRegisteredOI(MOD_ID, nTOptionsMenu = new NTOptionsMenu());
        }

        catch (Exception ex)
        {
            Debug.LogException(ex);
            LogError(ex);
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
            LogInfo("Unregister.... Nyctophobia creatures and items");
        }
        catch (Exception ex)
        {
            LogError(ex);
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
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            LogError(ex);
        }
    }

    private void ApplyItems()
    {
        try
        {
            BlueLanternHooks.Apply();
            BlueSpearHooks.Apply();
            BlueBombaHooks.Apply();
            RedFlareBombsHooks.Apply();
            AncientNeuronsHooks.Apply();
            CacaoFruitHooks.Apply();

            Content.Register(
                new BlueLanternFisob(),
                new BlueSpearFisob(),
                new BlueBombaFisob(),
                new AncientNeuronsFisobs(),
                new RedFlareBombFisob());
            LogInfo("Registering Items Nyctophobia");
        }
        catch (Exception ex)
        {
            LogError(ex);
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

            LogInfo("Registering Creatures Nyctophobia");
        }
        catch (Exception ex)
        {
            LogError(ex);
            LogInfo(ex);
            throw new Exception("Nyctophobia items failed to load!!");
        }
    }

    private void LoadAtlases()
    {
        try
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
        catch (Exception ex)
        {
            LogError(ex);
            throw new Exception("Failed to load Nyctophobia atlases!");
        }
    }

    private void PomObjects()
    {
        LanternStickObj lanternStickObj = new();

        Pom.Pom.RegisterManagedObject(lanternStickObj);
        Pom.Pom.RegisterManagedObject<CacaoFruitPlacer, CacaoFruitData, Pom.Pom.ManagedRepresentation>("CacaoFruit", MOD_NAME);
    }
}