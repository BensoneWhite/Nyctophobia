namespace Nyctophobia;

//This should be removed after Watcher DLC
public static class BigAcronymFix
{
    public static void Apply()
    {
        IL.Menu.FastTravelScreen.ctor += FixBigAcronymIL;
        IL.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += FixBigAcronymIL;
        IL.PlayerProgression.MiscProgressionData.ConditionalShelterData.GetShelterRegion += FixBigAcronymIL;
        IL.PlayerProgression.MiscProgressionData.SaveDiscoveredShelter += FixBigAcronymIL;
        IL.SaveState.SaveToString += FixBigAcronymIL;
    }

    public static void ApplyExpedition() =>
        IL.Expedition.ChallengeTools.ValidRegionPearl += FixBigAcronymIL;

    public static void FixBigAcronymIL(ILContext il)
    {
        var cursor = new ILCursor(il);

        // Look for the IL pattern: ldc.i4.0, ldc.i4.2, then a call to string.Substring
        while (cursor.TryGotoNext(MoveType.Before,
                   i => i.MatchLdcI4(0),
                   i => i.MatchLdcI4(2),
                   i => i.MatchCallOrCallvirt<string>(nameof(string.Substring))))
        {
            // Advance to the call instruction.
            cursor.Index += 2;
            // Remove the original call to string.Substring.
            cursor.Remove();
            // Insert custom delegate to perform the new substring operation.
            cursor.EmitDelegate((string text, int start, int length) =>
            {
                var underscorePos = text.IndexOf('_', start);
                return text.Substring(start, underscorePos >= 0 ? underscorePos - start : length);
            });
        }
    }
}