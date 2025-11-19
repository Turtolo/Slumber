namespace Slumber.Logic;

public class Pausemenu : BackseatComponent
{
    private PauseMenu Menu;

    public Pausemenu(object root) : base(root)
    {
        Menu = new PauseMenu();
        GumHelper.AddScreenToRoot(Menu);
    }
    
    public override void Update(GameTime gameTime)
    {

        
        if (Engine.Input.IsActionJustPressed("Pause"))
        {
            Engine.SceneManager.QueueFreezeCurrentScene();
            Menu.Main.Visible = true;
        }

        if (Engine.Input.IsActionJustPressed("Back"))
        {
            if (Menu.Main.Visible)
            {
                Engine.SceneManager.UnfreezeCurrentScene();
                Menu.Root.Visible = false;
            }
            if (Menu.Settings.Main.IsVisible)
            {
                Menu.Settings.IsVisible = false;
                Menu.Main.Visible = true;
                Menu.ResumeButton.IsFocused = true;
            }
            if (Menu.Settings.Controls.IsVisible)
            {
                Menu.Settings.Controls.IsVisible = false;
                Menu.Settings.Main.IsVisible = true;
                Menu.Settings.ControlButton.IsFocused = true;
            }
        }

        
    }
}
