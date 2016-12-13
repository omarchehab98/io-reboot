using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;
    public Camera camera;
    public RectTransform groupMenu, ball, pnlLeaderboard, pnlSettings, btnAudio, btnNewLevel, btnResetGame;
    public Button btnLeaderboard, btnSettings;
    public Sprite sprUnmute, sprMute;
    public Text txtCurrentLevel, lblPlay;
    public string strPlay, strResume;
	[HideInInspector]
	public Color backgroundColor;
    public CanvasGroup groupButtons;

    private bool frozen;
    private bool busy;

    private float thinkInterval = 0.01f;

    public void Awake()
    {
        gameManager = GetComponent<GameManager>();
    	backgroundColor = camera.backgroundColor;
        StartCoroutine(AnimationRotate());
        lblPlay.text = strPlay;
    }

    public void Start() {
        
        float iconSize = Screen.width * 0.2f;
        btnNewLevel.sizeDelta = new Vector2(iconSize, iconSize);
        btnNewLevel.localPosition = new Vector3(0, (Screen.height / 2) - (iconSize * 0.5f), 0);
        btnResetGame.sizeDelta = new Vector2(iconSize, iconSize);
        btnResetGame.localPosition = new Vector3(0, (-Screen.height / 2) + (iconSize * 0.5f), 0);
        btnAudio.sizeDelta = new Vector2(iconSize, iconSize);
        btnAudio.localPosition = new Vector3(0, (Screen.height / 2) - (iconSize * 2.5f), 0);
    }

	private float rotationSpeed = 0.07f;
    private IEnumerator AnimationRotate()
    {
        while (true)
        {
            ball.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, rotationSpeed));
            yield return new WaitForSeconds(thinkInterval);
        }
    }

    private Vector2 position;
    private bool dragged;
    public void ActionDown()
    {
        position = Input.mousePosition;
        dragged = false;
    }

    public void ActionDrag()
    {
        float dragThreshold = 12f;
        Vector2 deltaPosition = (Vector2)Input.mousePosition - position;
        if (Mathf.Abs(deltaPosition.x) > dragThreshold)
        {
            dragged = true;
        }
        if (!pnlLeaderboard.gameObject.activeInHierarchy)
        {
            if (deltaPosition.x > dragThreshold)
            {
                ActionLeaderboard();
            }
        }
        if (!pnlSettings.gameObject.activeInHierarchy)
        {
            if (deltaPosition.x < -dragThreshold)
            {
                ActionSettings();
            }
        }
        position = Input.mousePosition;
    }

    public void ActionUp()
    {
        if (!dragged)
        {
            if (pnlLeaderboard.gameObject.activeInHierarchy)
            {
                ActionLeaderboard();
            }
            else if (pnlSettings.gameObject.activeInHierarchy)
            {
                ActionSettings();
            }
            else
            {
				if (!frozen)
				{
	                if (lblPlay.text == strPlay)
					{
	                    StartCoroutine(AnimationStartGame());
	                }
	                else if (lblPlay.text == strResume)
					{
	                    StartCoroutine(AnimationResumeGame());
	                }
				}
            }
            dragged = false;
        }
    }

    public void ActionSettings()
    {
        if (!busy)
        {
            if (!pnlLeaderboard.gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimationOpenTab(pnlSettings, false, 0.3f));
            }
            else
            {
                StartCoroutine(AnimationOpenTab(pnlLeaderboard, true, 0.3f));
            }
        }
    }

    // Todo: Remove return when reactivated
    public void ActionLeaderboard()
    {
        return;
        if (!busy)
        {
            if (!pnlSettings.gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimationOpenTab(pnlLeaderboard, true, 0.3f));
            }
            else
            {
                StartCoroutine(AnimationOpenTab(pnlSettings, false, 0.3f));
            }
        }
    }

    public void ActionToggleMute()
    {
        Image image = btnAudio.GetComponent<Image>();
        if (image.sprite == sprUnmute)
        {
            image.sprite = sprMute;
            GetComponent<MusicManager>().Mute(true);
        }
        else
        {
            image.sprite = sprUnmute;
            GetComponent<MusicManager>().Mute(false);
        }
    }

    public void ActionNewLevel()
    {
        StartCoroutine(AnimationNewLevel());
        gameManager.DestroyLevel();
    }

    public void ActionResetGame()
    {
        GetComponent<ScoreManager>().EventResetPlayerScores();
        UpdateIndicator(0);
    }

    private IEnumerator AnimationNewLevel()
    {
        if (!frozen)
        {
            busy = false;
            ActionSettings();
            Freeze(true);
            yield return StartCoroutine(AnimationFadeOut());
            GetComponent<FlowManager>().EventMenuToEditor();
            lblPlay.text = strPlay;
        }
    }

    private IEnumerator AnimationStartGame()
    {
        if (!frozen)
        {
            Freeze(true);
            yield return StartCoroutine(AnimationFadeOut());
            GetComponent<FlowManager>().EventMenuToPlay();
        }
    }

    private IEnumerator AnimationResumeGame()
    {
        if (!frozen)
        {
            Freeze(true);
            yield return StartCoroutine(AnimationFadeOut());
            GetComponent<FlowManager>().EventPlayResume();
        }
    }

    private IEnumerator AnimationFadeOut()
    {

        yield return StartCoroutine(AnimationCameraColorOut(0.5f, backgroundColor, Color.white));
    }

    public IEnumerator AnimationFadeIn()
    {
        yield return StartCoroutine(AnimationCameraLerpIn(0.5f, Color.white, backgroundColor));
        Freeze(false);
    }

    private IEnumerator AnimationOpenTab(RectTransform panel, bool invert, float percentage)
    {
        busy = true;
        bool toggled = false;
        float pixels = Screen.width * percentage;
        float duration = 0.2f;
        float pixelsPerSecond = pixels / duration;
        if (invert)
        {
            pixelsPerSecond *= -1;
        }
        if (!panel.gameObject.activeInHierarchy)
        {
            toggled = true;
            panel.gameObject.SetActive(true);
            pixelsPerSecond *= -1;
            if (invert)
            {
                panel.offsetMin = new Vector2(-Screen.width * percentage, 0);
                panel.offsetMax = new Vector2(-Screen.width, 0);
            }
            else
            {
                panel.offsetMin = new Vector2(Screen.width, 0);
                panel.offsetMax = new Vector2(Screen.width * percentage, 0);
            }
        }

        float displacement = 0f;
        do
        {
            displacement += pixelsPerSecond * thinkInterval;
            if (Mathf.Abs(displacement) < Mathf.Abs(pixels)) {
	            groupMenu.offsetMin += new Vector2(pixelsPerSecond * thinkInterval, 0);
	            groupMenu.offsetMax += new Vector2(pixelsPerSecond * thinkInterval, 0);
	            panel.offsetMin += new Vector2(pixelsPerSecond * thinkInterval, 0);
	            panel.offsetMax += new Vector2(pixelsPerSecond * thinkInterval, 0);
	        }
            yield return new WaitForSeconds(thinkInterval);
        }
        while (Mathf.Abs(displacement) < Mathf.Abs(pixels));
        
        
        if (!toggled && panel.gameObject.activeInHierarchy)
        {
            panel.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(thinkInterval);
        busy = false;
    }

    public void Freeze(bool frozen)
    {
        this.frozen = frozen;
        btnLeaderboard.interactable = !frozen;
        btnSettings.interactable = !frozen;
        groupMenu.GetComponent<EventTrigger>().enabled = !frozen;
    }

	public IEnumerator AnimationRotateWheel(float duration)
	{
        string tmp = this.lblPlay.text;
        this.lblPlay.text = "";
		float oldSpeed = rotationSpeed;
		rotationSpeed = 1.75f;
		yield return StartCoroutine(AnimationCameraColorOut(0.25f, camera.backgroundColor, backgroundColor));
		yield return new WaitForSeconds(duration);
		yield return StartCoroutine(AnimationCameraColorOut(0.25f, camera.backgroundColor, Color.white));
		rotationSpeed = oldSpeed;
        this.lblPlay.text = tmp;
	}

	private IEnumerator AnimationCameraColorOut(float lerpDuration, Color original, Color target)
	{
        float elapsed = 0f;
		while (camera.backgroundColor != target)
		{
			elapsed += thinkInterval;
            Color color = Color.Lerp(original, target, elapsed / lerpDuration);
			camera.backgroundColor = color;
			txtCurrentLevel.color = color;

            groupButtons.alpha -= 0.01f;
			yield return new WaitForSeconds(thinkInterval);
		}
	}

    private IEnumerator AnimationCameraLerpIn(float lerpDuration, Color original, Color target)
    {
        float elapsed = 0f;
        while (camera.backgroundColor != target)
        {
            elapsed += thinkInterval;
            Color color = Color.Lerp(original, target, elapsed / lerpDuration);
            camera.backgroundColor = color;
            txtCurrentLevel.color = color;

            groupButtons.alpha += 0.01f;
            yield return new WaitForSeconds(thinkInterval);
        }
    }

	public void UpdateIndicator(int index)
	{
        if (index >= 0)
        {
            txtCurrentLevel.text = "Level " + (index + 1);
        }
        else
        {
            txtCurrentLevel.text = "Custom";
        }
	}
}