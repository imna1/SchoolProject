using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Object : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
{
    public ObjectType Shape;
    public Rigidbody2D rb;
    public PhysicsMaterial2D Mater;
    public Vector2 Force = new Vector2();
    public Vector2 Speed = new Vector2();
    public float AngularVelocity = 0;
    public bool IsStatic = true;
    public bool IsDragging;
    public bool AllowToDrag = true;
    public SpriteRenderer Renderer;
    public Color DefaultColor;
    public Color SelectedColor;
    public Color DeleteColor;

    private Camera _cam;
    private Vector3 _offset;
    private ObjectManager _instance;
    private float _friction = 0;
    private float _bounciness = 0;
    private bool _isSelected;
    private bool _isSimulated;
    private void Awake()
    {
        Mater = new PhysicsMaterial2D();
        Mater.friction = _friction;
        Mater.bounciness = _bounciness;
        _instance = ObjectManager.instance;
        _cam = Camera.main;
    }
    void Start()
    {
        _offset = transform.position - _cam.ScreenToWorldPoint(Input.mousePosition);
        if(AllowToDrag)
            IsDragging = true;
        GameManager.instance.OnSimulationStarts += StartSimulation;
        GameManager.instance.OnSimulationPauses += PauseSimulation;
        GameManager.instance.OnSimulationEnds += EndSimulation;
        GameManager.instance.OnSimulationUnpauses += UnpauseSimulation;
    }
    private void FixedUpdate()
    {
        if (_isSimulated)
        {
            rb.AddForce(Force, ForceMode2D.Force);
        }
    }
    private void Update()
    {
        if (!_isSimulated)
        {
            if (IsDragging)
            {
                transform.position = _cam.ScreenToWorldPoint(Input.mousePosition) + _offset;
                if (Input.GetMouseButtonUp(0))
                    IsDragging = false;
            }
        }
    }
    public void UpdateMaterialB(float a)
    {
        Mater.bounciness = a;
    }
    public void UpdateMaterialF(float a)
    {
        Mater.friction = a;
    }
    private void StartSimulation()
    {
        _isSimulated = true;
        if (!IsStatic)
        {
            rb.sharedMaterial = Mater;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Speed;
        }
        else
        {
            rb.sharedMaterial = null;
            if (Shape == ObjectType.Square)
                GetComponent<BoxCollider2D>().sharedMaterial = Mater;
            else if(Shape == ObjectType.Circle)
                GetComponent<CircleCollider2D>().sharedMaterial = Mater;
            else
                GetComponent<PolygonCollider2D>().sharedMaterial = Mater;
        }
    }
    private void PauseSimulation()
    {
        _isSimulated = false;
        AngularVelocity = rb.angularVelocity;
        Speed = rb.velocity;
        rb.bodyType = RigidbodyType2D.Static;
    }
    private void UnpauseSimulation()
    {
        _isSimulated = true;
        if (!IsStatic)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Speed;
            rb.angularVelocity = AngularVelocity;
        }
        else
        {
            rb.sharedMaterial = null;
            if (Shape == ObjectType.Square)
                GetComponent<BoxCollider2D>().sharedMaterial = Mater;
            else if (Shape == ObjectType.Circle)
                GetComponent<CircleCollider2D>().sharedMaterial = Mater;
            else
                GetComponent<PolygonCollider2D>().sharedMaterial = Mater;
        }
    }
    private void EndSimulation()
    {
        _isSimulated = false;
        rb.bodyType = RigidbodyType2D.Static;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isSelected)
        {
            _offset = transform.position - _cam.ScreenToWorldPoint(Input.mousePosition);
            IsDragging = true;
        }
        else
            _instance.SelectNewObject(transform);
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if(_isSelected)
            _instance.UpdateInspector();
    }
    public void SelectObject()
    {
        _isSelected = true;
        Renderer.color = SelectedColor;
    }
    public void DeselectObject()
    {
        _isSelected = false;
        Renderer.color = DefaultColor;
    }
    private void OnDestroy()
    {
        GameManager.instance.OnSimulationStarts -= StartSimulation;
        GameManager.instance.OnSimulationPauses -= PauseSimulation;
        GameManager.instance.OnSimulationEnds -= EndSimulation;
        GameManager.instance.OnSimulationUnpauses -= UnpauseSimulation;
    }
}
