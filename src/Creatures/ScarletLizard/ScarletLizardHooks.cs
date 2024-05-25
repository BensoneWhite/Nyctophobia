namespace Nyctophobia;

public class ScarletLizardHooks
{
    public static void Apply()
    {
        On.Lizard.ctor += Lizard_ctor;
        On.LizardGraphics.ctor += LizardGraphics_ctor;
        On.LizardBreeds.BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate += ScarletBreed;
        IL.YellowAI.Update += YellowAI_Update;
        IL.LizardCosmetics.Antennae.ctor += Antennae_ctor;
        On.AbstractCreature.ctor += AbstractCreature_ctor;
        On.Lizard.EnterAnimation += Lizard_EnterAnimation;
    }

    private static void Lizard_EnterAnimation(On.Lizard.orig_EnterAnimation orig, Lizard self, Lizard.Animation anim, bool forceAnimationChange)
    {
        if (self.Template.type == NTEnums.CreatureType.ScarletLizard)
        {
            if (!((!forceAnimationChange && (int)anim < (int)self.animation) || self.animation == anim))
            {
                if (anim == Lizard.Animation.PreyReSpotted && !self.safariControlled)
                {
                    if (self.AI.yellowAI.pack != null && self.AI.yellowAI.pack.PackLeader == self.abstractCreature)
                    {
                        self.voice.MakeSound(LizardVoice.Emotion.SpottedPreyFirstTime);
                    }
                    else
                    {
                        self.voice.MakeSound(LizardVoice.Emotion.ReSpottedPrey, Random.Range(0.1f, 0.25f));
                    }
                }
            }
        }

        orig(self, anim, forceAnimationChange);
    }

    private static void AbstractCreature_ctor(On.AbstractCreature.orig_ctor orig, AbstractCreature self, World world, CreatureTemplate creatureTemplate, Creature realizedCreature, WorldCoordinate pos, EntityID id)
    {
        orig(self, world, creatureTemplate, realizedCreature, pos, id);

        if (creatureTemplate.type == NTEnums.CreatureType.ScarletLizard)
        {
            self.personality.dominance = 1;
        }
    }

