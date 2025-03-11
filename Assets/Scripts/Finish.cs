using UnityEngine;

public class Finish : MonoBehaviour
{
    private Renderer _renderer;
    private float _offset;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        _offset += Time.deltaTime;
        _renderer.material.mainTextureOffset = new Vector2(_renderer.material.mainTextureOffset.x + Time.deltaTime * 1.5f, _renderer.material.mainTextureOffset.y + Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.Finish();
        }
    }
}
