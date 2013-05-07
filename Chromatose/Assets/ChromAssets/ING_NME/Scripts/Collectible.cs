using UnityEngine;
using System.Collections;

public class Collectible : ColourBeing {
	
	public Couleur colColour = Couleur.white;
	int closeDist = 50;
	Avatar avatar;
	Transform t;
	Transform avatarT;
	private float timer = 0.75f;
	private bool fading = false;
	private Vector2 velocity;
	private Transform collector;
	private float homeTiming = 0.7f;
	private float homeTimer = 0f;
	private bool justPutBack = false;
	private int clearDist = 150;
	private bool isShadow = false;
	
	
	// Use this for initialization
	void Start () {
		spriteInfo = GetComponent<tk2dSprite>();
		switch (colColour){
		case Couleur.white:
			colour.r = 0;
			colour.g = 0;
			colour.b = 0;
			break;
		case Couleur.red:
			colour.r = 255;
			colour.g = 0;
			colour.b = 0;
			break;
		case Couleur.green:
			colour.r = 0;
			colour.g = 255;
			colour.b = 0;
			break;
		case Couleur.blue:
			colour.r = 0;
			colour.g = 0;
			colour.b = 255;
			break;
		
		}
		avatar = GameObject.FindGameObjectWithTag("avatar").GetComponent<Avatar>();
		t = transform;
		avatarT = avatar.transform;
		velocity = Random.insideUnitCircle * Random.Range(55, 80);
		
		anim = gameObject.GetComponent<tk2dAnimatedSprite>();
	}
	
	// Update is called once per frame
	
	void Update () {
		if (!fading){
			Vector3 dist = avatarT.position - t.position;
			if (dist.magnitude < closeDist && !justPutBack){
				if (CheckSameColour(avatar.colour) || colColour == Couleur.white){
					ChromatoseManager.manager.AddCollectible(this);
					Gone = true;
					
					
				}
				if (isShadow){
					avatar.CancelOutline();
				}
			}
			else if (dist.magnitude > clearDist && justPutBack){
				justPutBack = false;
				Dead = false;
				Debug.Log("Shouldn't be dead no mo");
			}
		}
		else{
			if (colour.White)
				goto white;
			if (colour.Blue)
				goto blue;
		}
		return;		//if I'm not fading I'll skip this next
	white:
		t.Translate(velocity * Time.deltaTime);
		/*spriteInfo.color = new Color(spriteInfo.color.r, spriteInfo.color.g, spriteInfo.color.b, spriteInfo.color.a - Time.deltaTime);
		if (spriteInfo.color.a <= 0){
			Gone = true;
			fading = false;
			
		}*/
	
		return;
	blue:
		
		velocity = Vector2.Lerp(velocity, Vector2.zero, 0.01f);
		homeTimer += Time.deltaTime;
		Vector2 distanceToTarget = (Vector2)(collector.position - t.position);
		Vector2 blueVector = Vector2.Lerp(distanceToTarget * 40, Vector2.zero, homeTiming / homeTimer);
		t.Translate((blueVector + velocity) * Time.deltaTime);
		if (distanceToTarget.magnitude < 5){
			Gone = true;
			fading = false;
			
		}
		
		return;
	}
	/*
	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "avatar"){
			if (CheckSameColour(collider.gameObject.GetComponent<Avatar>().colour) || colColour == Couleur.white){
				ChromatoseManager.manager.AddCollectible(colColour);
				Gone = true;
			}
		}
	}
	*/
	override public void Trigger(){
		int direction = Random.Range(0, 359);
		t.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(0, direction, 0));
		Gone = false;
		fading = true;
		
		if (colour.Blue){
			collector = VectorFunctions.FindClosestOfTag(t.position, "blueCollector", 10000);
			anim.Play(anim.GetClipByName("bColl_lose"), 0);
		}
		if (colour.White){
			anim.Play(anim.GetClipByName("wColl_lose"), 0);
			anim.animationCompleteDelegate = GoneForever;
			anim.CurrentClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
		}
	}
	
	public void PutBack(Vector3 newPos){
		isShadow = true;
		justPutBack = true;
		t.position = (Vector3) newPos;
		Gone = false;
		Dead = true;
	}
	
	public void GoneForever(tk2dAnimatedSprite sprite, int index){
		Dead = true;
		Gone = true;
	}
	
}
