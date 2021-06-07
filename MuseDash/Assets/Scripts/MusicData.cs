using UnityEngine;

// ScriptableObject 腳本化物件
// 將資料製作成物件並保存在 Assets 內

// CreateAssetMenu 腳本化物件類別的屬性
// 設定選單的名稱以及生成物件的名稱
[CreateAssetMenu(menuName = "KID/音樂資料", fileName = "音樂遊戲資料檔")]
public class MusicData : ScriptableObject
{
    [Header("音樂")]
    public AudioClip music;
    [Header("音樂前面等待時間"), Range(0, 10)]
    public float timeWait = 2f;
    [Header("節點間隔時間"), Range(0, 5)]
    public float interval = 1f;
    [Header("節點移動速度"), Range(0, 1000)]
    public float speed = 300;

    // 陣列：儲存多筆相同類型的資料 - Array
    // 語法：類型[]
    // 陣列會有 Size 代表陣列資料的數量
    [Header("音樂節點")]
    public PointType[] points;
}

// 列舉 enum：下拉式選單 - 只能選取一個
/// <summary>
/// 音樂節點的類型：無節點、上方節點、下方節點、上下節點
/// </summary>
public enum PointType
{
    none, up, down, both
}