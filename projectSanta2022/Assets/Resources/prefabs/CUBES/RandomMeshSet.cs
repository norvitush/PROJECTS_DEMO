using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMeshSet : MonoBehaviour
{
    public Mesh[] mesh_arr;


    private void OnEnable()
    {
        Mesh randomMesh = mesh_arr[Random.Range(0, mesh_arr.Length)];
        this.GetComponent<MeshFilter>().mesh = randomMesh;
    }

}
