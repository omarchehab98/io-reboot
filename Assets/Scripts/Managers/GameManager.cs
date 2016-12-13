using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private FlowManager flowManager;
    private MenuManager menuManager;
    private LevelManager levelManager;
    private ScoreManager scoreManager;
    public Level loadedLevel { get; set; }
    private bool unscrambled;
    private float thinkInterval = 0.01f;
    private bool isDone;
    private bool stopFade;

	public void Awake()
    {
        flowManager = GetComponent<FlowManager>();
        levelManager = GetComponent<LevelManager>();
        scoreManager = GetComponent<ScoreManager>();
        menuManager = GetComponent<MenuManager>();
	}

    public void EventLoadLevel(int[,,] map, List<Vector2> ballPositions)
    {
        unscrambled = false;
        isDone = false;
        loadedLevel = levelManager.Load(map, ballPositions);
        loadedLevel.timer.onFinish = EventTimerFinish;
        loadedLevel.timer.Begin(5);
        StartCoroutine(AnimationFade(true));
    }

    public void EventLoadLevel(int index)
	{
        unscrambled = false;
        isDone = false;
		loadedLevel = levelManager.Load(index);
		loadedLevel.timer.onFinish = EventTimerFinish;
		loadedLevel.timer.Begin(levelManager.ScrambleLevelOrientation(loadedLevel));
		StartCoroutine(AnimationFade(true));
    }

    public void EventHome()
    {
		loadedLevel.group.transform.SetAsFirstSibling();
		loadedLevel.buttons[0].interactable = false;
        EventPause(true);
        StartCoroutine(AnimationFadeOut());
        flowManager.EventPlayPause();
    }

    public void EventResume()
	{
		loadedLevel.group.transform.SetAsLastSibling();
		loadedLevel.buttons[0].interactable = true;
        EventPause(false);
        StartCoroutine(AnimationFade(true));
    }

	public void EventTimerFinish()
	{
		EventPause(false);
        this.isDone = true;
	}

    public void EventPause(bool pause)
    {
        PauseTimer(pause);
        PauseBalls(pause);
        PauseChannels(pause);
    }

    private void PauseTimer(bool pause)
    {
        loadedLevel.timer.frozen = pause;
    }

    private void PauseBalls(bool pause)
    {
		if (pause == false)
		{
			if (loadedLevel.timer.duration != -1)
			{
				pause = true;
			}
		}
        foreach (GameObject ball in loadedLevel.balls)
        {
            ball.GetComponent<BehaviourBall>().frozen = pause;
        }
    }

    private void PauseChannels(bool pause)
    {
        foreach (GameObject channel in loadedLevel.channels)
        {
            channel.GetComponent<BehaviourChannel>().frozen = pause;
        }
    }

    public void EventLevelComplete(bool win) 
    {
        EventPause(true);
        if (win)
        {
            if (loadedLevel.index == -1)
            {
                StartCoroutine(AnimationLevelToEditor());
            }
            else
            {
                StartCoroutine(AnimationLevelWin());
            }
        }
        else
        {
            StartCoroutine(AnimationLevelLose());
        }
    }

    public void EventCheckForUnscrambled()
    {
        if (this.isDone) return;
        if (!this.unscrambled)
        {
            bool unscrambled = Utility.Unscrambled(loadedLevel);
            if (unscrambled)
            {
                this.unscrambled = true;
                foreach (GameObject channel in loadedLevel.channels)
                {
                    if (channel.GetComponent<BehaviourChannel>().channelType == 0)
                    {
                        channel.GetComponent<Image>().color = levelManager.colorUnscrambled;
                    }
                }
            }
        }
    }

    public IEnumerator AnimationLevelWin()
    {
        stopFade = true;
        StartCoroutine(AnimationFade(false));
        scoreManager.EventWinLevel(false);
        yield return StartCoroutine(menuManager.AnimationRotateWheel(1.25f));
        EventLoadLevel(scoreManager.levelCurrent);
    }

    public IEnumerator AnimationLevelLose()
    {
        #if UNITY_IOS
        Handheld.Vibrate();
        #endif
        #if UNITY_ANDROID
        Handheld.Vibrate();
        #endif
        this.isDone = false;
        if (levelManager.btnSpeed.speed)
        {
            levelManager.btnSpeed.EventSpeedToggle();
        }
        int ballCount = loadedLevel.balls.Length;
        foreach (GameObject ball in loadedLevel.balls)
        {
            Destroy(ball);
        }
        foreach (GameObject shade in GameObject.FindGameObjectsWithTag("Shade"))
        {
            Destroy(shade);
        }
        foreach (GameObject channel in GameObject.FindGameObjectsWithTag("ShadedChannel"))
        {
            if (channel.GetComponent<BehaviourChannel>().channelID != 5)
            {
                channel.tag = "Untagged";
            }
        }
        List<Vector2> ballPositions = loadedLevel.ballPositions;
        levelManager.SpawnBalls(loadedLevel, ballPositions);
        float preparationTime = Level.preperationTimeInitial;
        if (!this.unscrambled)
        {
            int secondsSinceEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            UnityEngine.Random.seed = secondsSinceEpoch;

            for (int n = 0; n < 3; n++)
            {
                for (int y = 0; y < loadedLevel.size.y; y++)
                {
                    for (int x = 0; x < loadedLevel.size.x; x++)
                    {
                        if (UnityEngine.Random.Range(0, 3) >= 1)
                        {
                            loadedLevel.channels[y, x].GetComponent<BehaviourChannel>().EventRotateChannel(false, false);
                            preparationTime += Level.preperationTimePerRotation;
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            for (int n = 0; n < 3; n++)
            {
                for (int y = 0; y < loadedLevel.size.y; y++)
                {
                    for (int x = 0; x < loadedLevel.size.x; x++)
                    {
                        if (!loadedLevel.channels[y, x].GetComponent<BehaviourChannel>().IsUnscrambled())
                        {
                            loadedLevel.channels[y, x].GetComponent<BehaviourChannel>().EventRotateChannel(false, false);
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        loadedLevel.timer.Begin(preparationTime);
        PauseChannels(false);
		PauseTimer(false);
    }

    public IEnumerator AnimationLevelToEditor()
    {
        StartCoroutine(AnimationFade(false));
        scoreManager.EventWinLevel(true);
        yield return StartCoroutine(menuManager.AnimationRotateWheel(1.25f));
        menuManager.UpdateIndicator(scoreManager.levelCurrent);
        flowManager.EventPlayToEditor();
    }

    public IEnumerator AnimationFade(bool fadeIn)
    {
        Level levelToBeDestroyed = loadedLevel;
        float fadeIntensity = 0.04f;
		if (fadeIn)
		{
            stopFade = false;
			while (levelToBeDestroyed.group.alpha < 1 && !stopFade)
			{
				levelToBeDestroyed.group.alpha += fadeIntensity;
				yield return new WaitForSeconds(thinkInterval);
			}
        }
        else
		{
			while (levelToBeDestroyed.group.alpha > 0)
			{
				levelToBeDestroyed.group.alpha -= fadeIntensity;
				yield return new WaitForSeconds(thinkInterval);
			}
            DestroyLevel(levelToBeDestroyed);
        }
    }

    public void DestroyLevel(Level level)
    {
        foreach (GameObject channel in level.channels)
        {
            Destroy(channel);
        }
        Destroy(level.group.gameObject);
    }

    public void DestroyLevel()
    {
        if (loadedLevel.group == null) return; 
        DestroyLevel(loadedLevel);
    }


    public IEnumerator AnimationFadeOut()
    {
        float fadeIntensity = 0.04f;
        loadedLevel.group.alpha = 1;
        while (loadedLevel.group.alpha > 0)
        {
            loadedLevel.group.alpha -= fadeIntensity;
            yield return new WaitForSeconds(thinkInterval);
        }
    }

    public void EventShadedChannel()
    {
        bool levelShaded = true;
        foreach (GameObject c in loadedLevel.channels)
        {
            if (c.tag != "ShadedChannel")
            {
                levelShaded = false;
                break;
            }
        }
        if (levelShaded)
        {
            EventLevelComplete(true);
            return;
        }
    }

}