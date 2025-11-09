using System;
using Microsoft.Xna.Framework;

public class IdleState : State
{
    public override void OnEnter()
    {
        Console.WriteLine("Entering Menu State");
    }

    public override void Update(GameTime gameTime)
    {
        
    }
    
    public override void OnExit()
    {
        Console.WriteLine("Exiting Menu State");
    }
}