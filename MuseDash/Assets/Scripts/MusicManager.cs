using UnityEngine;
using UnityEngine.UI;       // 引用 介面 API
using System.Collections;   // 引用 系統.集合 - 微軟提供的 API - 協同程序
using TMPro;

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
    [Header("判定位置：上方")]
    public Transform pointCheckUp;
    [Header("判定位置：下方")]
    public Transform pointCheckDown;
    [Header("判定距離：Perfect Great Miss")]
    public float rangePerfect = 0.5f;
    public float rangeGreat = 0.8f;
    public float rangeMiss = 1.1f;
    [Header("上方檢查節點名稱")]
    public string nameUp = "藍鳥";

    public AreaType areaType;

    /// <summary>
    /// 音源 - 音源物件
    /// </summary>
    private AudioSource aud;
    /// <summary>
    /// 介面：血條
    /// </summary>
    private Image imgHp;
    /// <summary>
    /// 介面：血量
    /// </summary>
    private Text textHp;
    /// <summary>
    /// 玩家資訊
    /// </summary>
    private Player player;
    /// <summary>
    /// 玩家血量最大值
    /// </summary>
    private float maxHp;
    /// <summary>
    /// 結束畫面
    /// </summary>
    private CanvasGroup groupFinal;
    #endregion

    #region 事件
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.8f);
        Gizmos.DrawSphere(pointCheckUp.position, rangePerfect);
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(pointCheckUp.position, rangeGreat);
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(pointCheckUp.position, rangeMiss);
    }

    /// <summary>
    /// 星星：上方
    /// </summary>
    private ParticleSystem psUp;
    /// <summary>
    /// 星星：下方
    /// </summary>
    private ParticleSystem psDown;

    private void Start()
    {
        #region 取得物件
        aud = GameObject.Find("音源物件").GetComponent<AudioSource>();   // 尋找並取得音源元件
        aud.clip = musicData.music;                                     // 指定音樂
        aud.Play();                                                     // 播放音樂

        imgHp = GameObject.Find("血條").GetComponent<Image>();
        textHp = GameObject.Find("血量").GetComponent<Text>();

        player = GameObject.Find("主角").GetComponent<Player>();
        maxHp = player.hp;                                                  // 取得一開始的血量 - 最大值
        textHp.text = player.hp + " / " + maxHp;                            // 遊戲開始時更新介面

        groupFinal = GameObject.Find("結束畫面").GetComponent<CanvasGroup>();

        Invoke("StartMusic", musicData.timeWait);                           // 等待後開始生成

        textScore = GameObject.Find("分數").GetComponent<Text>();
        #endregion
        psUp = GameObject.Find("星星：上方").GetComponent<ParticleSystem>();
        psDown = GameObject.Find("星星：下方").GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        CheckPoint();
        ClickCheck();
    }

    // 觸發事件：collision 觸發到的物件
    // collision 指的是 碰到 子物件 - 扣血區域 的其他碰撞物件
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit();
    }
    #endregion

    #region 方法
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

    /// <summary>
    /// 受傷：碰到音樂節點扣血、更新介面
    /// </summary>
    private void Hit()
    {
        player.hp -= 20;
        textHp.text = player.hp + " / " + maxHp;        // 更新血量文字介面
        imgHp.fillAmount = player.hp / maxHp;           // 更新血條圖片介面

        // 判斷式 只有一個分號 可以忽略 大括號
        if (player.hp <= 0) StartCoroutine(GameOver());
    }

    /// <summary>
    /// 遊戲結束
    /// </summary>
    private IEnumerator GameOver()
    {
        player.Dead();                                  // 玩家動畫

        for (int i = 0; i < 40; i++)                    // 40 次數
        {
            groupFinal.alpha += 0.03f;                  // 0.03 透明度
            yield return new WaitForSeconds(0.02f);     // 0.02 等待時間
        }

        groupFinal.interactable = true;                 // 啟動互動
        groupFinal.blocksRaycasts = true;
    }

    /// <summary>
    /// 用來保存進入檢查區域的音樂節點 - 作為刪除用
    /// </summary>
    private GameObject objPoint;

    // Unity 播放、暫停與逐格播放
    // Ctrl + P、Ctrl + Shift + P、Ctrl + Alt + P
    /// <summary>
    /// 檢查音樂節點進入判定區域
    /// </summary>
    private void CheckPoint()
    {
        Collider2D hitMiss = Physics2D.OverlapCircle(pointCheckUp.position, rangeMiss);
        Collider2D hitGreat = Physics2D.OverlapCircle(pointCheckUp.position, rangeGreat);
        Collider2D hitPerfect = Physics2D.OverlapCircle(pointCheckUp.position, rangePerfect);

        // 如果 碰撞物件存在 並且 名稱 包含 上方檢查節點名稱
        if (hitPerfect && hitPerfect.name.Contains(nameUp))
        {
            areaType = AreaType.perfect;
            objPoint = hitPerfect.gameObject;
        }
        else if (hitGreat && hitGreat.name.Contains(nameUp))
        {
            areaType = AreaType.great;
            objPoint = hitGreat.gameObject;
        }
        else if (hitMiss && hitMiss.name.Contains(nameUp))
        {
            areaType = AreaType.miss;
            objPoint = hitMiss.gameObject;
        }
        else
        {
            areaType = AreaType.none;
            objPoint = null;                // null 空值
        }
    }

    /// <summary>
    /// 按鍵檢查 - 跳躍與攻擊 檢查 節點的得分
    /// </summary>
    private void ClickCheck()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            switch (areaType)
            {
                case AreaType.none:
                    break;
                case AreaType.perfect:
                    AddScore(30);
                    Destroy(objPoint);
                    psUp.Play();
                    break;
                case AreaType.great:
                    AddScore(15);
                    Destroy(objPoint);
                    psUp.Play();
                    break;
                case AreaType.miss:
                    Destroy(objPoint);
                    break;
            }
        }
    }

    /// <summary>
    /// 文字介面：分數
    /// </summary>
    private Text textScore;
    /// <summary>
    /// 分數
    /// </summary>
    private int score;

    /// <summary>
    /// 添加分數
    /// </summary>
    /// <param name="add">要增加的分數</param>
    private void AddScore(int add)
    {
        score += add;
        textScore.text = "SCORE：" + score;
    }

    [Header("打擊文字")]
    public GameObject objClickText;

    /// <summary>
    /// 顯示打擊文字
    /// </summary>
    /// <param name="showText">打擊文字要顯示的文字</param>
    /// <returns></returns>
    private IEnumerator ShowText(string showText)
    {
        GameObject temp = Instantiate(objClickText);
        Text tempText = temp.GetComponent<Text>();
        tempText.text = showText;
        yield return new WaitForSeconds(0.1f);
    }
    #endregion
}

/// <summary>
/// 節點進入區域的類型：無、Perfect、Great、Miss
/// </summary>
public enum AreaType
{
    none, perfect, great, miss
}