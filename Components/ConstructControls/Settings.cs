using ConstructEngine.UI;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary.Graphics;

using System.Linq;
using ConstructEngine.UI;
using System;
using ConstructEngine;

namespace Slumber.Components.ConstructControls
{
    partial class Settings : IGumUpdatable
    {
        partial void CustomInitialize()
        {
            GumManager.Register(this);
            
            ControlButton.Click += (_, _) =>
            {
                Main.IsVisible = false;
                Controls.IsVisible = true;
                Controls.ResetBindsButton.IsFocused = true;
            };
        }

        public void Update(GameTime gameTime)
        {
            Controls.Update(Engine.DeltaTime);
        }

        


    }
}
