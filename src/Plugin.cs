using System.IO;
using System.Security.Permissions;
using BepInEx;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace Witness;

[BepInDependency("slime-cubed.slugbase")]
[BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin("Nankh.Witness", "Witness", "0.1.0")]
class Plugin : BaseUnityPlugin
{
    private void LogInfo(object data) => Logger.LogInfo(data);

    public bool IsInit;
    public bool IsPreInit;
    public bool IsPostInit;

    public static Texture2D TailTexture;

    public void OnEnable()
    {
        LogInfo("You game will explode!!!!, Witness is loading....");

        try
        {
            On.RainWorld.PreModsInit += RainWorld_PreModsInit;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
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

            NWFlyLogic.Init();
            NWRelativeHooks.Init();
            NWwhiskers.Init();

            EXRelativeHooks.Init();

            WSRelativeHooks.Init();

            LoadAtlases();

            ApplyCreatures();
            ApplyItems();

            TailTexture = new Texture2D(150, 75, TextureFormat.ARGB32, false);
            var tailTextureFile = AssetManager.ResolveFilePath("nt_atlases/nightwalkertail.png");
            var ExitailTextureFile = AssetManager.ResolveFilePath("nt_atlases/exiletail.png");
            if (File.Exists(tailTextureFile))
            {
                var rawData = File.ReadAllBytes(tailTextureFile);
                TailTexture.LoadImage(rawData);
            }
            if (File.Exists(ExitailTextureFile))
            {
                var rawData = File.ReadAllBytes(ExitailTextureFile);
                TailTexture.LoadImage(rawData);
            }
        }

        catch (Exception ex)
        {
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
        }
    }

    private void ApplyItems()
    {
    }

    private void ApplyCreatures()
    {
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