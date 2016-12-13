using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Timer {
    public GameObject gameObject { get; set; }

    public Timer(Transform parent, Color color, float height)
    {
        gameObject = new GameObject("Timer", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(BehaviourTimer) });

        gameObject.transform.SetParent(parent);

        Image image = gameObject.GetComponent<Image>();
        image.color = color;

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0.9f);
        rectTransform.anchorMax = new Vector2(1, 0.95f);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
    }
}
