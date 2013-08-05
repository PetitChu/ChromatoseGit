using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0414
[RequireComponent(typeof(Movement))]
public class Npc : ColourWell {
	
	private Movement movement;
	private Transform t;
	private Transform target;
	private Transform closestNode;
	
	private Avatar.LoseAllColourParticle losePart;
	
	//for the cyan guy
	private bool addingGreen;
	private bool waitingForMaxGreen;
	
	
	private ColourBeing.Colour initColour;
	private Pather myPather;
	private int currentNode = 0;

	private List<Transform> myPath;
	
	//elbow room variables
	private Transform[] allNPCs;
	private int elbowRoom = 25;
	private float elbowSpeed = 10f;
	private bool elbowing = false;
	private Vector3 elbowVector;
	private Transform elbowee;
	
	private bool announceOtherNPCsRequired = false;
		
	public bool AnnounceOtherNPCsRequired {
		get { return announceOtherNPCsRequired; }
		set { announceOtherNPCsRequired = value; }
	}
	
	//about fucking off and shit
	
											//vars about my animations and what they DO
	
	
	//prolly gon' put some stuff here
	
	private GameObject miscPart;
	private tk2dAnimatedSprite miscPartAnim;
	
	private bool showingMiscPart;
	
	public class SpeechBubble{				//THE BUBBLE AND ITS DECLARATION!
		//variables of various types
		public GameObject go;
		public tk2dSprite spriteInfo;
		private Transform t;
		private Renderer r;
		private Transform parent;
		private Vector2 offset = new Vector2(64, 25);
		private float timer;
		
		private string collectibleBubbleName = "redBubble_veryHappy";
		public string CollectibleBubbleName{ get { return collectibleBubbleName; } }
		
		private tk2dTextMesh myNumber;
		private Vector3 digitOffset = new Vector3(10, 3, -2);
		private int myDigit = 0;
		
		public int Digit{
			get{ return myDigit; }
			set{ myDigit = value; }
		}
		
		//getters & setters
		public bool Showing {
			get{ return r.enabled; }
			set{ r.enabled = value;
				 myNumber.renderer.enabled = value;
				 if (value == false){
					Digit = 0;
				}
			}
		}
		
		public string SpriteName{
			get{ return spriteInfo.CurrentSprite.name; }
			set{ spriteInfo.SetSprite(value); }
		}
		
		public SpeechBubble(Transform toFollow, tk2dSpriteCollectionData sprcol){  //constructor!
			
			go = new GameObject(toFollow.name + "Bubble");
			parent = toFollow;
			tk2dSprite.AddComponent(go, sprcol, 0);
			spriteInfo = go.GetComponent<tk2dSprite>();
			
			r = go.renderer;
			t = go.transform;
			//t.parent = parent;
			//t.localPosition = (Vector3) offset;
			GameObject numberObj = new GameObject(go.name + "Number");
			myNumber = numberObj.AddComponent<tk2dTextMesh>();
			myNumber.font = ChromatoseManager.manager.chromatoseFont;
			myNumber.anchor = TextAnchor.MiddleCenter;
			myNumber.maxChars = 1;
			myNumber.Commit();
		}
		
		public SpeechBubble(Transform toFollow): this(toFollow, ChromatoseManager.manager.bubbleCollection)
		{
		}
		
		public void Main(){
			
			
			if (timer > 0){
				timer -= Time.deltaTime;
			}
			else if (r.enabled){
				r.enabled = false;
				Blend(Color.white);
			}
			t.position = parent.position + (Vector3)offset;
			myNumber.transform.position = t.position + digitOffset;
			
		}
		
		public void ShowBubbleFor(string bubble, float time, int digit){
			string myDigitString = digit > 0? digit.ToString() : "";
			SpriteName = bubble + myDigitString;
			timer = time;
			r.enabled = true;
		}
		
		public void ShowBubbleFor(string bubble, float time){
			ShowBubbleFor(bubble, time, 0);
		}
		
		public void Blend(Color color){
			spriteInfo.color = color;
		}
		
		public override string ToString(){
			return SpriteName;
		}
		
		public void Flip(bool left, bool bottom){
			
			offset = new Vector2(left? -offset.x : offset.x, bottom? -offset.y : offset.y);
			
			t.localPosition = (Vector3) offset;
		}
		
		
	}
	
	private Npc.SpeechBubble myBubble;
												//PRIVATE VARS TO DO WITH THE BUBBLE
												//ALL THE PUBLIC VARIABLES
	public int detectRadius = 250;
	public int initialSpeechRange = 400;
	public string happyBubbleName = "";
	public string sadBubbleName = "";
	public bool waitForMessage = false;
	public bool bubbleOnLeftSide = false;
	public bool bubbleOnBottomSide = false;
	
