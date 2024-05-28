namespace Nyctophobia;

public class VoidMeltController
{
    public static void Apply()
    {
        IL.RainWorldGame.RawUpdate += RainWorldGameOnRawUpdate;
    }

    private static void RainWorldGameOnRawUpdate(ILContext il)
    {
        var cursor = new ILCursor(il);

        for (var n = 0; n < 2; n++)
        {
            cursor.GotoNext(MoveType.After,
                i => i.MatchLdfld<UpdatableAndDeletable>(nameof(UpdatableAndDeletable.room)),
                i => i.MatchLdfld<Room>(nameof(Room.roomSettings)),
                i => i.MatchLdsfld<RoomSettings.RoomEffect.Type>(nameof(RoomSettings.RoomEffect.Type.VoidMelt)),
                i => i.MatchCallOrCallvirt<RoomSettings>(nameof(RoomSettings.GetEffectAmount)));
        }

        cursor.MoveAfterLabels();
        cursor.Emit(OpCodes.Ldarg_0);
        cursor.EmitDelegate((float orig, RainWorldGame self) =>
        {
            foreach (var player in self.session.Players)
            {
                if (player.realizedCreature is { room: { } room })
                {
                    foreach (var effect in room.roomSettings.effects)
                    {
                        if (effect.type == NTEnums.RoomEffect.VoidMeltController)
                        {
                            return orig * effect.amount;
                        }
                    }
                }
            }

            return orig;
        });
    }
}