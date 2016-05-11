using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {
	private Animator animator;
	
	void Awake () {
		this.animator = GetComponent<Animator>();
	}
	
	public void setIdle(){
		animator.SetBool ("moving", false);
		animator.SetBool ("falling", false);
		animator.SetBool ("jumping", false);
	}
	
	public void setMoving(){
		animator.SetBool ("moving", true);
		animator.SetBool ("falling", false);
		animator.SetBool ("jumping", false);
	}
	
	public void setJumping(){
		animator.SetBool ("moving", false);
		animator.SetBool ("falling", false);
		animator.SetBool ("jumping", true);
	}
	
	public void setFalling(){
		animator.SetBool ("moving", false);
		animator.SetBool ("falling", true);
		animator.SetBool ("jumping", false);
	}
}

