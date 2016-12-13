using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Level
{
    public int index { get; set; }
    public CanvasGroup group { get; set; }
    public GameObject[,] channels { get; set; }
    public GameObject[] balls { get; set; }
    public BehaviourTimer timer { get; set; }
	public float preperationTime { get; set; }
    public Vector2 size { get; set; }
	public float channelSize { get; set; }
    public List<Vector2> ballPositions { get; set; }
	public static readonly float preperationTimeInitial = 2f;
	public static readonly float preperationTimePerRotation = 0.5f;
	public static readonly int maxBallCount = 4;
	public static readonly Vector2 maxLevelSize = new Vector2(7, 9);
	public static readonly Vector2 minLevelSize = new Vector2(2, 2);
	public List<Button> buttons { get; set; }

    public Level(Transform canvas, int index, Vector2 size)
    {
		this.channels = new GameObject[(int)size.y, (int)size.x];
		this.channelSize = Screen.width / ((int)size.x + 2);
		this.size = size;
		this.index = index;
		this.buttons = new List<Button>();
        LoadParent(canvas);
	}
	
	public Level(CanvasGroup group, Vector2 size, int index)
	{
		this.channels = new GameObject[(int)size.y, (int)size.x];
		this.channelSize = Screen.width / ((int)size.x + 2);
		this.size = size;
		this.group = group;
		this.index = index;
		this.buttons = new List<Button>();
	}

	public Level(Transform canvas, int index, Vector2 size, List<Vector2> ballPositions)
	{
		this.channels = new GameObject[(int)size.y, (int)size.x];
		this.channelSize = Screen.width / ((int)size.x + 2);
        this.balls = new GameObject[ballPositions.Count];
        this.ballPositions = ballPositions;
		this.size = size;
		this.index = index;
		this.buttons = new List<Button>();
		LoadParent(canvas);
	}

    private void LoadParent(Transform canvas)
    {
        GameObject levelParent = new GameObject("Level " + index, new Type[] { typeof(RectTransform), typeof(CanvasGroup) });
        levelParent.transform.SetParent(canvas);
        levelParent.transform.localPosition = Vector3.zero;

        levelParent.tag = "Level";

        levelParent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        levelParent.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        levelParent.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        levelParent.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        group = levelParent.GetComponent<CanvasGroup>();
    }

	public void Freeze(bool frozen)
	{
		foreach (Button button in buttons)
		{
			button.interactable = !frozen;
		}
	}


	public int[,,] GetMap() {
		int[,,] map = new int[(int) size.y, (int) size.x, 3];
		for (var y = 0; y < (int) size.y; y++)
		{
			for (var x = 0; x < (int) size.x; x++)
			{
				BehaviourChannel channel = channels[y, x].GetComponent<BehaviourChannel>();
				int[] data = channel.GetData();
				map[y, x, 0] = data[0];
				map[y, x, 1] = data[1];
				map[y, x, 2] = data[2];
			}
		}
		return map;
	}

	public string ToString()
	{
		return Compression.compress(this.GetMap(), this.ballPositions);
	}

	public string ToString(int[,,] map)
	{
		return Compression.compress(map, this.ballPositions);
	}

	public string ToString(List<Vector2> balls)
	{
		return Compression.compress(this.GetMap(), balls);
	}

	public string ToString(int[,,] map, List<Vector2> balls)
	{
		return Compression.compress(map, balls);
	}
}