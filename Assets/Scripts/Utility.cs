using UnityEngine;

public static class Utility {
    private static readonly bool[] pathsNull = { false, false, false, false };

    public static int ToLocal(int direction, int channelOrientation)
    {
        return (direction - channelOrientation + 4) % 4;
    }

    public static int GetChannelOrientation(GameObject channel)
    {
        return Mathf.Abs(Mathf.RoundToInt(channel.transform.rotation.eulerAngles.z / 90f)) % 4;
    }

    public static bool[] InnateChannels(GameObject channel)
    {
        int channelID = channel.GetComponent<BehaviourChannel>().channelID;
        int channelOrientation = GetChannelOrientation(channel);
        return InnateChannels(channelID, channelOrientation);
    }

    public static bool[] InnateChannels(int channelID, int channelOrientation)
    {
        //Debug.Log("Channel ID " + channelID + " Channel Orientation " + channelOrientation);
        bool[] paths = pathsNull;
        if (channelID == 0)
        {
            paths[ToLocal(0, channelOrientation)] = false;
            paths[ToLocal(1, channelOrientation)] = true;
            paths[ToLocal(2, channelOrientation)] = false;
            paths[ToLocal(3, channelOrientation)] = true;
        }
        else if (channelID == 1)
        {
            paths[ToLocal(0, channelOrientation)] = false;
            paths[ToLocal(1, channelOrientation)] = false;
            paths[ToLocal(2, channelOrientation)] = true;
            paths[ToLocal(3, channelOrientation)] = true;
        }
        else if (channelID == 2)
        {
            paths[ToLocal(0, channelOrientation)] = false;
            paths[ToLocal(1, channelOrientation)] = true;
            paths[ToLocal(2, channelOrientation)] = true;
            paths[ToLocal(3, channelOrientation)] = true;
        }
        else if (channelID == 3)
        {
            paths[ToLocal(0, channelOrientation)] = true;
            paths[ToLocal(1, channelOrientation)] = true;
            paths[ToLocal(2, channelOrientation)] = true;
            paths[ToLocal(3, channelOrientation)] = true;
        }
        else if (channelID == 4)
        {
            paths[ToLocal(0, channelOrientation)] = false;
            paths[ToLocal(1, channelOrientation)] = false;
            paths[ToLocal(2, channelOrientation)] = false;
            paths[ToLocal(3, channelOrientation)] = true;
        }
        //Debug.Log("Paths : {" + ((paths[0]) ? "N, " : "")+ ((paths[1]) ? "E, " : "")+ ((paths[2]) ? "S, " : "")+ ((paths[3]) ? "W" : "")+ "}");
        return paths;
    }

    public static Vector3 DirectionToVectorDirection(int direction)
    {
        if (direction == 0)
            return new Vector3(0, 1, 0);
        else if (direction == 1)
            return new Vector3(1, 0, 0);
        else if (direction == 2)
            return new Vector3(0, -1, 0);
        else if (direction == 3)
            return new Vector3(-1, 0, 0);
        return Vector3.zero;
    }

    public static Vector3 DirectionToVector(int direction, int channelOrientation)
    {
        return DirectionToVectorDirection(ToLocal(direction, channelOrientation));
    }

