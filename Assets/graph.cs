using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class graph : MonoBehaviour
{
    [SerializeField] private Transform point;
    [SerializeField, Range(10, 100)] private int resolution = 10;
    [SerializeField] private FunctionLibrary.FunctionName function;
    Transform[] points;

    private void Awake()
    {
        points = new Transform[resolution * resolution];
        var position = Vector3.zero;
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z++;
            }
            points[i] = Instantiate(point);
            points[i].localScale = scale;
            position.x = (x + 0.5f) * step - 1f;
            position.z = (z + 0.5f) * step - 1f;
            points[i].localPosition = position;
            points[i].transform.SetParent(transform, false);
        }
    }

    private void Update()
    {
        float time = Time.time;
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 positionPoint = point.localPosition;
            positionPoint.y = f(positionPoint.x, positionPoint.z, time);
            point.localPosition = positionPoint;
        }
    }
}
