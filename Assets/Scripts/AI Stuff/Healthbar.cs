using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
	public Animator anim;

	void Start(){
		int hp = anim.GetInteger("health");
		RectTransform rt = transform as RectTransform;
		float newx = (hp/10.0f) * rt.sizeDelta.x;
		transform.localPosition = new Vector3(newx - rt.sizeDelta.x/2,0,0);
	}


	void Update () {
		int hp = anim.GetInteger("health");

		RectTransform rt = transform as RectTransform;

		float newx = (hp/10.0f) * rt.sizeDelta.x;

		transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(newx - rt.sizeDelta.x/2,0,0),.1f);
	}

	void LateUpdate(){
		transform.rotation = Quaternion.identity;
		transform.localRotation = Quaternion.identity;
	}
}
