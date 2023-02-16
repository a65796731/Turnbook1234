using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutTest : MonoBehaviour
{
    public Transform _katana;
    public Transform _slice;
    Material[] _materials;

    [Range(-0.5f, 0.5f)]
    public float Y;
    // Start is called before the first frame update
    void Start()
    {
       
        var meshFilter = _slice.GetComponent<MeshFilter>();
        float centerX = meshFilter.sharedMesh.bounds.center.x;

        _materials = _slice.GetComponent<MeshRenderer>().materials;
        foreach (var material in _materials)
        {
            material.SetFloat("_PointX", centerX);
        }
    }
    float _pointY=float.MaxValue;
    // Update is called once per frame
    void Update()
    {
        var pos = _katana.position;
        pos.y= Y;
        float pointY = _slice.transform.InverseTransformPoint(pos).y;
        if (_pointY > pointY)
        {
            _pointY = pointY;
        }
        foreach (var material in _materials)
        {
            material.SetFloat("_PointY", _pointY);
        }
    }
}
