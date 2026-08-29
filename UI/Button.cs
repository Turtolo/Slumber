using Gum.Converters;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;

namespace Slumber;

public class CustomButton : Button
{
  public CustomButton()
  {
    var buttonVisual = (Gum.Forms.DefaultVisuals.V3.ButtonVisual)Visual; 
    
    NineSliceRuntime background = buttonVisual.Background;

    buttonVisual.Background.Texture = Core.Resources.Pixel.ToTexture();

    TextRuntime text = buttonVisual.TextInstance;

    text.UseCustomFont = true;
    text.CustomFontFile = "Fonts/RainHearts.fnt";
    text.FontScale = 1.0f;
    
    background.Color = new Color(0, 0, 0) * 0.6f;
    text.Color = Color.White;

    ContainerRuntime borderContainer = new ContainerRuntime();
    borderContainer.WidthUnits = DimensionUnitType.RelativeToParent;
    borderContainer.Width = 0;
    borderContainer.HeightUnits = DimensionUnitType.RelativeToParent;
    borderContainer.Height = 0;
    borderContainer.HasEvents = false;
    buttonVisual.Children.Add(borderContainer);

    float borderWidth = 2.0f;

    var top = CreateBorderLine();
    top.WidthUnits = DimensionUnitType.RelativeToParent;
    top.Width = 0;
    top.Height = borderWidth;
    top.Y = 0;
    borderContainer.Children.Add(top);

    var bottom = CreateBorderLine();
    bottom.WidthUnits = DimensionUnitType.RelativeToParent;
    bottom.Width = 0;
    bottom.Height = borderWidth;
    bottom.YUnits = GeneralUnitType.PixelsFromBaseline;
    bottom.Y = 0;
    borderContainer.Children.Add(bottom);

    var left = CreateBorderLine();
    left.Width = borderWidth;
    left.HeightUnits = DimensionUnitType.RelativeToParent;
    left.Height = 0;
    left.X = 0;
    borderContainer.Children.Add(left);

    var right = CreateBorderLine();
    right.Width = borderWidth;
    right.HeightUnits = DimensionUnitType.RelativeToParent;
    right.Height = 0;
    right.XUnits = GeneralUnitType.PixelsFromLarge;
    right.X = 0;
    borderContainer.Children.Add(right);

    borderContainer.Visible = false;

    StateSave enabledState = buttonVisual.States.Enabled;
    enabledState.Apply = () =>
    {
      background.Color = new Color(0, 0, 0) * 0.6f;
      borderContainer.Visible = false;
    };

    StateSave pushedState = buttonVisual.States.Pushed;
    pushedState.Apply = () =>
    {
      background.Color = new Color(64, 64, 64) * 0.42f;
      borderContainer.Visible = true;
    };

    StateSave focusedState = buttonVisual.States.Focused;
    focusedState.Apply = () =>
    {
      background.Color = new Color(0, 0, 0) * 0.6f;
      borderContainer.Visible = true;
      IsFocused = true;
    };

    StateSave highlightedFocused = buttonVisual.States.HighlightedFocused;
    highlightedFocused.Apply = focusedState.Apply;

    StateSave highlighted = buttonVisual.States.Highlighted;
    highlighted.Apply = focusedState.Apply;
  }

  public NineSliceRuntime CreateBorderLine()
  {
    var line = new NineSliceRuntime();
    line.Texture = Core.Resources.Pixel.ToTexture();
    line.Color = Color.White;
    return line;
  }
}
