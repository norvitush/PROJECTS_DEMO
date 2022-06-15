using UnityEngine;
using VOrb;

public class TextUp : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1.7f;

    public float LifeTime { get => _lifeTime; set => _lifeTime = value; }

    void Start()
    {
         
    }
    private void OnEnable()
    {        
        Invoke(nameof(PoolBack), LifeTime);
    }
    private void PoolBack()
    {       
        gameObject.SetActive(false);
        GameService.Instance.CoinsTextMesh.gameObject.GetComponent<Animator>().Play("SCORE_GAIN");
    }
    
}
