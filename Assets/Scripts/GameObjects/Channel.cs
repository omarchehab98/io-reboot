using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Channel
{
    public GameObject gameObject { get; set; }
	protected LevelManager levelManager;
	public Channel(GameManager gameManager, LevelManager levelManager, EditorManager editorManager, Transform levelParent, int channelID, int channelOrientation, int channelType, float channelSize, Vector2 position, Vector2 size)
	{
		this.levelManager = levelManager;

        gameObject = new GameObject("ChannelID " + channelID + " (" + position.x + ", " + position.y + ")", new Type[]{typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(EventTrigger)});

        switch (channelType)
        {
            case BehaviourBasic.id:
                gameObject.AddComponent<BehaviourBasic>();
                break;
            case BehaviourStatic.id:
                gameObject.AddComponent<BehaviourStatic>();
                break;
            case BehaviourSpeed.id:
                gameObject.AddComponent<BehaviourSpeed>();
                break;
            case BehaviourTeleport.id:
                gameObject.AddComponent<BehaviourTeleport>();
                break;
            case BehaviourShuffle.id:
                gameObject.AddComponent<BehaviourShuffle>();
                break;
            case BehaviourWeak.id:
                gameObject.AddComponent<BehaviourWeak>();
                break;
        }

        gameObject.GetComponent<BehaviourChannel>().Initialize(gameManager, levelManager, editorManager, channelID, channelType, position, channelOrientation);

        Image image = gameObject.GetComponent<Image>();
        image.sprite = levelManager.spriteChannels[channelID];
        image.color = levelManager.colorChannels[channelType];

        Transform transform = gameObject.transform;
        transform.SetParent(levelParent);
        transform.localPosition = new Vector3(channelSize * (position.x - ((size.x - 1) / 2)), channelSize * (position.y - ((size.y - 1) / 2)), 0);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, channelOrientation * 90));

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(channelSize, channelSize);

        Button button = gameObject.GetComponent<Button>();
        button.transition = Button.Transition.None;
        button.onClick.AddListener(() => gameObject.GetComponent<BehaviourChannel>().ActionClick());

        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
        
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener(new UnityAction<BaseEventData>(gameObject.GetComponent<BehaviourChannel>().ActionDown));
        eventTrigger.triggers.Add(entryPointerDown);

        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener(new UnityAction<BaseEventData>(gameObject.GetComponent<BehaviourChannel>().ActionDrag));
        eventTrigger.triggers.Add(entryDrag);
        
        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener(new UnityAction<BaseEventData>(gameObject.GetComponent<BehaviourChannel>().ActionUp));
        eventTrigger.triggers.Add(entryPointerUp);

	}
}