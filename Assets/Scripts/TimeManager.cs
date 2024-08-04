// using UnityEngine;

// public class TimeManager : MonoBehaviour {

// 	public float slowdownFactor = 0.05f;
// 	public float slowdownLength = 2f;

// 	void Update ()
// 	{
// 		Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
// 		Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
// 	}

// 	public void DoSlowmotion ()
// 	{
// 		Time.timeScale = slowdownFactor;
// 		Time.fixedDeltaTime = Time.timeScale * .02f;
// 	}

// }
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowdownFactor = 0.05f;
    public float slowdownLength = 2f;

    private float originalFixedDeltaTime;

    void Start()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (Time.timeScale < 1f)
        {
            RestoreTimeScale();
        }
    }

    void RestoreTimeScale()
    {
        Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
    }

    public void ActivateSlowmotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = originalFixedDeltaTime * slowdownFactor;
    }
}
