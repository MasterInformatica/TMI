using UnityEngine;
using System.Collections;


/**
 * Posibles tipos de comandos
 */
public enum ActionType{
	up,
	right,
	left,
	jump,
	play,
	restart,
	light,
	trash,
	pause,
	p1,
	p2,
	none
}

/**
 * Listener de las pulsadciones de los botones. Segun el comando a ejecutar
 * invoca a un metodo u otro 
 */
public class Action : MonoBehaviour {
	//Tipo de accion que representa el boton
	public ActionType actionType;

	public void OnClick(){
		switch (this.actionType) {
		case ActionType.play:
			Director.S.play ();
			break;
		case ActionType.restart:
			Director.S.restartMusic (); 
			break;
		case ActionType.light:
			changeColor();
			break;
		}
	}

	public void changeColor(){
		Debug.Log ("TODO: change color");
	}

	private int color = 100;

}
