using UnityEngine;

public class ModelListController : MonoBehaviour
{
    [SerializeField] private ModelSelector _modelSelector;

    private GameObject _template;
    private (GameObject gameObject, ListItemController controller)[] _items;

    void Awake()
    {
        _template = transform.GetChild(0).gameObject;
        _template.SetActive(false);
        _items = new (GameObject, ListItemController)[_modelSelector.AnimatableModels.Count];
        var counter = 0;
        foreach (var obj in _modelSelector.AnimatableModels)
        {
            var item = Instantiate(_template, transform);
            _items[counter] = (item, item.GetComponent<ListItemController>());
            _items[counter].gameObject.SetActive(true);
            _items[counter].controller.SetName(obj.name);
            _items[counter].controller.SetIndex(counter);
            _items[counter].controller.OnClick += HandleItemOnClick;
            counter++;
        }
        _items[_modelSelector.SelectedModel].controller.Select();
    }

    private void HandleItemOnClick(int index)
    {
        if (index == _modelSelector.SelectedModel)
            return;
        _items[_modelSelector.SelectedModel].controller.Deselect();
        _modelSelector.SelectModel(index);
        _items[_modelSelector.SelectedModel].controller.Select();
    }
}