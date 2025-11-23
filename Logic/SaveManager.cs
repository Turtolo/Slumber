namespace Slumber;

public class SaveManager
{
    public static PlayerData PlayerData = new();
    public static string FileName = "PlayerData.json";
    public static string Library = Path.Combine(FileSaver.ApplicationData, "Slumber");
    public static string FileSavePath { get => Path.Combine(Library, FileName); }

    public static void SaveData()
    {
        Player player;

        player = Node.AllInstances.OfType<Player>().FirstOrDefault();

        PlayerData.CurrentScene = Engine.SceneManager.GetCurrentScene().GetType().Name;

        FileSaver.SaveDataToJson(PlayerData, FileSavePath);

    }

    public static void LoadData()
    {
        FileSaver.LoadDataFromJson(PlayerData, FileSavePath);

        Engine.SceneManager.AddSceneFromString(PlayerData.CurrentScene);

        Player player = Node.AllInstances.OfType<Player>().FirstOrDefault();

        
        player.Location = PlayerData.CurrentPosition.ToPoint();
        

    }
}
