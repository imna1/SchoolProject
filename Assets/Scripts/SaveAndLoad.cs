using UnityEngine;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    public void Save(SaveData data, string name)
    {
        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, $"{name}.json");
        File.WriteAllText(path, json);
    }
    public SaveData Load(string name)
    {
        return JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + $"/{name}.json"));
    }
}

[System.Serializable]
public class Item
{
    public ObjectType Shape;
    public Vector3 Position;
    public Vector3 Size;
    public Quaternion Rotation;
    public bool IsStatic;
    public float Mass;
    public float Bounciness;
    public float Friction;
    public Vector2 Speed;
    public Vector2 Force;
}

[System.Serializable]
public class SaveData
{
    public float Gravity;
    public Item[] Items;
}
