using Gum.Converters;
using Gum.Forms.Controls;
using RenderingLibrary.Graphics;

namespace Slumber;

public class PauseMenu : Node2D
{
  public bool Open = false;

  public StackPanel Settings;
  public StackPanel MainPanel;

  public override void EnterTree()
  {
    MainPanel = new StackPanel();

    MainPanel.IsVisible = false;
    Visible = false;

    MainPanel.AddToRoot();
    
    MainPanel.XUnits = GeneralUnitType.PixelsFromMiddle;
    MainPanel.XOrigin = HorizontalAlignment.Center;
    
    MainPanel.YUnits = GeneralUnitType.PixelsFromMiddle;
    MainPanel.YOrigin = VerticalAlignment.Center;

    var startBtn = new CustomButton();
    MainPanel.AddChild(startBtn);

    startBtn.Y = 5;

    startBtn.Text = "Continue";
    startBtn.Click += (sender, args) =>
    {
      CloseMenu();
    };

    startBtn.IsFocused = true;

    var loadBtn = new CustomButton();
    MainPanel.AddChild(loadBtn);

    loadBtn.Y = 5;

    loadBtn.Text = "Load";
    loadBtn.Click += (sender, args) =>
    {
      CloseMenu();
      Main.GameManager.Load();
    };

    var exitBtn = new CustomButton();
    MainPanel.AddChild(exitBtn);

    exitBtn.Y = 5;

    exitBtn.Text = "Quit";
    exitBtn.Click += (sender, args) =>
    {
      CloseMenu();
      Main.GameManager.Transition("MainMenu", () => {});
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

    contrBtn.Text = "Controls";
    contrBtn.Click += (sender, args) =>
    {
    };

    var keybBtn = new CustomButton();
    Settings.AddChild(keybBtn);

    keybBtn.Y = 5;

    keybBtn.Text = "Keyboard";
    keybBtn.Click += (sender, args) =>
    {
    };

    var audBtn = new CustomButton();
    Settings.AddChild(audBtn);

    audBtn.Y = 5;

    audBtn.Text = "Audio";
    audBtn.Click += (sender, args) =>
    {
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

  public void OpenMenu()
  {
    Open = true;
    Visible = true;
    Core.Time.TimeScale = 0f;
  }

  public void CloseMenu()
  {
    Open = false;
    Visible = false;
    Core.Time.TimeScale = 1f;
  }

  public override void Process(float delta)
  {
    MainPanel.X = Transform.Global.Position.X;
    MainPanel.Y = Transform.Global.Position.Y;
    
    MainPanel.IsVisible = Material.Global.Visible;

    if (Core.Input.IsActionJustPressed("Pause"))
    {
      if (!Open)
      {
        OpenMenu();
      }
      else
      {
        CloseMenu();
      }
    }
  }
}
