using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // <— 新增

public class NextLevelLoader : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdDuration = 2f;      // 持续按下需要的时间
    private float holdTimer = 0f;

    [Header("UI Progress Bar")]
    public RectTransform progressBar;    // 拖入场景中那个 Image 的 RectTransform
    public float targetScaleX = 1f;      // 按满后 X 轴的目标缩放

    void Start()
    {
        // 初始确保进度条 X 缩放为 0
        if (progressBar != null)
        {
            Vector3 s = progressBar.localScale;
            s.x = 0f;
            progressBar.localScale = s;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.JoystickButton0))
        {
            holdTimer += Time.deltaTime;
            // 计算进度 [0,1]
            float progress = Mathf.Clamp01(holdTimer / holdDuration);

            // 更新进度条缩放
            if (progressBar != null)
            {
                Vector3 s = progressBar.localScale;
                s.x = Mathf.Lerp(0f, targetScaleX, progress);
                progressBar.localScale = s;
            }

            // 达到时间则加载下一关
            if (holdTimer >= holdDuration)
            {
                LoadNextLevel();
                holdTimer = 0f;  // 防止重复触发
            }
        }
        else
        {
            // 松开时重置计时和进度条
            holdTimer = 0f;
            if (progressBar != null)
            {
                Vector3 s = progressBar.localScale;
                s.x = 0f;
                progressBar.localScale = s;
            }
        }
    }

    void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextScene);
        else
            Debug.Log("No more scenes to load!");
    }
}
