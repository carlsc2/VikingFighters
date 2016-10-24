using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour {
	public Text buttontext;
	bool paused = false;

	public void pause(){
		paused = !paused;
		if(paused){
			Time.timeScale = 0;
			buttontext.text = "Unpause";
		}else{
			Time.timeScale = 1;
			buttontext.text = "Pause";
		}
	}
}
