using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    [Header("背景流動的速度"), Range(0, 500)]
    public float speed;

    // public Transform tra; 

    // 更新事件：一秒大約六十次 60FPS
    private void Update()
    {
        // 使用小寫開頭的 transform 代表此物件的 Transform 元件
        // Time.deltaTime 一個影格的時間
        transform.Translate(-speed * Time.deltaTime, 0, 0);
    }
}
