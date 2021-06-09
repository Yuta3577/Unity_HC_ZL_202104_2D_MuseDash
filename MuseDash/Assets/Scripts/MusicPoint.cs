using UnityEngine;

public class MusicPoint : MonoBehaviour
{
    [Header("移動速度")]
    public float speed;

    /// <summary>
    /// 移動節點
    /// </summary>
    private void Move()
    {
        transform.Translate(-speed * Time.deltaTime, 0, 0);
    }

    private void Update()
    {
        Move();
    }
}
