using UnityEngine;
using System.Collections;

public class PlayerMotion : MonoBehaviour {

	PlayerStatus ps;
	GroundCheck gc;
	Rigidbody2D rigi;
	Animator anim;
	AnimatorFunctions af;

	float moveAxis;	//move button input, player's move speed
	bool facingRight = true;	//sprite direction
	bool attackerRight = true;	//Where is enemy or trap attacks you?
	public float speed = 10f;

	bool sit = false;

	bool canJump = true;	//can you jump in this animation?
	int jumpCount = 0;	//Can you Double Jump?
	public float jumpPower = 10f;
	public int needMpDoubleJump = 10;

	bool canDash = true;	//can you use Dash skill in this animation?
	public int needMpDash = 20;
	public float dashPower = 5f;	//multiple value to speed in Dash Motion
	public float delayDashOrigin = 2f;
	float delayDash;

	float gravityBackUp;	//Flying Skill controls GravityScale in RigidBody. This saves gravityScale.
	bool isFlying = false;
	float mpAddBackup;
	public float delayFlyOrigin = 1f;
	float delayFly;

	public float needMpAirialAttack = 10f;

	float reduceHp;	//When you take damage, this get amount of damage.
	public float delayDamageOrigin = 0.1f;
	float delayDamage;

	//Animation Hashes
	public readonly static int animHash_Idle = Animator.StringToHash ("Base Layer.Idle");
	public readonly static int animHash_Run = Animator.StringToHash ("Base Layer.Run");
	public readonly static int animHash_Sit = Animator.StringToHash ("Base Layer.Sit");
	public readonly static int animHash_Crawl = Animator.StringToHash ("Base Layer.Crawl");
	public readonly static int animHash_Jump = Animator.StringToHash ("Base Layer.Jump");
	public readonly static int animHash_Fly = Animator.StringToHash ("Base Layer.Fly");

	public readonly static int animHash_AttackIdle = Animator.StringToHash ("Base Layer.Attack Idle");
	public readonly static int animHash_AttackIdle0 = Animator.StringToHash ("Base Layer.Attack Idle 0");
	public readonly static int animHash_AttackRun = Animator.StringToHash ("Base Layer.Attack Run");
	public readonly static int animHash_AttackRun0 = Animator.StringToHash ("Base Layer.Attack Run 0");
	public readonly static int animHash_AttackSit = Animator.StringToHash ("Base Layer.Attack Sit");
	public readonly static int animHash_AttackJump = Animator.StringToHash ("Base Layer.Attack Jump");
	public readonly static int animHash_AttackFly = Animator.StringToHash ("Base Layer.Attack Fly");

	public readonly static int animHash_JumpRolling = Animator.StringToHash ("Base Layer.Jump Rolling");
	public readonly static int animHash_Dash = Animator.StringToHash ("Base Layer.Dash");

	public readonly static int animHash_Hurt = Animator.StringToHash ("Base Layer.Hurt");
	public readonly static int animHash_HurtIdle = Animator.StringToHash ("Base Layer.Hurt Idle");
	public readonly static int animHash_HurtSit = Animator.StringToHash ("Base Layer.Hurt Sit");
	public readonly static int animHash_HurtJump = Animator.StringToHash ("Base Layer.Hurt Jump");
	public readonly static int animHash_Falling = Animator.StringToHash ("Base Layer.Falling");
	public readonly static int animHash_FallingCrash = Animator.StringToHash ("Base Layer.Falling Crash");
	public readonly static int animHash_Rise = Animator.StringToHash ("Base Layer.Rise");


	// Use this for initialization
	void Awake () {
		ps = GetComponent<PlayerStatus> ();
		gc = GetComponent<GroundCheck> ();
		rigi = GetComponent<Rigidbody2D> ();
		anim = GetComponentInChildren<Animator> ();
		af = GetComponentInChildren<AnimatorFunctions> ();
	}

	void Start () {
		gravityBackUp = rigi.gravityScale;
		mpAddBackup = ps.mpAdd;

		delayDash = delayDashOrigin;
		delayDamage = delayDamageOrigin;
		delayFly = delayFlyOrigin;
		StartCoroutine (Motions ());
	}

