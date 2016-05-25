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
	public int music;


	//Comprobamos si el nivel se ha superado y lo pintamos de verde
	public void Start(){
		//cambiar el material del fondo
		if(this.level > 0 && levelLoad.levelPassed[this.level-1]){
			MeshRenderer rmat;
			rmat = this.transform.parent.GetComponentInChildren<MeshRenderer>();
			
			rmat.material = this.bck_green;	
		}
	}


	public void OnMouseUpAsButton(){
		if (this.music != 0) {
			levelLoad.music = this.music;
			return;
		}

		if(this.level==0){ //cargamos el menu
			levelLoad.music = 1; // Reiniciamos la cancion selecionada.
			Application.LoadLevel("_Scene_menu");
		}
		else {//caso contrario cargamos el nivel correspondiente
			levelLoad.level = this.level;
			if(levelLoad.music == 0)
				levelLoad.music = 1;

			Application.LoadLevel("_Scene_0");
		}
	}
}
