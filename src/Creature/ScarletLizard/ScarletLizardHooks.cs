namespace Nyctophobia;

public class ScarletLizardHooks
{
    public static void Apply()
    {
        On.Lizard.ctor += Lizard_ctor;
        On.LizardGraphics.ctor += LizardGraphics_ctor;
        On.LizardBreeds.BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate += ScarletBreed;
    }

    private static void LizardGraphics_ctor(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
    {
        orig(self, ow);
        if (self.lizard.Template.type == NTEnums.CreatureType.ScarletLizard)
        {
            var state = Random.state;
            Random.InitState(self.lizard.abstractCreature.ID.RandomSeed);
            var num = self.startOfExtraSprites + self.extraSprites;
            self.ivarBodyColor = Color.yellow;

            num = self.AddCosmetic(num, new LizardCosmetics.Antennae(self, num));

            if (Random.value < 0.6f)
            {
                num = self.AddCosmetic(num, new LizardCosmetics.ShortBodyScales(self, num));
            }
            if (Random.value < 0.9f)
            {
                num = self.AddCosmetic(num, new LizardCosmetics.Whiskers(self, num));
            }
            if (Random.value < 0.6f)
            {
                var e = new LizardCosmetics.LongHeadScales(self, num)
                {
                    colored = false
                };
                e.numberOfSprites = e.scalesPositions.Length;
                var value = Random.value;
                var num2 = Mathf.Pow(Random.value, 0.45f);
                for (var i = 0; i < e.scalesPositions.Length; i++)
                {
                    e.scaleObjects[i] = new LizardScale(e)
                    {
                        length = Mathf.Lerp(10f, 30f, num),
                        width = Mathf.Lerp(1.0f, 1.4f, value * num)
                    };
                    e.backwardsFactors[i] = num2;
                }
                e.numberOfSprites = (e.colored ? (e.scalesPositions.Length * 2) : e.scalesPositions.Length);

                num = self.AddCosmetic(num, e);
            }
            if (Random.value < 0.15f)
            {
                var e = new LizardCosmetics.SpineSpikes(self, num)
                {
                    colored = 0,
                    graphic = 4,
                    spineLength = Mathf.Lerp(0.3f, 0.55f, Random.value) * 1
                };
                e.numberOfSprites = e.bumps;

                num = self.AddCosmetic(num, e);
            }
            if (Random.value < 0.99f)
            {
                var e = new LizardCosmetics.LongShoulderScales(self, num)
                {
                    rigor = 0f,
                    graphic = 4
                };
                e.GeneratePatchPattern(0.2f, Random.Range(6, 9), 0.9f, 2f);
                e.colored = false;
                var num4 = 0f;
                var num5 = 1f;
                var num2 = Mathf.Lerp(1f, 1f / Mathf.Lerp(1f, (float)e.scalesPositions.Length, Mathf.Pow(Random.value, 2f)), 0.5f);
                var num3 = Mathf.Lerp(5f, 15f, Random.value) * num2;
                var b = Mathf.Lerp(num3, 35f, Mathf.Pow(Random.value, 0.5f)) * num2;
                var p = Mathf.Lerp(0.1f, 0.9f, Random.value);
                e.scaleObjects = new LizardScale[e.scalesPositions.Length];
                e.backwardsFactors = new float[e.scalesPositions.Length];

                for (var i = 0; i < e.scalesPositions.Length; i++)
                {
                    if (e.scalesPositions[i].y > num4)
                    {
                        num4 = e.scalesPositions[i].y;
                    }
                    if (e.scalesPositions[i].y < num5)
                    {
                        num5 = e.scalesPositions[i].y;
                    }
                }

                for (var j = 0; j < e.scalesPositions.Length; j++)
                {
                    e.scaleObjects[j] = new LizardScale(e);
                    var num6 = Mathf.Pow(Mathf.InverseLerp(num5, num4, e.scalesPositions[j].y), p);
                    e.scaleObjects[j].length = (Mathf.Lerp(num3, b, Mathf.Lerp(Mathf.Sin(num6 * 3.1415927f), 1.1f, (num6 < 0.5f) ? 0.5f : 0.3f)));
                    e.scaleObjects[j].width = (Mathf.Lerp(1.0f, 1.2f, Mathf.Lerp(Mathf.Sin(num6 * 3.1415927f), 1.1f, (num6 < 0.5f) ? 0.5f : 0.3f)) * num2);
                    e.backwardsFactors[j] = e.scalesPositions[j].y * 0.7f;
                }
                e.numberOfSprites = (e.colored ? (e.scalesPositions.Length * 2) : e.scalesPositions.Length);

                num = self.AddCosmetic(num, e);
            }
            if (Random.value < 0.2f)
            {
                var e = new LizardCosmetics.TailFin(self, num)
                {
                    colored = false
                };
                e.numberOfSprites = e.bumps * 2;
                _ = self.AddCosmetic(num, e);
            }

            Random.state = state;
        }  
    }

    private static void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        if (self.Template.type == NTEnums.CreatureType.ScarletLizard)
        {
            self.effectColor = Custom.HSL2RGB(Custom.WrappedRandomVariation(0.0025f, 0.02f, 0.6f), 1f, Custom.ClampedRandomVariation(0.5f, 0.15f, 0.1f));
        }
    }

