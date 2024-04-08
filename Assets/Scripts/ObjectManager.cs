using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    static public ObjectManager instance;
    [HideInInspector] public Transform Selected;
    [HideInInspector] public Object SelectedScript;

    [SerializeField] private GameObject[] _fieldsToHide;
    [SerializeField] private GameObject _objInspector;
    [SerializeField] private GameObject _generalInspector;
    [SerializeField] private Transform _camera;
    [SerializeField] private TMP_InputField _xPos;
    [SerializeField] private TMP_InputField _yPos;
    [SerializeField] private TMP_InputField _xSize;
    [SerializeField] private TMP_InputField _ySize;
    [SerializeField] private TMP_InputField _angle;
    [SerializeField] private TMP_InputField _mass;
    [SerializeField] private TMP_InputField _friction;
    [SerializeField] private TMP_InputField _bounciness;
    [SerializeField] private TMP_InputField _speedValue;
    [SerializeField] private TMP_InputField _speedAngle;
    [SerializeField] private TMP_InputField _forceValue;
    [SerializeField] private TMP_InputField _forceAngle;
    [SerializeField] private TMP_InputField _gravity;
    [SerializeField] private Toggle _isStatic;
    [SerializeField] private float _camSpeed;

    private GameManager _gameManager;
    private bool _isSimulated = false;
    private bool _isPlaying = false;
    private bool _dragging = false;
    private bool _isOpenGeneralInspector = true;
    private float _ratio;
    private Vector2 _lastClick;
    private Camera _cam;
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
        _cam = Camera.main;
        _ratio = 1280f / Screen.width;
        _xPos.onEndEdit.AddListener(delegate { UpdateObjectPosition(); });
        _yPos.onEndEdit.AddListener(delegate { UpdateObjectPosition(); });
        _xSize.onEndEdit.AddListener(delegate { UpdateObjectScale(); });
        _ySize.onEndEdit.AddListener(delegate { UpdateObjectScale(); });
        _angle.onEndEdit.AddListener(delegate { UpdateObjectRotation(); });
        _friction.onEndEdit.AddListener(delegate { UpdateObjectFriction(); });
        _bounciness.onEndEdit.AddListener(delegate { UpdateObjectBounciness(); });
        _mass.onEndEdit.AddListener(delegate { UpdateObjectMass(); });
        _speedValue.onEndEdit.AddListener(delegate { UpdateObjectSpeed(); });
        _speedAngle.onEndEdit.AddListener(delegate { UpdateObjectSpeed(); });
        _forceValue.onEndEdit.AddListener(delegate { UpdateObjectForce(); });
        _forceAngle.onEndEdit.AddListener(delegate { UpdateObjectForce(); });
        _gravity.onEndEdit.AddListener(delegate { UpdateGravity(); });
        _isStatic.onValueChanged.AddListener(delegate { UpdateObjectStatic(); });

        _gameManager = GameManager.instance;
        _gameManager.OnSimulationStarts += StartSimulation;
        _gameManager.OnSimulationUnpauses += UnpauseSimulation;
        _gameManager.OnSimulationPauses += PauseSimulation;
        _gameManager.OnSimulationEnds += EndSimulation;

        UpdateInspector();
    }
    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if(Physics2D.Raycast(_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
            {
                _isOpenGeneralInspector = false;
                UpdateInspector();
            }
            else if (EventSystem.current.IsPointerOverGameObject())
            {
            }
            else
            {
                _dragging = false;
                _isOpenGeneralInspector = true;
                if (Selected != null)
                    SelectedScript.DeselectObject();
                Selected = null;
                UpdateInspector();
            }
        }
        if (Selected != null && _isSimulated && !_isOpenGeneralInspector)
            UpdateInspector();
        if (_isOpenGeneralInspector)
        {
            if (Input.GetMouseButton(0) && _dragging)
            {
                Vector2 click = Input.mousePosition;
                _camera.position += (Vector3)(_lastClick - click) * 0.003f * _ratio * _cam.orthographicSize;
                _lastClick = click;
            }
            _lastClick = Input.mousePosition;
            _dragging = true;
        }
        float mv = Input.GetAxis("Mouse ScrollWheel");
        if(mv != 0f)
        {
            _cam.orthographicSize -= _cam.orthographicSize * _camSpeed * Mathf.Sign(mv);
        }
    }

    private void StartSimulation()
    {
        _isSimulated = true;
        _isPlaying = true;
    }
    private void UnpauseSimulation()
    {
        _isSimulated = true;
        _isPlaying = true;
    }
    private void PauseSimulation()
    {
        _isSimulated = false;
        _isPlaying = true;
    }
    private void EndSimulation()
    {
        _isSimulated = false;
        _isPlaying = false;
        UpdateInspector();
    }
    public void SelectNewObject(Transform obj)
    {
        if(Selected != null)
            SelectedScript.DeselectObject();
        Selected = obj;
        SelectedScript = obj.GetComponent<Object>();
        SelectedScript.SelectObject();
        UpdateInspector();
    }
    public void UpdateInspector()
    {
        if(Selected == null)
        {
            _generalInspector.SetActive(true);
            _objInspector.SetActive(false);
            _gravity.text = (-Physics2D.gravity.y).ToString();
            /*
            _xPos.text =  " ";
            _yPos.text =  " ";
            _xSize.text = " ";
            _ySize.text = " ";
            _angle.text = " ";
            _bounciness.text = " ";
            _friction.text = " ";
            _isStatic.SetIsOnWithoutNotify(true);
            foreach (var item in _fieldsToHide)
            {
                item.SetActive(false);
            }
            */
            return;
        }
        _generalInspector.SetActive(false);
        _objInspector.SetActive(true);
        _xPos.text = Selected.position.x.ToString();
        _yPos.text = Selected.position.y.ToString();
        _xSize.text = Selected.localScale.x.ToString();
        _ySize.text = Selected.localScale.y.ToString();
        _angle.text = Selected.eulerAngles.z.ToString();
        _bounciness.text = SelectedScript.Mater.bounciness.ToString();
        _friction.text = SelectedScript.Mater.friction.ToString();
        if (SelectedScript.IsStatic)
        {
            _isStatic.SetIsOnWithoutNotify(true);
            foreach (var item in _fieldsToHide)
            {
                item.SetActive(false);
            }
        }
        else
        {
            _isStatic.SetIsOnWithoutNotify(false);
            foreach (var item in _fieldsToHide)
            {
                item.SetActive(true);
            }
            _mass.text = SelectedScript.rb.mass.ToString();
            Vector2 vec = _isPlaying ? SelectedScript.rb.velocity : SelectedScript.Speed;
            _speedValue.text = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y).ToString();
            _speedAngle.text = (Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg).ToString();
            vec = SelectedScript.Force;
            _forceValue.text = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y).ToString();
            _forceAngle.text = (Mathf.Tan(vec.y / vec.x) * Mathf.Rad2Deg).ToString();
        }
    }
    public void UpdateObjectPosition()
    {
        if (Selected == null)
            return;
        float x;
        float y;
        float.TryParse(_xPos.text, out x);
        float.TryParse(_yPos.text, out y);
        Selected.position = new Vector2(x, y);
    }
    public void UpdateObjectScale()
    {
        if (Selected == null)
            return;
        float x;
        float y;
        float.TryParse(_xSize.text, out x);
        float.TryParse(_ySize.text, out y);
        Selected.localScale = new Vector2(x, y);
    }
    public void UpdateObjectRotation()
    {
        if (Selected == null)
            return;
        float z;
        float.TryParse(_angle.text, out z);
        Selected.rotation = Quaternion.Euler(Selected.rotation.x, Selected.rotation.y, z);
    }
    public void UpdateObjectMass()
    {
        if (Selected == null)
            return;
        float mass;
        float.TryParse(_mass.text, out mass);
        SelectedScript.rb.mass = mass;
    }
    public void UpdateObjectFriction()
    {
        if (Selected == null)
            return;
        float mass;
        float.TryParse(_friction.text, out mass);
        SelectedScript.Mater.friction = mass;
    }
    public void UpdateObjectBounciness()
    {
        if (Selected == null)
            return;
        float mass;
        float.TryParse(_bounciness.text, out mass);
        SelectedScript.Mater.bounciness = mass;
    }
    public void UpdateObjectSpeed()
    {
        if (Selected == null)
            return;
        float value;
        float angle;
        float.TryParse(_speedValue.text, out value);
        float.TryParse(_speedAngle.text, out angle);
        SelectedScript.Speed = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * value, Mathf.Sin(angle * Mathf.Deg2Rad) * value);
    }
    public void UpdateObjectForce()
    {
        if (Selected == null)
            return;
        float value;
        float angle;
        float.TryParse(_forceValue.text, out value);
        float.TryParse(_forceAngle.text, out angle);
        SelectedScript.Force = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * value, Mathf.Sin(angle * Mathf.Deg2Rad) * value);
    }
    public void UpdateObjectStatic()
    {
        if (Selected == null)
            return;
        SelectedScript.IsStatic = _isStatic.isOn;
        if (SelectedScript.IsStatic)
        {
            foreach (var item in _fieldsToHide)
            {
                item.SetActive(false);
            }
        }
        else
        {
            foreach (var item in _fieldsToHide)
            {
                item.SetActive(true);
            }
            _mass.text = SelectedScript.rb.mass.ToString();
            Vector2 vec = SelectedScript.rb.velocity;
            _speedValue.text = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y).ToString();
            _speedAngle.text = (Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg).ToString();
            vec = SelectedScript.Force;
            _forceValue.text = Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y).ToString();
            _forceAngle.text = (Mathf.Tan(vec.y / vec.x) * Mathf.Rad2Deg).ToString();
        }
    }
    public void UpdateGravity()
    {
        float value;
        float.TryParse(_gravity.text, out value);
        Physics2D.gravity = new Vector2(0, -value);
    }
    public void DeleteObject(GameObject obj)
    {
        Selected = null;
        _gameManager.objects.Remove(obj.transform);
        Destroy(obj);
    }
}
