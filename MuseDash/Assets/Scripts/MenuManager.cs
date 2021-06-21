using UnityEngine;
using UnityEngine.SceneManagement;  // 引用 場景管理 API

public class MenuManager : MonoBehaviour
{
    // 按鈕跟程式溝通的方法
    // 需要一個【公開】的方法
    /// <summary>
    /// 開始遊戲
    /// </summary>
    public void StartGame()
    {
        // 延遲呼叫("方法名稱"，延遲時間)；
        Invoke("DelayStart", 1.3f);
    }

    private void DelayStart()
    {
        // 綠色蚯蚓：過時程式，未來會被刪除，不建議使用
        // Application.LoadLevel("遊戲場景");
        SceneManager.LoadScene("遊戲場景");
    }

    /// <summary>
    /// 離開遊戲
    /// </summary>
    public void QuitGame()
    {
        Invoke("DelayQuit", 1.5f);
    }

    private void DelayQuit()
    {
        // 應用程式.離開遊戲() - 關閉遊戲
        Application.Quit();
    }
}
