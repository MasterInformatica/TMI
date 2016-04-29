using UnityEngine;
using System.Collections;

public class PanelBoton : MonoBehaviour {
	public int numPanel = 0;
	public Transform controler;
	void Start(){
		controler = this.transform.parent.parent;
	}
	public void onClick() {
		Debug.Log ("Click " + numPanel); 
		controler.GetComponent<PanelSwitch>().moveToPanel (numPanel);
	}
}
