using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/** Esta clase representa un panel al que se le pueden incorporar las distintas
 * acciones a realizar. Se crea con el numero te celdas solicitadas.
 * Asimismo, tiene metodos para iluminar el elemento de alguna posicion. 
 * Este panel no guarda informacion de las acciones de cada casilla, solamente esta
 * para controlar la parte grafica */

public class Panel : MonoBehaviour {

	public int numColumns;//tamaño del panel

	public GameObject celdaPrefab;//Prefab para construir el panel

	public Sprite nullSprite;	//Sprite nulo

	
	public float leftCorner = -2.5f; //Coordenadas para saber donde empezar a colocar


	//materiales para cambiar el color de fondo
	public Material lightOff_mat;
	public Material lightOn_mat;


	public bool ______________________;


	public List<GameObject> celdas;

	// Id del panel.
	public int _id;

	//indica cuantas celdas validas hay para insertar la siguiente
	public int numRellenos;


	// Construye el panel con el tamaño y el id pasados. Utiliza el prefab como celdas nulas.
	public void buildPanel(int size, int id){
		this._id = id;

		GameObject go;
		for (int i=0; i < size; i++) {
			go = Instantiate (this.celdaPrefab);
			go.transform.parent = this.transform;

			go.transform.localPosition = new Vector3 (this.leftCorner + (i % this.numColumns),
			                                     -1 * (i / this.numColumns), 0);

			this.celdas.Add (go);
		}

		this.numRellenos = 0;
	}


	//Elimina todas las celdas del panel y las vuelve a crear nulas
	public void restartPanel(){
		this.numRellenos = 0;
		int tam = this.celdas.Count;
		foreach (GameObject g in this.celdas)
			Destroy(g);

		this.celdas = new List<GameObject>();
		this.buildPanel(tam, this._id);
	}


	//Modifica la celda actual por el sprite pasado por parametro
	public void insertCell(Sprite spr){
		if (this.numRellenos >= this.celdas.Count)
			return;

		SpriteRenderer sprr = this.celdas [numRellenos++].GetComponentInChildren<SpriteRenderer> ();
		sprr.sprite = spr;
	}


	//Modifica el fondo de todas las celdas a apagado
	public void switchOffAllLIghts(){
		for(int i=0; i<this.celdas.Count; i++)
			this.SwitchLight(i, false);
	}

	//Modifica el fondo de una celda en concreto
	public void SwitchLight (int i, bool status){
		if (i >= this.celdas.Count)
			return;

		Material newmat = status ? this.lightOn_mat : this.lightOff_mat;

		MeshRenderer rmat;
		rmat = this.celdas[i].GetComponentInChildren<MeshRenderer>();

		rmat.material = newmat;				

	}

	// Modifica todas las celdas para ponerlas a null
	public void clear(){
		SpriteRenderer sr;

		for(int i=0; i<this.celdas.Count; i++){
			sr = this.celdas[i].GetComponentInChildren<SpriteRenderer>();	

			sr.sprite = this.nullSprite;
		}

		this.numRellenos = 0;
	}


	//Colorea todas las celdas del panel con el fondo activo
	public void activatePanel(){
		//coloreamos a todas las celdas para que esten activas
		for(int i = 0; i<this.celdas.Count; i++)
			this.SwitchLight(i, true);

		//avisamos al director
		Director.S.panelActive(this._id);
	}


	//Colorea todas las celdas del panel con el fondo desactivado
	public void deactivatePanel(){
		//borramos el color de todas las celdas
		for(int i = 0; i<this.celdas.Count; i++)
			this.SwitchLight(i, false);
	}
}
