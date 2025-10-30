//Code for ConstructControls/ControlComponent (Container)
using GumRuntime;
using System.Linq;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace Slumber.Components.ConstructControls;
partial class ControlComponent : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("ConstructControls/ControlComponent");
#if DEBUG
if(element == null) throw new System.InvalidOperationException("Could not find an element named ConstructControls/ControlComponent - did you forget to load a Gum project?");
#endif
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ControlComponent(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ControlComponent)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("ConstructControls/ControlComponent", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public NineSliceRuntime BackgroundPanel { get; protected set; }
    public ContainerRuntime Wrapper { get; protected set; }

    public ControlComponent(InteractiveGue visual) : base(visual)
    {
    }
    public ControlComponent()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        BackgroundPanel = this.Visual?.GetGraphicalUiElementByName("BackgroundPanel") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        Wrapper = this.Visual?.GetGraphicalUiElementByName("Wrapper") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
