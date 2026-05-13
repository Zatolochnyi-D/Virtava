using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ListItemController : MonoBehaviour, IPointerClickHandler
{
    public event Action<int> OnClick;

    [SerializeField] private TextMeshProUGUI _nameTextBar;
    [SerializeField] private GameObject _selectionMarker;

    private int _index = -1;

    void Awake()
    {
        _selectionMarker.SetActive(false);
    }

    public void SetName(string name)
    {
        _nameTextBar.text = name;
    }

    public void SetIndex(int index)
    {
        _index = index;
    }

    public void Select()
    {
        _selectionMarker.SetActive(true);
    }

    public void Deselect()
    {
        _selectionMarker.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(_index);
    }
}