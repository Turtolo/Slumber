//Code for ConstructControls/ConstructSlider (Container)
using GumRuntime;
using System.Linq;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Slumber.Components.Controls;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace Slumber.Components.ConstructControls;
partial class ConstructSlider : global::Gum.Forms.Controls.Slider
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("ConstructControls/ConstructSlider");
#if DEBUG
if(element == null) throw new System.InvalidOperationException("Could not find an element named ConstructControls/ConstructSlider - did you forget to load a Gum project?");
#endif
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ConstructSlider(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ConstructSlider)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("ConstructControls/ConstructSlider", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum SliderCategory
    {
        Enabled,
        Disabled,
        DisabledFocused,
        Focused,
        Highlighted,
        HighlightedFocused,
        Pushed,
    }

    SliderCategory? _sliderCategoryState;
    public SliderCategory? SliderCategoryState
    {
        get => _sliderCategoryState;
        set
        {
            _sliderCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("SliderCategory"))
                {
                    var category = Visual.Categories["SliderCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "SliderCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public ContainerRuntime TrackInstance { get; protected set; }
    public NineSliceRuntime TrackBackground { get; protected set; }
    public ButtonStandard ThumbInstance { get; protected set; }
    public NineSliceRuntime FocusedIndicator { get; protected set; }
    public NineSliceRuntime TrackPercentOverlay { get; protected set; }

    public float SliderPercent
    {
        get => ThumbInstance.Visual.X;
        set => ThumbInstance.Visual.X = value;
    }

    public float TrackPercentOverlayWidth
    {
        get => TrackPercentOverlay.Width;
        set => TrackPercentOverlay.Width = value;
    }

    public ConstructSlider(InteractiveGue visual) : base(visual)
    {
    }
    public ConstructSlider()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        TrackInstance = this.Visual?.GetGraphicalUiElementByName("TrackInstance") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        TrackBackground = this.Visual?.GetGraphicalUiElementByName("TrackBackground") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        ThumbInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonStandard>(this.Visual,"ThumbInstance");
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        TrackPercentOverlay = this.Visual?.GetGraphicalUiElementByName("TrackPercentOverlay") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