    private static void Antennae_ctor(ILContext il)
    {
        var cursor = new ILCursor(il);

        cursor.GotoNext(MoveType.Before, i => i.MatchStfld<Antennae>(nameof(Antennae.length)));

        cursor.MoveAfterLabels();
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate((float oldValue, Antennae self) =>
        {
            if (self.lGraphics.lizard.abstractCreature.creatureTemplate.type == NTEnums.CreatureType.ScarletLizard)
            {
                var state = Random.state;
                Random.InitState(self.lGraphics.lizard.abstractCreature.ID.RandomSeed);

                var result = Random.Range(0.75f, 1);

                Random.state = state;
                return result;
            }

            return oldValue;
        });
    }
    private static void YellowAI_Update(ILContext il)
    {
        var cursor = new ILCursor(il);

        var loc = -1;
        cursor.GotoNext(MoveType.After, i => i.MatchLdsfld<CreatureType>(nameof(CreatureType.YellowLizard)));
        cursor.GotoPrev(MoveType.After,
            i => i.MatchLdfld<AbstractRoom>(nameof(AbstractRoom.creatures)),
            i => i.MatchLdloc(out loc));
        cursor.GotoNext(MoveType.Before, i => i.MatchBrfalse(out _));

        cursor.MoveAfterLabels();
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldloc, loc);
        cursor.EmitDelegate((YellowAI self, int i) => self.lizard.room.abstractRoom.creatures[i].creatureTemplate.type == NTEnums.CreatureType.ScarletLizard);
        cursor.Emit(OpCodes.Or);
    }

    private static void LizardGraphics_ctor(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
    {
        orig(self, ow);
        if (self.lizard.Template.type == NTEnums.CreatureType.ScarletLizard)
        {
            Random.State state = Random.state;
            Random.InitState(self.lizard.abstractCreature.ID.RandomSeed);
            int num = self.startOfExtraSprites + self.extraSprites;
            self.ivarBodyColor = Color.yellow;

            num = self.AddCosmetic(num, new Antennae(self, num));

            if (Random.value < 0.6f)
            {
                num = self.AddCosmetic(num, new ShortBodyScales(self, num));
            }
            if (Random.value < 0.9f)
            {
                num = self.AddCosmetic(num, new Whiskers(self, num));
            }
            if (Random.value < 0.6f)
            {
                LongHeadScales e = new(self, num)
                {
                    colored = false
                };
                e.numberOfSprites = e.scalesPositions.Length;
                float value = Random.value;
                float num2 = Mathf.Pow(Random.value, 0.45f);
                for (int i = 0; i < e.scalesPositions.Length; i++)
                {
                    e.scaleObjects[i] = new LizardScale(e)
                    {
                        length = Mathf.Lerp(10f, 30f, num),
                        width = Mathf.Lerp(1.0f, 1.4f, value * num)
                    };
                    e.backwardsFactors[i] = num2;
                }
                e.numberOfSprites = e.colored ? (e.scalesPositions.Length * 2) : e.scalesPositions.Length;

                num = self.AddCosmetic(num, e);
            }
            if (Random.value < 0.15f)
            {
                SpineSpikes e = new(self, num)
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
                LongShoulderScales e = new(self, num)
                {
                    rigor = 0f,
                    graphic = 4
                };
                e.GeneratePatchPattern(0.2f, Random.Range(6, 9), 0.9f, 2f);
                e.colored = false;
                float num4 = 0f;
                float num5 = 1f;
                float num2 = Mathf.Lerp(1f, 1f / Mathf.Lerp(1f, e.scalesPositions.Length, Mathf.Pow(Random.value, 2f)), 0.5f);
                float num3 = Mathf.Lerp(5f, 15f, Random.value) * num2;
                float b = Mathf.Lerp(num3, 35f, Mathf.Pow(Random.value, 0.5f)) * num2;
                float p = Mathf.Lerp(0.1f, 0.9f, Random.value);
                e.scaleObjects = new LizardScale[e.scalesPositions.Length];
                e.backwardsFactors = new float[e.scalesPositions.Length];

                for (int i = 0; i < e.scalesPositions.Length; i++)
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

                for (int j = 0; j < e.scalesPositions.Length; j++)
                {
                    e.scaleObjects[j] = new LizardScale(e);
                    float num6 = Mathf.Pow(Mathf.InverseLerp(num5, num4, e.scalesPositions[j].y), p);
                    e.scaleObjects[j].length = Mathf.Lerp(num3, b, Mathf.Lerp(Mathf.Sin(num6 * 3.1415927f), 1.1f, (num6 < 0.5f) ? 0.5f : 0.3f));
                    e.scaleObjects[j].width = Mathf.Lerp(1.0f, 1.2f, Mathf.Lerp(Mathf.Sin(num6 * 3.1415927f), 1.1f, (num6 < 0.5f) ? 0.5f : 0.3f)) * num2;
                    e.backwardsFactors[j] = e.scalesPositions[j].y * 0.7f;
                }
                e.numberOfSprites = e.colored ? (e.scalesPositions.Length * 2) : e.scalesPositions.Length;

                num = self.AddCosmetic(num, e);
            }
            if (Random.value < 0.2f)
            {
                TailFin e = new(self, num)
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

    private static CreatureTemplate ScarletBreed(On.LizardBreeds.orig_BreedTemplate_Type_CreatureTemplate_CreatureTemplate_CreatureTemplate_CreatureTemplate orig, CreatureType type, CreatureTemplate lizardAncestor, CreatureTemplate pinkTemplate, CreatureTemplate blueTemplate, CreatureTemplate greenTemplate)
    {
        if (type == NTEnums.CreatureType.ScarletLizard)
        {
            var temp = orig(CreatureType.BlueLizard, lizardAncestor, pinkTemplate, blueTemplate, greenTemplate);

            var lizardBreedParams = (LizardBreedParams)temp.breedParameters;

            temp.type = type;
            temp.name = "ScarletLizard";

            //lizardBreedParams.terrainSpeeds[0] = new(1f, 1f, 1f, 1f); //OffScreen
            lizardBreedParams.terrainSpeeds[1] = new(1.35f, 1f, 1f, 1f); //Floor
            lizardBreedParams.terrainSpeeds[2] = new(1.35f, 1f, 1f, 1f); //Corridor, Tunel
            lizardBreedParams.terrainSpeeds[3] = new(0.9f, 1f, 1f, 1f); //Climb, poles
            //lizardBreedParams.terrainSpeeds[4] = new(0.1f, 1f, 1f, 1f); //Wall
            //lizardBreedParams.terrainSpeeds[5] = new(0.1f, 1f, 1f, 1f); //Ceiling
            //lizardBreedParams.terrainSpeeds[6] = new(1f, 1f, 1f, 1f); //Air
            //lizardBreedParams.terrainSpeeds[7] = new(1f, 1f, 1f, 1f); //Solid

            temp.pathingPreferencesTiles[1] = new(0.5f, Allowed);
            temp.pathingPreferencesTiles[2] = new(0.5f, Allowed);
            temp.pathingPreferencesTiles[3] = new(1f, Allowed);
            temp.pathingPreferencesTiles[4] = new(10f, Unallowed);
            temp.pathingPreferencesTiles[5] = new(10f, Unallowed);
            temp.pathingPreferencesTiles[6] = new(10f, Unallowed);

            lizardBreedParams.bodyRadFac = 1f;
            lizardBreedParams.pullDownFac = 1f;

            lizardBreedParams.biteDelay = 10;
            lizardBreedParams.biteInFront = 20f;
            lizardBreedParams.biteHomingSpeed = 1.4f;
            lizardBreedParams.biteChance = 0.9f;
            lizardBreedParams.attemptBiteRadius = 90f;
            lizardBreedParams.getFreeBiteChance = 0.5f;
            lizardBreedParams.biteDamage = 0.9f;
            lizardBreedParams.biteDamageChance = 0.7f;
            lizardBreedParams.toughness = 1.3f;
            lizardBreedParams.stunToughness = 1f;

            lizardBreedParams.aggressionCurveExponent = 0.095f;
            lizardBreedParams.idleCounterSubtractWhenCloseToIdlePos = 0;

            lizardBreedParams.regainFootingCounter = 4;
            lizardBreedParams.floorLeverage = 1f;
            lizardBreedParams.maxMusclePower = 4f;
            lizardBreedParams.wiggleDelay = 15;
            lizardBreedParams.bodyStiffnes = 0f;
            lizardBreedParams.swimSpeed = 0.75f;
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
            lizardBreedParams.stepLength = 0.6f;
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
            lizardBreedParams.jawOpenLowerJawFac = 0.7f;
            lizardBreedParams.jawOpenMoveJawsApart = 23f;
            lizardBreedParams.headGraphics = new int[5];
            lizardBreedParams.framesBetweenLookFocusChange = 20;
            lizardBreedParams.tongue = false;
            lizardBreedParams.tongueAttackRange = 140f;
            lizardBreedParams.tongueWarmUp = 10;
            lizardBreedParams.tongueSegments = 5;
            lizardBreedParams.tongueChance = 0.25f;
            lizardBreedParams.tamingDifficulty = 10f;
            lizardBreedParams.tailSegments = Random.Range(4, 6);
            lizardBreedParams.tailLengthFactor = 1.4f;
            lizardBreedParams.danger = 2f;
            lizardBreedParams.standardColor = Color.red;
            lizardBreedParams.headSize = 1f;
            lizardBreedParams.bodySizeFac = 1.2f;
            lizardBreedParams.limbSize = 1f;
            lizardBreedParams.bodyLengthFac = 1f;
            lizardBreedParams.bodyMass = 1.2f;
            lizardBreedParams.limbQuickness = 1.6f;
            lizardBreedParams.wiggleSpeed = 2f;
            lizardBreedParams.baseSpeed = 1.6f;

            temp.waterPathingResistance = 5f;
            temp.visualRadius = 950f;
            temp.waterVision = 0.4f;
            temp.throughSurfaceVision = 0.85f;
            temp.breedParameters = lizardBreedParams;
            temp.baseDamageResistance = lizardBreedParams.toughness * 2f;
            temp.baseStunResistance = lizardBreedParams.toughness;

            temp.damageRestistances[(int)Creature.DamageType.Bite, 0] = 2.5f;
            temp.damageRestistances[(int)Creature.DamageType.Bite, 1] = 3f;
            temp.meatPoints = 8;
            temp.doPreBakedPathing = false;
            temp.preBakedPathingAncestor = StaticWorld.GetCreatureTemplate(CreatureType.YellowLizard);
            temp.requireAImap = true;
            temp.virtualCreature = false;
            temp.pickupAction = "Bite";
            temp.jumpAction = "Call";
            temp.throwAction = "Launch";
            temp.wormGrassImmune = false;
            temp.canSwim = true;
            temp.dangerousToPlayer = 10f;

            return temp;
        }

        return orig(type, lizardAncestor, pinkTemplate, blueTemplate, greenTemplate);
    }
}