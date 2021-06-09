using UnityEngine;
using System.Collections;   // 引用 系統.集合 - 微軟提供的 API - 協同程序

public class MusicManager : MonoBehaviour
{
    #region 欄位
    [Header("生成物件：上方")]
    public GameObject objUp;
    [Header("生成物件：下方")]
    public GameObject objDown;
    [Header("生成位置：上方")]
    public Transform pointUp;
    [Header("生成位置：下方")]
    public Transform pointDown;
    [Header("此關卡音樂資料")]
    public MusicData musicData;

    /// <summary>
    /// 音源 - 音源物件
    /// </summary>
    private AudioSource aud;
    #endregion

    private void Start()
    {
        aud = GameObject.Find("音源物件").GetComponent<AudioSource>();   // 尋找並取得音源元件
        aud.clip = musicData.music;                                     // 指定音樂
        aud.Play();                                                     // 播放音樂

        Invoke("StartMusic", musicData.timeWait);       // 等待後開始生成
    }

    /// <summary>
    /// 開始音樂節點生成
    /// </summary>
    private void StartMusic()
    {
        print("開始生成");

        // 呼叫協同程序方法
        StartCoroutine(SpawnPoint());
    }

    /// <summary>
    /// 間隔生成節點
    /// 協同程序方法 - 允許等待
    /// 必須用特定方式呼叫才會執行
    /// IEnumerator 傳回等待時間
    /// </summary>
    private IEnumerator SpawnPoint()
    {
        // for 迴圈
        // 陣列數量：陣列.Length - 陣列在面板上顯示的 Size
        for (int i = 0; i < musicData.points.Length; i++)
        {
            switch (musicData.points[i])
            {
                case PointType.none:
                    break;
                case PointType.up:
                    // 生成(物件，座標，角度)
                    // Quaternion.identity 零角度
                    // 物件 = 生成物件 - 會將生成的物件資訊儲存於物件內
                    GameObject oUp = Instantiate(objUp, pointUp.position, Quaternion.identity);
                    // 物件.添加元件<元件>().欄位 = 值
                    oUp.AddComponent<MusicPoint>().speed = musicData.speed;
                    break;
                case PointType.down:
                    GameObject oDo = Instantiate(objDown, pointDown.position, Quaternion.identity);
                    oDo.AddComponent<MusicPoint>().speed = musicData.speed;
                    break;
                case PointType.both:
                    GameObject oBUp = Instantiate(objUp, pointUp.position, Quaternion.identity);
                    GameObject oBDo = Instantiate(objDown, pointDown.position, Quaternion.identity);
                    oBUp.AddComponent<MusicPoint>().speed = musicData.speed;
                    oBDo.AddComponent<MusicPoint>().speed = musicData.speed;
                    break;
            }

            // Object.Instantiate(objUp);       // 原本寫法
            // Instantiate(objUp);              // 簡寫 - 省略 Object：生成(物件)

            // 等待秒數(秒數)
            yield return new WaitForSeconds(musicData.interval);
        }
    }
}
