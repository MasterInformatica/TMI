using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelSwitch : MonoBehaviour {
	public int currentPanel;
	public GameObject[] PanelList;
	public GameObject[] btns;
	// Use this for initialization
	void Start () {
		currentPanel = 0;
	}
	

	public void moveToPanel(int numPanel){
		GameObject go;
		if (numPanel < PanelList.GetLength(0) && numPanel != currentPanel) {
			btns[currentPanel].GetComponent<PanelBoton>().lightBoton(false);
			go = PanelList[currentPanel];
			go.SetActive(false);
			go = PanelList[numPanel];
			btns[numPanel].GetComponent<PanelBoton>().lightBoton(true);
			go.SetActive(true);
			currentPanel = numPanel;
		}
	}
}
