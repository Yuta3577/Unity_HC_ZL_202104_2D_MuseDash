using UnityEngine;

public class MusicManager : MonoBehaviour
{
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

        // Object.Instantiate(objUp);   // 原本寫法
        Instantiate(objUp);             // 簡寫 - 省略 Object：生成(物件)
    }
}