    public static Vector2 WorldToArrayPosition(Level level, Vector3 worldPosition, int direction)
    {
		worldPosition += new Vector3(level.channelSize * ((level.size.x - 1) / 2), level.channelSize * ((level.size.y - 1) / 2), 0);
        worldPosition /= level.channelSize;
        if (direction != -1)
        {
            if (direction == 1)
            {
                worldPosition = new Vector2(Mathf.CeilToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
            }
            else if (direction == 3)
            {
                worldPosition = new Vector2(Mathf.FloorToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
            }
            else if (direction == 0)
            {
                worldPosition = new Vector2(Mathf.RoundToInt(worldPosition.x), Mathf.CeilToInt(worldPosition.y));
            }
            else if (direction == 2)
            {
                worldPosition = new Vector2(Mathf.RoundToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
            }
        }
        else
        {
            worldPosition = new Vector2(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
        }
        //Debug.Log("{" + worldPosition.x + "," + worldPosition.y + "}");
        return worldPosition;
    }
	
	public static Vector3 WorldToArrayPosition(Level level, Vector3 worldPosition)
	{
		return WorldToArrayPosition(level, worldPosition, -1);
	}

	public static Vector3 MouseToArrayPosition(Level level, Vector3 position)
	{
		position -= new Vector3(Screen.width, Screen.height, 0) / 2;

		return WorldToArrayPosition(level, position);
	}

    public static int InverseDirection(int direction)
    {
        return (direction + 2) % 4;
    }
	
	public static Level IncreaseLevelSize(GameManager gameManager, LevelManager levelManager, Level level, Sides side)
	{
		EditorManager editorManager = levelManager.GetComponent<EditorManager>();
		Vector2 size = level.size;
		Sprite sprGrid = editorManager.sprGrid;
		Color colorGrid= editorManager.colorGrid;
		Level newLevel = new Level();
		if (side == Sides.Left)
		{
			size += new Vector2(1, 0);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < level.size.y; y++)
			{
				for (int x = 0; x < level.size.x; x++)
				{
					newLevel.channels[y, x + 1] = level.channels[y, x];
					newLevel.channels[y, x + 1].GetComponent<BehaviourChannel>().position += new Vector2(1, 0);
				}
			}
			for (int y = 0; y < level.size.y; y++)
			{
				newLevel.channels[y, 0] = new Channel(gameManager, levelManager, editorManager, level.group.transform, 5, 0, 0, newLevel.channelSize, new Vector2(0, y), newLevel.size).gameObject;
				new Grid(newLevel.channels[y, 0].transform, sprGrid, colorGrid, new Vector2(level.channelSize, level.channelSize));
			}
		}
		else if (side == Sides.Down)
		{
			size += new Vector2(0, 1);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < level.size.y; y++)
			{
				for (int x = 0; x < level.size.x; x++)
				{
					newLevel.channels[y + 1, x] = level.channels[y, x];
					newLevel.channels[y + 1, x].GetComponent<BehaviourChannel>().position += new Vector2(0, 1);
				}
			}
			for (int x = 0; x < level.size.x; x++)
			{
				newLevel.channels[0, x] = new Channel(gameManager, levelManager, editorManager, level.group.transform, 5, 0, 0, newLevel.channelSize, new Vector2(x, 0), newLevel.size).gameObject;
				new Grid(newLevel.channels[0, x].transform, sprGrid, colorGrid, new Vector2(level.channelSize, level.channelSize));
			}
		}
		else if (side == Sides.Right)
		{
			size += new Vector2(1, 0);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < level.size.y; y++)
			{
				for (int x = 0; x < level.size.x; x++)
				{
					newLevel.channels[y, x] = level.channels[y, x];
				}
			}
			for (int y = 0; y < level.size.y; y++)
			{
				newLevel.channels[y, (int)newLevel.size.x - 1] = new Channel(gameManager, levelManager, editorManager, level.group.transform, 5, 0, 0, newLevel.channelSize, new Vector2(newLevel.size.x - 1, y), newLevel.size).gameObject;
				new Grid(newLevel.channels[y, (int)newLevel.size.x - 1].transform, sprGrid, colorGrid, new Vector2(level.channelSize, level.channelSize));
			}
		}
		else if (side == Sides.Up)
		{
			size += new Vector2(0, 1);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < level.size.y; y++)
			{
				for (int x = 0; x < level.size.x; x++)
				{
					newLevel.channels[y, x] = level.channels[y, x];
				}
			}
			for (int x = 0; x < level.size.x; x++)
			{
				newLevel.channels[(int)newLevel.size.y - 1, x] = new Channel(gameManager, levelManager, editorManager, level.group.transform, 5, 0, 0, newLevel.channelSize, new Vector2(x, newLevel.size.y - 1), newLevel.size).gameObject;
				new Grid(newLevel.channels[(int)newLevel.size.y - 1, x].transform, sprGrid, colorGrid, new Vector2(level.channelSize, level.channelSize));
			}
		}
		return newLevel;
	}

