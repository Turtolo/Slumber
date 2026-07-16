using System.Linq;

namespace Slumber;

public class SceneChange : Node2D
{
  public Area2D Area; 

  public string SceneName;

  public override void Process(float delta)
  {
    if (Area.GetAnyBody() is Player)
    {
      Type t = null;
      
      t = Type.GetType(SceneName);

      if (t == null)
      {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          t = assembly.GetTypes().FirstOrDefault(x => x.Name == SceneName || x.FullName == SceneName);
          if (t != null) break; 
        }
      }

      if (t != null)
      {
        var instance = Activator.CreateInstance(t);

        if (instance is Node scene)
        {
          Core.Tree.SetScene(scene);
        }
        else
        {
          Console.WriteLine($"Error: {SceneName} is not a Node.");
        }
      }
      else
      {
        Console.WriteLine($"Error: Could not find type matching '{SceneName}' in any assembly.");
      }
    }
  }
}

