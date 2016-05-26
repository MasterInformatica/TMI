using UnityEngine;
using System.Collections;

/**
 * Clase estatica que almacena el estado actual de los niveles, asi como del nivel actual
 **/
public static class levelLoad{
	public static int level = -1;
	public static bool[] levelPassed = {false, false, false, false,
		false, false, false, false};
	public static int music = -1;
}


/**
 * Listener de los botones del menu. Carga el menu correspondiente
 **/
public class Menu : MonoBehaviour {

	public int level;
	public Material bck_green;
	public Material bck_orange;
	public Material bck_white;
	public int music;

	private GameObject[] music_selectors;


	//Comprobamos si el nivel se ha superado y lo pintamos de verde
	public void Start(){
		//cambiar el material del fondo
		MeshRenderer rmat = this.transform.parent.GetComponentInChildren<MeshRenderer>();
		if(this.level > 0 && levelLoad.levelPassed[this.level-1]){
			rmat.material = this.bck_green;	
		}

		// Music selector.
		if (music != 0) {
			music_selectors = GameObject.FindGameObjectsWithTag ("music_selection");

			if(levelLoad.music == -1)
				levelLoad.music = 1;
		
			if (levelLoad.music == music) {
				rmat.material = this.bck_orange;
			}

			if (levelLoad.music != music) {
				rmat.material = this.bck_white;
			}
		}
	}


	public void OnMouseUpAsButton(){
		if (this.music != 0) {
			levelLoad.music = this.music;

			MeshRenderer rmat;
			foreach (GameObject music_selector in music_selectors){
				rmat = music_selector.GetComponentInChildren<MeshRenderer>();
				rmat.material = this.bck_white;
			}

			rmat = this.transform.parent.GetComponentInChildren<MeshRenderer>();
			rmat.material = this.bck_orange;

			return;
		}

		if(this.level==0){ //cargamos el menu
			Application.LoadLevel("_Scene_menu");
		}
		else {//caso contrario cargamos el nivel correspondiente
			levelLoad.level = this.level;
			Application.LoadLevel("_Scene_0");
		}
	}
}
