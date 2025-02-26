﻿using UnityEngine;
using UnityEngine.UI;       // 引用 介面 API
using System.Linq;          // 引用 查詢語言 API LinQ
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
    [Header("下方檢查節點名稱")]
    public string nameDown = "粉紅兔";
    [Header("打擊文字")]
    public GameObject objClickText;
    [Header("打擊文字的顏色：Perfect、Great、Miss")]
    public Color cPerfect;
    public Color cGreat;
    public Color cMiss;

    /// <summary>
    /// 紀錄上方進來的節點類型
    /// </summary>
    public AreaType areaTypeUp;
    /// <summary>
    /// 紀錄下方進來的節點類型
    /// </summary>
    public AreaType areaTypeDown;

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
    /// <summary>
    /// 星星：上方
    /// </summary>
    private ParticleSystem psUp;
    /// <summary>
    /// 星星：下方
    /// </summary>
    private ParticleSystem psDown;
    /// <summary>
    /// 畫布
    /// </summary>
    private Transform traCanvas;
    /// <summary>
    /// 文字介面：分數
    /// </summary>
    private Text textScore;
    /// <summary>
    /// 分數
    /// </summary>
    private int score;
    /// <summary>
    /// 用來保存進入檢查區域的音樂節點 - 作為刪除用
    /// </summary>
    private GameObject objPointUp;
    private GameObject objPointDown;
    /// <summary>
    /// 打擊文字上方的位置
    /// </summary>
    private Vector2 v2TextUp = new Vector2(-300, 150);
    /// <summary>
    /// 打擊文字下方的位置
    /// </summary>
    private Vector2 v2TextDown = new Vector2(-300, -250);
    /// <summary>
    /// 介面文字：連擊數量
    /// </summary>
    private Text textCombo;
    /// <summary>
    /// 連擊數量
    /// </summary>
    private int combo;
    /// <summary>
    /// 歌曲進度
    /// </summary>
    private Image imgProgressBar;
    /// <summary>
    /// 音樂長度
    /// </summary>
    private float musicLength;
    /// <summary>
    /// 音樂結束
    /// </summary>
    private bool musicEnd;
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

        Gizmos.color = new Color(0, 0, 1, 0.8f);
        Gizmos.DrawSphere(pointCheckDown.position, rangePerfect);
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(pointCheckDown.position, rangeGreat);
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(pointCheckDown.position, rangeMiss);
    }

    /// <summary>
    /// 結束評分畫面：評分畫面
    /// </summary>
    private CanvasGroup groupEndScore;
    private Text textEndScore;

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
       
        psUp = GameObject.Find("星星：上方").GetComponent<ParticleSystem>();
        psDown = GameObject.Find("星星：下方").GetComponent<ParticleSystem>();
        traCanvas = GameObject.Find("畫布").transform;

        textCombo = GameObject.Find("連擊數量").GetComponent<Text>();
        imgProgressBar = GameObject.Find("歌曲進度").GetComponent<Image>();
        musicLength = musicData.music.length;
        groupEndScore = GameObject.Find("評分畫面").GetComponent<CanvasGroup>();
        #endregion

        textEndScore = GameObject.Find("評分").GetComponent<Text>();
    }

    private void Update()
    {
        // 如果需要將方法執行資料保存在參數內 必須 使用 關鍵字 out
        // out 作用：輸入的參數會保存資料
        CheckPoint(pointCheckUp, nameUp, out objPointUp, out areaTypeUp);
        CheckPoint(pointCheckDown, nameDown, out objPointDown, out areaTypeDown);
        ClickCheck(KeyCode.F, areaTypeUp, objPointUp, v2TextUp);
        ClickCheck(KeyCode.J, areaTypeDown, objPointDown, v2TextDown);
        ProgressBarEffect();
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
    /// 歌曲進度條效果：隨著時間更新
    /// 判定載入時間是否超出音樂時間並處理 MusicEnd 方法
    /// </summary>
    private void ProgressBarEffect()
    {
        // Time.timeSinceLevelLoad 場景載入時間
        imgProgressBar.fillAmount = Time.timeSinceLevelLoad / musicLength;
        // 尚未結束 並且 載入時間 超出 音樂時間 就結束
        if (!musicEnd && Time.timeSinceLevelLoad >= musicLength) MusicEnd();
    }

    /// <summary>
    /// 音樂結束：跳出結束畫面與評分
    /// </summary>
    private void MusicEnd()
    {
        musicEnd = true;
        StartCoroutine(EndAndScore());
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

    /// <summary>
    /// 受傷：碰到音樂節點扣血、更新介面
    /// </summary>
    private void Hit()
    {
        player.hp -= 20;
        textHp.text = player.hp + " / " + maxHp;        // 更新血量文字介面
        imgHp.fillAmount = player.hp / maxHp;           // 更新血條圖片介面
        Combo(0, 0);

        // 判斷式 只有一個分號 可以忽略 大括號
        if (player.hp <= 0) StartCoroutine(GameOver());
    }

    /// <summary>
    /// 有打擊到的節點數量
    /// </summary>
    private int clickPoint;

    /// <summary>
    /// 結束與分數處理
    /// 評分準則
    /// S：節點全部都有打到
    /// A：百分之 90
    /// B：百分之 80
    /// C：百分之 80 以下
    /// </summary>
    private IEnumerator EndAndScore()
    {
        // 先處理分數
        int total = musicData.points.Length;        // 節點總數

        // => 黏巴達 Lambda
        // 空節點數量 = 節點陣列.搜尋資料(資料 => 資料 等於 空節點).轉清單().數量
        int noneCount = musicData.points.Where(x => x == PointType.none).ToList().Count;

        print("總數：" + total + " | 空節點：" + noneCount);

        total -= noneCount;

        string score;

        if (clickPoint / total == 1) score = "S";
        else if ((float)clickPoint / (float)total >= 0.9f) score = "A";
        else if ((float)clickPoint / (float)total >= 0.8f) score = "B";
        else score = "C";

        textEndScore.text = score;

        yield return StartCoroutine(ShowPanel(groupEndScore));
    }

    /// <summary>
    /// 遊戲結束
    /// </summary>
    private IEnumerator GameOver()
    {
        player.Dead();                                  // 玩家動畫
        yield return StartCoroutine(ShowPanel(groupFinal));
    }

    /// <summary>
    /// 顯示群組面板效果
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    private IEnumerator ShowPanel(CanvasGroup group)
    {
        for (int i = 0; i < 40; i++)                    // 40 次數
        {
            group.alpha += 0.03f;                  // 0.03 透明度
            yield return new WaitForSeconds(0.02f);     // 0.02 等待時間
        }

        group.interactable = true;                 // 啟動互動
        group.blocksRaycasts = true;
    }

    // Unity 播放、暫停與逐格播放
    // Ctrl + P、Ctrl + Shift + P、Ctrl + Alt + P
    /// <summary>
    /// 檢查音樂節點進入判定區域
    /// </summary>
    /// <param name="pointCheck">檢查位置</param>
    /// <param name="name">節點名稱</param>
    /// <param name="objPoint">要將結點保存的物件</param>
    /// <param name="areaType">要記錄進入的節點類型</param>
    private void CheckPoint(Transform pointCheck, string name, out GameObject objPoint, out AreaType areaType)
    {
        Collider2D hitMiss = Physics2D.OverlapCircle(pointCheck.position, rangeMiss);
        Collider2D hitGreat = Physics2D.OverlapCircle(pointCheck.position, rangeGreat);
        Collider2D hitPerfect = Physics2D.OverlapCircle(pointCheck.position, rangePerfect);

        // 如果 碰撞物件存在 並且 名稱 包含 上方檢查節點名稱
        if (hitPerfect && hitPerfect.name.Contains(name))
        {
            areaType = AreaType.perfect;
            objPoint = hitPerfect.gameObject;
        }
        else if (hitGreat && hitGreat.name.Contains(name))
        {
            areaType = AreaType.great;
            objPoint = hitGreat.gameObject;
        }
        else if (hitMiss && hitMiss.name.Contains(name))
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
    /// <param name="key">按鍵</param>
    /// <param name="areaType">進入節點的區域類型</param>
    /// <param name="objPoint">要刪除的節點物件</param>
    /// <param name="v2Text">要顯示文字的位置</param>
    private void ClickCheck(KeyCode key, AreaType areaType, GameObject objPoint, Vector2 v2Text)
    {
        if (Input.GetKeyDown(key))
        {
            switch (areaType)
            {
                case AreaType.none:
                    break;
                case AreaType.perfect:
                    AddScore(30);
                    Destroy(objPoint);
                    psUp.Play();
                    StartCoroutine(ShowText("PERFECT", v2Text, cPerfect));
                    Combo();
                    break;
                case AreaType.great:
                    AddScore(15);
                    Destroy(objPoint);
                    psUp.Play();
                    StartCoroutine(ShowText("GREAT", v2Text, cGreat));
                    Combo();
                    break;
                case AreaType.miss:
                    Destroy(objPoint);
                    StartCoroutine(ShowText("MISS", v2Text, cMiss));
                    Combo(0, 0);
                    break;
            }
        }
    }

    /// <summary>
    /// 連擊：次數增加、顯示連擊文字並更新文字內容
    /// </summary>
    /// <param name="add">要添加的連擊次數</param>
    /// <param name="a">連擊文字的透明度</param>
    private void Combo(int add = 1, float a = 1)
    {
        if (add == 0) combo = 0;            // 連擊失敗，次數歸零

        combo += add;
        Color c = textCombo.color;
        c.a = a;
        textCombo.color = c;
        textCombo.text = "COMBO " + combo;
    }

    /// <summary>
    /// 添加分數
    /// 紀錄打擊到的節點數量
    /// </summary>
    /// <param name="add">要增加的分數</param>
    private void AddScore(int add)
    {
        clickPoint++;
        score += add;
        textScore.text = "SCORE：" + score;
    }

    /// <summary>
    /// 顯示打擊文字
    /// </summary>
    /// <param name="showText">打擊文字要顯示的文字</param>
    /// <param name="v2Pos">打擊文字的座標</param>
    /// <param name="color">打擊文字的顏色</param>
    private IEnumerator ShowText(string showText, Vector2 v2Pos, Color color)
    {
        GameObject temp = Instantiate(objClickText, traCanvas);
        Text tempText = temp.GetComponent<Text>();
        tempText.text = showText;
        RectTransform rect = temp.GetComponent<RectTransform>();
        tempText.color = color;                                     // 指定顏色
        rect.anchoredPosition = v2Pos;                              // 指定座標

        #region 上升效果
        float up = 10;                                              // 一次上升多少
        for (int i = 0; i < 10; i++)
        {
            // Vector2.up = new Vector2(0, 1)
            rect.anchoredPosition += Vector2.up * up;               // 打擊文字.座標 累加
            yield return new WaitForSeconds(0.02f);
        }
        #endregion

        #region 淡出
        for (int i = 0; i < 10; i++)
        {
            tempText.color -= new Color(0, 0, 0, 0.1f);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(temp);
        #endregion
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