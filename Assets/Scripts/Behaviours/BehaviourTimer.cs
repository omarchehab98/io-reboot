using UnityEngine;
using System.Collections;

public class BehaviourTimer : MonoBehaviour {
    public bool frozen { get; set; }
	public EventFinished onFinish { get; set; }
	public float duration { get; set; }
    public float elapsed { get; set; }
	private RectTransform rect;

	public void Awake()
	{
		rect = GetComponent<RectTransform>();
        duration = 1;
	}

	public void Begin(float duration)
	{
		this.duration = duration;
		this.elapsed = 0;
	}

	public void Update()
	{
		if (!frozen && duration != -1)
		{
			elapsed += Time.deltaTime;
			rect.offsetMax = new Vector2((Screen.width - ((elapsed / duration) * Screen.width)) - Screen.width, rect.offsetMax.y);
			if (elapsed >= duration)
			{
				duration = -1;
				if (onFinish != null)
				{
					onFinish();
				}
			}
		}
	}

	public delegate void EventFinished();
}
