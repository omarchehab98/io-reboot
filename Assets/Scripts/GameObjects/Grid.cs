using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Grid {
    public GameObject gameObject;

    public Grid(Transform parent, Sprite sprite, Color color, Vector2 size)
    {
        gameObject = new GameObject("Grid", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(BehaviourGrid) });

        gameObject.transform.SetParent(parent);
		gameObject.GetComponent<Image>().color = color;
		gameObject.GetComponent<Image>().sprite = sprite;
		gameObject.GetComponent<Image>().raycastTarget = false;
        gameObject.GetComponent<RectTransform>().localPosition = Vector2.zero;
        gameObject.GetComponent<RectTransform>().sizeDelta = size;
    }
}
