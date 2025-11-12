using ConstructEngine;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using RenderingLibrary.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Slumber.Components.ConstructControls
{
    partial class ControlComponent
    {
        partial void CustomInitialize()
        {


            List<ConstructButtonDuo> actionButtons = new();

            foreach (var kvp in Core.Input.KeyboardBinds)
            {
                var keybindButton = new ConstructButtonDuo();

                KeybindButtons.AddChild(keybindButton);

                keybindButton.Width = 400;

                keybindButton.X = 125;

                keybindButton.TextLeft = kvp.Key;
                keybindButton.TextRight = kvp.Value.ToString();

                actionButtons.Add(keybindButton);

                keybindButton.Click += (_, _) =>
                {
                    keybindButton.TextLeft = "Press key to rebind.....";
                    keybindButton.TextRight = "";
                };
            }
            

        }
    }
}
