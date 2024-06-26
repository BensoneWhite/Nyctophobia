﻿global using BepInEx;
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
global using Music;
global using CoralBrain;
global using JollyCoop;
global using System.Text.RegularExpressions;
global using Noise;
global using Smoke;
global using static PhysicalObject;
global using static Weapon;
global using static Pom.Pom;
global using BombFragment = ScavengerBomb.BombFragment;
global using LizardCosmetics;
global using Mono.Cecil.Cil;
global using MonoMod.Cil;
global using static SharedPhysics;
global using static CreatureCommunities;
global using static CreatureTemplate;
global using static MultiplayerUnlocks;
global using static RoomCamera;
global using static AbstractPhysicalObject;
global using Menu;
global using static CreatureTemplate.Relationship.Type;
global using SlugBase.SaveData;
global using System.Globalization;
global using static Nyctophobia.Constants;
global using Expedition;
global using Modding.Passages;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]