using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class BehaviourChannel : MonoBehaviour
{
    protected GameManager gameManager;
    protected EditorManager editorManager;
    protected LevelManager levelManager;
    private RectTransform rectTransform;
	public int channelID { get; set; }
	public int channelType { get; set; }
    public Vector2 position { get; set; }
    private int unscrambledOrientation;

    public bool isDead { get; set; }
    private int balls;
    public bool frozen { get; set; }
    public bool isRotating { get; set; }
    private bool dragged;

    private Vector3 mousePosition;
    private int rotateSpeed = 9;
    private int dragRotationThreshold = 75;

    public bool editorMode { get; set; }
    public void setEditorMode(bool editorMode) {
        this.editorMode = editorMode;
    }

    public int[] GetData() {
        return new int[]{ channelID, GetOrientation(), channelType };
    }

    public void Initialize(GameManager gameManager, LevelManager levelManager, EditorManager editorManager, int channelID, int channelType, Vector2 position, int unscrambledOrientation)
    {
        this.gameManager = gameManager;
        this.levelManager = levelManager;
        this.editorManager = editorManager;
		this.channelID = channelID;
		this.channelType = channelType;
        this.position = position;
        this.unscrambledOrientation = unscrambledOrientation;
        this.setEditorMode(false);
        this.rectTransform = this.GetComponent<RectTransform>();
    }

    public virtual void Start()
    {
        this.isDead = false;
        this.isRotating = false;
        this.balls = 0;
    }

    public virtual void ActionClick()
    {
        if (!frozen)
        {
            if (!dragged)
            {
                EventRotateChannel(false, true);
            }
            else
            {
                dragged = false;
            }
        }
    }

    public virtual void ActionDown(BaseEventData baseEvent)
    {
        if (!frozen)
        {
            dragged = false;
            mousePosition = Input.mousePosition;
        }
    }

    public virtual void ActionDrag(BaseEventData baseEvent)
    {
        if (!frozen)
        {
            if(!editorMode)
            {
                float angle = AngleBetweenPoints(transform.position, Input.mousePosition);
                if (angle > dragRotationThreshold || angle < -dragRotationThreshold)
                {
                    EventRotateChannel(angle < -dragRotationThreshold, true);
                    dragged = true;
                }
                if (Math.Abs(angle) > 90)
                {
                    mousePosition = Input.mousePosition;
                }
            }
            else
            {
                if (!dragged && Vector3.Distance(Input.mousePosition, mousePosition) > rectTransform.rect.width / 2) {
                    dragged = true;
                    editorManager.ActionToolsBasicDown(channelID);
                    editorManager.selectedTool.transform.rotation = this.transform.rotation;
                    this.setInvisbile(true);
                }
                if (dragged) {
                    editorManager.ActionToolsBasicDrag(channelID);
                }
            }
        }
    }

    public virtual void ActionUp(BaseEventData baseEvent)
    {
        if (!frozen)
        {
            if (!editorMode)
            {

            }
            else
            {
                if (dragged)
                {
                    int x = (int) position.x, y = (int) position.y;
                    editorManager.level.channels[y, x] = new Channel(gameManager, levelManager, editorManager, editorManager.level.group.transform, 5, 0, 0, editorManager.level.channelSize, new Vector2(x, y), editorManager.level.size).gameObject;
                    editorManager.level.channels[y, x].GetComponent<Button>().onClick.AddListener(() => editorManager.ActionFocusLevel());
                    this.GetComponentInChildren<BehaviourGrid>().transform.SetParent(editorManager.level.channels[y, x].transform);
                    Destroy(this.gameObject);
                    editorManager.ActionToolsBasicUp(channelID, false);
                }
            }
        }
    }

    public virtual void EventEnteredChannel(BehaviourBall ball)
    {
        balls++;
    }

    public virtual void EventReachedOrigin(BehaviourBall ball)
    {
        balls--;
    }

    public virtual void EventRotateChannel(bool clockwise, bool checkForUnscrambled)
    {
        // IF IS EMPTY TILE
        if (this.channelID == 5) {
            return;
        }
        int direction = 1;
        if (clockwise)
        {
            direction = -1;
        }
        if (!isRotating)
        {
            StartCoroutine(AnimationRotate(direction, checkForUnscrambled));
        }
        AdjustShadeDirections(clockwise);
    }

    public virtual void EventPlacedShade(BehaviourBall ball)
    {

    }

    private IEnumerator AnimationRotate(int amount, bool checkForUnscrambled)
    {
        isRotating = true;
        BehaviourBall[] balls = transform.GetComponentsInChildren<BehaviourBall>();
        foreach (BehaviourBall ball in balls)
        {
            if (ball.gameObject.activeInHierarchy)
            {
                ball.EventChannelRotate(-1);
            }
        }
        for (int i = 0; i < 90; i += rotateSpeed)
        {
            int speed = rotateSpeed;
            while (i + speed > 90)
            {
                speed--;
            }
            speed *= amount;
            transform.rotation *= Quaternion.Euler(0, 0, speed);
            yield return new WaitForSeconds(0.01f);
        }
        if (gameManager.loadedLevel.index >= 0) {
            if (checkForUnscrambled)
            {
                gameManager.EventCheckForUnscrambled();
            }
        } else {
            editorManager.EventPlayCheck();
        }
        isRotating = false;
    }

    private void AdjustShadeDirections(bool inverted)
    {
        int change = 3;
        if (inverted)
        {
            change = 1;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.tag == "Shade")
            {
                BehaviourShade shade = child.GetComponent<BehaviourShade>();
                if (shade.direction != -1)
                {
                    shade.direction = (shade.direction + change) % 4;
                }
            }
        }
    }

    private float AngleBetweenPoints(Vector2 position1, Vector2 position2)
    {
        Vector2 fromLine = position2 - position1;
        Vector2 toLine = mousePosition - transform.position;

        float angle = Vector2.Angle(fromLine, toLine);

        Vector3 cross = Vector3.Cross(fromLine, toLine);

        if (cross.z > 0)
        {
            return -angle;
        }

        return angle;
    }

    public int GetOrientation()
    {
		return Utility.GetChannelOrientation(gameObject);
    }

    public bool IsUnscrambled()
    {
        if (GetOrientation() == unscrambledOrientation)
        {
            return true;
        }
        return false;
    }

    protected void setInvisbile(bool invisbile)
    {
        Color color =  this.GetComponent<Image>().color;
        color.a = (invisbile) ? 0 : 1;
        this.GetComponent<Image>().color = color;
    }
}