namespace Nyctophobia;

// Berserker mode will be the corruption mode
public class NWBerserkerMode
{
    private const int PacificationDuration = 20 * 40;

    private static readonly ConditionalWeakTable<ArtificialIntelligence, PacifiedAIData> PacifiedAIs = new();

    private static void PacifyAI(ArtificialIntelligence ai) => PacifiedAIs.GetValue(ai, _ => new PacifiedAIData()).PacifiedTime = PacificationDuration;

    private class PacifiedAIData
    {
        public int PacifiedTime;
    }

    public void Apply()
    {
        On.Player.Update += Player_Update;
        On.ArtificialIntelligence.Update += ArtificialIntelligence_Update;
    }

    private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        if (!self.IsNightWalker(out var nw)) return;

        if (nw.BerserkerMode[self])
        {
            if (self.abstractCreature.abstractAI?.RealAI is { } ai)
            {
                PacifyAI(ai);
            }
        }
    }

    private void ArtificialIntelligence_Update(On.ArtificialIntelligence.orig_Update orig, ArtificialIntelligence self)
    {
        orig(self);

        if (PacifiedAIs.TryGetValue(self, out var pacifiedAIData))
        {
            if (self.threatTracker != null)
            {
                foreach (var threat in self.threatTracker.threatCreatures.ToList())
                {
                    if (threat.creature.representedCreature.creatureTemplate.type == CreatureType.Slugcat)
                    {
                        threat.Destroy(true);
                        threat.creature.Destroy();
                    }
                }
            }

            if (self.preyTracker != null)
            {
                foreach (var prey in self.preyTracker.prey.ToList())
                {
                    if (prey.critRep.representedCreature.creatureTemplate.type == CreatureType.Slugcat)
                    {
                        self.preyTracker.ForgetPrey(prey.critRep.representedCreature);
                        prey.critRep.Destroy();
                    }
                }
            }

            if (self.agressionTracker != null)
            {
                foreach (var creature in self.agressionTracker.creatures.ToList())
                {
                    if (creature.crit.representedCreature.creatureTemplate.type == CreatureType.Slugcat)
                    {
                        self.agressionTracker.ForgetCreature(creature.crit.representedCreature);
                        creature.crit.Destroy();
                    }
                }
            }

            pacifiedAIData.PacifiedTime--;
            if (pacifiedAIData.PacifiedTime <= 0)
            {
                PacifiedAIs.Remove(self);
            }
        }
    }
}