using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Transform canvas;
    public Sprite[] spriteChannels, spriteCracks;
    public Color[] colorChannels;
    public Sprite spriteBalls, spriteHome, spriteSpeedFast, spriteSpeedMedium;
    public Color colorBall, colorShade, colorTimer, colorHome, colorUnscrambled, colorSpeed;
    private readonly string delimiter = ",";

    public Level Load(int index)
    {
        try
        {
            string levelData = (Resources.Load("Levels/" + index) as TextAsset).text;
            List<Vector2> balls = new List<Vector2>();
            int delimiterIndex = levelData.IndexOf('-');
            for (var i = delimiterIndex + 1; i < levelData.Length; i += 2) {
            	balls.Add(new Vector2(Compression.fromBase62String(levelData.Substring(i, 1))[0], Compression.fromBase62String(levelData.Substring(i + 1, 1))[0]));
            }
            int [,,] map = Compression.uncompress(levelData.Substring(0, delimiterIndex));
            return GenerateMap(index, map, balls, new Vector2(map.GetLength(1), map.GetLength(0)));
        }
        catch (NullReferenceException)
        {
            return Random(index);
        }
    }

    public Level Load(int[,,] map, List<Vector2> ballPositions)
    {
		return GenerateMap(-1, map, ballPositions, new Vector2(map.GetLength(1), map.GetLength(0)));
    }

    private Level Random(int index)
    {
        UnityEngine.Random.seed = 100 + index;
		Vector2 size = GetRandomSize(index);
		int [,,] map = new int[(int)size.y, (int)size.x, 3];
		for (int y = 0; y < size.y; y++)
		{
			for (int x = 0; x < size.x; x++)
			{
				int [] channel = GetRandomChannel(map, size, x, y);
				map[y, x, 0] = channel[0];
				map[y, x, 1] = channel[1];
				map[y, x, 2] = channel[2];
			}
		}

        List<Vector2> ballPositions = GetRandomBallPositions(map, index, size);

        return GenerateMap(index, map, ballPositions, size);
	}
	
	private Vector2 GetRandomSize(int index)
	{
		Vector2 size = new Vector2(Mathf.Sqrt(index), Mathf.Sqrt(index) + 1);
		size = new Vector2(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));
		size.x = (size.x < 1) ? 1 : size.x;
		size.y = (size.y < 2) ? 2 : size.y;
		if (size.x > Level.maxLevelSize.x)
			size.x = Level.maxLevelSize.x;
		if (size.y > Level.maxLevelSize.y)
			size.y = Level.maxLevelSize.y;
		return size;
	}
	
	private int GetRandomBallCount(int index)
	{
		float b = (-1 + Mathf.Sqrt(3)) / -2;
		float a = (2 - b) / Mathf.Sqrt(45);
		int ballCount = Mathf.FloorToInt(a * Mathf.Sqrt(index) + b + 1);
		
		if (ballCount < 1)
			ballCount = 1;
		if (ballCount > Level.maxBallCount)
			ballCount = Level.maxBallCount;
		return ballCount;
	}

    private int[] GetRandomChannel(int[, ,] map, Vector2 size, int x, int y)
    {
        int channelID = -1, channelOrientation = -1, channelType = -1;

        object[] potentialChannels = PotentialChannels(map, size, new Vector2(x, y));
        bool[] restricted = (bool[])potentialChannels[0];
        bool[][] orientations = (bool[][])potentialChannels[1];
        List<int> channelIDs = new List<int>();
        for (int i = 0; i < restricted.Length; i++)
        {
            if (!restricted[i])
            {
                channelIDs.Add(i);
            }
        }
        channelID = channelIDs[UnityEngine.Random.Range(0, channelIDs.Count)];

        List<int> possibleOrientations = new List<int>();
        for (int i = 0; i < orientations[channelID].Length; i++)
        {
            if (orientations[channelID][i])
            {
                possibleOrientations.Add(i);
            }
        }
        channelOrientation = possibleOrientations[UnityEngine.Random.Range(0, possibleOrientations.Count)];

        channelType = 0;
        return new int[] { channelID, channelOrientation, channelType };
    }

    private List<Vector2> GetRandomBallPositions(int[,,] map, int index, Vector2 size)
    {
        List<Vector2> ballPositions = new List<Vector2>();
        int ballCount = GetRandomBallCount(index);
        for (int i = 0; i < ballCount; i++)
        {
            Vector2 position;
            bool redo = true;
            do
            {
				redo = false;
                position = new Vector2((int)UnityEngine.Random.Range(0, (int)size.x), (int)UnityEngine.Random.Range(0, (int)size.y));
                if (map[(int)position.y, (int)position.x, 0] == 5)
                {
                    redo = true;
                    continue;
                }
                foreach (Vector2 ballPosition in ballPositions)
                {
                    if (ballPosition == position)
                    {
                        redo = true;
                        break;
                    }
                }
                if (redo)
                {
                    continue;
                }
            }
            while (redo);
            ballPositions.Add(position);
        }
        return ballPositions;
    }

	private object[] PotentialChannels(int[,,] map, Vector2 size, Vector2 origin)
	{
		bool[] restrictions = { true, true, true, true, true, true };
		bool[][] orientations = { new bool[]{false, false, false, false},
			new bool[]{false, false, false, false},
			new bool[]{false, false, false, false},
			new bool[]{false, false, false, false},
			new bool[]{false, false, false, false},
			new bool[]{false, false, false, false} };
		bool[] paths = PotentialConnections(map, size, origin);
		//Debug.Log("Paths : {" + ((paths[0]) ? "N, " : "")+ ((paths[1]) ? "E, " : "")+ ((paths[2]) ? "S, " : "")+ ((paths[3]) ? "W" : "")+ "}");
		
		//0 path
		if (!paths[0] && !paths[1] && !paths[2] && !paths[3])
		{
			restrictions[5] = false;
			orientations[5][0] = true;
		}
		//1 path
		if (paths[0] && !paths[1] && !paths[2] && !paths[3])
			//N
		{
			restrictions[4] = false;
			orientations[4][3] = true;
		}
		else if (!paths[0] && paths[1] && !paths[2] && !paths[3])
			//E
		{
			restrictions[4] = false;
			orientations[4][2] = true;
		}
		else if (!paths[0] && !paths[1] && paths[2] && !paths[3])
			//S
		{
			restrictions[4] = false;
			orientations[4][1] = true;
		}
		else if (!paths[0] && !paths[1] && !paths[2] && paths[3])
			//W
		{
			restrictions[4] = false;
			orientations[4][0] = true;
		}
		//2 paths (corner)
		else if (paths[0] && paths[1] && !paths[2] && !paths[3])
			//N, E
		{
			restrictions[1] = false;
			orientations[1][2] = true;
			restrictions[4] = false;
			orientations[4][2] = true;
			orientations[4][3] = true;
		}
		else if (!paths[0] && paths[1] && paths[2] && !paths[3])
			//E, S
		{
			restrictions[1] = false;
			orientations[1][1] = true;
			restrictions[4] = false;
			orientations[4][1] = true;
		}
		else if (!paths[0] && !paths[1] && paths[2] && paths[3])
			//S, W
		{
			restrictions[1] = false;
			orientations[1][0] = true;
		}
		else if (paths[0] && !paths[1] && !paths[2] && paths[3])
			//N, W
		{
			restrictions[1] = false;
			orientations[1][3] = true;
			restrictions[4] = false;
			orientations[4][0] = true;
		}
		//2 paths (linear)
		else if (paths[0] && !paths[1] && paths[2] && !paths[3])
			//N, S
		{
			restrictions[0] = false;
			orientations[0][1] = true;
			restrictions[4] = false;
			orientations[4][1] = true;
		}
		else if (!paths[0] && paths[1] && !paths[2] && paths[3])
			//E, W
		{
			restrictions[0] = false;
			orientations[0][0] = true;
			restrictions[4] = false;
			orientations[4][0] = true;
		}
		//3 paths
		else if (paths[0] && paths[1] && paths[2] && !paths[3])
			//N, E, S
		{
			restrictions[0] = false;
			orientations[0][1] = true;
			restrictions[1] = false;
			orientations[1][1] = true;
			restrictions[2] = false;
			orientations[2][1] = true;
			restrictions[4] = false;
			orientations[4][1] = true;
		}
		else if (!paths[0] && paths[1] && paths[2] && paths[3])
			//E, S, W
		{
			restrictions[1] = false;
			orientations[1][0] = true;
			restrictions[2] = false;
			orientations[2][0] = true;
		}
		else if (paths[0] && !paths[1] && paths[2] && paths[3])
			//N, S, W
		{
			restrictions[1] = false;
			orientations[1][0] = true;
			restrictions[2] = false;
			orientations[2][3] = true;
		}
		else if (paths[0] && paths[1] && !paths[2] && paths[3])
			//N, E, W
		{
			restrictions[0] = false;
			orientations[0][0] = true;
			restrictions[1] = false;
			orientations[1][3] = true;
			restrictions[2] = false;
			orientations[2][2] = true;
			restrictions[4] = false;
			orientations[4][0] = true;
		}
		else if (paths[0] && paths[1] && paths[2] && paths[3])
			//N, E, S, W
		{
			restrictions[1] = false;
			orientations[1][0] = true;
			restrictions[2] = false;
			orientations[2][0] = true;
			orientations[2][3] = true;
			restrictions[3] = false;
			orientations[3][0] = true;
		}
		
		return new object[]{ restrictions, orientations };
	}

	private bool[] PotentialConnections(int[,,] map, Vector2 size, Vector2 origin)
	{
		bool[] paths = { true, true, true, true };  //allow north, east, south, west
		if (origin.x == 0)  //if west most column
			paths[3] = false;   //restrict west
		if (origin.x == size.x - 1)   //if east most column
			paths[1] = false;   //restrict east
		if (origin.y == 0)  //if south most row
			paths[2] = false;   //restrict south
		if (origin.y == size.y - 1)   //if north most row
			paths[0] = false;   //restrict north
		if (paths[2])   //if there is a channel south of this channel
			if (!CanConnect(map, new Vector2(origin.x, origin.y - 1), 0))    //if channel south doesnt extend north
				paths[2] = false;   //restrict south
		if (paths[3])   //if there is a channel west of this channel
			if (!CanConnect(map, new Vector2(origin.x - 1, origin.y), 1))    //if channel west doesnt extend east
				paths[3] = false;   //restrict west
		
		return paths;
	}

	private bool CanConnect(int [,,] map, Vector2 origin, int direction)
	{
		int channelID = map[(int)origin.y, (int)origin.x, 0];
		int channelOrientation =  map[(int)origin.y, (int)origin.x, 1];
		
		if (Utility.InnateChannels(channelID, channelOrientation)[direction])
			return true;
		return false;
	}

    [HideInInspector]
    public ButtonSpeed btnSpeed;
    public Level GenerateMap(int index, int[,,] map, List<Vector2> ballPositions, Vector2 size)
    {
        Level level = new Level(canvas, index, size, ballPositions);

        level.timer = new Timer(level.group.transform, colorTimer, 0.04f).gameObject.GetComponent<BehaviourTimer>();

        GameObject solution = new GameObject("Solution", new Type[] { typeof(RectTransform), typeof(CanvasRenderer) });
        solution.transform.SetParent(level.group.transform);
		solution.GetComponent<RectTransform>().anchorMin = new Vector2(0.05f, 0.05f);
		solution.GetComponent<RectTransform>().anchorMax = new Vector2(0.25f, 0.18f);
        solution.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        solution.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        GameObject btnHome = new GameObject("ButtonReturn", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button) });
        btnHome.transform.SetParent(level.group.transform);
        btnHome.GetComponent<Image>().color = colorHome;
        btnHome.GetComponent<Image>().sprite = spriteHome;
        btnHome.GetComponent<Image>().preserveAspect = true;
		btnHome.GetComponent<RectTransform>().anchorMin = new Vector2(0.4f, 0);
		btnHome.GetComponent<RectTransform>().anchorMax = new Vector2(0.6f, 0.13f);
        btnHome.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        btnHome.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        btnHome.GetComponent<Button>().onClick.AddListener(() => GetComponent<GameManager>().EventHome());
		level.buttons.Add(btnHome.GetComponent<Button>());

        GameObject btnSpeed = new GameObject("ButtonSpeed", new Type[] { typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(EventTrigger), typeof(ButtonSpeed) });
        btnSpeed.transform.SetParent(level.group.transform);
        btnSpeed.GetComponent<Image>().color = colorSpeed;
        btnSpeed.GetComponent<Image>().sprite = spriteSpeedMedium;
        btnSpeed.GetComponent<Image>().preserveAspect = true;
		btnSpeed.GetComponent<RectTransform>().anchorMin = new Vector2(0.8f, 0);
		btnSpeed.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.13f);
        btnSpeed.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        btnSpeed.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        this.btnSpeed = btnSpeed.GetComponent<ButtonSpeed>();
        btnSpeed.GetComponent<ButtonSpeed>().spriteSpeedFast = spriteSpeedFast;
        btnSpeed.GetComponent<ButtonSpeed>().spriteSpeedMedium = spriteSpeedMedium;
        btnSpeed.GetComponent<ButtonSpeed>().timer = level.timer;
        EventTrigger eventTrigger = btnSpeed.GetComponent<EventTrigger>();
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener(new UnityAction<BaseEventData>(btnSpeed.GetComponent<ButtonSpeed>().EventSpeedDown));
        eventTrigger.triggers.Add(entryPointerDown);
        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener(new UnityAction<BaseEventData>(btnSpeed.GetComponent<ButtonSpeed>().EventSpeedUp));
		eventTrigger.triggers.Add(entryPointerUp);
		level.buttons.Add(btnSpeed.GetComponent<Button>());

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                level.channels[y, x] = new Channel(GetComponent<GameManager>(), this, GetComponent<EditorManager>(), level.group.transform, map[y, x, 0], map[y, x, 1], map[y, x, 2], level.channelSize, new Vector2(x, y), size).gameObject;
        		GameObject channelCopy = UnityEngine.Object.Instantiate(level.channels[y, x]);
        		channelCopy.transform.SetParent(solution.transform);
        		channelCopy.GetComponent<Image>().color = colorUnscrambled;
        		Destroy(channelCopy.GetComponent<Button>());
        		Destroy(channelCopy.GetComponent<EventTrigger>());
				if (level.channels[y, x].GetComponent<BehaviourChannel>().channelID == 5)
				{
					level.channels[y, x].tag = "ShadedChannel";
				}
			}
        }
		solution.GetComponent<RectTransform>().localScale = new Vector3(0.25f,0.25f,0.25f);
        SpawnBalls(level, ballPositions);
        return level;
    }

    public void SpawnBalls(Level level, List<Vector2> ballPositions)
    {
        for (int i = 0; i < level.balls.Length; i++)
        {
            level.balls[i] = new Ball(GetComponent<GameManager>(), this, level, ballPositions[i]).gameObject;
            level.balls[i].GetComponent<BehaviourBall>().btnSpeed = btnSpeed;
        }
    }

	public float ScrambleLevelOrientation(Level level)
	{
		float preparationTime = Level.preperationTimeInitial;
		foreach (GameObject c in level.channels)
		{
			if (c.GetComponent<BehaviourChannel>().channelType != BehaviourStatic.id)
			{
				int randomOrientation = UnityEngine.Random.Range(0, 3);
				int currentOrientation = (int)(c.transform.localEulerAngles.z / 90);
				c.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (currentOrientation - randomOrientation) * 90));
				preparationTime += randomOrientation * Level.preperationTimePerRotation;
			}
		}
		return preparationTime;
	}
}