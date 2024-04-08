using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ObjectManager _objectManager;
    private Object _obj;
    private bool _trashing;
    private void Start()
    {
        _objectManager = ObjectManager.instance;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_objectManager.Selected == null)
            return;
        _obj = _objectManager.Selected.GetComponent<Object>();
        if (_obj.IsDragging)
        {
            _trashing = true;
            _obj.Renderer.color = _obj.DeleteColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_trashing)
        {
            _obj.Renderer.color = _obj.SelectedColor;
            _trashing = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && _trashing && _obj.IsDragging)
        {
            _objectManager.DeleteObject(_obj.gameObject);
            _trashing = false;
        }
    }
}
