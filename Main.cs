using System;

using ImGuiNET;
using Gum.Forms;
using Gum.Forms.Controls;
using MonoGameGum;
using Gum.Converters;
using RenderingLibrary.Graphics;

namespace Slumber
{
  public class Main : Core
  {
    public Main() { }

    public static GameManager GameManager;

    public static Gum.GumService GumUI => Gum.GumService.Default;

    protected override void Initialize()
    {
      base.Initialize();

      GumUI.Initialize(this);

      GumUI.UseKeyboardDefaults();
      GumUI.UseGamepadDefaults();

      FrameworkElement.TabKeyCombos.Add(new KeyCombo()
      {
          PushedKey = Gum.Forms.Input.Keys.Down
      });
      FrameworkElement.TabReverseKeyCombos.Add(new KeyCombo()
      {
          PushedKey = Gum.Forms.Input.Keys.Up
      });

      ClassDB.Initialize(typeof(Main).Assembly);

      Token.Anchor.SetAnchor<MainMenu>();

      GameManager = new GameManager();

      Input.AddBind("MoveLeft", new InputAction(Keys.A), new InputAction(Buttons.LeftThumbstickLeft), new InputAction(Buttons.DPadLeft));
      Input.AddBind("MoveRight", new InputAction(Keys.D), new InputAction(Buttons.LeftThumbstickRight), new InputAction(Buttons.DPadRight));
      Input.AddBind("MoveDown", new InputAction(Keys.S), new InputAction(Buttons.LeftThumbstickDown), new InputAction(Buttons.DPadDown));
      Input.AddBind("MoveUp", new InputAction(Keys.W), new InputAction(Buttons.LeftThumbstickUp), new InputAction(Buttons.DPadUp));

      Input.AddBind("CamLeft", new InputAction(Keys.A), new InputAction(Buttons.LeftThumbstickLeft), new InputAction(Buttons.DPadLeft));
      Input.AddBind("CamRight", new InputAction(Keys.D), new InputAction(Buttons.LeftThumbstickRight), new InputAction(Buttons.DPadRight));
      Input.AddBind("CamDown", new InputAction(Keys.Down), new InputAction(Buttons.LeftThumbstickDown), new InputAction(Buttons.DPadDown));
      Input.AddBind("CamUp", new InputAction(Keys.Up), new InputAction(Buttons.LeftThumbstickUp), new InputAction(Buttons.DPadUp));

      Input.AddBind("Jump", new InputAction(Keys.Space), new InputAction(Buttons.X));
      Input.AddBind("Dash", new InputAction(Keys.E), new InputAction(Buttons.LeftShoulder));

      Input.AddBind("Interact", new InputAction(Keys.E), new InputAction(Buttons.A));

      Input.AddBind("Attack", new InputAction(Keys.K), new InputAction(Buttons.Y));

      Input.AddBind("Pause", new InputAction(Keys.Escape), new InputAction(Buttons.Start));
      Input.AddBind("Back", new InputAction(Keys.X), new InputAction(Buttons.B));

      Prefs.Graphics.Fullscreen = true;

      Prefs.General.ShowCollision = false;

      Prefs.Graphics.MouseVisible = false;

      ExitOnEsc = false;

      Prefs.Apply();
    }

    protected override void LoadContent()
    {
      base.LoadContent();
    }

    protected override void UnloadContent()
    {
      base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {

      base.Update(gameTime);

      GumUI.Update(gameTime);

      GumUI.Root.HasEvents = false;

      if (Core.Input.Keyboard.WasKeyJustPressed(Keys.R))
        Core.Token.Anchor.ReloadCurrentAnchor();

    }

    bool showCollision;
    float gravity;
    float iniTermVel;
    float secTermVel;
    float fallGrav;
    float movSpe;
    float jmpFrc;

    bool init;

    protected override void Draw(GameTime gameTime)
    {
      base.Draw(gameTime);
      
      GumUI.Draw();

      #if DEBUG

      var player = Core.Token.Get<Player>();

      var camera = Core.Token.Get<PixelCamera>();
      
      if (player == null || Graphics.IsFullScreen)
        return;

      if (!init)
      {
        gravity = player.Properties.BaseGravity;
        iniTermVel = player.Properties.InitialTerminalVelocity;
        secTermVel = player.Properties.SecondaryTerminalVelocity;
        fallGrav = player.Properties.FallGravity;
        movSpe = player.Properties.MoveSpeed;
        jmpFrc = player.Properties.JumpForce;

        init = true;
      }

      Core.Prefs.General.ShowCollision = showCollision;
      Core.Prefs.Apply();

      Core.ImGuiRenderer.BeforeLayout(gameTime);  
      ImGui.Begin("Player");  
      ImGui.Text($"Velocity: {player.Velocity.ToString()}");
      ImGui.Text($"Position: {player.Transform.Global.Position.ToString()}");
      ImGui.Text($"Term: {MathF.Round(player.Properties.CurrentTerminalVelocity / 100f) * 100f}");
      ImGui.Text($"State: {player.Get<StateMachine>()?.Current}");
      ImGui.Text($"Count: {Core.Token.GetAll<Player>().Count}");

      ImGui.PushItemWidth(150);

      ImGui.InputFloat("BaseGravity", ref gravity);
      ImGui.SameLine();
      if (ImGui.Button("Reset##BaseGravity")) 
      {
          gravity = 950f;
      }


      ImGui.InputFloat("InitialTerminal", ref iniTermVel);
      ImGui.SameLine();
      if (ImGui.Button("Reset##InitialTerminal")) 
      {
          iniTermVel = 250f;
      }


      ImGui.InputFloat("SecondaryTerminal", ref secTermVel);
      ImGui.SameLine();
      if (ImGui.Button("Reset##SecondaryTerminal")) 
      {
          secTermVel = 6400f;
      }

      ImGui.InputFloat("FallGravity", ref fallGrav);
      ImGui.SameLine();
      if (ImGui.Button("Reset##FallGravity")) 
      {
          fallGrav = 1500f;
      }

      ImGui.InputFloat("MoveSpeed", ref movSpe);
      ImGui.SameLine();
      if (ImGui.Button("Reset##MoveSpeed")) 
      {
        movSpe = 130f;
      }

      ImGui.InputFloat("JumpForce", ref jmpFrc);
      ImGui.SameLine();
      if (ImGui.Button("Reset##JumpForce")) 
      {
        jmpFrc = -360;
      }

      ImGui.PopItemWidth();
      ImGui.End();

      ImGui.Begin("Debug");

      ImGui.Checkbox("Show-Collision", ref showCollision);

      ImGui.End();

      Core.ImGuiRenderer.AfterLayout();

      player.Properties = player.Properties with
      {
        BaseGravity = gravity,
        InitialTerminalVelocity = iniTermVel,
        SecondaryTerminalVelocity = secTermVel,
        FallGravity = fallGrav,
        MoveSpeed = movSpe,
        JumpForce = jmpFrc
      };  

      #endif
    }
  }
}
