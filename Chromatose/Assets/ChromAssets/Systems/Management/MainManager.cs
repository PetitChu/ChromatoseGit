using UnityEngine;
using System.Collections;

[SerializeAll]
public class MainManager : MonoBehaviour {
	
	
	public enum _WhiteRoomEnum{
		Tuto, ModuleBlanc_2, ModuleBlanc_3, ModuleBlanc_4, None
	}
	public enum _RoomTypeEnum{
		WhiteRoom, RedRoom, BlueRoom, RedAndBlueRoom, FinalBoss
	}
	
	public _WhiteRoomEnum _WhiteRoom;
	public _RoomTypeEnum _RoomType;
	
	
	private ChromatoseManager _Manager;
	private GameObject _Avatar;
	private Avatar _AvatarScript;
	private ChromatoseCamera _Cam;
	
	
	
	
	//VARIABLE STATISTIQUE
	private int _CurLevelUnlocked = 0;
	private float _TotalPlayingTime = 0;
	private int _TotalNPCDead = 0;
	
	private int _RedCollCollected = 0;
	private int _BlueCollCollected = 0;
	private int _WhiteCollCollected = 0;
	private int _ComicThumbsCollected = 0;
	
	private int _DeadCount = 0;
	
	
	
	//VARIABLE HIGHSCORE
	
	
	
	//ACCESSEUR GET/SET	
		
	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}
	
	// Use this for initialization
	void Start () {
	
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
