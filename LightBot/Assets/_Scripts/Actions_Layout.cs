using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Script que se encarga de la interfaz de la parte derecha.
 * Comandos, paneles y demas
 */
public class Actions_Layout : MonoBehaviour {
	public static Actions_Layout S; //Singleton para poder acceder

	public GameObject panelPrefab; //Prefab para construir los paneles de comandos

	// Esta pareja relaciona el sprite con el tipo de accion. Sirve para modificar los paneles de ordenes
	public List<Sprite> arrows_sprite; 
	public List<ActionType> actions_type; 

	//Sprites para el boton de play/stop
	public Sprite play_Sprite;
	public Sprite stop_Sprite;
	public bool playStatus; //Estamos reproduciendo o grabando ordenes?

	
	public bool _________________________;


	public float panelOffset = -3.0f; //ofset entre paneles para colocarlos

	public List<Panel> paneles; //lista de los paneles segun se van creando
	



	public void Awake(){
		S = this;
	}



	public void Start(){
		this.playStatus = true;
		this.createPanels(3); //en un principio solo creamos 3 paneles

		this.paneles[0].activatePanel(); //El primer panel es el activo
	}


	// Crea num paneles a partir del prefab y los posiciona segun el offset. 
	// Guarda los paneles en el atributo para poder acceder
	public void createPanels(int num){
		for (int i=0; i<num; i++) {
			GameObject go = Instantiate (this.panelPrefab);
			Panel p = go.GetComponent<Panel> ();

			p.buildPanel(12, i);
			p.transform.localPosition = new Vector3(p.transform.position.x,
			                                     p.transform.position.y + i*this.panelOffset,
			                                     p.transform.position.z);
			this.paneles.Add (p);
		}

	}

	// Dada una accion pasada por parametro (at), la inserta en el
	// panel numero n
	public void insertInPanel(int n, ActionType at){
		if (n > this.paneles.Count) //puede que no haya paneles 
			return;


		//Buscamos la figura a dibujar
		int i = 0;
		for (i=0; i<this.actions_type.Count; i++)
			if (this.actions_type [i] == at)
				break;


		//Comprobacion por si las moscas
		if (i > this.arrows_sprite.Count)
			return;


		//insertamos en el panel
		this.paneles[n].insertCell(this.arrows_sprite[i]);
	}


	// Por cada panel, elimina los colores del fondo (estado activo)
	public void switchOffAllLights(){
		foreach(Panel p in this.paneles){
			p.switchOffAllLIghts();

		}
	}


	// Dado el numero de panel, ilumina o apaga (status) la casilla numero i
	public void lightCube(int panel, int i, bool status){
		if (this.paneles.Count <= panel)
			return;

		this.paneles [panel].SwitchLight (i, status);
	}


	//Resetea todos los paneles
	public void clearPanels(){
		foreach (Panel p in this.paneles) {
			p.clear ();
		}
	}


	//Resetea toda la interfaz. Basta con resetear todos los paneles
	public void restartLayout(){
		foreach (Panel p in this.paneles){
			p.restartPanel();
		}
	}


	// Cambia el boton de play por el de stop y viceversa
	public void changeButton(){
		SpriteRenderer sprr = (SpriteRenderer)(GameObject.Find("Action_play").GetComponentInChildren<SpriteRenderer> ());

		sprr.sprite = (this.playStatus) ? this.stop_Sprite : this.play_Sprite;
		this.playStatus = ! this.playStatus;
	}
}
