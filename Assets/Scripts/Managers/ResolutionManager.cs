using UnityEngine;
using System.Collections;

public class ResolutionManager : MonoBehaviour {
	public GameObject managers;
	void Awake () {
		#if UNITY_STANDALONE
        	Screen.SetResolution(320, 512, false);
        #endif
		#if UNITY_IOS
			managers.SetActive(true);
    	#endif
		#if UNITY_ANDROID
			managers.SetActive(true);
    	#endif
			
		#if UNITY_EDITOR
			managers.SetActive(true);
		#endif
	}
	
	#if UNITY_STANDALONE
	void Update () {
		if(Screen.width == 320 && Screen.height == 512) {
			managers.SetActive(true);
		}
}
    #endif
}
