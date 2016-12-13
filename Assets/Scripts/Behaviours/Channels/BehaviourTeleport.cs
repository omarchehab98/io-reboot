using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourTeleport : BehaviourChannel
{
    public const int id = 3;
    private static readonly float teleportDelay = 1.5f;

    public override void Start()
    {
        base.Start();

	}

    public override void EventReachedOrigin(BehaviourBall ball)
    {
        base.EventReachedOrigin(ball);
        StartCoroutine(AnimationTeleportToTile(ball, 0));
    }

    private IEnumerator AnimationTeleportToTile(BehaviourBall ball, int tile)
    {
        Image image = ball.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        Color initialColor = image.color;
        Color teleportColor = image.color;

        teleportColor = new Color(teleportColor.r, teleportColor.g, teleportColor.b, 1);

        ball.frozen = true;

        float elapsed = 0f;
        while (image.color != teleportColor)
        {
            yield return new WaitForSeconds(0.01f);
            elapsed += Time.deltaTime;
            image.color = Color.Lerp(image.color, teleportColor, elapsed / (teleportDelay * 0.5f));
        }
        elapsed = 0f;

        teleportColor = new Color(teleportColor.r, teleportColor.g, teleportColor.b, 0);
        while (image.color.a > 0)
        {
            yield return new WaitForSeconds(0.01f);
            elapsed += Time.deltaTime;
            image.color = Color.Lerp(image.color, teleportColor, elapsed / (teleportDelay * 0.5f));
        }
        ball.state = BehaviourBall.stateUndecided;

        int x;
        do
        {
            x = UnityEngine.Random.Range(0, (int)gameManager.loadedLevel.size.x);
        }
        while (x == (int)position.x);

        int y;
        do
        {
            y = UnityEngine.Random.Range(0, (int)gameManager.loadedLevel.size.y);
        }
        while (y == (int)position.y);

        ball.EventTeleportTo(gameManager.loadedLevel.channels[y, x].transform);
        
        image.color = initialColor;
        ball.frozen = true;
    }
}