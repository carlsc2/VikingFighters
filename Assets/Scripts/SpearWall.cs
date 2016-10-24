using UnityEngine;
using System.Collections;

public class SpearWall : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		Animator anim = col.transform.root.GetComponentInChildren<Animator>();
		anim.SetInteger("health",0);
	}
}
