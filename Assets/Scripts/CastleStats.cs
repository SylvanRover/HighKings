using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CastleStats : MonoBehaviour {

    public Image healthbarImage;
    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    // Unit Variables
    public int castleID;
    public int ownership = 0;
    public float healthMax;
    public float healthCurrent;
    public float damage;
    public float attackRange;
    public float armourPoints;
    public float lineOfSight;

    public int Ownership {

        get { return ownership; }
        set {
            ownership = value;
            if (ownership == 0) {
                healthbarImage.color = neutralOwned;
            }
            if (ownership == 1) {
                healthbarImage.color = playerOwned;
            }
            if (ownership == 2) {
                healthbarImage.color = enemyOwned;
            }
        }
    }

    public Text castleNameText;

    public RectTransform healthRect;
    public RectTransform damageRect;
    public GameObject healthbar;
    public float healthbarWidth = 54;
    public float healthbarFadeTime = 3;
    public Animator anim;
    public float wait = 0.5f;
    public Vector2 currentSize;
    private Vector2 endSize;
    public Vector2 resetSize;
    public float currentTime = 0f;
    public float damageDuration = 0.4f;
    public float damageDurationWait = 0.5f;
    public bool animateDamage = false;

    private float startTime;

    //Event Trigger nn Click but not after Drag
    public bool onUp = false;
    public bool onDrag = false;

    public void OnDragEnd() {
        onDrag = true;
    }

    public void OnPointerUp() {
        if (!onDrag) {
            onUp = true;
        }
    }

    //Healbar
    public IEnumerator HealthbarFade() {
        anim = healthbar.GetComponent<Animator>();
        yield return new WaitForSeconds(healthbarFadeTime);
        anim.SetBool("On", false);
    }

    IEnumerator AnimateDamage() {
        currentSize = damageRect.sizeDelta;
        yield return new WaitForSeconds(damageDurationWait);
        animateDamage = true;
    }

    public void Damage(float value) {
        anim = healthbar.GetComponent<Animator>();
        anim.SetBool("On", true);
        healthCurrent -= value;
        //endSize = new Vector2 ((healthCurrent/healthMax) * healthbarWidth, health.sizeDelta.y);
        healthRect.sizeDelta = new Vector2((healthCurrent / healthMax) * healthbarWidth, healthRect.sizeDelta.y);
        StopAllCoroutines();
        StartCoroutine(AnimateDamage());
        StartCoroutine(HealthbarFade());
    }

    void Start() {
        // Setting Health
        healthCurrent = healthMax;
        resetSize = healthRect.sizeDelta;
        anim = healthbar.GetComponent<Animator>();

        SetCastleType();

        if (healthbarImage != null) {
            if (ownership == 0) {
                healthbarImage.color = neutralOwned;
            }
            if (ownership == 1) {
                healthbarImage.color = playerOwned;
            }
            if (ownership == 2) {
                healthbarImage.color = enemyOwned;
            }
        }
    }

    public void SetCastleType() {
        // Setting Castle Variables
        if (castleID == 0) {
            healthMax = 10;
            healthCurrent = 10;
            damage = 1;
            attackRange = 3;
            armourPoints = 0;
            lineOfSight = 4;
        }
    }

    void Update() {

        if (animateDamage) {
            if (currentTime <= damageDuration) {
                currentTime += Time.deltaTime;
                damageRect.sizeDelta = Vector2.Lerp(currentSize, healthRect.sizeDelta, currentTime / damageDuration);
            }
            else {
                //damage.sizeDelta.y = currentSize;
                currentTime = 0f;
            }
            if (damageRect.sizeDelta == healthRect.sizeDelta) {
                animateDamage = false;
            }
        }
        
    }
}