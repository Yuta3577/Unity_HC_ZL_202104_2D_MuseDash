using UnityEngine;

public class API : MonoBehaviour
{
    // 非靜態 API
    // 與靜態差別
    // 1. 非靜態沒有 static 關鍵字
    // 2. 需要實體物件
    // 3. 實體物件
    // 3-1. (階層面板) 遊戲物件 GameObject 白色線條的盒子
    // 3-2. (屬性面板) 元件 

    // 先新增一個類型為 類別 的欄位 物件名稱
    public GameObject obj1;
    public GameObject obj2;
    public Transform tra1;
    
    private void Start()
    {
        print("隨機：" + Random.value);

        // 1. 取得非靜態屬性
        // 語法：物件名稱.屬性名稱
        print("圖層：" + obj1.layer);
        print("標籤：" + obj1.tag);
        print("座標：" + tra1.position);

        // 2. 設定非靜態屬性
        // 語法：物件名稱.屬性名稱 指定 值
        obj2.layer = 5;
        tra1.position = new Vector3(2, 3, 4);
    }

    private void Update()
    {
        // 3. 使用非靜態方法
        // 語法：物件名稱.方法名稱(對應的參數)
        tra1.Translate(0.1f, 0, 0);
    }
}
