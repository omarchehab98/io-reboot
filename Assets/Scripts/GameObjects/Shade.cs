using System;
using UnityEngine;
using UnityEngine.UI;

public class Shade
{
    public GameObject gameObject { get; set; }

    public Shade(GameManager gameManager, LevelManager levelManager, Transform parent, int offsetDirection)
    {
        float shadeOffset = gameManager.loadedLevel.channelSize * 0.328125f;
        float shadeWidth = gameManager.loadedLevel.channelSize * 0.175f;
        float shadeLength = gameManager.loadedLevel.channelSize * 0.34375f;
        float shadeBaseSize = gameManager.loadedLevel.channelSize * 0.3125f;

        gameObject = new GameObject("Shade", new Type[] {typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(BehaviourShade)});
        gameObject.tag = "Shade";
        BehaviourShade behaviourShade = gameObject.GetComponent<BehaviourShade>();
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(shadeBaseSize, shadeBaseSize);
        gameObject.GetComponent<Image>().color = levelManager.colorShade;

        gameObject.transform.SetParent(parent);
        gameObject.transform.SetAsFirstSibling();
        gameObject.transform.localPosition = Vector3.zero;
        behaviourShade.offsetDirection = offsetDirection;
        behaviourShade.Calibrate(gameManager.loadedLevel.channelSize);
        behaviourShade.Adjust();
    }
}