    private static CreatureTemplate ScarletBreed(On.LizardBreeds.orig_BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate orig, CreatureTemplate.Type type, CreatureTemplate lizardAncestor, CreatureTemplate pinkTemplate, CreatureTemplate blueTemplate, CreatureTemplate greenTemplate)
    {
        var result = orig(type, lizardAncestor, pinkTemplate, blueTemplate, greenTemplate);

        if (type == NTEnums.CreatureType.ScarletLizard)
        {
            var lizardBreedParams = new LizardBreedParams(type)
            {
                terrainSpeeds = new LizardBreedParams.SpeedMultiplier[Enum.GetNames(typeof(AItile.Accessibility)).Length]
            };
            for (var i = 0; i < lizardBreedParams.terrainSpeeds.Length; i++)
            {
                lizardBreedParams.terrainSpeeds[i] = new LizardBreedParams.SpeedMultiplier(0.1f, 1f, 1f, 1f);
            }

            lizardBreedParams.bodyRadFac = 1f;
            lizardBreedParams.pullDownFac = 1f;
            var tileTypeResistances = new List<TileTypeResistance>();
            var tileConnectionResistances = new List<TileConnectionResistance>();

            lizardBreedParams.terrainSpeeds[1] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
            tileTypeResistances.Add(new TileTypeResistance(AItile.Accessibility.Floor, 1f, 0));
            lizardBreedParams.terrainSpeeds[2] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
            tileTypeResistances.Add(new TileTypeResistance(AItile.Accessibility.Corridor, 1f, 0));
            lizardBreedParams.terrainSpeeds[3] = new LizardBreedParams.SpeedMultiplier(1f, 1f, 1f, 1f);
            tileTypeResistances.Add(new TileTypeResistance(AItile.Accessibility.Climb, 2f, 0));
            lizardBreedParams.terrainSpeeds[4] = new LizardBreedParams.SpeedMultiplier(0.8f, 1f, 1f, 1f);
            tileTypeResistances.Add(new TileTypeResistance(AItile.Accessibility.Wall, 3f, 0));
            lizardBreedParams.terrainSpeeds[5] = new LizardBreedParams.SpeedMultiplier(0.6f, 1f, 1f, 1f);
            tileTypeResistances.Add(new TileTypeResistance(AItile.Accessibility.Ceiling, 3f, 0));
            tileConnectionResistances.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToFloor, 1f, 0));
            tileConnectionResistances.Add(new TileConnectionResistance(MovementConnection.MovementType.DropToClimb, 1f, 0));
            tileConnectionResistances.Add(new TileConnectionResistance(MovementConnection.MovementType.ShortCut, 1f, 0));
            tileConnectionResistances.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachOverGap, 1f, 0));
            tileConnectionResistances.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachUp, 1f, 0));
            tileConnectionResistances.Add(new TileConnectionResistance(MovementConnection.MovementType.ReachDown, 1f, 0));
            tileConnectionResistances.Add(new TileConnectionResistance(MovementConnection.MovementType.CeilingSlope, 1f, 0));
            lizardBreedParams.biteDelay = 14;
            lizardBreedParams.biteInFront = 20f;
            lizardBreedParams.biteHomingSpeed = 1.4f;
            lizardBreedParams.biteChance = 0.4f;
            lizardBreedParams.attemptBiteRadius = 90f;
            lizardBreedParams.getFreeBiteChance = 0.5f;
            lizardBreedParams.biteDamage = 0.7f;
            lizardBreedParams.biteDamageChance = 0.2f;
            lizardBreedParams.toughness = 0.5f;
            lizardBreedParams.stunToughness = 0.75f;
            lizardBreedParams.regainFootingCounter = 4;
            lizardBreedParams.floorLeverage = 1f;
            lizardBreedParams.maxMusclePower = 2f;
            lizardBreedParams.aggressionCurveExponent = 0.875f;
            lizardBreedParams.wiggleDelay = 15;
            lizardBreedParams.bodyStiffnes = 0f;
            lizardBreedParams.swimSpeed = 0.35f;
            lizardBreedParams.idleCounterSubtractWhenCloseToIdlePos = 0;
            lizardBreedParams.headShieldAngle = 100f;
            lizardBreedParams.canExitLounge = true;
            lizardBreedParams.canExitLoungeWarmUp = true;
            lizardBreedParams.findLoungeDirection = 1f;
            lizardBreedParams.loungeDistance = 80f;
            lizardBreedParams.preLoungeCrouch = 35;
            lizardBreedParams.preLoungeCrouchMovement = -0.3f;
            lizardBreedParams.loungeSpeed = 2.5f;
            lizardBreedParams.loungeMaximumFrames = 20;
            lizardBreedParams.loungePropulsionFrames = 8;
            lizardBreedParams.loungeJumpyness = 0.9f;
            lizardBreedParams.loungeDelay = 310;
            lizardBreedParams.riskOfDoubleLoungeDelay = 0.8f;
            lizardBreedParams.postLoungeStun = 20;
            lizardBreedParams.loungeTendensy = 0.01f;
            lizardBreedParams.perfectVisionAngle = Mathf.Lerp(1f, -1f, 1f / 18f);
            lizardBreedParams.periferalVisionAngle = Mathf.Lerp(1f, -1f, 11f / 24f);
            lizardBreedParams.biteDominance = 0.1f;
            lizardBreedParams.limbThickness = 1f;
            lizardBreedParams.stepLength = 0.4f;
            lizardBreedParams.liftFeet = 0f;
            lizardBreedParams.feetDown = 0f;
            lizardBreedParams.noGripSpeed = 0.2f;
            lizardBreedParams.limbSpeed = 6f;
            lizardBreedParams.limbGripDelay = 1;
            lizardBreedParams.smoothenLegMovement = true;
            lizardBreedParams.legPairDisplacement = 0f;
            lizardBreedParams.walkBob = 0.4f;
            lizardBreedParams.tailStiffness = 400f;
            lizardBreedParams.tailStiffnessDecline = 0.1f;
            lizardBreedParams.tailColorationStart = 0.1f;
            lizardBreedParams.tailColorationExponent = 1.2f;
            lizardBreedParams.neckStiffness = 0f;
            lizardBreedParams.jawOpenAngle = 105f;
            lizardBreedParams.jawOpenLowerJawFac = 0.55f;
            lizardBreedParams.jawOpenMoveJawsApart = 20f;
            lizardBreedParams.headGraphics = new int[5];
            lizardBreedParams.framesBetweenLookFocusChange = 20;
            lizardBreedParams.tongue = false;
            lizardBreedParams.tongueAttackRange = 140f;
            lizardBreedParams.tongueWarmUp = 10;
            lizardBreedParams.tongueSegments = 5;
            lizardBreedParams.tongueChance = 0.25f;
            lizardBreedParams.tamingDifficulty = 1.1f;
            lizardBreedParams.tailSegments = Random.Range(2, 4);
            lizardBreedParams.tailLengthFactor = 1.4f;
            lizardBreedParams.danger = 1f;
            lizardBreedParams.standardColor = Color.red;
            lizardBreedParams.headSize = 1f;
            lizardBreedParams.bodySizeFac = 1.2f;
            lizardBreedParams.limbSize = 1f;
            lizardBreedParams.bodyLengthFac = 1f;
            lizardBreedParams.bodyMass = 1.2f;
            lizardBreedParams.limbQuickness = 1.6f;
            lizardBreedParams.wiggleSpeed = 2f;
            lizardBreedParams.baseSpeed = 1.6f;

            result = new CreatureTemplate(type, lizardAncestor, tileTypeResistances, tileConnectionResistances, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 0f))
            {
                name = "ScarletLizard",
                waterPathingResistance = 5f,
                visualRadius = 950f,
                waterVision = 0.4f,
                throughSurfaceVision = 0.85f,
                breedParameters = lizardBreedParams,
                baseDamageResistance = lizardBreedParams.toughness * 2f,
                baseStunResistance = lizardBreedParams.toughness
            };
            result.damageRestistances[(int)Creature.DamageType.Bite, 0] = 2.5f;
            result.damageRestistances[(int)Creature.DamageType.Bite, 1] = 3f;
            result.meatPoints = 3;
            result.doPreBakedPathing = false;
            result.preBakedPathingAncestor = pinkTemplate;
            result.virtualCreature = false;
            result.pickupAction = "Bite";
            result.jumpAction = "Call";
            result.throwAction = "Launch";
        }

        return result;
    }
}
