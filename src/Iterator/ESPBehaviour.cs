namespace Nyctophobia;

public class ESPBehavior : OracleBehavior
{
    public bool dialogstarted;
    public ESPBehavior(Oracle oracle) : base(oracle)
    {
        dialogstarted = true;
    }
    public override DialogBox dialogBox
    {
        get
        {
            if (oracle.room.game.cameras[0].hud.dialogBox == null)
            {
                oracle.room.game.cameras[0].hud.InitDialogBox();
                oracle.room.game.cameras[0].hud.dialogBox.defaultYPos = -10f;
            }
            return oracle.room.game.cameras[0].hud.dialogBox;
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (player != null)
        {
            if (Vector2.Distance(oracle.firstChunk.pos, player.firstChunk.pos) > 100)
            {
                oracle.firstChunk.vel = (player.firstChunk.pos - oracle.firstChunk.pos).normalized * 5;
            }
            lookPoint = player.bodyChunks[0].pos;
            if (dialogstarted)
            {
                //oracle.room.game.cameras[0].hud.textPrompt.AddMessage("Time, Infinite Majesty", 5, 100, false, false);
                //Dialog here, like "dialogBox.NewMessage("Welcome back here, V1", 60);
                dialogBox.NewMessage("Witness??????", 60);
                dialogstarted = false;
                oracle.room.gravity = 0;
            }
        }
    }
}
