using System;

using ImGuiNET;

namespace Slumber
{
  public class Main : Core
  {
    public Main() { }

    protected override void Initialize()
    {
      base.Initialize();

      ClassDB.Initialize(typeof(Main).Assembly);

      Tree.SetScene(Core.Token.Create<GardensTest>());

      Input.AddBind("MoveLeft", new InputAction(Keys.A), new InputAction(Buttons.LeftThumbstickLeft), new InputAction(Buttons.DPadLeft));
      Input.AddBind("MoveRight", new InputAction(Keys.D), new InputAction(Buttons.LeftThumbstickRight), new InputAction(Buttons.DPadRight));
      Input.AddBind("MoveDown", new InputAction(Keys.S), new InputAction(Buttons.LeftThumbstickDown), new InputAction(Buttons.DPadDown));
      Input.AddBind("MoveUp", new InputAction(Keys.W), new InputAction(Buttons.LeftThumbstickUp), new InputAction(Buttons.DPadUp));

      Input.AddBind("Jump", new InputAction(Keys.Space), new InputAction(Buttons.A));
      Input.AddBind("Dash", new InputAction(Keys.E), new InputAction(Buttons.LeftShoulder));

      Input.AddBind("Attack", new InputAction(Keys.K), new InputAction(Buttons.Y));

      Input.AddBind("Pause", new InputAction(Keys.Escape), new InputAction(Buttons.Start));
      Input.AddBind("Back", new InputAction(Keys.X), new InputAction(Buttons.B));

      Prefs.Graphics.Fullscreen = false;

      Prefs.General.ShowCollision = false;

      Prefs.Graphics.MouseVisible = true;

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

      if (Input.Keyboard.WasKeyJustPressed(Keys.R))
        Tree.ReloadCurrentScene();
    }

    private bool showCollision;
    
    protected override void Draw(GameTime gameTime)
    {

      base.Draw(gameTime);
      
      #if DEBUG

      var player = Core.Token.Get<Player>();
      
      if (player == null)
        return;

      Core.Prefs.General.ShowCollision = showCollision;
      Core.Prefs.Apply();

      Core.ImGuiRenderer.BeforeLayout(gameTime);  
      
      ImGui.Begin("Player");  
      ImGui.Text($"Velocity: {Core.Token.Get<Player>()?.Velocity.ToString()}");

      ImGui.PushItemWidth(150); 

      ImGui.InputFloat("BaseGravity", ref player.BaseGravity);
      ImGui.SameLine();
      if (ImGui.Button("Reset##BaseGravity")) 
      {
          player.BaseGravity = 950f;
      }


      ImGui.InputFloat("InitialTerminal", ref player.InitialTerminalVelocity);
      ImGui.SameLine();
      if (ImGui.Button("Reset##InitialTerminal")) 
      {
          player.InitialTerminalVelocity = 300f;
      }


      ImGui.InputFloat("SecondaryTerminal", ref player.SecondaryTerminalVelocity);
      ImGui.SameLine();
      if (ImGui.Button("Reset##SecondaryTerminal")) 
      {
          player.SecondaryTerminalVelocity = 800f;
      }

      ImGui.InputFloat("FallGravity", ref player.FallGravity);
      ImGui.SameLine();
      if (ImGui.Button("Reset##FallGravity")) 
      {
          player.FallGravity = 1950f;
      }

      ImGui.InputFloat("MoveSpeed", ref player.MoveSpeed);
      ImGui.SameLine();
      if (ImGui.Button("Reset##MoveSpeed")) 
      {
          player.MoveSpeed = 130f;
      }

      ImGui.InputFloat("JumpForce", ref player.JumpForce);
      ImGui.SameLine();
      if (ImGui.Button("Reset##JumpForce")) 
      {
          player.JumpForce = -350;
      }

      ImGui.PopItemWidth();
      ImGui.End();

      ImGui.Begin("Debug");

      ImGui.Checkbox("Show-Collision", ref showCollision);

      ImGui.End();


      Core.ImGuiRenderer.AfterLayout();

      #endif
    }
  }
}
