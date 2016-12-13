using UnityEngine;

public class BehaviourShuffle : BehaviourChannel
{
    public const int id = 4;

    public override void Start()
    {
        base.Start();
    }

    public override void EventEnteredChannel(BehaviourBall ball)
    {
        base.EventEnteredChannel(ball);
        //Shuffle adjacent tiles
    }
}