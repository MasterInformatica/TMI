using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelBoton : MonoBehaviour {
	public int numPanel = 0;
	public Transform controler;
	public Color color;
	void Start(){
		controler = this.transform.parent.parent;
	}
	public void onClick() {
		controler.GetComponent<PanelSwitch>().moveToPanel (numPanel);

	}

	public void lightBoton(bool on){
		Color c = on ? Color.yellow : color;
		gameObject.GetComponent<Image> ().color = c;
	}
}
