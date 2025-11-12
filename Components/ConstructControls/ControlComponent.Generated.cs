//Code for ConstructControls/ControlComponent (Container)
using GumRuntime;
using System.Linq;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Slumber.Components.ConstructControls;
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
    public Wrapper WrapperInstance { get; protected set; }
    public ContainerRuntime KeybindButtons { get; protected set; }
    public ConstructButton ResetBindsButton { get; protected set; }

    public ControlComponent(InteractiveGue visual) : base(visual)
    {
    }
    public ControlComponent()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        WrapperInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Wrapper>(this.Visual,"WrapperInstance");
        KeybindButtons = this.Visual?.GetGraphicalUiElementByName("KeybindButtons") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        ResetBindsButton = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructButton>(this.Visual,"ResetBindsButton");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