	IEnumerator Motions(){
		while (true) {
			if((gc.grounded && !anim.GetBool("grounded")) || (!gc.grounded && anim.GetBool("grounded")))
				anim.SetBool ("grounded", gc.grounded);

			if (delayDash < delayDashOrigin)
				delayDash += Time.deltaTime;
			if (delayDamage < delayDamageOrigin)
				delayDamage += Time.deltaTime;
			if (delayFly < delayFlyOrigin)
				delayFly += Time.deltaTime;

			if (ps.die == false) {	//Your Motion Functions are here.
				Move ();
				Sit ();
				Jump ();
				Attack ();
				Dash ();
				Fly ();
			} else {
				if(rigi.gravityScale != gravityBackUp)
					rigi.gravityScale = gravityBackUp;	//Else floating after die.
				if(!anim.GetBool("die"))
					anim.SetBool ("die", true);
			}
			yield return null;
		}
	}

	void Move () {

		if (anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Dash) {	//auto moving, Don't move y
			if (facingRight)
				rigi.velocity = new Vector2 (dashPower * speed, 0);
			else
				rigi.velocity = new Vector2 (-dashPower * speed, 0);

		} else if (
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackIdle ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackIdle0 ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackRun ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackRun0 ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackSit) {	//stop, but can Dash
			if(canJump)
				canJump = false;
			if(!canDash)
				canDash = true;
			rigi.velocity = new Vector2 (0, rigi.velocity.y);

		} else if (
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_FallingCrash ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Rise) {	//stop, You can nothing.
			if(canJump)
				canJump = false;
			if(canDash)
				canDash = false;
			rigi.velocity = new Vector2 (0, rigi.velocity.y);

		} else if (
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Hurt ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_HurtIdle ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_HurtJump ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Falling) {	//Knock Back after Damage
			if(canJump)
				canJump = false;
			if(canDash)
				canDash = false;
			if(attackerRight)
				rigi.velocity = new Vector2 (-speed * 1.5f, rigi.velocity.y);
			else
				rigi.velocity = new Vector2 (speed * 1.5f, rigi.velocity.y);

		} else if (
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackJump ||
			anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_AttackFly) {	//Don't move x,y
			if(canJump)
				canJump = false;
			if(!canDash)
				canDash = true;
			rigi.velocity = Vector2.zero;

		} else {
			//Others : Idle, Run, Sit, Crawl, Jump, Sit...
			//Normal moving : moveAxis, move take user's input.
			//moveAxis : Key -> Actual Character Moving.
			//move : Key -> Animation parameter.

			if(!canJump)
				canJump = true;
			if(!canDash)
				canDash = true;

			moveAxis = Input.GetAxis ("Horizontal");	//Key Input -> Rigidbody Velocity
			if (sit) {	//sitting -> slow speed
				moveAxis *= 0.2f;
			}

			//Finally, moveAxis, move set character's moving motion.
			rigi.velocity = new Vector2 (moveAxis * speed, rigi.velocity.y);

			anim.SetFloat ("move", Mathf.Abs(moveAxis));

			//Sprite Inverse.
			if (moveAxis > 0 && !facingRight)
				Flip ();
			else if (moveAxis < 0 && facingRight)
				Flip ();
		}
	}
	void Flip () {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}





	void Sit () {
		if (gc.grounded && Input.GetAxisRaw ("Vertical") < 0 && !sit) {
			sit = true;
			anim.SetBool ("sit", sit);
		}
		if ((!gc.grounded && sit) || gc.grounded && Input.GetAxisRaw ("Vertical") >= 0 && sit) {
			sit = false;
			anim.SetBool ("sit", sit);
		}
	}






	void Jump () {
		if (gc.grounded && jumpCount != 0){
			jumpCount = 0;	//Grounded, Not jump -> 0
		}

		if (canJump) {
			if (gc.grounded && !sit && Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") > 0) {	//First Input, Key:Up
				rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
			}

			if (jumpCount == 0 && !gc.grounded && Input.GetButtonUp ("Vertical")) {	//End of First Input -> You can Second Input
				jumpCount = 1;	//Grounded -> jump -> 1
			}

			if (jumpCount == 1 && ps.mp >= needMpDoubleJump && Input.GetButtonDown ("Vertical") && Input.GetAxisRaw ("Vertical") > 0) {	//Second Input, KeyDown:Up, Need SP
				rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower * 1.5f);
				jumpCount = 2;	//Airial -> jump -> 2 -> No jump
				ps.mp -= needMpDoubleJump;
				anim.Play ("Jump Rolling");
				transform.FindChild ("Effect Sandstar").GetComponent<ParticleSystem> ().Play ();
			}
		}

