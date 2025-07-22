namespace Nyctophobia;

public class NTOptionsMenu : OptionInterface
{
    private OpKeyBinder DashKeyBinder;
    private OpLabel DashKeyLabel;

    private OpLabel Options;

    private OpCheckBox FestiveDays;
    private OpLabel FestiveDaysLabel;

    public static Configurable<KeyCode> Dash { get; private set; }
    public static Configurable<bool> DisableFestiveDays { get; private set; }

    public NTOptionsMenu()
    {
        Dash = config.Bind<KeyCode>("dash", 0);
        DisableFestiveDays = config.Bind("FestiveDays", false);
    }

    public override void Update()
    {
        base.Update();

        Color colorOn = new Color(0.6627f, 0.6431f, 0.698f);

        DashKeyBinder.greyedOut = false;
        FestiveDays.greyedOut = false;

        DashKeyLabel.color = colorOn;
        Options.color = colorOn;
        FestiveDaysLabel.color = colorOn;
    }

    public override void Initialize()
    {
        var controller = OpKeyBinder.BindController.AnyController;

        var optionsTab = new OpTab(this, Translate("Options"));
        Tabs = [optionsTab];

        var tabContainer = new OpContainer(new Vector2(0, 0));
        optionsTab.AddItems(tabContainer);

        UIelement[] uiElements =
        [
            Options = new OpLabel(0f, 580f, Translate("Options"), bigText: true),

            DashKeyBinder = new OpKeyBinder(Dash, new Vector2(10f, 530f), new Vector2(150f, 10f), false, controller),
            DashKeyLabel = new OpLabel(170f, 535f, Translate("Dash keybind"))
            {
                description = Translate("This is going to be your dash keybind for Exile campaign")
            },

            FestiveDays = new OpCheckBox(DisableFestiveDays, 10f, 450f),
            FestiveDaysLabel = new OpLabel(45f, 490f, Translate("Disable Festive Days"), false)
            {
                description = Translate("Disable all the event days for Nyctophobia")
            },
        ];

        optionsTab.AddItems(uiElements);
    }
}