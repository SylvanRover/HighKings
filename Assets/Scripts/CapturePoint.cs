using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CapturePoint : MonoBehaviour {

    public int captureState = 0;
    public int resourceAmount;
    public bool isTower = false;
    public bool isBarracks = false;
    //public Sprite captureIcon;

    public int ownership = -1;

    public Image dropZoneImage;
    private Renderer hexRenderer;

    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    public int Ownership {

        get { return ownership; }
        set {
            ownership = value;
            if (ownership == -1) {
                for (int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", neutralOwned);
                }
                //hexRenderer.material.SetColor("_Color", neutralOwned);
            }
            if (ownership == 0) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", playerOwned);
                }
                //hexRenderer.material.SetColor("_Color", playerOwned);
            }
            if (ownership == 1) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", enemyOwned);
                }
                //hexRenderer.material.SetColor("_Color", enemyOwned);
            }
        }
    }

    void Start() {
        Ownership = ownership;
    }   

}
