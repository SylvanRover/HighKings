using UnityEngine;
using System.Collections;

public class EnemyCampCollision : MonoBehaviour {
		
	public MakePrefabAppear p;
	public MakePrefabAppear f;
	private Transform spawnedArmyPoint;
	private Transform spawnedArmy;
	public Animator anim;
	public SelectionRingAnim selectionRing;
	private Animator pAnim;
	public float wait = 2f;
	public float campReset = 30f;
		
	public bool battleVictory = false;

	public MakePrefabAppear spawnVFX;
	public TileStats tileStats;
	public BusyCommander busyEnemy;

	IEnumerator CleanUp() {
		yield return new WaitForSeconds (5);
		p.Clear();
	}

	IEnumerator VictoryReset() {
		yield return new WaitForSeconds (wait);
		p.Clear();
		busyEnemy.busyEnemy = false;
	}

	IEnumerator LossReset() {
		yield return new WaitForSeconds (wait);
		if (tileStats.tileType == "PvP") {
			busyEnemy.busyEnemyText = "Shielded";
		}
		if (tileStats.tileType == "PvE") {
			busyEnemy.busyEnemyText = "Rebuilding";
		}
		spawnVFX.Appear ();
		anim.SetBool ("Destroyed", true);
		p.Clear();
		selectionRing.Deselect ();
		selectionRing.Normal ();

		pAnim.SetBool ("Is_Dead", false);
		yield return new WaitForSeconds (campReset);
		anim.SetBool ("Destroyed", false);
		busyEnemy.busyEnemy = false;
		busyEnemy.busyEnemyText = "Attack";
	}

	public void BattleLoss(){
		spawnedArmyPoint = transform.GetChild (0);
		spawnedArmy = spawnedArmyPoint.transform.GetChild (0);
		pAnim = spawnedArmy.GetComponentInChildren<Animator> ();
		pAnim.SetBool("Is_Attacking", false);
		pAnim.SetBool ("Is_Dead", true);
		StartCoroutine(LossReset());
	}

	public void BattleVictory(){
		spawnedArmyPoint = transform.GetChild (0);
		spawnedArmy = spawnedArmyPoint.transform.GetChild (0);
		pAnim = spawnedArmy.GetComponentInChildren<Animator> ();
		pAnim.SetBool("Is_Attacking", false);
		pAnim.SetTrigger("Is_Victorious");
		StartCoroutine(VictoryReset());
	}

	void Start() {
		anim = GetComponent<Animator> ();
		spawnedArmyPoint = transform.GetChild (0);
		tileStats = GetComponent<TileStats> ();
		//spawnedArmy = spawnedArmyPoint.transform.GetChild (0);
	}
	
	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Player") {
			if (tileStats.tileType == "PvP" || tileStats.tileType == "PvE"){
				p.Appear(col.gameObject.transform.position);
				spawnedArmyPoint = transform.GetChild (0);
				spawnedArmy = spawnedArmyPoint.transform.GetChild (0);
				pAnim = spawnedArmy.GetComponentInChildren<Animator> ();
				pAnim.SetBool("Is_Attacking", true);
				f.Clear();
			}
		}
		if (tileStats.tileType == "Resource") {
			f.Clear();
		}
	}
	void OnCollisionExit(Collision col) {
		if (col.gameObject.tag == "Player" && gameObject.tag != "Resource") {
			gameObject.tag = "Untagged";
			StartCoroutine (CleanUp ());
		}
		if (col.gameObject.tag == "Player" && gameObject.tag == "Resource") {
		}
	}
}
