namespace Nyctophobia;

public class NTOptionsMenu : OptionInterface
{

    private OpKeyBinder DashBind;
    private OpLabel DashBindLabel;

    public static Configurable<KeyCode> Dash;

    public NTOptionsMenu()
    {
        Dash = config.Bind<KeyCode>("dash", 0);
    }

    public override void Update()
    {
        base.Update();

        Color colorOff = new(0.1451f, 0.1412f, 0.1529f);
        Color colorOn = new(0.6627f, 0.6431f, 0.698f);

        DashBind.greyedOut = false;
        DashBindLabel.color = colorOn;
    }

    public override void Initialize()
    {
        OpKeyBinder.BindController controllerNumber;
        controllerNumber = OpKeyBinder.BindController.AnyController;

        var opTab1 = new OpTab(this, "Options");

        Tabs = new[] { opTab1 };

        OpContainer tab1Container = new(new Vector2(0, 0));
        opTab1.AddItems(tab1Container);

        UIelement[] UIArrayElements1 = new UIelement[]
        {
            new OpLabel(0f, 580f, "Options", bigText: true),

            DashBind = new(Dash, new Vector2(10f, 530f), new Vector2(150f, 10f), false, controllerNumber),
            DashBindLabel = new(170f, 535f, "keybind for WAWA"),
        };
        opTab1.AddItems(UIArrayElements1);
    }
}
