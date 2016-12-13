

using System;
using UnityEngine;
using UnityEngine.UI;
public class Ball
{
    public GameObject gameObject { get; set; }

    public Ball(GameManager gameManager, LevelManager levelManager, Level level, Vector2 position)
    {
        gameObject = new GameObject("Ball", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(BehaviourBall) });
        gameObject.tag = "Ball";
        BehaviourBall ballBehaviour = gameObject.GetComponent<BehaviourBall>();
        ballBehaviour.Initialize(gameManager, levelManager, position);

        Image image = gameObject.GetComponent<Image>();
        image.sprite = levelManager.spriteBalls;
        image.color = levelManager.colorBall;

        Transform parent = level.channels[(int)position.y, (int)position.x].transform;
        gameObject.transform.SetParent(parent);
        gameObject.transform.localPosition = new Vector3(0, 0, 0);

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(level.channelSize / 4, level.channelSize / 4);
    }
}