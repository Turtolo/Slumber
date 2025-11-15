using ConstructEngine;
using ConstructEngine.Input;
using ConstructEngine.Util;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slumber.Components.ConstructControls
{
    partial class ControlComponent
    {
        private bool isRebinding;
        private bool waitingForKeyRelease;
        private ConstructButtonDuo currentRebindButton = null;
        private string currentAction;
        private bool resetBinds;
        private List<ConstructButtonDuo> ButtonList = new(); 

        partial void CustomInitialize()
        {
            foreach (var kvp in Engine.Input.Binds)
            {
                var keybindButton = new ConstructButtonDuo();
                KeybindButtons.AddChild(keybindButton);

                keybindButton.Width = 400;
                keybindButton.X = 125;
                keybindButton.TextLeft = kvp.Key;
                keybindButton.TextRight = kvp.Value.FirstOrDefault().Key.ToString();

                keybindButton.Click += (_, _) =>
                {
                    if (!isRebinding && !waitingForKeyRelease)
                    {
                        currentRebindButton = keybindButton;
                        currentAction = kvp.Key;
                        keybindButton.TextRight = "Press a key...";

                        waitingForKeyRelease = true;
                    }
                };

                ButtonList.Add(keybindButton); 
            }

            ResetBindsButton.Click += (_, _) =>
            {
                Engine.Input.ResetBinds();  
                resetBinds = true;
            };
        }

        public void Update(float deltaTime)
        {
            var keyboardState = Keyboard.GetState();

            if (waitingForKeyRelease)
            {
                if (!keyboardState.GetPressedKeys().Any())
                {
                    waitingForKeyRelease = false;
                    isRebinding = true;
                }
                return;
            }

            if (isRebinding)
            {
                var key = Engine.Input.Keyboard.GetFirstKeyDown();
                var mouse = Engine.Input.Mouse.GetFirstButtonDown();
                var button = Engine.Input.CurrentGamePad.GetFirstButtonDown();

                if (key != Keys.None)
                {
                    Engine.Input.Rebind(currentAction, key);
                    currentRebindButton.TextRight = key.ToString();
                    isRebinding = false;
                    currentRebindButton = null;
                }

                if (mouse != MouseButton.None)
                {
                    Engine.Input.Rebind(currentAction, mouse);
                    currentRebindButton.TextRight = mouse.ToString();
                    isRebinding = false;
                    currentRebindButton = null;
                }

                if (button != Buttons.None)
                {
                    Engine.Input.Rebind(currentAction, button);
                    currentRebindButton.TextRight = button.ToString();
                    isRebinding = false;
                    currentRebindButton = null;
                }
            }

            if (resetBinds)
            {
                for (int i = 0; i < ButtonList.Count; i++)
                {
                    var button = ButtonList[i];
                    var action = button.TextLeft;
                    if (Engine.Input.InitialBinds.TryGetValue(action, out var keyList))
                    {
                        button.TextRight = keyList.FirstOrDefault().Key.ToString();
                    }
                }

                resetBinds = false;
            }

        }
    }
}
