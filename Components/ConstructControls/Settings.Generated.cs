//Code for ConstructControls/Settings (Container)
using GumRuntime;
using System.Linq;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Slumber.Components.ConstructControls;
using Slumber.Components.Controls;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace Slumber.Components.ConstructControls;
partial class Settings : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("ConstructControls/Settings");
#if DEBUG
if(element == null) throw new System.InvalidOperationException("Could not find an element named ConstructControls/Settings - did you forget to load a Gum project?");
#endif
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new Settings(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(Settings)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("ConstructControls/Settings", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public NineSliceRuntime BackgroundPanel { get; protected set; }
    public ContainerRuntime NonPanel { get; protected set; }
    public ConstructLabel SFXLabel { get; protected set; }
    public ConstructSlider SFXSlider { get; protected set; }
    public ConstructLabel MusicLabel { get; protected set; }
    public ConstructSlider MusicSlider { get; protected set; }
    public ConstructLabel MasterLabel { get; protected set; }
    public ConstructSlider MasterSlider { get; protected set; }
    public ConstructLabel VsyncLabel { get; protected set; }
    public ConstructCheckBox VsyncCheckbox { get; protected set; }
    public ContainerRuntime HorizontalGrid { get; protected set; }
    public ConstructButton Back { get; protected set; }

    public Settings(InteractiveGue visual) : base(visual)
    {
    }
    public Settings()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        BackgroundPanel = this.Visual?.GetGraphicalUiElementByName("BackgroundPanel") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        NonPanel = this.Visual?.GetGraphicalUiElementByName("NonPanel") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        SFXLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructLabel>(this.Visual,"SFXLabel");
        SFXSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructSlider>(this.Visual,"SFXSlider");
        MusicLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructLabel>(this.Visual,"MusicLabel");
        MusicSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructSlider>(this.Visual,"MusicSlider");
        MasterLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructLabel>(this.Visual,"MasterLabel");
        MasterSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructSlider>(this.Visual,"MasterSlider");
        VsyncLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructLabel>(this.Visual,"VsyncLabel");
        VsyncCheckbox = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructCheckBox>(this.Visual,"VsyncCheckbox");
        HorizontalGrid = this.Visual?.GetGraphicalUiElementByName("HorizontalGrid") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        Back = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructButton>(this.Visual,"Back");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
