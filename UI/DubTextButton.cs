using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;

namespace Slumber;

public class DubTextButton : CustomButton
{
  public string LeftText { get; set; }
  public string RightText { get; set; }
  
  public DubTextButton(string leftText, string rightText)
  {
    LeftText = leftText;
    RightText = rightText;

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

    var rightLabel = new Label();
    rightLabel.WidthUnits = DimensionUnitType.RelativeToChildren;
    rightLabel.Width = 0;
    rightLabel.HeightUnits = DimensionUnitType.RelativeToChildren;
    rightLabel.Height = 0;
    rightLabel.XUnits = GeneralUnitType.PixelsFromLarge;
    rightLabel.X = -15;
    rightLabel.XOrigin = HorizontalAlignment.Right;
    rightLabel.Y = 0;
    rightLabel.YUnits = GeneralUnitType.PixelsFromMiddle;
    rightLabel.YOrigin = VerticalAlignment.Center;
    rightLabel.Text = RightText;
    AddChild(rightLabel);


    var textRightVisual = (Gum.Forms.DefaultVisuals.V3.LabelVisual)rightLabel.Visual;

    textRightVisual.UseCustomFont = true;
    textRightVisual.CustomFontFile = "Fonts/RainHearts.fnt";
    textRightVisual.FontScale = 1.0f;
    textRightVisual.Color = Color.White;


    var textLeftVisual = (Gum.Forms.DefaultVisuals.V3.LabelVisual)leftLabel.Visual;

    textLeftVisual.UseCustomFont = true;
    textLeftVisual.CustomFontFile = "Fonts/RainHearts.fnt";
    textLeftVisual.FontScale = 1.0f;
    textLeftVisual.Color = Color.White;
  }
}
