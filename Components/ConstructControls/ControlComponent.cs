using ConstructEngine;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using System.Collections.Generic;

namespace Slumber.Components.ConstructControls
{
    partial class ControlComponent
    {
        private bool isWaitingForKey = false;
        private bool waitingForKeyRelease = false;
        private ConstructButtonDuo currentRebindButton = null;
        private string currentAction = null; // store which action we're rebinding

        partial void CustomInitialize()
        {
            List<ConstructButtonDuo> actionButtons = new();

            foreach (var kvp in Core.Input.KeyboardBinds)
            {
                var keybindButton = new ConstructButtonDuo();

                KeybindButtons.AddChild(keybindButton);

                keybindButton.Width = 400;
                keybindButton.X = 125;

                keybindButton.TextLeft = kvp.Key;       // the action name
                keybindButton.TextRight = kvp.Value.ToString(); // the bound key

                actionButtons.Add(keybindButton);

                keybindButton.Click += (_, _) =>
                {
                    if (isWaitingForKey)
                        return;

                    isWaitingForKey = true;
                    waitingForKeyRelease = true;
                    currentRebindButton = keybindButton;
                    currentAction = kvp.Key;

                    keybindButton.TextRight = "Press a key...";
                };
            }
        }

        public void Update()
        {
            if (isWaitingForKey && currentRebindButton != null)
            {
                var state = Keyboard.GetState();
                Keys[] pressed = state.GetPressedKeys();

                if (waitingForKeyRelease)
                {
                    if (pressed.Length == 0)
                        waitingForKeyRelease = false;

                    return;
                }

                if (pressed.Length > 0)
                {
                    Keys newKey = pressed[0];
                    
                    currentRebindButton.TextRight = newKey.ToString();

                    Core.Input.KeyboardBinds[currentAction] = newKey;

                    isWaitingForKey = false;
                    waitingForKeyRelease = false;
                    currentRebindButton = null;
                    currentAction = null;
                }
            }
        }
    }
}
