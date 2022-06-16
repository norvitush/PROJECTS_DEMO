using UnityEngine;

public class meshScroller : MonoBehaviour
{
    private Material _meshRendererMaterial;
    [SerializeField] private float _speed = 1;
    private float _yOffset = 0;
    private void Start()
    {
        _yOffset = 0;
        _meshRendererMaterial = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {
        _yOffset -= _speed * Time.unscaledDeltaTime;
        _meshRendererMaterial.SetTextureOffset("_BaseMap", new Vector2(0,_yOffset));

    }
}
