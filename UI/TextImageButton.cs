
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using RenderingLibrary.Graphics;

namespace Slumber;

public class TextAndSpriteButton : CustomButton
{
    public string LeftText { get; set; }
    public MTexture RightTexture { get; set; }
  
    public TextAndSpriteButton(string leftText, MTexture rightTexture)
    {
        LeftText = leftText;
        RightTexture = rightTexture;

        this.Text = string.Empty; 

        this.WidthUnits = DimensionUnitType.Absolute;
        this.Width = 400; 
        
        this.HeightUnits = DimensionUnitType.RelativeToChildren;
        this.Height = 12;

        var leftLabel = new Label();
        leftLabel.WidthUnits = DimensionUnitType.RelativeToChildren;
        leftLabel.Width = 0;
        leftLabel.HeightUnits = DimensionUnitType.RelativeToChildren;
        leftLabel.Height = 0;
        leftLabel.X = 15;
        leftLabel.Y = 0;
        leftLabel.YUnits = GeneralUnitType.PixelsFromMiddle;
        leftLabel.YOrigin = VerticalAlignment.Center;
        leftLabel.Text = LeftText;
        AddChild(leftLabel);

        var textLeftVisual = (Gum.Forms.DefaultVisuals.V3.LabelVisual)leftLabel.Visual;
        textLeftVisual.UseCustomFont = true;
        textLeftVisual.CustomFontFile = "Fonts/RainHearts.fnt";
        textLeftVisual.FontScale = 1.0f;
        textLeftVisual.Color = Color.White;

        var rightSprite = new SpriteRuntime();
        
        rightSprite.WidthUnits = DimensionUnitType.Absolute;
        rightSprite.Width = 24; 
        rightSprite.HeightUnits = DimensionUnitType.Absolute;
        rightSprite.Height = 24;

        rightSprite.XUnits = GeneralUnitType.PixelsFromLarge;
        rightSprite.X = -15;
        rightSprite.XOrigin = HorizontalAlignment.Right;
        rightSprite.Y = 0;
        rightSprite.YUnits = GeneralUnitType.PixelsFromMiddle;
        rightSprite.YOrigin = VerticalAlignment.Center;

        rightSprite.Texture = RightTexture.ToTexture();

        this.Visual.Children.Add(rightSprite);
    }
}
