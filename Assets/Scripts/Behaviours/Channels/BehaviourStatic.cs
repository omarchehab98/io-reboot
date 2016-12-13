using UnityEngine;

public class BehaviourStatic : BehaviourChannel
{
    public const int id = 1;

    public override void Start()
	{
        base.Start();

	}

    public override void EventRotateChannel(bool inverted, bool checkForUnscrambled)
    {
    	if(editorMode) {
    		base.EventRotateChannel(inverted, checkForUnscrambled);
    	}
    }
}