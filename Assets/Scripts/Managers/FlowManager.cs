using UnityEngine;

public class FlowManager : MonoBehaviour
{
    private GameManager gameManager;
    private LevelManager levelManager;
    private MenuManager menuManager;
    private ScoreManager scoreManager;
    private EditorManager editorManager;

    public void Start()
    {
        this.gameManager = GetComponent<GameManager>();
        this.levelManager = GetComponent<LevelManager>();
        this.menuManager = GetComponent<MenuManager>();
        this.scoreManager = GetComponent<ScoreManager>();
        this.editorManager = GetComponent<EditorManager>();
        this.menuManager.ActionToggleMute();
        scoreManager.GetPlayerScores();
    }

    public void EventMenuToPlay()
    {
        scoreManager.levelCurrent = scoreManager.levelMaximum;
		gameManager.EventLoadLevel(scoreManager.levelCurrent);
    }

    public void EventMenuToEditor()
    {
        gameManager.loadedLevel = editorManager.EventLoadEmptyLevel(new Vector2(2, 2));
    }

    public void EventEditorToMenu()
    {
        StartCoroutine(menuManager.AnimationFadeIn());
    }

    public void EventPlayToEditor()
    {
        editorManager.EventPlayToEditor();
    }

    public void EventPlayPause()
    {
        menuManager.lblPlay.text = menuManager.strResume;
        StartCoroutine(menuManager.AnimationFadeIn());
    }

    public void EventPlayResume()
    {
        gameManager.EventResume();
    }
}