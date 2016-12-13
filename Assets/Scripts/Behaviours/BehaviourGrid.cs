using UnityEngine;
using System.Collections;

public class BehaviourGrid : MonoBehaviour {

	void LateUpdate ()
	{
		this.gameObject.transform.rotation = Quaternion.identity;
	}
}
