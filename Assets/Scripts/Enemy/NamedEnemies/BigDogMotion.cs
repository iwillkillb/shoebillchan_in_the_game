using UnityEngine;
using System.Collections;

public class BigDogMotion : EnemyMotion {

	bool isMoving = false;	//Stop(Idle) or Move

	public bool chasePlayer = true;

	//Animation Hashes
	public readonly static int animHash_Idle = Animator.StringToHash ("Base Layer.Idle");
	public readonly static int animHash_Run = Animator.StringToHash ("Base Layer.Walk");
	public readonly static int animHash_Sit = Animator.StringToHash ("Base Layer.Sit");
	public readonly static int animHash_Jump = Animator.StringToHash ("Base Layer.Jump");
	public readonly static int animHash_Landing = Animator.StringToHash ("Base Layer.Landing");
	public readonly static int animHash_AttackFront = Animator.StringToHash ("Base Layer.Attack Front");
	public readonly static int animHash_AttackBack = Animator.StringToHash ("Base Layer.Attack Back");




	void Start () {
		
		//Setting Effect Object's Scales
		transform.FindChild ("Effect Damage").localScale = transform.localScale * 0.5f;
		transform.FindChild ("Effect Dust").localScale = transform.localScale * 0.5f;
		if (es.die == false) {
			if (canMove) {
				StartCoroutine (MovingDecide ());
			}
		}
	}



	void FixedUpdate () {
		
		if (delayAttack < delayAttackOrigin)
			delayAttack += Time.deltaTime;
		if (delayJump < delayJumpOrigin)
			delayJump += Time.deltaTime;
		if (delayTurnAround < delayTurnAroundOrigin)
			delayTurnAround += Time.deltaTime;
		
		if (!es.die) {
			Move ();
			Jump ();
		} else {
			if(gc.grounded)
				anim.Play ("Sit");
			StopCoroutine (MovingDecide ());
		}

		if((gc.grounded && !anim.GetBool("grounded")) || (!gc.grounded && anim.GetBool("grounded")))
			anim.SetBool ("grounded", gc.grounded);
		
		//Jumping Motion && Falling && contact with ground...
		if (anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Jump && rigi.velocity.y < 0 && gc.grounded) {
			anim.Play ("Landing");
			transform.FindChild ("Effect Dust").GetComponent<ParticleSystem> ().Play ();
			GameObject.Find ("Main Camera").GetComponent<CameraSetting> ().CameraShake ();
		}
	}






	IEnumerator MovingDecide(){	//MOVE OR IDLE
		while (true) {
			if (Random.Range (0, 2) > 0)
				isMoving = true;
			else
				isMoving = false;
			anim.SetBool ("move", isMoving);

			yield return new WaitForSeconds (1);
		}
	}



	void Move () {
		if (gc.grounded) {	//select Direction
			
			if (chasePlayer) {
				if (player != null) {	//Move to Direction of Player
					if (transform.position.x < player.transform.position.x)
						TurnAround (true);
					else
						TurnAround (false);
				} else {	//If There is no Player, Don't move.
					if (isMoving == true)
						isMoving = false;
				}
			} else {	//NO CHASE -> Keep MLL and MLR
				if (transform.position.x < mllInitPosition.x)
					TurnAround (true);
				if (transform.position.x > mlrInitPosition.x)
					TurnAround (false);
			}
		}

		if (isMoving) {
			//Continue Attack or Landing Motion -> X Speed is 0
			if (anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackFront ||
				anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackBack) {
				rigi.velocity = new Vector2 (0, rigi.velocity.y);

			} else if(anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Landing) {
				rigi.velocity = Vector2.zero;

			} else {
				if (goRight) {	//Actual Moving
					rigi.velocity = new Vector2 (speed, rigi.velocity.y);
				} else {
					rigi.velocity = new Vector2 (-speed, rigi.velocity.y);
				}
			}
		} else {
			rigi.velocity = new Vector2 (0, rigi.velocity.y);
		}

		//Turn Around Sprite
		if (rigi.velocity.x > 0 && !facingRight)
			Flip ();
		else if (rigi.velocity.x < 0 && facingRight)
			Flip ();
	}



	void Jump () {
		//BigDog doesn't detect walls in front. It just JUMP...
		if (gc.grounded && delayJump >= delayJumpOrigin) {
			delayJump = 0;
			rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
			anim.Play ("Jump");
		}
	}




	void OnCollisionStay2D(Collision2D col){	//ATTACK!	Contact with Player (Only Foward, Not Back)...
		if (canAttack && col.gameObject.tag == "Player" && gc.grounded && delayAttack >= delayAttackOrigin &&
			anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash_Sit &&
			anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash_Jump &&
			anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash_Landing) {	//If mob's current animation is one of them, mob can't attack.
			if ((facingRight && transform.position.x < col.transform.position.x) ||
			    (!facingRight && transform.position.x > col.transform.position.x)) {
				delayAttack = 0;
				anim.Play ("Attack Front");
			} else if ((!facingRight && transform.position.x < col.transform.position.x) ||
			        (facingRight && transform.position.x > col.transform.position.x)) {
				delayAttack = 0;
				anim.Play ("Attack Back");
			}
		}
	}



	void OnTriggerEnter2D(Collider2D col){	//DAMAGE!	Take player's attack
		if (col.tag == "Player Attack") {
			transform.FindChild ("Effect Damage").GetComponent<ParticleSystem> ().Play ();
			transform.FindChild ("Animator").GetComponent<SoundMaker> ().SoundOn (1);
			transform.FindChild ("Animator").GetComponent<SoundMaker> ().SoundOn (2);

			if(hitEffect != null)
				Instantiate (hitEffect, col.transform.position, col.transform.rotation);

			if (es.die == false) {
				//If mob takes attack behind, turn around but, mob can't attack quickly.
				if (transform.position.x > col.transform.parent.position.x && goRight == true) {
					goRight = false;
					delayAttack = 0;
				}
				if (transform.position.x < col.transform.parent.position.x && goRight == false) {
					goRight = true;
					delayAttack = 0;
				}
				es.hp -= col.GetComponentInParent<AnimatorFunctions> ().attackPower;
			}
		}
	}
}
