//Code for TitleScreen
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

namespace Slumber.Screens;
partial class TitleScreen : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("TitleScreen");
#if DEBUG
if(element == null) throw new System.InvalidOperationException("Could not find an element named TitleScreen - did you forget to load a Gum project?");
#endif
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new TitleScreen(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(TitleScreen)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("TitleScreen", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ContainerRuntime Main { get; protected set; }
    public ContainerRuntime Root { get; protected set; }
    public ConstructButton StartButton { get; protected set; }
    public ConstructButton SettingsButton { get; protected set; }
    public ConstructButton QuitButton { get; protected set; }
    public Settings Settings { get; protected set; }
    public ControlComponent Controls { get; protected set; }

    public TitleScreen(InteractiveGue visual) : base(visual)
    {
    }
    public TitleScreen()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Main = this.Visual?.GetGraphicalUiElementByName("Main") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        Root = this.Visual?.GetGraphicalUiElementByName("Root") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        StartButton = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructButton>(this.Visual,"StartButton");
        SettingsButton = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructButton>(this.Visual,"SettingsButton");
        QuitButton = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ConstructButton>(this.Visual,"QuitButton");
        Settings = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Settings>(this.Visual,"Settings");
        Controls = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ControlComponent>(this.Visual,"Controls");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
