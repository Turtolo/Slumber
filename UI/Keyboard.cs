using System.Collections.Generic;
using Gum.Converters;
using Gum.Forms.Controls;
using RenderingLibrary.Graphics;

namespace Slumber;

public class Keyboard : StackPanel
{
  public Keyboard()
  {
    var keyLookUp = new Dictionary<string, string>
    {
      {"MoveLeft", "Left"},
    };

    this.XUnits = GeneralUnitType.PixelsFromMiddle;
    this.XOrigin = HorizontalAlignment.Center;
    
    this.YUnits = GeneralUnitType.PixelsFromMiddle;
    this.YOrigin = VerticalAlignment.Center;

    this.Height = 20;
    this.Width = 20;

    var texture = Core.Resource.Load<MTexture>("Graphics/DPadUp");

    var kbvBox = new StackPanel();
    AddChild(kbvBox);

    kbvBox.XUnits = GeneralUnitType.PixelsFromMiddle;
    kbvBox.XOrigin = HorizontalAlignment.Center;
    
    kbvBox.YUnits = GeneralUnitType.PixelsFromMiddle;
    kbvBox.YOrigin = VerticalAlignment.Center;
    
    foreach(var bind in Core.Input.Binds)
    {
      string valResult = "";
      foreach (var val in bind.Value)
      {
        string i = string.Empty;
        if (val.HasKey)
          valResult += $" {val.Key} ";
        else if (val.HasButton)
          valResult += $" {val.Button} ";
        else if (val.HasMouseButton)
          valResult += $" {val.MouseButton} ";
      }
      
      var button = new TextAndSpriteButton(bind.Key, texture);
      button.Y = 5;

      kbvBox.AddChild(button);
    }

    var resetBtn = new CustomButton();
    AddChild(resetBtn);

    resetBtn.Y = 5;

    resetBtn.Text = "Reset";
    resetBtn.Click += (sender, args) =>
    {
      Core.Quit();
    };

  }
}
