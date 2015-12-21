using UnityEngine;
using System.Collections;

public class ArmyCollision : MonoBehaviour {

	public Transform child;
	public Transform child2;
	public DestroyHero childObj;
	public bool childActivated = false;
	public Animator anim;
	public ProgressLine armyLine;
	public float wait = 1f;
	public float recoveryTime = 1f;

	private float spawnFixTime = 0.1f;

	public float battleAdvantage = 1f;
	public float battleHandicap = 1f;
	public bool battleVictory = false;
	public bool randomVictory = true;
	private EnemyCampCollision opponent;

	public ArmyStats armyStats;
	private Transform opponentChild;
	private Transform opponentChild2;
	private ArmyStats opponentStats;
	private bool collided = false;

	public float minDamage = 5f;
	public float maxDamage = 20f;
	public float opponentMinDamage = 5f;
	public float opponentMaxDamage = 20f;
	
	public float battleTime = 6f;
	public float attackTime = 1f;
	public float afterBattleTime = 5;
	public bool battleTiming = false;
	public float battleCountdown = 0;
	
	private ResourceHarvest colResource;
	public float harvestTime = 30f;
	public bool harvestTiming = false;
	public float harvestCountdown = 0;
	
	void BattleTimer(float time){
		battleTiming = true;
		battleCountdown = time;
	}

	void HarvestTimer(float time){
		harvestTiming = true;
		harvestCountdown = time;
	}
	
	IEnumerator ArmyStopWait() {
		yield return new WaitForSeconds (wait);
		armyLine.ClearPath();
		armyLine._go = false;
	}	

	// Battling
	IEnumerator BattleLength() {
		BattleTimer (battleTime);
		armyStats.actionMoving = false;
		armyStats.actionBattleAttacking = true;
		yield return new WaitForSeconds (spawnFixTime);
		opponentChild2 = opponentChild.transform.GetChild (0);
		opponentStats = opponentChild2.gameObject.GetComponent<ArmyStats> ();
		// Damage Being applied
		yield return new WaitForSeconds (attackTime);
		armyStats.Damage(Random.Range(opponentMinDamage*battleHandicap,opponentMaxDamage*battleHandicap));
		opponentStats.Damage(Random.Range(minDamage*battleAdvantage,maxDamage*battleAdvantage));
		yield return new WaitForSeconds (attackTime);
		armyStats.Damage(Random.Range(opponentMinDamage*battleHandicap,opponentMaxDamage*battleHandicap));
		opponentStats.Damage(Random.Range(minDamage*battleAdvantage,maxDamage*battleAdvantage));
		yield return new WaitForSeconds (attackTime);
		armyStats.Damage(Random.Range(opponentMinDamage*battleHandicap,opponentMaxDamage*battleHandicap));
		opponentStats.Damage(Random.Range(minDamage*battleAdvantage,maxDamage*battleAdvantage));
		yield return new WaitForSeconds (attackTime);
		armyStats.Damage(Random.Range(opponentMinDamage*battleHandicap,opponentMaxDamage*battleHandicap));
		opponentStats.Damage(Random.Range(minDamage*battleAdvantage,maxDamage*battleAdvantage));
		yield return new WaitForSeconds (attackTime);
		armyStats.Damage(Random.Range(opponentMinDamage*battleHandicap,opponentMaxDamage*battleHandicap));
		opponentStats.Damage (Random.Range (minDamage * battleAdvantage, maxDamage * battleAdvantage));
		yield return new WaitForSeconds (attackTime/2);

		// If nobody dies, the one with most health wins
		if (armyStats.healthCurrent > opponentStats.healthCurrent){
			battleVictory = true;
		} else {
			battleVictory = false;
		}

		if (battleVictory) {
			armyLine.IsAttacking = false;
			anim.SetBool("Is_Attacking", false);
			anim.SetTrigger("Is_Victorious");
			opponent.BattleLoss();
			yield return new WaitForSeconds (afterBattleTime);
			armyLine.AllowLineEditing ();
			armyLine.PathHome();
			armyStats.actionBattleAttacking = false;
			armyStats.actionMoving = true;
		} else {
			armyLine.IsAttacking = false;
			anim.SetBool("Is_Attacking", false);
			anim.SetBool("Is_Dead", true);
			anim.SetBool("Is_Injured", true);
			opponent.BattleVictory();			
			yield return new WaitForSeconds (recoveryTime);
			anim.SetTrigger("Is_Recovering");
			yield return new WaitForSeconds (afterBattleTime);
			armyLine.AllowLineEditing ();
			armyLine.PathHome();
			anim.SetBool("Is_Dead", false);
			armyStats.actionBattleAttacking = false;
			armyStats.actionMoving = true;
		}
	}

