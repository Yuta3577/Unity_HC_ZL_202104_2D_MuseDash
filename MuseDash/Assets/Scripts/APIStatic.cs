using UnityEngine;

public class APIStatic : MonoBehaviour
{
    public Vector3 a = new Vector3(1, 1, 1);
    public Vector3 b = new Vector3(22, 22, 22);

    // 開始事件：播放後執行一次
    private void Start()
    {
        #region 認識
        // 使用靜態 API
        // 屬性
        // 可以取得 Get、可以設定 Set
        // 如果看到 Read Only 只能取得

        // 1. 取得靜態的屬性
        // 語法：類別.靜態屬性
        print("隨機：" + Random.value);

        // 2. 設定靜態的屬性
        // 語法：類別.靜態屬性 指定 值；
        Cursor.visible = false;

        // 方法
        // 3. 使用靜態的方法
        // 語法：類別.靜態方法(對應的參數)；
        int r = Random.Range(50, 150);
        print("隨機攻擊力：" + r);
        #endregion

        print("所有攝影機的數量：" + Camera.allCamerasCount);
        print("2D 重力：" + Physics2D.gravity);

        Physics2D.gravity = new Vector2(0, -20);
        print("2D 重力：" + Physics2D.gravity);

        Application.OpenURL("https://unity.com/");
        float number = Mathf.Floor(9.999f);
        print("9.999 去小數點：" + number);

        float dis = Vector3.Distance(a, b);
        print("兩點的距離：" + dis);
    }

    // 更新事件：一秒約執行六十次，60 FPS 一秒有 60 幀
    private void Update()
    {
        print("按下任意鍵：" + Input.anyKeyDown);
        // print("遊戲時間：" + Time.timeSinceLevelLoad);

        bool down = Input.GetKeyDown(KeyCode.Space);
        print("是否按下空白鍵：" + down);
    }
}
