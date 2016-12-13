using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSpeed : MonoBehaviour
{
    public BehaviourTimer timer { get; set; }
    private float clickThreshold = 0.25f;
    private float elapsed;
    public bool speed { get; set; }
    public Sprite spriteSpeedFast, spriteSpeedMedium;

    public static readonly float speedScale = 2.5f;

    void Update()
    {
        if (speed)
        {
            elapsed += Time.deltaTime;
            if (timer.duration != 0)
            {
                timer.elapsed += Time.deltaTime * (speedScale - 1.0f);
            }
        }
    }

    public void EventSpeedDown(BaseEventData evt)
    {
        EventSpeedToggle();
        elapsed = 0;
    }

    public void EventSpeedUp(BaseEventData evt)
    {
        if (elapsed > clickThreshold)
        {
            EventSpeedToggle();
        }
    }

    public void EventSpeedToggle()
    {
        if (!speed)
        {
            speed = true;
            GetComponent<Image>().sprite = spriteSpeedFast;
        }
        else
        {
            speed = false;
            GetComponent<Image>().sprite = spriteSpeedMedium;
        }
    }
}