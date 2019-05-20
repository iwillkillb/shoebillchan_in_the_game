using UnityEngine;
using System.Collections;

public class EnemyMotion : MonoBehaviour {

	protected Rigidbody2D rigi;
	protected GroundCheck gc;
	protected EnemyStatus es;
	protected Animator anim;
	protected Transform wallCheck;

	protected Transform mll, mlr;
	protected Vector3 mllInitPosition, mlrInitPosition;	//Child Object 'Move Limit Left/Right' INITIAL position

	public float detectWallDistance = 3f;
	protected bool isWall;

	public bool canMove = true;
	protected bool facingRight = true;
	public bool goRight = true;	//false : go to left / true : go to right
	public float speed = 10f;

	public bool canJump = true;
	public float jumpPower = 15f;
	public float delayJumpOrigin = 2f;
	protected float delayJump;

	public float delayTurnAroundOrigin = 2f;
	protected float delayTurnAround;

	public bool canAttack = true;
	public float attackPower = 30f;
	public float delayAttackOrigin = 1.5f;
	protected float delayAttack;

	protected GameObject player;

	public GameObject hitEffect;

	// Use this for initialization. Awake() is used Named enemies too...
	protected void Awake () {
		rigi = GetComponentInParent<Rigidbody2D> ();
		gc = GetComponent<GroundCheck> ();
		es = GetComponent<EnemyStatus> ();
		anim = GetComponentInChildren<Animator> ();
		wallCheck = transform.FindChild ("Wall Check");

		if (transform.FindChild ("Move Limit Left").position.x < transform.FindChild ("Move Limit Right").position.x) {
			mll = transform.FindChild ("Move Limit Left");
			mlr = transform.FindChild ("Move Limit Right");
		} else {	//You can make mistake that swap the position of two objects.
			mll = transform.FindChild ("Move Limit Right");
			mlr = transform.FindChild ("Move Limit Left");
		}
		mllInitPosition = mll.position;
		mlrInitPosition = mlr.position;

		delayAttack = delayAttackOrigin;
		delayJump = delayJumpOrigin;
		delayTurnAround = delayTurnAroundOrigin;

		jumpPower *= (transform.localScale.y * 0.5f);	//Too BIG to leap wall...
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Start () {	//Named enemies use different Start(). -> Their Protected Start()
		//Setting Effect Object's Scales
		transform.FindChild ("Effect Damage").localScale = transform.localScale * 0.5f;
		transform.FindChild ("Effect Attack").localScale = transform.localScale * 0.5f;
	}

	void FixedUpdate () {
		if (delayAttack < delayAttackOrigin)
			delayAttack += Time.deltaTime;
		if (delayJump < delayJumpOrigin)
			delayJump += Time.deltaTime;
		if (delayTurnAround < delayTurnAroundOrigin)
			delayTurnAround += Time.deltaTime;

		if (!es.die) {
			if (canMove)
				Move ();
			if (canJump)
				Jump ();
		} else {
			rigi.velocity = Vector2.zero;
		}
	}



	void Move () {
		if (gc.grounded) {	//select Direction
			if (transform.position.x < mllInitPosition.x)
				TurnAround (true);
			if (transform.position.x > mlrInitPosition.x)
				TurnAround (false);
		}

		if (rigi.velocity.x == 0) {	//If mob stucks or meet undetected wall, mob turns around.
			if (goRight)
				TurnAround (false);
			else
				TurnAround (true);
		}

		if (goRight) {	//Actual Moving
			rigi.velocity = new Vector2 (speed, rigi.velocity.y);
		} else {
			rigi.velocity = new Vector2 (-speed, rigi.velocity.y);
		}

		//Turn Around Sprite
		if (rigi.velocity.x > 0 && !facingRight)
			Flip ();
		else if (rigi.velocity.x < 0 && facingRight)
			Flip ();
	}
	protected void Flip () {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	protected void TurnAround (bool afterRight) {
		if (delayTurnAround >= delayTurnAroundOrigin) {
			delayTurnAround = 0;
			goRight = afterRight;
		}
	}



	void Jump () {
		if (gc.grounded && delayJump >= delayJumpOrigin) {
			isWall = Physics2D.OverlapCircle (wallCheck.position, 0.1f, gc.whatIsGround);
			delayJump = 0;
			if(isWall)
				rigi.velocity = new Vector2 (rigi.velocity.x, jumpPower);
		}
	}




	void OnCollisionStay2D(Collision2D col){	//ATTACK!	Contact with Player (Only Foward, Not Back)...
		if (canAttack && col.gameObject.tag == "Player" && gc.grounded && delayAttack >= delayAttackOrigin) {
			if ((facingRight && transform.position.x < col.transform.position.x) ||
				(!facingRight && transform.position.x > col.transform.position.x)) {
				transform.FindChild ("Effect Attack").GetComponent<ParticleSystem> ().Play ();
				delayAttack = 0;
				anim.Play ("Attack");
			}
		}
	}



	void OnTriggerEnter2D(Collider2D col){	//DAMAGE!	Take player's attack
		if (col.tag == "Player Attack" && es.die == false) {
			transform.FindChild ("Effect Damage").GetComponent<ParticleSystem> ().Play ();
			transform.FindChild ("Animator").GetComponent<SoundMaker> ().SoundOn (1);
			transform.FindChild ("Animator").GetComponent<SoundMaker> ().SoundOn (2);

			if(hitEffect != null)
				Instantiate (hitEffect, col.transform.position, col.transform.rotation);

			//If mob takes attack behind, turn around but, mob can't attack quickly.
			if (transform.position.x > col.transform.parent.position.x && goRight == true) {
				goRight = false;
				delayAttack = 0;
			}
			if (transform.position.x < col.transform.parent.position.x && goRight == false) {
				goRight = true;
				delayAttack = 0;
			}

			anim.Play ("Hurt");
			es.hp -= col.GetComponentInParent<AnimatorFunctions>().attackPower;

			if(es.hp <= 0 && player != null)
				player.GetComponent<PlayerStatus> ().score += es.score;
		}
	}
}
