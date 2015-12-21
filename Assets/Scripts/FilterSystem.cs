using UnityEngine;
using System.Collections;

public class FilterSystem : MonoBehaviour {

	public CommanderMGMT cMGMT;
	
	public int defaultPveMin = 2;
	public int defaultPveMax = 2;
	public int defaultPvpMin = 2;
	public int defaultPvpMax = 2;
	public int currentPveMin = 2;
	public int currentPveMax = 2;
	public int currentPvpMin = 2;
	public int currentPvpMax = 2;
	public int filtermax = 10;
	public int minPve = 1;
	public int maxPve = 34;
	public int minPvp = 1;
	public int maxPvp = 34;
	public int minPveGlobal = 1;
	public int maxPveGlobal = 34;
	public int minPvpGlobal = 1;
	public int maxPvpGlobal = 34;
	public bool pveShowAll = false;
	public bool pvpShowAll = false;
	public bool globalFilter = false;
	public bool ghostFilter = true;

	public OptionsPanel options;
		
	public void GlobalFilter(bool On){
		globalFilter = On;
	}

	public void GhostFilter(bool On){
		ghostFilter = On;
	}

	public void AdjustPveMin(float value){
		currentPveMin = (int) value;
	}
	public void AdjustPveMax(float value){
		currentPveMax = (int) value;
	}
	public void AdjustPvpMin(float value){
		currentPvpMin = (int) value;
	}
	public void AdjustPvpMax(float value){
		currentPvpMax = (int) value;
	}

	public void ResetPveDefault(){
		currentPveMin = defaultPveMin;
		currentPveMax = defaultPveMax;
	}
	public void ResetPvpDefault(){
		currentPvpMin = defaultPvpMin;
		currentPvpMax = defaultPvpMax;
	}

	void Start(){
		cMGMT = GetComponent<CommanderMGMT> ();
		
		minPveGlobal = cMGMT.cLvlGlobLow;
		maxPveGlobal = cMGMT.cLvlGlobHigh;;
		minPvpGlobal = cMGMT.cLvlGlobLow;;
		maxPvpGlobal = cMGMT.cLvlGlobHigh;;
	}

	void Update(){
		pveShowAll = options.pveShowAll;
		pvpShowAll = options.pvpShowAll;
	}
}
