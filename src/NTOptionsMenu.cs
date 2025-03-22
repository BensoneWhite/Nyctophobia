namespace Nyctophobia;

public class NTOptionsMenu : OptionInterface
{
    private OpKeyBinder _dashKeyBinder;
    private OpLabel _dashKeyLabel;

    public static Configurable<KeyCode> Dash { get; private set; }
    public static Configurable<bool> DisableFestiveDays { get; private set; }
    public static Configurable<bool> DisablePrideDay { get; private set; }

    public NTOptionsMenu()
    {
        Dash = config.Bind<KeyCode>("dash", 0);
        DisableFestiveDays = config.Bind("FestiveDays", false);
        DisablePrideDay = config.Bind("PrideDay", false);
    }

    public override void Update()
    {
        base.Update();

        Color colorOn = new Color(0.6627f, 0.6431f, 0.698f);

        _dashKeyBinder.greyedOut = false;
        _dashKeyLabel.color = colorOn;
    }

    public override void Initialize()
    {
        var controller = OpKeyBinder.BindController.AnyController;

        var optionsTab = new OpTab(this, "Options");
        Tabs = [optionsTab];

        var tabContainer = new OpContainer(new Vector2(0, 0));
        optionsTab.AddItems(tabContainer);

        UIelement[] uiElements =
        [
            new OpLabel(0f, 580f, "Options", bigText: true),

            _dashKeyBinder = new OpKeyBinder(Dash, new Vector2(10f, 530f), new Vector2(150f, 10f), false, controller),
            _dashKeyLabel = new OpLabel(170f, 535f, "Dash keybind")
            {
                description = Translate("This is going to be your dash keybind for Exile campaign")
            },

            new OpCheckBox(DisableFestiveDays, 10f, 450f),
            new OpLabel(45f, 490f, "Disable Festive Days", false)
            {
                description = Translate("Disable all the event days for Nyctophobia")
            },

            new OpCheckBox(DisablePrideDay, 10f, 410f),
            new OpLabel(45f, 450f, "Disable Pride Day", false)
            {
                description = Translate("Disable all the 1st June changes")
            },
        ];

        optionsTab.AddItems(uiElements);
    }
}