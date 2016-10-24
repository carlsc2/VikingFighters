using UnityEngine;
using System.Collections;

public class Blade : MonoBehaviour {
	private Animator anim;
	private bool isColliding = false;

	// Use this for initialization
	void Start () {
		anim = transform.parent.parent.GetComponent<Animator>();
	}

	void OnTriggerEnter2D(Collider2D col){
		Animator anim2 = col.transform.root.GetComponentInChildren<Animator>();
		if(col.name == "shield"){
			if(!isColliding){
				if(anim2.GetCurrentAnimatorStateInfo(0).IsName("parry")){//if opponent is parrying
					anim.SetTrigger("knockback");//self gets knocked back
				}
				else{//else hurt foe
					anim2.SetTrigger("gethit");
					anim2.SetInteger("health", anim2.GetInteger("health")-1);
				}
				GetComponent<Collider2D>().enabled = false;
				isColliding = true;
			}
		}
		if(col.name == "body"){
			if(!isColliding){
				isColliding = true;
				anim2.SetTrigger("gethit");
				anim2.SetInteger("health", anim2.GetInteger("health")-3);
			}
		}
		if(col.name == "head"){
			if(!isColliding){
				isColliding = true;
				anim2.SetTrigger("gethit");
				anim2.SetInteger("health", anim2.GetInteger("health")-5);
			}
		}
	}

	void Update(){
		isColliding = false;
	}
}
