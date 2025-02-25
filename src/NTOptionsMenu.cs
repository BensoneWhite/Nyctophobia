namespace Nyctophobia;

public class NTOptionsMenu : OptionInterface
{
    // Private UI fields use underscore naming per C# convention.
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

        // If it is Pride Day, choose random colors; otherwise use fixed theme colors.
        Color colorOn = IsPrideDay
            ? new Color(Random.value, Random.value, Random.value)
            : new Color(0.6627f, 0.6431f, 0.698f);

        _dashKeyBinder.greyedOut = false;
        _dashKeyLabel.color = colorOn;
    }

    public override void Initialize()
    {
        // Check for any Controller input
        var controller = OpKeyBinder.BindController.AnyController;

        // Create the main options tab.
        var optionsTab = new OpTab(this, "Options");
        Tabs = [optionsTab];

        // Create a container for layout.
        var tabContainer = new OpContainer(new Vector2(0, 0));
        optionsTab.AddItems(tabContainer);

        // Build the UI elements for the options menu.
        UIelement[] uiElements =
        [
            // Title of the label
            new OpLabel(0f, 580f, "Options", bigText: true),

            // Dash keybind and its label
            _dashKeyBinder = new OpKeyBinder(Dash, new Vector2(10f, 530f), new Vector2(150f, 10f), false, controller),
            _dashKeyLabel = new OpLabel(170f, 535f, "Dash keybind")
            {
                description = Translate("This is going to be your dash keybind for Exile campaign")
            },

            // Option to disable Festive Days
            new OpCheckBox(DisableFestiveDays, 10f, 450f),
            new OpLabel(45f, 490f, "Disable Festive Days", false)
            {
                description = Translate("Disable all the event days for Nyctophobia")
            },

            // Option to disable Pride Day
            new OpCheckBox(DisablePrideDay, 10f, 410f),
            new OpLabel(45f, 450f, "Disable Pride Day", false)
            {
                description = Translate("Disable all the 1st June changes")
            },
        ];

        // Add all the UI elements to the options tab.
        optionsTab.AddItems(uiElements);
    }
}