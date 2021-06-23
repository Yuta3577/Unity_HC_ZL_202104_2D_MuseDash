using UnityEngine;

public class Player : MonoBehaviour
{
    #region 欄位
    [Header("跳躍高度"), Range(0, 3000)]
    public int jump = 100;
    [Header("血量"), Range(0, 2000)]
    public float hp = 500;
    [Header("是否在地板上")]
    public bool isGround = false;
    [Header("音效")]
    public AudioClip soundJump;
    public AudioClip soundAttack;
    [Header("檢查地板的半徑"), Range(0.1f, 1f)]
    public float groundRadius = 0.5f;
    [Header("檢查地板的位移")]
    public Vector3 groundOffset;

    private int score;
    private AudioSource aud;
    private Rigidbody2D rig;
    private Animator ani;
    #endregion

    #region 方法
    /// <summary>
    /// 跳躍
    /// </summary>
    private void Jump()
    {
        // 如果 玩家按下跳躍 就 跳躍 並且 在地板上
        // 原本寫法：isGround == true 可以簡寫為 isGround
        if (Input.GetKeyDown(KeyCode.F) && isGround)
        {
            ani.SetTrigger("跳躍觸發");
            // rig.AddForce(new Vector2(0, jump));              // 剛體.添加推力(二維向量)
            transform.position = new Vector3(-6, 2.1f, 0);      // 停留在空中
            rig.velocity = Vector2.zero;                        // 剛體的加速度歸零
            aud.PlayOneShot(soundJump);
        }
        // 否則 按下跳躍 並且 在空中(不在地板上) 跳躍動畫、座標 2.1
        // != 不等於
        // 原本寫法：isGround != true 可以簡寫為 !isGround
        else if (Input.GetKeyDown(KeyCode.F) && !isGround)
        {
            ani.SetTrigger("跳躍觸發");
            transform.position = new Vector3(-6, 2.1f, 0);  // 停留在空中
            rig.velocity = Vector2.zero;                    // 剛體的加速度歸零
            aud.PlayOneShot(soundJump);
        }

        // 碰到的物件 = 2D 物理.覆蓋圓圈(中心點，半徑，圖層)
        // 圖層 LayerMask 寫法：1 << 圖層編號
        Collider2D hit = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, 1 << 8);

        // 如果 碰到的東西存在 並且 碰到的名稱 等於 地板碰撞 就代表在地板上
        // 並且 &&
        // 等於 ==
        // 否則 就取消 在地板上 布林值
        // 否則 else { 程式區塊 }
        if (hit && hit.name == "地板碰撞")
        {
            isGround = true;
        }
        else
        {
            isGround = false;                       // 是否在地板上 = 否
        }
    }

    /// <summary>
    /// 攻擊
    /// </summary>
    private void Attack()
    {
        // 如果 玩家按下攻擊 就 攻擊
        if (Input.GetKeyDown(KeyCode.J))
        {
            ani.SetTrigger("攻擊觸發");

            // 回到地面 -6, -2.39
            transform.position = new Vector3(-6, -2.39f, 0);
            // 音源.播放一次音效(音效，音量)
            aud.PlayOneShot(soundAttack);
        }
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">收到的傷害</param>
    private void Hit(float damage)
    {

    }

    /// <summary>
    /// 死亡
    /// </summary>
    public bool Dead()
    {
        ani.SetBool("死亡開關", hp <= 0);
        return ani.GetBool("死亡開關");
    }

    /// <summary>
    /// 加分
    /// </summary>
    /// <param name="add">要加的分數</param>
    private void AddScore(int add)
    {

    }
    #endregion

    #region 事件
    private void Start()
    {
        // 動畫元件 = 取得元件<泛型>()；
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        aud = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Dead()) return;

        Jump();
        Attack();
    }

    // 繪製圖示：輔助用
    private void OnDrawGizmos()
    {
        // 1. 決定顏色
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        // 2. 繪製圖形
        // transfrom 代表此腳本的物件變形元件
        Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);
    }
    #endregion
}
