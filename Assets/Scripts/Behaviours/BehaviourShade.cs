using UnityEngine;

public class BehaviourShade : MonoBehaviour
{
    private RectTransform rectTransform;
    public int direction { get; set; }
    public int offsetDirection { get; set; }

    private float shadeOffset, shadeWidth, shadeLength, shadeBaseSize;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Calibrate(float channelSize)
    {
        shadeOffset = channelSize * 0.328125f;
        shadeWidth = channelSize * 0.175f;
        shadeLength = channelSize * 0.34375f;
        shadeBaseSize = channelSize * 0.3125f;
    }

    public void Adjust()
    {
        if (offsetDirection != -1)
        {
            direction = (offsetDirection + 2) % 4;
            rectTransform.sizeDelta = ShadeDirection(offsetDirection) * shadeWidth;
            rectTransform.sizeDelta += ShadeDirection((offsetDirection + 1) % 4) * shadeLength;
            gameObject.transform.position -= Utility.DirectionToVectorDirection(offsetDirection) * shadeOffset;
        }
        else
        {
            direction = -1;
        }
    }

    private Vector2 ShadeDirection(int direction)
    {
        if (direction == 0)
            return new Vector3(1, 0, 0);
        else if (direction == 1)
            return new Vector3(0, 1, 0);
        else if (direction == 2)
            return new Vector3(1, 0, 0);
        else if (direction == 3)
            return new Vector3(0, 1, 0);
        return Vector2.zero;
    }
}