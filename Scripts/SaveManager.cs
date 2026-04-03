using System.IO;

namespace Slumber;

public class SaveManager
{
    public static PlayerData PlayerData = new();
    public static string FileName = "PlayerData.save";
    public static string Library = Path.Combine(FileSaver.ApplicationData, "Slumber");
    public static string FileSavePath { get => Path.Combine(Library, FileName); }

    public static void SaveData()
    {
        PlayerData.CurrentStage = Engine.Stage.GetCurrentStage().GetType().Name;

        FileSaver.SaveData(PlayerData, FileSavePath, FileFormat.Binary);
    }

    public static void LoadData()
    {
        FileSaver.LoadData(PlayerData, FileSavePath, FileFormat.Binary);

        Engine.Stage.AddStageFromString(PlayerData.CurrentStage);
    }
}
