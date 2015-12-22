using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CapturePoint : MonoBehaviour {

    public int captureState = 0;
    public int resourceAmount;
    public bool isTower = false;
    public bool isBarracks = false;
    public Sprite captureIcon;

    public int ownership = 0;

    public Image dropZoneImage;

    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    public int Ownership {

        get { return ownership; }
        set {
            ownership = value;
            if (ownership == 0) {
                dropZoneImage.color = neutralOwned;
            }
            if (ownership == 1) {
                dropZoneImage.color = playerOwned;
            }
            if (ownership == 2) {
                dropZoneImage.color = enemyOwned;
            }
        }
    }

    void Start() {
        if (dropZoneImage != null) {
            if (ownership == 0) {
                dropZoneImage.color = neutralOwned;
            }
            if (ownership == 1) {
                dropZoneImage.color = playerOwned;
            }
            if (ownership == 2) {
                dropZoneImage.color = enemyOwned;
            }
        }
    }   

}
