using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        Debug.Log("File JSON sẽ được lưu tại: " + filePath);

        Invoke(nameof(ReadFile), 2f);
    }

    private void ReadFile()
    {
        FileManager fileManager = new FileManager();
        GameData data = fileManager.LoadData();
        Debug.Log("Loaded Player: " + data.playerName + ", High Score: " + data.highScore);
    }

    // 📌 1️⃣ Tạo file JSON mặc định
    public void CreateDefaultFile()
    {
        GameData defaultData = new GameData
        {
            highScore = 0,
            playerName = "DefaultPlayer",
            brickPositions = new List<Vector3> { new Vector3(0, 0, 0) }
        };

        SaveData(defaultData);
        Debug.Log("Tạo file JSON mặc định thành công!");
    }

    // 📌 2️⃣ Lưu dữ liệu vào file JSON
    public void SaveData(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Dữ liệu đã được lưu vào file JSON!");
    }

    // 📌 3️⃣ Đọc dữ liệu từ file JSON
    public GameData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.LogWarning("⚠ File JSON chưa tồn tại, hãy tạo trước!");
            return null;
        }
    }
}
