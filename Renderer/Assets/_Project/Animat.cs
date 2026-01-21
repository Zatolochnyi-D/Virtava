using System.Collections.Generic;
using UnityEngine;

public class Animat : MonoBehaviour
{
    [SerializeField] private GameObject _cubePrefab;

    private List<GameObject> _cubes = new();
    
    public void Animate(NormalizedLandmarkPointsList points)
    {
        for (int i = 0; i < points.Points.Count; i++)
        {
            var vector = new Vector3(points.Points[i].X, points.Points[i].Y, points.Points[i].Z);
            if (_cubes.Count <= i)
                _cubes.Add(Instantiate(_cubePrefab, transform));
            _cubes[i].transform.position = vector;
        }
    }
}