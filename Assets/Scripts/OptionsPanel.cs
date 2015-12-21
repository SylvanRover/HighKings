using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour {

	public FilterSystem filter;

	public Text pveMin;
	public Text pveMax;
	public Text pvpMin;
	public Text pvpMax;
	public Slider pveMinSlider;
	public Slider pveMaxSlider;
	public Slider pvpMinSlider;
	public Slider pvpMaxSlider;

	public bool pveShowAll = false;
	public bool pvpShowAll = false;
	private int PrePveMin;
	private int PrePveMax;
	private int PrePvpMin;
	private int PrePvpMax;

	public void ShowAllPve(bool show){
		if (show) {
			pveShowAll = show;
			PrePveMin = filter.currentPveMin;
			PrePveMax = filter.currentPveMax;
		}
		pveShowAll = show;
		pveMinSlider.interactable = !show;
		pveMaxSlider.interactable = !show;
	}
	public void ShowAllPvp(bool show){
		if (show) {
			pvpShowAll = show;
			PrePvpMin = filter.currentPveMin;
			PrePvpMax = filter.currentPvpMax;
		}
		pvpShowAll = show;
		pvpMinSlider.interactable = !show;
		pvpMaxSlider.interactable = !show;
	}

	void Update(){

		// Show All
		if (pveShowAll) {	
			pveMin.text = ("PvE Min: " + 1f);
			pveMax.text = ("PvE Max: " + filter.maxPve);
		} else {
			pveMin.text = ("PvE Range - " + filter.currentPveMin);
			pveMax.text = ("PvE Range + " + filter.currentPveMax);	
			pveMinSlider.maxValue = filter.filtermax;
			pveMinSlider.value = filter.currentPveMin;
			pveMaxSlider.maxValue = filter.filtermax;
			pveMaxSlider.value = filter.currentPveMax;
		}

		if (pvpShowAll) {	
			pvpMin.text = ("PvP Min: " + 1f);
			pvpMax.text = ("PvP Max: " + filter.maxPvp);
		} else {	
			pvpMin.text = ("PvP Range - " + filter.currentPvpMin);
			pvpMax.text = ("PvP Range + " + filter.currentPvpMax);
			pvpMinSlider.maxValue = filter.filtermax;
			pvpMinSlider.value = filter.currentPvpMin;
			pvpMaxSlider.maxValue = filter.filtermax;
			pvpMaxSlider.value = filter.currentPvpMax;
		}
	}

}
