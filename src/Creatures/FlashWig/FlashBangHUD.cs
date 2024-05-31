namespace Nyctophobia;

public class FlashBangHUD : HudPart
{
    private const string AfterImageSpriteName = "FlashWig_AfterImageTexture";
    
    private FSprite AfterImageSprite;
    private FSprite FlashSprite;
    private FAtlas AfterImageAtlas;

    private int CurrentFlashDuration;
    private int CurrentFlashMaxDuration;

    public FlashBangHUD(HUD.HUD hud) : base(hud)
    {
        AfterImageSprite = new FSprite("pixel")
        {
            anchorX = 0,
            anchorY = 0,
            isVisible = false
        };

        FlashSprite = new FSprite("pixel")
        {
            scaleX = 1366,
            scaleY = 768,
            anchorX = 0,
            anchorY = 0,
            isVisible = false
        };
        
        hud.fContainers[0].AddChild(AfterImageSprite);
        hud.fContainers[0].AddChild(FlashSprite);
    }

    public void Flash(Texture afterImage, int duration, Color color)
    {
        CurrentFlashDuration = CurrentFlashMaxDuration = duration;

        UnloadAtlas();
        AfterImageAtlas = Futile.atlasManager.LoadAtlasFromTexture(AfterImageSpriteName, afterImage, false);
        AfterImageSprite.element = AfterImageAtlas.elements.First();

        FlashSprite.color = color;
    }

    private void UnloadAtlas()
    {
        if (Futile.atlasManager.DoesContainAtlas(AfterImageSpriteName))
        {
            Futile.atlasManager.UnloadAtlas(AfterImageSpriteName);
        }
    }

    public override void Update()
    {
        base.Update();

        if (CurrentFlashDuration > 0)
        {
            CurrentFlashDuration--;
        }
    }

    public override void Draw(float timeStacker)
    {
        base.Draw(timeStacker);

        if (CurrentFlashDuration > 0)
        {
            AfterImageSprite.isVisible = true;
            FlashSprite.isVisible = true;
            
            AfterImageSprite.MoveToFront();
            FlashSprite.MoveToFront();

            var flashIntensity = (CurrentFlashDuration - timeStacker) / CurrentFlashMaxDuration;

            AfterImageSprite.alpha = Mathf.Lerp(0, 1, 1 - Mathf.Pow(1 - flashIntensity, 2.3f));
            FlashSprite.alpha = Mathf.Lerp(0, 1, Mathf.Pow(flashIntensity, 3.2f));
        }
        else
        {
            AfterImageSprite.isVisible = false;
            FlashSprite.isVisible = false;
        }
    }

    public override void ClearSprites()
    {
        base.ClearSprites();
        
        FlashSprite.RemoveFromContainer();
        AfterImageSprite.RemoveFromContainer();
        UnloadAtlas();
    }
}