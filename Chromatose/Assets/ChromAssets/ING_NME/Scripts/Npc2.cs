using UnityEngine;
using System.Collections;

public class Npc2 : MonoBehaviour {
	
	public enum _TypeNPCEnum{
		Blue, Red
	}
	
	public _TypeNPCEnum typeNpc;
	
	private ChromatoseManager _Manager;
	private tk2dAnimatedSprite _MainAnim;
	private Avatar _AvatarScript;
	private Avatar.LoseAllColourParticle losePart;
	
	private Color myColor = Color.red;

	private string _redBounceString = "rNPC_bounce";
	private string _greyBounceString = "rNPC_bounceGrey";
	private string _blueBounceString = "bNPC_bounce";
	
	void Start () {
		Setup();
	}
	
	void Update () {
		if(losePart != null){
			losePart.Fade();
		}
	}
	
	void OnTriggerStay(Collider other){
		if(other.tag != "avatar") return;
		
		_Manager.UpdateAction(Actions.Absorb, Trigger);		//this tells the hud that I want to do something. But I'll have to wait in line!
			
	}
	
	void Trigger(){
		_AvatarScript.FillBucket(myColor);
		_MainAnim.Play("rNPC_redToGrey");
		_MainAnim.animationCompleteDelegate = GreyBounce;	

		losePart = new Avatar.LoseAllColourParticle(_AvatarScript.particleCollection, _AvatarScript.partAnimations, this.transform, myColor);
		//StartCoroutine(DelaiBeforeFade(1.0f));
		
	}
		
	void Setup(){
		_MainAnim = GetComponent<tk2dAnimatedSprite>();
		_Manager = ChromatoseManager.manager;
		_AvatarScript = GameObject.FindGameObjectWithTag("avatar").GetComponent<Avatar>();
		
		switch(typeNpc){
		case _TypeNPCEnum.Red:
			_MainAnim.Play(_redBounceString);			
			myColor = Color.red;
			break;
		case _TypeNPCEnum.Blue:
			_MainAnim.Play(_blueBounceString);
			myColor = Color.blue;
			break;			
		}
	}
	
	public void GreyBounce(tk2dAnimatedSprite clip, int index){
		_MainAnim.Play(_greyBounceString);
	}
	
	IEnumerator DelaiBeforeFade(float delai){
		yield return new WaitForSeconds(delai);
		losePart.Fade();
	}
}
