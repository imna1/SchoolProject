using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectIcon : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector3 _zOffset;
    private ObjectManager _objectManager;
    private GameManager _gameManager;
    private Camera _cam;
    private void Awake()
    {
        _cam = Camera.main;
    }
    private void Start()
    {
        _objectManager = ObjectManager.instance;
        _gameManager = GameManager.instance;
    }
    public void Init()
    {
        var obj = Instantiate(_prefab, _cam.ScreenToWorldPoint(Input.mousePosition) + _zOffset, Quaternion.identity);
        _objectManager.SelectNewObject(obj.transform);
        _gameManager.objects.Add(obj.transform);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Init();
    }
}
