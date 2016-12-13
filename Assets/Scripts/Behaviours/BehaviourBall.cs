using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourBall : MonoBehaviour
{
    public static readonly int stateUndecided = 0;
    public static readonly int stateMovingToChannel = 1;
    public static readonly int stateMovingToOrigin = 2;
    public static readonly int stateDead = 3;

    public GameManager gameManager { get; set; }
    public LevelManager levelManager { get; set; }

    public int state { get; set; }
    public Vector2 position { get; set; }

    private BehaviourChannel currentChannel;
    private int direction;

    private Vector3 moveScale;
    private float ballSpeed = 1f;
    private float elapsed, sum;
    private readonly float thinkInterval = 0.01f;

    public bool frozen { get; set; }
    public float effectSpeedElapsed { get; set; }
    public ButtonSpeed btnSpeed { get; set; }

    public void Initialize(GameManager gameManager, LevelManager levelManager, Vector2 position)
    {
        this.gameManager = gameManager;
        this.levelManager = levelManager;
        this.position = position;
    }

    public void Start()
    {
        frozen = true;
        currentChannel = GetCurrentChannel();
        moveScale = new Vector3(gameManager.loadedLevel.channelSize * thinkInterval * ballSpeed, gameManager.loadedLevel.channelSize * thinkInterval * ballSpeed, 0);
   
        elapsed = 0;
        sum = 0;
    }

    public void Update()
    {
        if (!frozen)
        {
            if (effectSpeedElapsed > 0)
            {
                effectSpeedElapsed -= Time.deltaTime;
            }
            elapsed += Time.deltaTime;
            if (elapsed >= thinkInterval)
            {
                elapsed -= thinkInterval;
                if (state == stateUndecided && !transform.parent.GetComponent<BehaviourChannel>().isRotating)
                {
                    EventDecide();
                }
                else if (state == stateMovingToChannel || state == stateMovingToOrigin)
                {
                    EventMove(state == stateMovingToOrigin);
                }
            }
        }
    }

    public void EventDecide()
    {
        Constraints();
        int channelID = currentChannel.channelID;
		int channelOrientation = currentChannel.GetOrientation();
        EventDropShade(transform.localPosition, -1);

        bool[] paths = Utility.InnateChannels(channelID, channelOrientation);
        List<Vector2> potentialPaths = new List<Vector2>();
        List<int> brightestPaths = new List<int>();
        int min = int.MaxValue;
        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i])
            {
                int shadeCount = ShadeCount(i);
                potentialPaths.Add(new Vector2(i, shadeCount));
                if (shadeCount < min)
                {
                    min = shadeCount;
                }
            }
        }
        for (int i = 0; i < potentialPaths.Count; i++)
        {
            if (potentialPaths[i].y == min)
            {
                brightestPaths.Add((int)potentialPaths[i].x);
            }
        }
        if (brightestPaths.Count > 0)
        {
            direction = brightestPaths[UnityEngine.Random.Range(0, brightestPaths.Count)];
            state = stateMovingToChannel;
        }
        else
        {
            EventReachedOrigin();
        }
    }

    private void EventMove(bool toOrigin)
    {
        try
        {
            if (transform.GetComponentInParent<BehaviourChannel>().isDead)
            {
                Debug.LogError("Cause:\nChannel is dead");
                EventDeath();
                return;
            }
            if (!transform.GetComponentInParent<BehaviourChannel>().isRotating)
            {
                float movement = moveScale.x;
                if (effectSpeedElapsed > 0)
                {
                    movement *= BehaviourSpeed.speedScale;
                }
                if (btnSpeed.speed)
                {
                    movement *= ButtonSpeed.speedScale;
                }
                sum += movement;
                if (sum >= gameManager.loadedLevel.channelSize / 2)
                {
                    sum = 0;
                    if (!toOrigin)
                    {
                        EventEnterChannel();
                        state = stateMovingToOrigin;
                    }
                    else
                    {
                        EventReachedOrigin();
                    }
                }
                Vector3 delta = Vector3.zero;
                if (direction == 0) delta = new Vector3(0, 1, 0);
                else if (direction == 1) delta = new Vector3(1, 0, 0);
                else if (direction == 2) delta = new Vector3(0, -1, 0);
                else if (direction == 3) delta = new Vector3(-1, 0, 0);
                delta.Scale(moveScale);
                if (effectSpeedElapsed > 0)
                {
                    delta.Scale(new Vector3(BehaviourSpeed.speedScale, BehaviourSpeed.speedScale, BehaviourSpeed.speedScale));
                }
                if (btnSpeed.speed)
                {
                    delta.Scale(new Vector3(ButtonSpeed.speedScale, ButtonSpeed.speedScale, ButtonSpeed.speedScale));
                }
                transform.position += delta;
                foreach (GameObject ball in gameManager.loadedLevel.balls)
                {
                    if (ball != this.gameObject)
                    {
                        float collisionRadius = GetComponent<RectTransform>().sizeDelta.x * 0.75f;
                        if (Vector3.Distance(transform.position, ball.transform.position) <= collisionRadius)
                        {
                            Debug.LogError("Cause:\nCollision");
                            EventDeath();
                        }
                    }
                }
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("Dead: NullReferenceException: " + e.StackTrace);
            EventDeath();
            return;
        }
    }

    private void EventEnterChannel()
    {
        EventDropShade(transform.localPosition, Utility.InverseDirection(direction));
        transform.SetParent(transform.parent.parent);
        transform.SetAsLastSibling();
        Vector3 worldPosition = GetComponent<RectTransform>().localPosition;
        position = Utility.WorldToArrayPosition(gameManager.loadedLevel, worldPosition, direction);
        if (position.x < 0 || position.x >= gameManager.loadedLevel.size.x || position.y < 0 || position.y >= gameManager.loadedLevel.size.y || currentChannel.GetComponent<BehaviourChannel>().isRotating)
        {
            Debug.LogError("Cause:\nOut of map bounds");
            EventDeath();
            return;
        }
        else
        {
			currentChannel = gameManager.loadedLevel.channels[(int)position.y, (int)position.x].GetComponent<BehaviourChannel>();
            currentChannel.GetComponent<BehaviourChannel>().EventPlacedShade(this);
            int channelID = currentChannel.GetComponent<BehaviourChannel>().channelID;
            int channelOrientation = currentChannel.GetComponent<BehaviourChannel>().GetOrientation();
            bool[] paths = Utility.InnateChannels(channelID, channelOrientation);
            if (!paths[Utility.InverseDirection(direction)])
            {
                Debug.LogError("Cause:\nNo receipient channel");
                EventDeath();
                return;
			}
			transform.SetParent(currentChannel.transform);
			transform.parent.transform.SetAsLastSibling();
            currentChannel.EventEnteredChannel(this);
            EventDropShade(transform.localPosition, direction);
        }

    }

    private void EventReachedOrigin()
    {
        state = stateUndecided;
        direction = -1;
        currentChannel.EventReachedOrigin(this);
    }

    private void EventDeath()
    {
        gameManager.EventLevelComplete(false);
    }

    public void EventChannelRotate(int amount)
    {
        direction = (direction + amount) % 4;
        if (direction < 0)
            direction += 4;
    }

    public void EventTeleportTo(Transform channel)
    {
        transform.SetParent(channel);
        transform.localPosition = Vector3.zero;
        position = channel.GetComponent<BehaviourChannel>().position;
        EventDropShade(transform.localPosition, -1);
    }

    private void EventDropShade(Vector3 localPosition, int offsetDirection)
    {
        if (state == stateDead)
        {
            return;
        }
        Transform channel = currentChannel.transform;
        new Shade(gameManager, levelManager, channel, offsetDirection);

        if (channel.tag != "ShadedChannel")
        {
            int minUniqueShadeCount = GetChannelMinUniqueShadeCount(channel.GetComponent<BehaviourChannel>().channelID);
            List<int> directions = new List<int>();
            for (int i = 0; i < channel.childCount; i++)
            {
                if (channel.GetChild(i).tag != "Ball" && channel.GetChild(i).tag != "Crack")
                {
                    if (!directions.Contains(channel.GetChild(i).GetComponent<BehaviourShade>().direction))
                    {
                        directions.Add(channel.GetChild(i).GetComponent<BehaviourShade>().direction);
                    }
                }
            }
            if (directions.Count == minUniqueShadeCount)
            {
                channel.tag = "ShadedChannel";
                gameManager.EventShadedChannel();
            }
        }
    }

    private int ShadeCount(int direction)
    {
        Transform channel = currentChannel.transform;
        int count = 0;
        for (int i = 0; i < channel.childCount; i++)
        {
            Transform shade = channel.GetChild(i);
            if (channel.GetChild(i).tag == "Shade")
            {
                int shadeDirection = shade.GetComponent<BehaviourShade>().direction;

                if (direction == shadeDirection)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void Constraints()
    {
        transform.localPosition = Vector3.zero;
    }

    private int GetChannelMinUniqueShadeCount(int channelID)
    {
        if (channelID == 0)
        {
            return 3;
        }
        else if (channelID == 1)
        {
            return 3;
        }
        else if (channelID == 2)
        {
            return 4;
        }
        else if (channelID == 3)
        {
            return 5;
        }
        else if (channelID == 4)
        {
            return 2;
        }
        return -1;
    }

    private BehaviourChannel GetCurrentChannel()
    {
        BehaviourChannel channel = gameManager.loadedLevel.channels[Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.x)].GetComponent<BehaviourChannel>();
        return channel;
    }
}