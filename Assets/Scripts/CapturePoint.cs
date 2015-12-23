using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CapturePoint : MonoBehaviour {

    public int captureState = 0;
    public int resourceAmount;
    public bool isTower = false;
    public bool isBarracks = false;
    public Sprite captureIcon;

    public int ownership = -1;

    public Image dropZoneImage;
    public Renderer hexRenderer;

    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    public int Ownership {

        get { return ownership; }
        set {
            ownership = value;
            if (ownership == -1) {
                //dropZoneImage.color = neutralOwned;
                hexRenderer.material.SetColor("_Color", neutralOwned);
            }
            if (ownership == 0) {
                //dropZoneImage.color = playerOwned;
                hexRenderer.material.SetColor("_Color", playerOwned);
            }
            if (ownership == 1) {
                //dropZoneImage.color = enemyOwned;
                hexRenderer.material.SetColor("_Color", enemyOwned);
            }
        }
    }

    void Start() {
        if (dropZoneImage != null) {
            if (ownership == -1) {
                //dropZoneImage.color = neutralOwned;
                hexRenderer.material.SetColor("_Color", neutralOwned);
            }
            if (ownership == 0) {
                //dropZoneImage.color = playerOwned;
                hexRenderer.material.SetColor("_Color", playerOwned);
            }
            if (ownership == 1) {
                //dropZoneImage.color = enemyOwned;
                hexRenderer.material.SetColor("_Color", enemyOwned);
            }
        }
    }   

}