	// Harvesting
	IEnumerator HarvestLength(){
		HarvestTimer (harvestTime);
		yield return new WaitForSeconds (harvestTime);
		armyLine.IsAttacking = false;
		armyLine.AllowLineEditing ();
		armyLine.PathHome();
		armyStats.actionHarvesting = false;
		armyStats.actionMoving = true;
	}

	public void DetermineBattle(){
		if (randomVictory) {
			battleVictory = Random.value < 0.5;
		}
	}

	void Start() {
		child = transform.GetChild (0);
		//child2 = child.transform.GetChild (0);
		//anim = child2.GetComponent<Animator> ();
		//childActivated = armyStats.commanderIsActive;
	}

	public void spawnedChild(){
		child2 = child.transform.GetChild (0);
		if (childObj == null) {
			childObj = child2.GetComponent<DestroyHero> ();
		}
		if (anim == null) {
			anim = child2.GetComponent<Animator> ();
		}
		childActivated = true;
		armyStats.selectionRing.Pressed ();
		armyStats.selectionRing.Selected ();
		armyStats.actionIdle = false;
		armyStats.actionMoving = true;
	}

	void OnCollisionEnter(Collision col) {
		// PvE and PvP
		if (col.gameObject.tag == "Enemy" && armyLine.IsAttacking) {
			armyLine.ClearPath();
			if (anim != null){
				anim.SetBool ("Is_Attacking", true);
			}
			opponent = col.gameObject.GetComponent<EnemyCampCollision>();
			opponentChild = col.transform.GetChild (0);
			StartCoroutine(ArmyStopWait());
			StartCoroutine(BattleLength());
			collided = true;
		}

		// Home
		if (col.gameObject.tag == "Home" && !armyLine.IsAttacking && armyStats.commanderIsActive) {
			armyStats.actionMoving = false;
			armyStats.actionIdle = true;
			if (anim != null){
				anim.SetBool ("Is_Injured", false);
			}
			StartCoroutine(ArmyStopWait());
			armyStats.anim.SetBool("On", true);
			StartCoroutine(armyStats.HealthbarFade());
			armyStats.healthCurrent = armyStats.healthMax;
			armyStats.health.sizeDelta = armyStats.resetSize;
			armyStats.damage.sizeDelta = armyStats.resetSize;
			armyStats.commanderIsActive = false;
			armyStats.selectionRing.Deselect();
			armyStats.selectionRing.Normal();
			childActivated = false;
			child.transform.rotation = Quaternion.identity;

			//Debug.LogError("Destroying " + childObj);
			childObj.DestroyWhenHome();
		}

		// Resource Tile
		if (col.gameObject.tag == "Resource" && armyLine.IsAttacking) {
			colResource = col.transform.GetComponent<ResourceHarvest>();
			harvestCountdown = colResource.harvestCountdown;
			harvestTime = colResource.harvestTime;
			armyStats.actionMoving = false;
			armyStats.actionHarvesting = true;
			StartCoroutine(ArmyStopWait());
			collided = true;
			//childObj.DestroyWhenHome();
			StartCoroutine(HarvestLength());
		}
	}

	void Update () {
		if (anim != null) {
			anim.SetFloat ("ArmySpeed", armyLine.GetCurrentVelocity ());
		}
		if (collided) {
			if (opponentStats != null && opponentStats.healthCurrent <= 0) {
				battleVictory = true;
			}
		}
		if (armyStats.healthCurrent <= 0) {
			battleVictory = false;
		}

		// Battle Timer	
		
		if (battleTiming) {
			battleCountdown -= Time.deltaTime;
			if (battleCountdown<= 0){
				
				battleTiming = false;
			}
		}
		if (harvestTiming) {
			harvestCountdown -= Time.deltaTime;
			if (harvestCountdown<= 0){
				
				harvestTiming = false;
			}
		}
	}
}
