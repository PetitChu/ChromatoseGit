using UnityEngine;
using System.Collections;

public class ColourWell : ColourBeing {
	int colourToAdd;
	public int giveRate = 8;
	protected Avatar avatar;
	
	// Use this for initialization
	void Start () {
		if (colour.r > colourConsiderMin){
			colourToAdd = 0;
		}
		else if(colour.g > colourConsiderMin){
			colourToAdd = 1;
		}
		else if(colour.b > colourConsiderMin){
			colourToAdd = 2;
		}
		
		manager = ChromatoseManager.manager;
		avatar = GameObject.Find("Avatar").GetComponent<Avatar>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerStay(Collider collider){
		
		if (collider != avatar.collider) return;
		if (colour.Red)
			avatar.OnRedWell = true;
		
		manager.UpdateAction(Actions.Absorb, Trigger);		//this tells the manager that I want to do something. But I'll have to wait in line!
		
		
	}
	
	
	override public void Trigger(){
		
		
		
		
		/*****************************************************/
		avatar.TakeColour(colour);		//A remettre si sa fonctionne pas
		
		if (colour.Red){				//<--- A Dupliquer pour le Bleu t le Vert
			avatar.colour.b = 0;
			avatar.colour.g = 0;
		}
		if (colour.Blue){
			avatar.colour.r = 0;
			avatar.colour.g = 0;
		}
		Debug.Log("Should be setting colour");
	}
}
