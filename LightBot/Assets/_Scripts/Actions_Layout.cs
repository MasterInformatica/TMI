using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * Script que se encarga de la interfaz de la parte derecha.
 * Comandos, paneles y demas
 */
public class Actions_Layout : MonoBehaviour {
	public static Actions_Layout S; //Singleton para poder acceder

	public GameObject play_btn; 

	//Sprites para el boton de play/stop
	public Sprite play_Sprite;
	public Sprite stop_Sprite;
	public bool playStatus; //Estamos reproduciendo o grabando ordenes?

	
	public bool _________________________;

	public List<Panel> paneles; //lista de los paneles segun se van creando
	
	public void Awake(){
		S = this;
	}

	public void Start(){
		this.playStatus = true;
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
	public List< List<Action> > getActions(){
		List< List<Action> > lst = new List< List<Action> > ();
		foreach (Panel p in paneles) {
			lst.Add(p.getActions());
		}
		return lst;
	}

	//Resetea toda la interfaz. Basta con resetear todos los paneles
	public void restartLayout(){
		foreach (Panel p in this.paneles){
			p.restartPanel(); 
		}
	}


	// Cambia el boton de play por el de stop y viceversa
	public void changeButton(){
		this.play_btn.GetComponent<Image> ().overrideSprite = (this.playStatus) ? this.stop_Sprite : this.play_Sprite;
		//this.play_btn.GetComponent<Button> ().targetGraphic = 
		Debug.Log (" TODO change play button.");
		/*SpriteRenderer sprr = (SpriteRenderer)(GameObject.Find("Action_play").GetComponentInChildren<SpriteRenderer> ());

		sprr.sprite = (this.playStatus) ? this.stop_Sprite : this.play_Sprite;*/
		this.playStatus = ! this.playStatus;
	}
}
