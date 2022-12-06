using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = System.Drawing.Color;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    [SerializeField] private int xSize, ySize;

    private Vector3[] _vertices;
    private Mesh _mesh;

    private void Awake()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        WaitForSeconds wait = new WaitForSeconds(.05f);
        _vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                _vertices[i] = new Vector3(x, y);
                yield return wait;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_vertices == null)       
        {
            return;
        }
        
        Gizmos.color = UnityEngine.Color.black;
        foreach (var t in _vertices)
        {
            Gizmos.DrawSphere(t, .1f);
        }
    }
}
