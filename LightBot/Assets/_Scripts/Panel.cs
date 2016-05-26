using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


/** Esta clase representa un panel al que se le pueden incorporar las distintas
 * acciones a realizar. Se crea con el numero te celdas solicitadas.
 * Asimismo, tiene metodos para iluminar el elemento de alguna posicion. 
 * Este panel no guarda informacion de las acciones de cada casilla, solamente esta
 * para controlar la parte grafica */

public class Panel : MonoBehaviour {

	//materiales para cambiar el color de fondo
	public Material lightOff_mat;
	public Material lightOn_mat;


	public bool ______________________;


	// Id del panel.
	public int _id;

	// Construye el panel con el tamaño y el id pasados. Utiliza el prefab como celdas nulas.
	public void buildPanel(int id){
		this._id = id;
		clear ();
	}


	//Elimina todas las celdas del panel y las vuelve a crear nulas
	public void restartPanel(){
		this.buildPanel(this._id);
	}


	public List<Action> getActions(){
		List<Action> lst = new  List<Action> ();
		foreach (Transform child in this.gameObject.transform)
		{

			Action at = child.GetComponent<Action>();
			if(at != null)
				lst.Add(at);
		}
		return lst;
	}



	//Modifica el fondo de todas las celdas a apagado
	public void switchOffAllLIghts(){
		for(int i=0; i<this.transform.childCount; i++)
			this.SwitchLight(i, false);
	}

	//Modifica el fondo de una celda en concreto
	public void SwitchLight (int i, bool status){
		if (i >= this.transform.childCount)
			return;

		Color newmat = status ? Color.yellow : Color.black;

		this.transform.GetChild(i).GetComponent<Image>().color = newmat;
	}

	// Modifica todas las celdas para ponerlas a null
	public void clear(){
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

}

