global using BepInEx;
global using BepInEx.Logging;
global using DevInterface;
global using Fisobs.Core;
global using Fisobs.Creatures;
global using Fisobs.Items;
global using Fisobs.Properties;
global using Fisobs.Sandbox;
global using HUD;
global using Menu.Remix.MixedUI;
global using MonoMod.RuntimeDetour;
global using MoreSlugcats;
global using RWCustom;
global using SlugBase.DataTypes;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Security.Permissions;
global using UnityEngine;
global using static MoreSlugcats.SingularityBomb;
global using static PathCost.Legality;
global using static Player;
global using CreatureType = CreatureTemplate.Type;
global using Random = UnityEngine.Random;
global using Menu.Remix;
global using VoidSea;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]