	public Vector2 fuckOffReference = new Vector2(1, 1);
	
	void Awake () {
		tag = "npc";
	}
	// Use this for initialization
	void Start () {
		
	//	tk2dSprite.AddComponent<tk2dSprite>(myBubble.go, ChromatoseManager.manager.bubbleCollection, bubbleName);
		movement = GetComponent<Movement>();
		t = transform;
		spriteInfo = GetComponent<tk2dSprite>();
		anim = GetComponent<tk2dAnimatedSprite>();
		manager = ChromatoseManager.manager;
		
		avatar = GameObject.FindGameObjectWithTag("avatar").GetComponent<Avatar>();
		
		
		initColour = new ColourBeing.Colour(colour.r, colour.g, colour.b);
		
		
		
		//My bubble!
		myBubble = new Npc.SpeechBubble(this.transform);
		
		if (bubbleOnLeftSide || bubbleOnBottomSide)
			myBubble.Flip(bubbleOnLeftSide, bubbleOnBottomSide);
		
		//allNPCs = FindSceneObjectsOfType(typeof(Npc)) as Transform[];
		GameObject[] allTheNPCs = GameObject.FindGameObjectsWithTag("npc");
		List<Transform> npcList = new List<Transform>();
		
		foreach (GameObject go in allTheNPCs){
			npcList.Add(go.transform);
		}
		
		allNPCs = npcList.ToArray();
		float animOffset = Random.Range(0f, 1f);
		
		anim.Play(animOffset);
		
		
		//Particle:  instantiate it 
		miscPart = new GameObject(name + "Shadow");
		tk2dAnimatedSprite.AddComponent<tk2dAnimatedSprite>(miscPart, anim.Collection, 0);
		miscPartAnim = miscPart.GetComponent<tk2dAnimatedSprite>();
		miscPart.transform.SetParent(gameObject);
		miscPartAnim.anim = anim.anim;
		miscPartAnim.renderer.enabled = false;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//<^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^>
		//<---------------SETUP SECTION!--------------->
		//<vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv>
		
		//myBubble.Main();
		if (losePart != null){
			losePart.Fade();
		}
		//.....................................................................tumbleweed
		
		//<^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^>
		//<--------------CHECKS SECTION!--------------->
		//<vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv>
		
		//sad, lonely, formerly glorious section...
		
		//<^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^>
		//<--------------UPDATE SECTION!--------------->
		//<vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv>
		
							//Don't want you NPCs all on top of each other and everything!
		/*
		if (!elbowing){
			foreach (Transform tr in allNPCs){
				if (tr == t) continue;
				Vector3 dist = t.position - tr.position;
				if (dist.magnitude < elbowRoom){
					elbowVector = dist.normalized * Time.deltaTime * elbowSpeed;
					elbowee = tr;
					elbowing = true;
				}
			}
		}
		else{
			t.Translate(elbowVector, Space.World);
			Vector3 dist = elbowee.position - transform.position;
			if (dist.magnitude > elbowRoom){
				elbowing = false;
				elbowee = null;
				elbowVector = Vector3.zero;
			}
		}*/
							//Was I red and now I'm not?
		
		
							//this is for if I'm showing my particles
		/*
		if (showingMiscPart){
			miscPart.renderer.enabled = true;
			miscPart.transform.position = t.position + Vector3.forward;
			miscPart.transform.rotation = t.rotation;
			miscPartAnim.color = spriteInfo.color;
		}*/
		
						//show speech bubbles! If I have a happy one or a sad one and I'm happy or sad, respectively, show that shit
		string toShow = colour.White ? sadBubbleName : happyBubbleName;
		Vector3 diff = avatar.transform.position - t.position;
		
		if (diff.magnitude < initialSpeechRange && toShow != ""){
			myBubble.Showing = true;
			myBubble.SpriteName = toShow;
		}
		
		
		
		
		//end :)
	}
	
	void OnTriggerStay(Collider collider){
		
		if (collider != avatar.collider || colour.White) return;
		
		manager.UpdateAction(Actions.Absorb, Trigger);		//this tells the hud that I want to do something. But I'll have to wait in line!
		
		
	}
	
	override public void Trigger(){
		
		
		Color blendColor;
		if (colour.Red){
			blendColor = Color.red;
		}
		else if (colour.Green){
			blendColor = Color.green;
		}
		else {
			blendColor = Color.blue;
		}
		losePart = new Avatar.LoseAllColourParticle(avatar.particleCollection, avatar.partAnimations, t, blendColor);
		
		
		avatar.TakeColour(colour);
		colour = new Colour();
		anim.Play("rNPC_redToGrey");
		anim.animationCompleteDelegate = GreyBounce;
	}
	
	public void GreyBounce(tk2dAnimatedSprite clip, int index){
		anim.Play("rNPC_bounceGrey");
	}
}
