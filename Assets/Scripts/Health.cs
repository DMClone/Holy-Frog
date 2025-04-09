using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] protected int _maxHP;
    protected int _HP;

    protected virtual void Awake()
    {
        _HP = _maxHP;
    }

    public void OnReset()
    {
        _HP = _maxHP;
        transform.localPosition = Vector3.zero;
    }
}
