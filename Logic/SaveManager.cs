using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using ConstructEngine;
using ConstructEngine.Components.Entity;
using ConstructEngine.Directory;
using Slumber.Entities;

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

        player = Entity.EntityList.OfType<Player>().FirstOrDefault();

        PlayerData.CurrentScene = Core.SceneManager.GetCurrentScene().GetType().Name;

        FileSaver.SaveDataToJson(PlayerData, FileSavePath);

    }

    public static void LoadData()
    {
        FileSaver.LoadDataFromJson(PlayerData, FileSavePath);

        Core.SceneManager.AddSceneFromString(PlayerData.CurrentScene);

        Player player;

        player = Entity.EntityList.OfType<Player>().FirstOrDefault();

        player.KinematicBase.Position = PlayerData.CurrentPosition;

    }
}
