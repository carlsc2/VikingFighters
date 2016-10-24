using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	private Animator anim;
	public bool enable_input = false;
	public GameObject sword;
	public GameObject head;
	private int fscale = 1;

	public int movecount = 0;
	public int hitcount = 0;

	private NeuralNet NN;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		NN = GetComponent<NeuralNet>();
	}
	
	// Update is called once per frame
	void Update () {
		if(enable_input){
			if(Input.GetKeyDown(KeyCode.Z)){
				Stab();
			}
			if(Input.GetKeyDown(KeyCode.X)){
				Slash();
			}
			if(Input.GetKeyDown(KeyCode.C)){
				JumpBack();
			}
			if(Input.GetKeyDown(KeyCode.V)){
				JumpForward();
			}
			if(Input.GetKeyDown(KeyCode.B)){
				Parry();
			}
		}

		if(anim.GetCurrentAnimatorStateInfo(0).IsName("static") && anim.GetBool("jumpback")){
			transform.localPosition = new Vector3(transform.localPosition.x - 1.25f*fscale, 0,0);
		}
	}

	void LateUpdate(){
		GameObject foe = NN.opponent;
		if(anim.GetInteger("health") > 0){
			if(foe != null && fscale == 1 && foe.transform.position.x < transform.position.x && Mathf.Abs(foe.transform.position.x - transform.position.x) >= 2){
				Flip();
			}else if(foe != null && fscale == -1 && foe.transform.position.x > transform.position.x && Mathf.Abs(foe.transform.position.x - transform.position.x) >= 2){
				Flip();
			}
		}
	}

	// -------- player control functions -----------------

	public void Stab(){
		if(!anim.GetBool("jumpback")){
			anim.SetTrigger("stab");
		}
	}

	public void Slash(){
		if(!anim.GetBool("jumpback")){
			anim.SetTrigger("slash");
		}
	}

	public void JumpForward(){
		if(!anim.GetBool("jumpback")){
			anim.SetTrigger("jumpforward");
		}
	}

	public void JumpBack(){
		if(!anim.GetBool("jumpback")){
			anim.SetBool("jumpback",true);
		}
	}

	public void Parry(){
		if(!anim.GetBool("jumpback")){
			anim.SetTrigger("parry");
		}
	}

	//--------- these functions are for animation triggers ---------------------

	public void jbb(){
		anim.SetInteger("movecount",anim.GetInteger("movecount")+1);
		anim.SetBool("jumpback",false);
	}

	public void jfd(){
		anim.SetInteger("movecount",anim.GetInteger("movecount")+1);
		transform.localPosition = new Vector3(transform.localPosition.x + 1.25f*fscale, 0,0);
	}

	public void sfd(){
		anim.SetInteger("movecount",anim.GetInteger("movecount")+1);
		anim.SetInteger("hitcount",anim.GetInteger("hitcount")+1);
		transform.localPosition = new Vector3(transform.localPosition.x + .52f*fscale, 0,0);
	}

	public void kb(){
		transform.localPosition = new Vector3(transform.localPosition.x - .52f*fscale, 0,0);
	}

	public void dead(){
		anim.enabled = false;
		foreach(Collider2D col in GetComponentsInChildren<Collider2D>()){
			col.enabled = false;
		}
		NN.enabled = false;
	}

	public void acs(){
		sword.GetComponent<Collider2D>().enabled = true;
	}

	public void Flip(){
		transform.RotateAround(head.transform.position,Vector3.up,180);
		fscale *= -1;
	}
}
