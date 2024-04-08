using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    //used in Objects and ObjectManager
    public UnityAction OnSimulationStarts;
    public UnityAction OnSimulationEnds;
    public UnityAction OnSimulationPauses;
    public UnityAction OnSimulationUnpauses;

    public List<Transform> objects;
    public SaveAndLoad SandL;

    [SerializeField] private Button _play;
    [SerializeField] private Button _end;
    [SerializeField] private Button _pause;

    [SerializeField] private GameObject _square;
    [SerializeField] private GameObject _circle;
    [SerializeField] private GameObject _hex;
    private bool _isPaused = false;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        SaveData data = SandL.Load(SceneLoader.instance.SceneName);
        Item[] items = data.Items;
        Physics2D.gravity = new Vector2(0, -data.Gravity);
        for (int i = 0; i < items.Length; i++)
        {
            GameObject obj;
            if (items[i].Shape == ObjectType.Square)
            {
                obj = Instantiate(_square);
                objects.Add(obj.transform);
            }
            else if (items[i].Shape == ObjectType.Circle)
            {
                obj = Instantiate(_circle);
                objects.Add(obj.transform);
            }
            else
            {
                obj = Instantiate(_hex);
                objects.Add(obj.transform);
            }
        }
        for (int i = 0; i < items.Length; i++)
        {
            var script = objects[i].GetComponent<Object>();
            script.Shape = items[i].Shape;
            objects[i].position = items[i].Position;
            objects[i].localScale = items[i].Size;
            objects[i].rotation = items[i].Rotation;
            script.IsStatic = items[i].IsStatic;
            script.rb.mass = items[i].Mass;
            script.Speed = items[i].Speed;
            script.Force = items[i].Force;
            script.Mater.friction = items[i].Friction;
            script.Mater.bounciness = items[i].Bounciness;
            script.AllowToDrag = false;
        }
    }
    public void QuitToMenu()
    {
        Item[] items = new Item[objects.Count];
        for (int i = 0; i < objects.Count; i++)
        {
            var script = objects[i].GetComponent<Object>();
            Item item = new Item();
            item.Shape = script.Shape;
            item.Position = objects[i].position;
            item.Size = objects[i].localScale;
            item.Rotation = objects[i].rotation;
            item.IsStatic = script.IsStatic;
            item.Mass = script.rb.mass;
            item.Speed = script.Speed;
            item.Force = script.Force;
            item.Friction = script.Mater.friction;
            item.Bounciness = script.Mater.bounciness;
            items[i] = item;
        }
        SaveData data = new SaveData();
        data.Items = items;
        data.Gravity = -Physics2D.gravity.y;
        SandL.Save(data, SceneLoader.instance.SceneName);

        SceneLoader.instance.LoadMenu();
    }
    public void StartSimulation()
    {
        _play.interactable = false;
        _end.interactable = true;
        _pause.interactable = true;

        Item[] items = new Item[objects.Count];
        for (int i = 0; i < objects.Count; i++)
        {
            var script = objects[i].GetComponent<Object>();
            Item item = new Item();
            item.Shape = script.Shape;
            item.Position = objects[i].position;
            item.Size = objects[i].localScale;
            item.Rotation = objects[i].rotation;
            item.IsStatic = script.IsStatic;
            item.Mass = script.rb.mass;
            item.Speed = script.Speed;
            item.Force = script.Force;
            item.Friction = script.Mater.friction;
            item.Bounciness = script.Mater.bounciness;
            items[i] = item;
        }
        SaveData data = new SaveData();
        data.Items = items;
        data.Gravity = -Physics2D.gravity.y;
        SandL.Save(data, SceneLoader.instance.SceneName);

        if (OnSimulationStarts != null)
           OnSimulationStarts.Invoke();
    }
    public void StopSimulation()
    {
        _play.interactable = true;
        _end.interactable = false;
        _pause.interactable = false;

        if (OnSimulationEnds != null)
            OnSimulationEnds.Invoke();
        SaveData data = SandL.Load(SceneLoader.instance.SceneName);
        Item[] items = data.Items;
        for (int i = 0; i < items.Length; i++)
        {
            var script = objects[i].GetComponent<Object>();
            script.Shape = items[i].Shape;
            objects[i].position = items[i].Position;
            objects[i].localScale = items[i].Size;
            objects[i].rotation = items[i].Rotation;
            script.IsStatic = items[i].IsStatic;
            script.rb.mass = items[i].Mass;
            script.Speed = items[i].Speed;
            script.Force = items[i].Force;
            script.Mater.friction = items[i].Friction;
            script.Mater.bounciness = items[i].Bounciness;
        }
    }
    public void PauseSimulation()
    {
        if (_isPaused)
        {
            _isPaused = false;
            if (OnSimulationUnpauses != null)
                OnSimulationUnpauses.Invoke();
        }
        else
        {
            _isPaused = true;
            if (OnSimulationPauses != null)
                OnSimulationPauses.Invoke();
        }
    }
}
public enum ObjectType
{
    Square,
    Circle,
    Other
}
