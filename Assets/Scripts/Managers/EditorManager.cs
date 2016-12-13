using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class EditorManager : MonoBehaviour
{
	public Level level { get; set; }
    public Transform canvas;
    public Color colorGrid, colorTools, colorPanelTools, colorSelection, colorDelete, colorHighlighted, colorPlay, colorPlayDisabled, colorInstructions, colorMenu, colorPressed;
    public Sprite sprGrid, sprTools, sprDelete, sprPlay, sprMenu;
    private Text txtInstructions;
    private InputField txtDebug;
    public Font fontRighteous;

	private GameManager gameManager;
	private LevelManager levelManager;
	private FlowManager flowManager;
    public void Awake()
	{
		this.gameManager = GetComponent<GameManager>();
		this.levelManager = GetComponent<LevelManager>();
		this.flowManager = GetComponent<FlowManager>();
    }

    private GameObject[] toolChannelIDs, toolChannelTypes;
	private RectTransform group, pnlTools, toolsTypes;
	private GameObject btnPlay;
	private GameObject btnMenu;
    public Level EventLoadEmptyLevel(Vector2 size)
    {
        this.level = new Level(canvas, -1, size);

        GameObject instructions =  new GameObject("txtInstructions", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Text) });
        instructions.transform.SetParent(this.level.group.transform);
        instructions.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.9f);
        instructions.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.95f);
        instructions.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        instructions.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        txtInstructions = instructions.GetComponent<Text>();
        txtInstructions.font = fontRighteous;
        txtInstructions.alignment = TextAnchor.MiddleCenter;
        txtInstructions.resizeTextForBestFit = true;
        txtInstructions.resizeTextMaxSize = 200;
        txtInstructions.color = colorInstructions;
        txtInstructions.text = "";

        GameObject gODebug =  Instantiate(Resources.Load("Debug", typeof(GameObject))) as GameObject;
        gODebug.transform.SetParent(this.level.group.transform);
        gODebug.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        gODebug.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        txtDebug = gODebug.GetComponent<InputField>();
        txtDebug.readOnly = false;
        // txtDebug.onValueChange.AddListener(() => ActionChangeLevel()); // Todo : finish loading level

        this.group = level.group.GetComponent<RectTransform>();
		this.group.gameObject.AddComponent<Image>();
		Button groupButton = this.group.gameObject.AddComponent<Button>();
		groupButton.onClick.AddListener(() => ActionFocusLevel());
		groupButton.transition = Selectable.Transition.None;
        for (int y = 0; y < level.size.y; y++)
        {
			for (int x = 0; x < level.size.x; x++)
            {
				level.channels[y, x] = new Channel(GetComponent<GameManager>(), GetComponent<LevelManager>(), this, level.group.transform, 5, 0, 0, level.channelSize, new Vector2(x, y), size).gameObject;
				level.channels[y, x].GetComponent<Button>().onClick.AddListener(() => ActionFocusLevel());
				new Grid(level.channels[y, x].transform, sprGrid, colorGrid, new Vector2(level.channelSize, level.channelSize));
            }
        }
        
        GameObject btnTools = new GameObject("ButtonTools", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button) });
        btnTools.transform.SetParent(level.group.transform);
        btnTools.GetComponent<Image>().color = colorTools;
        btnTools.GetComponent<Image>().sprite = sprTools;
        btnTools.GetComponent<Image>().preserveAspect = true;
        btnTools.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        btnTools.GetComponent<RectTransform>().anchorMax = new Vector2(0.2f, 0.13f);
        btnTools.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        btnTools.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        btnTools.GetComponent<Button>().onClick.AddListener(() => ActionTools());
		
		btnPlay = new GameObject("ButtonPlay", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button) });
		btnPlay.transform.SetParent(level.group.transform);
		btnPlay.GetComponent<Image>().color = colorPlay;
		btnPlay.GetComponent<Image>().sprite = sprPlay;
        btnPlay.GetComponent<Image>().preserveAspect = true;
		btnPlay.GetComponent<RectTransform>().anchorMin = new Vector2(0.8f, 0);
		btnPlay.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.13f);
		btnPlay.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
		btnPlay.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
		ColorBlock colorsPlay = btnPlay.GetComponent<Button>().colors;
		colorsPlay.disabledColor = colorPlayDisabled;
		colorsPlay.pressedColor = colorPressed;
		btnPlay.GetComponent<Button>().colors = colorsPlay;
		btnPlay.GetComponent<Button>().onClick.AddListener(() => ActionPlay());
		btnPlay.GetComponent<Button>().interactable = false;
		
		btnMenu = new GameObject("ButtonMenu", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button) });
		btnMenu.transform.SetParent(level.group.transform);
		btnMenu.GetComponent<Image>().color = colorMenu;
		btnMenu.GetComponent<Image>().sprite = sprMenu;
        btnMenu.GetComponent<Image>().preserveAspect = true;
		btnMenu.GetComponent<RectTransform>().anchorMin = new Vector2(0.4f, 0);
		btnMenu.GetComponent<RectTransform>().anchorMax = new Vector2(0.6f, 0.13f);
		btnMenu.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
		btnMenu.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
		ColorBlock colorsMenu = btnMenu.GetComponent<Button>().colors;
		colorsMenu.pressedColor = colorPressed;
		btnMenu.GetComponent<Button>().colors = colorsMenu;
		btnMenu.GetComponent<Button>().onClick.AddListener(() => ActionMenu());

        GameObject pnlTools = new GameObject("PanelTools", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image) } );
        pnlTools.transform.SetParent(canvas);
        pnlTools.GetComponent<Image>().color = colorPanelTools;
        pnlTools.GetComponent<Image>().preserveAspect = true;
        pnlTools.GetComponent<Image>().raycastTarget = false;
        pnlTools.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        pnlTools.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        pnlTools.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        pnlTools.GetComponent<RectTransform>().offsetMax = new Vector2(0, 1);
        pnlTools.SetActive(false);
        this.pnlTools = pnlTools.GetComponent<RectTransform>();
		
		GameObject toolsBasic = new GameObject ("Tools: Basic");
		toolsBasic.transform.SetParent (pnlTools.transform);
		toolsBasic.transform.localPosition = Vector2.zero;

		Vector2 toolSize = new Vector2 (Screen.width * 0.15f, Screen.width * 0.15f);
        toolChannelIDs = new GameObject[7];
        for (int i = 0; i < toolChannelIDs.Length; i++)
        {
			Sprite sprite = null;
			Color color = new Color();
			Vector2 tempSize = new Vector2();
			if (i <= 4)
			{
				sprite = levelManager.spriteChannels[i];
				color = levelManager.colorChannels[0];
				tempSize = toolSize;
			}
			else if (i == 5)
			{
				sprite = levelManager.spriteBalls;
				color = levelManager.colorBall;
				tempSize = toolSize * 0.5f;
			}
			else if (i == 6)
			{
				sprite = sprDelete;
				color = colorDelete;
				tempSize = toolSize;
			}
			toolChannelIDs[i] = new Tool(toolsBasic.transform, sprite, color, new Vector2((Screen.width * 0.2f) / 2, Screen.height / 2 - (Screen.width * 0.2f) * (i + 0.5f)), tempSize).gameObject;

			int ii = i;

			EventTrigger eventTrigger = toolChannelIDs[i].GetComponent<EventTrigger>();
			
			EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
			entryPointerDown.eventID = EventTriggerType.PointerDown;
			entryPointerDown.callback.AddListener(new UnityAction<BaseEventData>(delegate{ActionToolsBasicDown(ii);}));
			eventTrigger.triggers.Add(entryPointerDown);
			
			EventTrigger.Entry entryDrag = new EventTrigger.Entry();
			entryDrag.eventID = EventTriggerType.Drag;
			entryDrag.callback.AddListener(new UnityAction<BaseEventData>(delegate{ActionToolsBasicDrag(ii);}));
			eventTrigger.triggers.Add(entryDrag);
			
			EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
			entryPointerUp.eventID = EventTriggerType.PointerUp;
			entryPointerUp.callback.AddListener(new UnityAction<BaseEventData>(delegate{ActionToolsBasicUp(ii, true);}));
			eventTrigger.triggers.Add(entryPointerUp);
		}

		GameObject toolsTypes = new GameObject ("Tools: Types");
		toolsTypes.transform.SetParent (pnlTools.transform);
		toolsTypes.transform.localPosition = Vector2.zero;
		this.toolsTypes = toolsTypes.GetComponent<RectTransform>();

        toolChannelTypes = new GameObject[levelManager.colorChannels.Length];
        for (int i = 0; i < toolChannelTypes.Length; i++)
        {
			toolChannelTypes[i] = new Tool(toolsTypes.transform, null, levelManager.colorChannels[i], new Vector2(-(Screen.width * 0.2f) / 2, Screen.height / 2 - (Screen.width * 0.2f) * (i + 0.5f)), toolSize).gameObject;
			int ii = i;
			toolChannelTypes[i].GetComponent<Button>().onClick.AddListener(() => ActionSelectChannelType(ii));
		}
        selectionChannelType = 0;
        GameObject grid = new Grid(toolChannelTypes[selectionChannelType].transform, sprGrid, colorSelection, new Vector2(level.channelSize, level.channelSize)).gameObject;
        toolChannelTypes[selectionChannelType].GetComponent<RectTransform>().sizeDelta = grid.GetComponent<RectTransform>().sizeDelta;
        toolChannelTypes[3].SetActive(false);
        toolChannelTypes[4].SetActive(false);
        toolChannelTypes[5].SetActive(false);
        this.EventPlayCheck();
        return level;
    }

    public void ActionTools()
    {

        StartCoroutine(AnimationOpenTab(pnlTools, 0.4f));
    }

    private float thinkInterval = 0.01f;
    private bool busy;
	private bool open;
    private IEnumerator AnimationOpenTab(RectTransform panel, float percentage)
    {
        busy = true;
        bool toggled = false;
        float pixels = Screen.width * percentage;
        float duration = 0.2f;
        float pixelsPerSecond = pixels / duration;

        if (!open)
        {
            toggled = true;
			panel.gameObject.SetActive(true);
			open = true;
            panel.offsetMin = new Vector2(-Screen.width * percentage, 0);
            panel.offsetMax = new Vector2(-Screen.width, 0);
        }
        else
        {
            pixelsPerSecond *= -1;
        }
        float displacement = 0f;
        do
        {
        	float dx = pixelsPerSecond * thinkInterval;
            displacement += pixelsPerSecond * thinkInterval;
        	if (Math.Abs(displacement) <= Math.Abs(pixels))
        	{
	            group.offsetMin += new Vector2(dx, 0);
	            group.offsetMax += new Vector2(dx, 0);
	            panel.offsetMin += new Vector2(dx, 0);
	            panel.offsetMax += new Vector2(dx, 0);
        	}
        	// TODO: Else set dx to the amount needed to fully open. Currently it does not open or close completely.
            yield return new WaitForSeconds(thinkInterval);
        } while (Mathf.Abs(displacement) < Mathf.Abs(pixels));

		if (!toggled && open)
        {
			open = false;
        }
        yield return new WaitForSeconds(thinkInterval);
        busy = false;
    }

    private int selectionChannelType;
	public void ActionSelectChannelType(int id)
	{
		Transform grid = toolChannelTypes [selectionChannelType].transform.GetChild (0); 
		toolChannelTypes [selectionChannelType].GetComponent<RectTransform>().sizeDelta = toolChannelTypes [id].GetComponent<RectTransform>().sizeDelta;
		toolChannelTypes [id].GetComponent<RectTransform>().sizeDelta = grid.GetComponent<RectTransform>().sizeDelta; 
		grid.SetParent (toolChannelTypes [id].transform);
		grid.localPosition = Vector2.zero;
		this.selectionChannelType = id;
		for (int i = 0; i <= 4; i++)
		{
			toolChannelIDs [i].GetComponent<Image> ().color = toolChannelTypes [id].GetComponent<Image> ().color;
		}
    }

	public Transform selectedTool { get; set; }
	public void ActionToolsBasicDown(int index)
	{
		selectedTool = Instantiate(toolChannelIDs[index]).transform;
		selectedTool.SetParent(canvas);
		selectedTool.position = Input.mousePosition;
		Destroy(selectedTool.GetComponent<Button>());
		Destroy(selectedTool.GetComponent<EventTrigger>());
	}

	public void ActionToolsBasicDrag(int index)
	{
		selectedTool.position = Input.mousePosition;
		if(!open)
		{
			EventToolDragOutsidePanel();
		}
		if (!busy && open && Input.mousePosition.x > Screen.width * 0.4f)
		{
			EventToolDragLeavePanel(index);
		}
	}

	public void ActionToolsBasicUp(int index, bool openTab)
	{
		if(openTab && !busy && !open)
		{
			ActionTools();
		}

		GameObject[] selected = GameObject.FindGameObjectsWithTag("Selected");
		foreach (GameObject g in selected)
		{
			EventReplaceChannelWithSelection(g, index);
		}
		Destroy(selectedTool.gameObject);
	}

	public void ActionFocusLevel()
	{
		if(!busy && open)
		{
			ActionTools();
		}
	}

	public void ActionChangeLevel() {
		// Todo : New level data is typed
		// validate
		// load
	}

	private void EventReplaceChannelWithSelection(GameObject selectedGrid, int index)
	{
		GameObject channel = selectedGrid.transform.parent.gameObject;

		int x = (int)channel.GetComponent<BehaviourChannel>().position.x;
		int y = (int)channel.GetComponent<BehaviourChannel>().position.y;
		selectedGrid.GetComponent<Image>().color = colorGrid;
		selectedGrid.tag = "Untagged";

		// IF SELECTION IS A CHANNEL OR ERASER
		if (index <= 4 || index == 6)
		{
			selectedGrid.transform.SetParent(channel.transform.parent);
			int type = selectionChannelType;
			if (index == 6)
			{
				index = 5;
				type = 0;
			}
			Destroy(channel);

			level.channels[y, x] = new Channel(GetComponent<GameManager>(), GetComponent<LevelManager>(), this, level.group.transform, index, 0, type, level.channelSize, new Vector2(x, y), level.size).gameObject;
			level.channels[y, x].GetComponent<Button>().onClick.AddListener(() => ActionFocusLevel());
			level.channels[y, x].GetComponent<BehaviourChannel>().setEditorMode(true);
			
			selectedGrid.transform.SetParent(level.channels[y, x].transform);
			if (index <= 4)
			{
				EventIncreaseLevelSize(x, y);
			}
			else if (index == 5)
			{
				EventDecreaseLevelSize(x, y);
			}
		}
		// IF SELECTION IS A BALL
		else if (index == 5)
		{
			if (channel.GetComponent<BehaviourChannel>().channelID != 5)
			{
				if (channel.tag != "Ball")
				{
					channel.tag = "Ball";
					GameObject dummyBall = Instantiate(selectedTool.gameObject);
					dummyBall.transform.SetParent(channel.transform);
					dummyBall.transform.SetAsLastSibling();
					dummyBall.transform.localPosition = Vector3.zero;
					dummyBall.name = "Ball";
				}
			}
		}
		this.EventPlayCheck();
	}

	private void EventToolDragOutsidePanel()
	{
		GameObject child = null;
		Vector2 selection = Utility.MouseToArrayPosition(level, Input.mousePosition);
		if (selection.x >= 0 && selection.x < level.size.x && selection.y >= 0 && selection.y < level.size.y)
		{
			child = level.channels[(int)selection.y, (int)selection.x].transform.GetChild(0).gameObject;
			if (child.tag != "Selected")
			{
				child.GetComponent<Image>().color = colorHighlighted;
				child.tag = "Selected";
			}
		}
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Selected"))
		{
			if (child == null || g != child)
			{
				g.GetComponent<Image>().color = colorGrid;
				g.tag = "Untagged";
			}
		}
	}

	private void EventToolDragLeavePanel(int index)
	{
		ActionTools();
		if (index <= 4)
		{
			selectedTool.GetComponent<RectTransform>().sizeDelta = new Vector2(level.channelSize, level.channelSize);
		}
		if (index == 5)
		{
			selectedTool.GetComponent<RectTransform>().sizeDelta = new Vector2(level.channelSize / 4, level.channelSize / 4);
		}
		else if (index == 6)
		{
			selectedTool.GetComponent<RectTransform>().sizeDelta = new Vector2(level.channelSize, level.channelSize);
		}
	}

	private void EventIncreaseLevelSize(int x, int y)
	{
		bool expand;
		if (level.size.y <= Level.maxLevelSize.y)
		{
			expand = false;
			for (int i = 0; i < level.size.x; i ++)
			{
				if(level.channels[0, i].GetComponent<BehaviourChannel>().channelID != 5)
				{
					expand = true;
					break;
				}
			}
			if (expand)
			{
				level = Utility.IncreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Down);
			}
			expand = false;
			for (int i = 0; i < level.size.x; i ++)
			{
				if(level.channels[(int)level.size.y - 1, i].GetComponent<BehaviourChannel>().channelID != 5)
				{
					expand = true;
					break;
				}
			}
			if (expand)
			{
				level = Utility.IncreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Up);
			}
		}
		
		if (level.size.x <= Level.maxLevelSize.x)
		{
			expand = false;
			for (int i = 0; i < level.size.y; i ++)
			{
				if(level.channels[i, 0].GetComponent<BehaviourChannel>().channelID != 5)
				{
					expand = true;
					break;
				}
			}
			if (expand)
			{
				level = Utility.IncreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Left);
			}
			expand = false;
			for (int i = 0; i < level.size.y; i ++)
			{
				if(level.channels[i, (int)level.size.x - 1].GetComponent<BehaviourChannel>().channelID != 5)
				{
					expand = true;
					break;
				}
			}
			if (expand)
			{
				level = Utility.IncreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Right);
			}
		}
		EventUpdateChannelGraphics();
	}
	
	private void EventDecreaseLevelSize(int x, int y)
	{
		bool shrink; 
		if (level.size.y > Level.minLevelSize.y)
		{
			if (y == 0 || y == 1)
			{
				shrink = true;
				for (int i = 0; i < level.size.x; i ++)
				{
					if(level.channels[0, i].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				for (int i = 0; i < level.size.x; i ++)
				{
					if(level.channels[1, i].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				if (shrink)
				{
					level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Down);
				}
			}
			if (y == level.size.y - 1 || y == level.size.y - 2)
			{
				shrink = true;
				for (int i = 0; i < level.size.x; i ++)
				{
					if(level.channels[(int)level.size.y - 1, i].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				for (int i = 0; i < level.size.x; i ++)
				{
					if(level.channels[(int)level.size.y - 2, i].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				if (shrink)
				{
					level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Up);
				}
			}
		}
		
		if (level.size.x > Level.minLevelSize.x)
		{
			if (x == 0 || x == 1)
			{
				shrink = true;
				for (int i = 0; i < level.size.y; i ++)
				{
					if(level.channels[i, 0].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				for (int i = 0; i < level.size.y; i ++)
				{
					if(level.channels[i, 1].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				if (shrink)
				{
					level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Left);
				}
			}
			if (x == level.size.x - 1 || x == level.size.x - 2)
			{
				shrink = true;
				for (int i = 0; i < level.size.y; i ++)
				{
					if(level.channels[i, (int)level.size.x - 1].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				for (int i = 0; i < level.size.y; i ++)
				{
					if(level.channels[i, (int)level.size.x - 2].GetComponent<BehaviourChannel>().channelID != 5)
					{
						shrink = false;
						break;
					}
				}
				if (shrink)
				{
					level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Right);
				}
			}
		}
		EventUpdateChannelGraphics();
	}

	private void EventTrimLevelSize()
	{
		bool shrink; 
		if (level.size.y >= Level.minLevelSize.y)
		{
			shrink = true;
			for (int i = 0; i < level.size.x; i ++)
			{
				if(level.channels[0, i].GetComponent<BehaviourChannel>().channelID != 5)
				{
					shrink = false;
					break;
				}
			}
			if (shrink)
			{
				level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Down);
			}
			shrink = true;
			for (int i = 0; i < level.size.x; i ++)
			{
				if(level.channels[(int)level.size.y - 1, i].GetComponent<BehaviourChannel>().channelID != 5)
				{
					shrink = false;
					break;
				}
			}
			if (shrink)
			{
				level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Up);
			}
		}
		
		if (level.size.x >= Level.minLevelSize.x)
		{
			shrink = true;
			for (int i = 0; i < level.size.y; i ++)
			{
				if(level.channels[i, 0].GetComponent<BehaviourChannel>().channelID != 5)
				{
					shrink = false;
					break;
				}
			}
			if (shrink)
			{
				level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Left);
			}
			shrink = true;
			for (int i = 0; i < level.size.y; i ++)
			{
				if(level.channels[i, (int)level.size.x - 1].GetComponent<BehaviourChannel>().channelID != 5)
				{
					shrink = false;
					break;
				}
			}
			if (shrink)
			{
				level = Utility.DecreaseLevelSize(gameManager, levelManager, level, Utility.Sides.Right);
			}
		}
		EventUpdateChannelGraphics();
	}

	private void EventUpdateChannelGraphics()
	{
		for (int y = 0; y < level.size.y; y++)
		{
			for(int x = 0; x < level.size.x; x++)
			{
				level.channels[y, x].transform.localPosition = new Vector3(level.channelSize * (x - ((level.size.x - 1) / 2)), level.channelSize * (y - ((level.size.y - 1) / 2)), 0);
				level.channels[y, x].GetComponent<RectTransform>().sizeDelta = new Vector2(level.channelSize, level.channelSize);
				if (level.channels[y, x].transform.childCount > 0)
				{
					level.channels[y, x].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(level.channelSize, level.channelSize);
				}
			}
		}
	}

	public void EventPlayCheck()
	{
		bool empty = true;
		foreach (GameObject channel in level.channels)
		{
			if (channel.GetComponent<BehaviourChannel>().channelID != 5)
			{
				empty = false;
				break;
			}
		}
		bool interactable = false;
		if (!empty)
		{
			int ballCount = GameObject.FindGameObjectsWithTag("Ball").Length;
			if (ballCount > 0)
			{
				if (ballCount <= Level.maxBallCount) 
				{
					int channelCount = 0;
					foreach (GameObject channel in level.channels)
					{
						if (channel.GetComponent<BehaviourChannel>().channelID != 5)
						{
							channelCount ++;
						}
					}
					if (ballCount != channelCount)
					{
						if (Utility.Unscrambled(level))
						{
							interactable = true;
		        			txtInstructions.text = "Alright you're good to go";
						}
						else
						{
		        			txtInstructions.text = "Enclose the level with channels";
						}
					}
					else
					{
	        			txtInstructions.text = "Seriously? Put some more channels";
					}
				}
				else
				{
        			txtInstructions.text = "Ey! Too many balls";
				}
			}
			else
			{
        		txtInstructions.text = "Place a ball onto the level";
			}
		}
		else
		{
        	txtInstructions.text = "Put a channel onto the level";
		}

    	txtDebug.text = "";
        txtDebug.readOnly = false;

		btnPlay.GetComponent<Button>().interactable = interactable;
	}

	private void ActionPlay()
	{
        List<Vector2> balls = new List<Vector2>();
		for (int y = 0; y < level.size.y; y++) { 
			for (int x = 0; x < level.size.x; x++) {
				if (level.channels[y, x].tag == "Ball") balls.Add(new Vector2(x, y));
			}
		}
        if (txtDebug.text == "") {
        	EventTrimLevelSize();
        	txtDebug.text = level.ToString(balls);
        	txtDebug.readOnly = true;
		} else {
        	level.group.gameObject.SetActive(false);
        	gameManager.EventLoadLevel(level.GetMap(), balls);
    	}
	}

	public void EventPlayToEditor()
	{
        level.group.gameObject.SetActive(true);
        EventIncreaseLevelSize(0, 0);
	}

	private void ActionMenu()
	{
		Destroy(level.group.gameObject);
		Destroy(pnlTools.gameObject);
		flowManager.EventEditorToMenu();
	}
}