using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelSwitch : MonoBehaviour {
	public int currentPanel;
	public GameObject[] PanelList;
	// Use this for initialization
	void Start () {
		currentPanel = 0;
	}
	

	public void moveToPanel(int numPanel){
		GameObject go;
		if (numPanel < PanelList.GetLength(0)) {
			go = PanelList[currentPanel];
			go.SetActive(false);//.GetComponent<RectTransform>().enabled = false;
			go = PanelList[numPanel];
			go.SetActive(true);//.GetComponent<RectTransform>().enabled = true;
			currentPanel = numPanel;
		}
	}
}
