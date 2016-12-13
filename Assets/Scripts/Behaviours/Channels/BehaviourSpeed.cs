using UnityEngine;

public class BehaviourSpeed : BehaviourChannel
{
    public const int id = 2;
    private float speedDuration = 1.0f;
    public static readonly float speedScale = 3.0f;

	public override void Start()
    {
        base.Start();

    }

    public override void EventEnteredChannel(BehaviourBall ball)
    {
        base.EventEnteredChannel(ball);
        ball.effectSpeedElapsed = speedDuration;
    }

}