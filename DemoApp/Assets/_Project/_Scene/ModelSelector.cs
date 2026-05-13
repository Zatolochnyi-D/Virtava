using System.Collections.Generic;
using UnityEngine;

public class ModelSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] _animatableModels;

    private int _selectedModel = 0;

    public IReadOnlyCollection<GameObject> AnimatableModels => _animatableModels;
    public int SelectedModel => _selectedModel;

    void Awake()
    {
        for (int i = 0; i < _animatableModels.Length; i++)
        {
            _animatableModels[i] = Instantiate(_animatableModels[i]);
            _animatableModels[i].SetActive(false);
        }
        _animatableModels[_selectedModel].SetActive(true);
    }

    public void SelectModel(int modelIndex)
    {
        _animatableModels[_selectedModel].SetActive(false);
        _animatableModels[modelIndex].SetActive(true);
        _selectedModel = modelIndex;
    }
}