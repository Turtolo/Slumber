using System;
using System.Linq;
using ConstructEngine;
using ConstructEngine.Components.Entity;
using ConstructEngine.Directory;
using Slumber.Entities;

namespace Slumber;

public class SaveManager
{
    public static PlayerData PlayerData = new();
    public static string SaveLocation = "PlayerData.json";

    public static void SaveData()
    {
        Player player;

        player = Entity.EntityList.OfType<Player>().FirstOrDefault();

        PlayerData.CurrentScene = Core.SceneManager.GetCurrentScene().GetType().Name;
        
        FileSaver.SaveDataToJson(PlayerData, "", SaveLocation);
    }

    public static void LoadData()
    {
        FileSaver.LoadDataFromJson(PlayerData, SaveLocation);

        Core.SceneManager.AddSceneFromString(PlayerData.CurrentScene);

        Player player;

        player = Entity.EntityList.OfType<Player>().FirstOrDefault();

        player.KinematicBase.Position = PlayerData.CurrentPosition;

    }
}
