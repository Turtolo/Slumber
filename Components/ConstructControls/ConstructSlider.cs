using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;
using System;
using System.Linq;
using System.Numerics;

namespace Slumber.Components.ConstructControls
{
    partial class ConstructSlider
    {
        partial void CustomInitialize()
        {
            TrackPercentOverlay.Width = (float)(Value * (Width / 100));

            ValueChanged += (_, _) =>
            {
                TrackPercentOverlay.Width = (float)(Value * (Width / 100));
            };
        }
    }
}
