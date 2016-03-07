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


	
	public void OnMouseUpAsButton()
	{
		switch (this.actionType) {
		case ActionType.play:
			Director.S.play();
			break;
		case ActionType.trash:
			Director.S.trash();
			break;
		case ActionType.none: //si pulsamos en una accion vacia, es que estamos en el panel
			Panel p = transform.parent.transform.parent.GetComponent<Panel>(); 
			p.activatePanel();
			break;
		default: //acciones para el robot
			Director.S.addAction (this.actionType);
			break;
		}
	}
}
