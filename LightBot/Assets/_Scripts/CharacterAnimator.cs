using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {

	private Vector3 lastPosition; 
	private bool grounded = false;
	private Animator animator;

	void Awake () {
		this.lastPosition = this.transform.position;
		this.animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		float moving = lastPosition.x + lastPosition.z - this.transform.position.x - transform.position.z;
		if ((moving < 0.005) && (moving > -0.005)) {
			animator.SetBool ("moving", false);
		}else {
			animator.SetBool ("moving", true);
		}

		float falling = -lastPosition.y + transform.position.y;
		if (falling < 0.005 && falling > -0.005) {
			animator.SetBool ("falling", false);
			animator.SetBool ("jumping", false);	
		}
		if (falling >= 0.005) animator.SetBool ("jumping", true);
		//if (falling <= -0.001) animator.SetBool ("falling", true);

		this.lastPosition = this.transform.position;
	}
}