		/*
		if (rigi.velocity.y < 0 && Input.GetAxisRaw ("Vertical") > 0 && 	//If your status is JUMP/FLY, pressing jump key, and falling, Then down speed is half.
			(anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Jump || 
				anim.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash_Fly)) {
			rigi.velocity = new Vector2 (rigi.velocity.x, rigi.velocity.y * 0.5f);
		}*/
	}

	void Fly () {
		if (Input.GetButtonDown("Fly") && delayFly >= delayFlyOrigin) {	//Flying On
			delayFly = 0;
			if(isFlying == false)
				isFlying = true;
			rigi.gravityScale = 0;
			ps.mpAdd = 0;
			jumpCount = 2;	//If you Fly already, you cannot double jump.
			transform.FindChild ("Effect Sandstar").GetComponent<ParticleSystem> ().Play ();
		}
		if (Input.GetButtonUp("Fly") || ps.mp <= 0) {	//Flying Off
			if(isFlying == true)
				isFlying = false;
			ps.mpAdd = mpAddBackup;
			rigi.gravityScale = gravityBackUp;
		}

		if (isFlying) {
			ps.mp -= Time.deltaTime * mpAddBackup;
			rigi.velocity = new Vector2 (rigi.velocity.x * 2, Input.GetAxisRaw("Vertical") * speed);
		}
	}





	//Damage
	void OnTriggerEnter2D (Collider2D col) {	//If you contect traps or enemy's attack...

		if (col.tag == "Enemy Attack" && delayDamage >= delayDamageOrigin) {
			delayDamage = 0;

			if (col.GetComponentInParent<BigDogMotion> () != null)
				reduceHp = col.GetComponentInParent<BigDogMotion> ().attackPower;
			else if (col.GetComponentInParent<EnemyMotion> () != null)
				reduceHp = col.GetComponentInParent<EnemyMotion> ().attackPower;
			else if (col.GetComponent<BulletMotion> () != null)
				reduceHp = col.GetComponent<BulletMotion> ().damage;
			ps.hp -= reduceHp;

			if (transform.position.x < col.transform.position.x)
				attackerRight = true;
			else
				attackerRight = false;

			DamageMotion ();
		}
		else if(col.tag == "Trap" && delayDamage >= delayDamageOrigin){
			delayDamage = 0;

			if (col.GetComponentInParent<TrapSetting> () != null)
				reduceHp = col.GetComponentInParent<TrapSetting> ().trapDamage;
			ps.hp -= reduceHp;

			if (transform.position.x < col.transform.position.x)
				attackerRight = true;
			else
				attackerRight = false;

			DamageMotion ();
		}
	}

	void DamageMotion () {
		if (ps.hp > 0 && reduceHp < ps.hp_Original * 0.2f &&
			anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash_HurtIdle &&
			anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash_HurtJump &&
			anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash_Falling &&
			anim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash_FallingCrash) {
			if (gc.grounded) {
				if (sit)
					anim.Play ("Hurt Sit");
				else
					anim.Play ("Hurt Idle");
			} else {
				anim.Play ("Hurt Jump");
				GameObject.Find ("Main Camera").GetComponent<CameraSetting> ().CameraShake ();
			}
		} else {
			anim.Play ("Hurt Jump");
			GameObject.Find ("Main Camera").GetComponent<CameraSetting> ().CameraShake ();
		}

		transform.FindChild ("Animator").transform.FindChild("Hip").transform.FindChild("Chest").
		transform.FindChild("Head").transform.FindChild ("Effect Damage").GetComponent<ParticleSystem> ().Play ();
	}




	void Attack () {
		if (Input.GetButtonDown ("Attack")) {
			if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash_Idle) {	//This state can be attack state.
				anim.Play("Attack Idle");
			} else if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash_Run){
				anim.Play ("Attack Run");
			} else if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash_Jump && ps.mp >= needMpAirialAttack){
				ps.mp -= needMpAirialAttack;
				anim.Play ("Attack Jump");
			} else if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash_Fly && ps.mp >= needMpAirialAttack){
				ps.mp -= needMpAirialAttack;
				anim.Play ("Attack Fly");
			} else if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash_Sit ||
				anim.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash_Crawl){
				anim.Play ("Attack Sit");
			}  else {
				if (af.atkInputEnabled) {
					af.atkInputEnabled = false;
					af.atkInputNow = true;
				}
			}
		}
	}




	void Dash () {
		if (canDash && delayDash >= delayDashOrigin && ps.mp > needMpDash && Input.GetButtonDown("Skill")) {
			delayDash = 0;
			ps.mp -= needMpDash;
			anim.Play ("Dash");
			transform.FindChild ("Effect Sandstar").GetComponent<ParticleSystem> ().Play ();
		}
	}
}
