

using System.IO;
using System.Linq;
using DotTiled.Serialization;
using Gum.Converters;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Gum.Wireframe;
using MonoTile;
using RenderingLibrary.Graphics;

namespace Slumber;

public class MainMenu : Scene
{
  public StackPanel MainPanel;
  public StackPanel Settings;

  public override void EnterTree()
  {
    base.EnterTree();

    Core.Time.TimeScale = 1f; 

    BuildUI();
    
    new CanvasAnchor().Set(n =>
    {
      n.BackBufferColor = new Color(44, 41, 38);
    });
  }

  public void BuildUI()
  {
    MainPanel = new StackPanel();
    MainPanel.AddToRoot();
    
    MainPanel.XUnits = GeneralUnitType.PixelsFromMiddle;
    MainPanel.XOrigin = HorizontalAlignment.Center;
    
    MainPanel.YUnits = GeneralUnitType.PixelsFromMiddle;
    MainPanel.YOrigin = VerticalAlignment.Center;

    var startBtn = new CustomButton();
    MainPanel.AddChild(startBtn);

    startBtn.Y = 5;


    if (!File.Exists("Saved/Persistence"))
    {
      startBtn.Text = "Start";
      startBtn.Click += (sender, args) =>
      {
        Main.GameManager.Change("Caverns1", "door_1");
        MainPanel.RemoveFromRoot();
      };
    }
    else
    {
      startBtn.Text = "Continue";
      startBtn.Click += (sender, args) =>
      {
        Main.GameManager.Load();
        MainPanel.RemoveFromRoot();
      };
    }

    startBtn.IsFocused = true;

    var setBtn = new CustomButton();
    MainPanel.AddChild(setBtn);

    setBtn.Y = 5;

    setBtn.Text = "Settings";
    setBtn.Click += (sender, args) =>
    {
      MainPanel.IsVisible = false;
      Settings.IsVisible = true;
      Settings.Children.FirstOrDefault()?.IsFocused = true; 
    };
    
    var exitBtn = new CustomButton();
    MainPanel.AddChild(exitBtn);

    exitBtn.Y = 5;

    exitBtn.Text = "Quit";
    exitBtn.Click += (sender, args) =>
    {
      Core.Quit();
    };

    Settings = new StackPanel();
    Settings.AddToRoot();

    Settings.XUnits = GeneralUnitType.PixelsFromMiddle;
    Settings.XOrigin = HorizontalAlignment.Center;
    
    Settings.YUnits = GeneralUnitType.PixelsFromMiddle;
    Settings.YOrigin = VerticalAlignment.Center;

    Settings.IsVisible = false;

    var contrBtn = new CustomButton();
    Settings.AddChild(contrBtn);

    contrBtn.Y = 5;

    contrBtn.Text = "Control";
    contrBtn.Click += (sender, args) =>
    {
      Core.Quit();
    };

    var backBtn = new CustomButton();
    Settings.AddChild(backBtn);

    backBtn.Y = 40;

    backBtn.Text = "Back";
    backBtn.Click += (sender, args) =>
    {
      Settings.IsVisible = false;
      MainPanel.IsVisible = true;
      startBtn.IsFocused = true;
    };
  }


  public override void ExitTree()
  {
    base.ExitTree();
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);
  }

  public override void Process(float delta)
  {
    base.Process(delta);
  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }
}
