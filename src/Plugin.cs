using System;
using System.IO;
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using BepInEx;
using On;
using SlugBase.Features;
using UnityEngine;
using static SlugcatStats;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace Witness
{
    [BepInDependency("slime-cubed.slugbase")]
    [BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("Nankh.Witness", "Witness", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        static bool _initialized;

        public static Texture2D TailTexture;

        private void LogInfo(object data) => Logger.LogInfo(data);

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            LogInfo("You game will explode!!!!, Witness is loading....");
            On.RainWorld.OnModsInit += WrapInit(LoadResources);
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (_initialized) { return; } _initialized = true;
                On.RainWorld.OnModsInit += WrapInit(LoadResources);
                NWRelativeHooks.Init();
                NWFlyLogic.Init();
                WSRelativeHooks.Init();
                NWEnums.RegisterVaules();
                TailTexture = new Texture2D(150, 75, TextureFormat.ARGB32, false);
                var tailTextureFile = AssetManager.ResolveFilePath("textures/nightwalkertail.png");
                if (File.Exists(tailTextureFile))
                {
                    var rawData = File.ReadAllBytes(tailTextureFile);
                    TailTexture.LoadImage(rawData);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogMessage("WHOOPS something go wrong");
            }
            NWwhiskers.Init();
        }

        private static void LoadResources(RainWorld rainWorld)
        {
            Futile.atlasManager.LoadAtlas("atlases/slugcula");
        }

        public static On.RainWorld.hook_OnModsInit WrapInit(Action<RainWorld> loadResources)
        {
            return (orig, self) =>
            {
                orig(self);

                try
                {
                    if (!_initialized)
                    {
                        _initialized = true;
                        loadResources(self);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            };
        }

    }
}