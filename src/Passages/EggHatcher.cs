namespace Nyctophobia;

public class EggHatcher : CustomPassage
{
    public override WinState.EndgameID ID => NTEnums.Passage.EggHatcher;
    public override string DisplayName => "Egg Hatcher";
    public override MenuScene.SceneID Scene => MenuScene.SceneID.MainMenu;
    public override bool IsNotched => true;
    public override int ExpeditionScore => 80;
    public override bool IsAvailableForSlugcat(SlugcatStats.Name name) => name == NTEnums.Witness;
    public override WinState.EndgameTracker CreateTracker() => new WinState.IntegerTracker(ID, 0, 0, 1, 3);
    public bool Hatched = false;
    public int HatchedEggs = 0;

    public override void OnWin(WinState winState, RainWorldGame game, WinState.EndgameTracker tracker)
    {
        if (game.session is StoryGameSession storyGameSession && storyGameSession.saveState.saveStateNumber.value != "Witness") return;

        var world = game.world;

        bool anyEggHatched = false;

        // Get the world rooms
        for (int m = 0; m < world.NumberOfRooms; m++)
        {
            // Get the rooms then check by Shelter
            AbstractRoom rm = world.GetAbstractRoom(world.firstRoomIndex + m);
            if (!rm.shelter) continue;

            for (int o = 0; o < rm.entities.Count; o++)
            {
                // Check if NeedleEgg hatched from RegionState
                if (rm.entities[o] is AbstractPhysicalObject entity && entity.type == AbstractObjectType.NeedleEgg)
                {
                    if (entity.realizedObject is NeedleEgg)
                    {
                        anyEggHatched = true;
                        HatchedEggs++;
                    }
                }
            }
        }

        if (!anyEggHatched)
        {
            HatchedEggs--;
        }

        // Check by eggs hatched
        if (tracker.GoalAlreadyFullfilled) return;

        var integerTracker = (WinState.IntegerTracker)tracker;
        integerTracker.SetProgress(integerTracker.progress + HatchedEggs);
    }

    public override void OnDeath(WinState winState, WinState.EndgameTracker tracker)
    {
        // Check by eggs hatched
        if (tracker.GoalAlreadyFullfilled) return;

        HatchedEggs = 0;

        var integerTracker = (WinState.IntegerTracker)tracker;
        integerTracker.SetProgress(0);
    }

}