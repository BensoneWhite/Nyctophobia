namespace Nyctophobia;

public class NTOptionsMenu : OptionInterface
{
    private OpKeyBinder DashBind;
    private OpLabel DashBindLabel;
    public static Configurable<KeyCode> Dash;
    public static Configurable<bool> Boykisser;
    public static Configurable<bool> FestiveDays;
    public static Configurable<bool> PrideDay;

    public NTOptionsMenu()
    {
        Dash = config.Bind<KeyCode>("dash", 0);
        Boykisser = config.Bind("Boykisser", false);
        FestiveDays = config.Bind("FestiveDays", false);
        PrideDay = config.Bind("PrideDay", false);
    }

    public override void Update()
    {
        base.Update();

        Color colorOff;
        Color colorOn;

        if (IsPrideDay)
        {
            colorOff = new(Random.value, Random.value, Random.value);
            colorOn = new(Random.value, Random.value, Random.value);
        }
        else
        {
            colorOff = new(0.1451f, 0.1412f, 0.1529f);
            colorOn = new(0.6627f, 0.6431f, 0.698f);
        }

        DashBind.greyedOut = false;
        DashBindLabel.color = colorOn;
    }

    public override void Initialize()
    {
        OpKeyBinder.BindController controllerNumber;
        controllerNumber = OpKeyBinder.BindController.AnyController;

        OpTab opTab1 = new(this, "Options");

        Tabs = [opTab1];

        OpContainer tab1Container = new(new Vector2(0, 0));
        opTab1.AddItems(tab1Container);

        UIelement[] UIArrayElements1 =
        [
            new OpLabel(0f, 580f, "Options", bigText: true),

            DashBind = new(Dash, new Vector2(10f, 530f), new Vector2(150f, 10f), false, controllerNumber),
            DashBindLabel = new(170f, 535f, "Dash keybind") {description = Translate("This is going to be your dash keybind for Exile campaign") },

            new OpCheckBox(Boykisser, 10f, 490f),
            new OpLabel(45f, 490f, "Disable the Boykisser Spawn", false) {description = Translate("Don't you like kissing boys?")},

            new OpCheckBox(FestiveDays, 10f, 450f),
            new OpLabel(45f, 450f, "Disable Festive Days", false) {description = Translate("Disable all the Festive days for Nyctophobia")},

            new OpCheckBox(PrideDay, 10f, 410f),
            new OpLabel(45f, 410f, "Disable Pride Day", false) {description = Translate("Disable all the 1st June features that last 1 day")},
        ];
        opTab1.AddItems(UIArrayElements1);
    }
}