using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tool
{
    public GameObject gameObject;

    public Tool(Transform parent, Sprite sprite, Color color, Vector2 position, Vector2 size)
    {
        gameObject = new GameObject("Tool", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(EventTrigger) });
        
		gameObject.transform.SetParent(parent);
        gameObject.GetComponent<Image>().color = color;
        gameObject.GetComponent<Image>().sprite = sprite;
        gameObject.GetComponent<RectTransform>().sizeDelta = size;
        gameObject.GetComponent<RectTransform>().localPosition = position;
    }
}