	public static Level DecreaseLevelSize(GameManager gameManager, LevelManager levelManager, Level level, Sides side)
	{
		Vector2 size = level.size;
		Sprite sprGrid = levelManager.GetComponent<EditorManager>().sprGrid;
		Color colorGrid= levelManager.GetComponent<EditorManager>().colorGrid;
		Level newLevel = new Level();
		if (side == Sides.Left)
		{
			size -= new Vector2(1, 0);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < newLevel.size.y; y++)
			{
				for (int x = 0; x < newLevel.size.x; x++)
				{
					newLevel.channels[y, x] = level.channels[y, x + 1];
					newLevel.channels[y, x].GetComponent<BehaviourChannel>().position -= new Vector2(1, 0);
				}
			}
			for (int y = 0; y < level.size.y; y++)
			{
				MonoBehaviour.Destroy(level.channels[y, 0]);
			}
		}
		else if (side == Sides.Down)
		{
			size -= new Vector2(0, 1);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < newLevel.size.y; y++)
			{
				for (int x = 0; x < newLevel.size.x; x++)
				{
					newLevel.channels[y, x] = level.channels[y + 1, x];
					newLevel.channels[y, x].GetComponent<BehaviourChannel>().position -= new Vector2(0, 1);
				}
			}
			for (int x = 0; x < level.size.x; x++)
			{
				MonoBehaviour.Destroy(level.channels[0, x]);
			}
		}
		else if (side == Sides.Right)
		{
			size -= new Vector2(1, 0);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < newLevel.size.y; y++)
			{
				for (int x = 0; x < newLevel.size.x; x++)
				{
					newLevel.channels[y, x] = level.channels[y, x];
				}
			}
			for (int y = 0; y < level.size.y; y++)
			{
				MonoBehaviour.Destroy(level.channels[y, (int)level.size.x - 1]);
			}
		}
		else if (side == Sides.Up)
		{
			size -= new Vector2(0, 1);
			newLevel = new Level(level.group, size, -1);
			
			for (int y = 0; y < newLevel.size.y; y++)
			{
				for (int x = 0; x < newLevel.size.x; x++)
				{
					newLevel.channels[y, x] = level.channels[y, x];
				}
			}
			for (int x = 0; x < level.size.x; x++)
			{
				MonoBehaviour.Destroy(level.channels[(int)level.size.y - 1, x]);
			}
		}
		return newLevel;
	}

	public enum Sides {
		Left = 1,
		Down = 2,
		Right = 3,
		Up = 4
	};

	public static bool Unscrambled(Level level)
	{
		bool scrambled = false;
		for (int y = 0; y < level.size.y; y++)
		{
			for (int x = 0; x < level.size.x; x++)
			{
				if (!Connected(level, x, y))
				{
					scrambled = true;
					break;
				}
			}
			if (scrambled)
			{
				break;
			}
		}
		return !scrambled;
	}
	
	private static bool Connected(Level level, int x, int y)
	{
		GameObject channel = level.channels[y, x];
		//N, E, S, W
		if (channel.GetComponent<BehaviourChannel>().channelID == 5) return true;
		bool[] paths = InnateChannels(channel);
		bool[] connected = { false, false, false, false };
		if (paths[0])
		{
			if (y + 1 < level.size.y)
			{
				if (InnateChannels(level.channels[y + 1, x])[2])
					connected[0] = true;
				else
					return false;
			}
		}
		else
			connected[0] = true;
		paths = InnateChannels(channel);
		if (paths[1])
		{
			if (x + 1 < level.size.x)
			{
				if (InnateChannels(level.channels[y, x + 1])[3])
					connected[1] = true;
				else
					return false;
			}
		}
		else
			connected[1] = true;
		paths = InnateChannels(channel);
		if (paths[2])
		{
			if (y - 1 >= 0)
			{
				if (InnateChannels(level.channels[y - 1, x])[0])
					connected[2] = true;
				else
					return false;
			}
		}
		else
			connected[2] = true;
		paths = InnateChannels(channel);
		if (paths[3])
		{
			if (x - 1 >= 0)
			{
				if (InnateChannels(level.channels[y, x - 1])[1])
					connected[3] = true;
				else
					return false;
			}
		}
		else
			connected[3] = true;
		return connected[0] && connected[1] && connected[2] && connected[3];
	}

}
