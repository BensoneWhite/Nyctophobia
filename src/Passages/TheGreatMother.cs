namespace Nyctophobia;

public class TheGreatMother : CustomPassage
{
    public override WinState.EndgameID ID => NTEnums.Passage.TheGreatMother;
    public override string DisplayName => "The Great Mother";
    public override MenuScene.SceneID Scene => MenuScene.SceneID.MainMenu;
    public override bool IsNotched => true;
    public override int ExpeditionScore => 80;
    public override bool IsAvailableForSlugcat(SlugcatStats.Name name) => true;
    public override WinState.EndgameTracker CreateTracker() => new WinState.IntegerTracker(ID, 0, 0, 1, 5);

    public int pupCount;

    public override void OnWin(WinState winState, RainWorldGame game, WinState.EndgameTracker tracker)
    {
        pupCount = 0;

        for (int r = 0; r < game.GetStorySession.playerSessionRecords.Length; r++)
        {
            PlayerSessionRecord record = game.GetStorySession.playerSessionRecords[r];
            pupCount = record.pupCountInDen;
        }

        if (tracker.GoalAlreadyFullfilled) return;

        var integerTracker = (WinState.IntegerTracker)tracker;

        if (pupCount >= 3)
        {
            integerTracker.SetProgress(integerTracker.progress + 1);
        }
    }

    public override void OnDeath(WinState winState, WinState.EndgameTracker tracker)
    {
        if (tracker.GoalAlreadyFullfilled) return;

        var integerTracker = (WinState.IntegerTracker)tracker;

        if (pupCount > 3)
        {
            integerTracker.SetProgress(0);
        }
    }
}