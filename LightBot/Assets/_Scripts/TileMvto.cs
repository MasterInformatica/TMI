using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum tipoMovimiento{
	horizontal, 
	vertical,
	frente, 
	none
}



/**
 * Esta clase permite realizar movimientos a partir de llamadas startMvto(duracion).
 *  Si se realiza una llamada startMvto pero no ha finalizado el mvto anterior (duracion) ignora la llamada.
 *  Si finaliza un movimiento, espera a una llamada startMvto para comenzar el siguiente.
 *  Se puede tener en cuenta que haya un ciclo de espera entre movimiento y movimiento.
 **/

public class TileMvto : MonoBehaviour {
	//Determina si hay que esperar entre un movimiento o no.
	public bool esperaEntreMovimientos = true;

	public bool ______________________________;

	public float TAM; //Tamaño a mover

	//Relacionado con el cálculo del movimiento
	public float mvtoStart;
	public float mvtoStop;
	Vector3 poi;
	Vector3 originalPosition;

	// Relacionado con el movimiento actual
	public float eps = 0.1f;
	bool isMoving;

	// Estados que definen el movimiento de la casilla
	public List<Vector3> estados;
	int estadosIdx;




	void Awake () {
		estadosIdx = 0;

		estados = new List<Vector3>();
		originalPosition = transform.position;

	}



	/** Inicializa el script para moverse según tm en el numero de pasos indicado */
	public void initMvto(tipoMovimiento tm, int pasos){
		Vector3 incr = Vector3.zero;

		switch(tm){
		case tipoMovimiento.horizontal:
			incr = new Vector3(TAM, 0, 0);
			break;
		case tipoMovimiento.vertical:
			incr = new Vector3(0, TAM, 0);
			break;
		case tipoMovimiento.frente:
			incr = new Vector3(0, 0, TAM);
			break;
		}

		//Movimiento directo con pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(incr);
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0,0,0));
		}

		//Movimiento reverso con pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(-1*incr);
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0,0,0));
		}

		//REGISTRO LISTENER DE LOS BEATS
		MusicManager.S.registerTileListener(this.startMvto);
	}
		



	public void startMvto(float duracion){

		if(isMoving && Mathf.Abs(Time.time - mvtoStop) >= Time.deltaTime) {
			//me mandan moverme, pero estoy actualmente moviendome y me queda más de un frame para acabar el movimiento.
			//Aunque el tiempo de los frames no tiene por qué ser siempre el mismo, nos fiamos del tiempo del último
			//frame para suponer que el tiempo del frame actual va a ser el mismo.
			return;
		}

		mvtoStart = Time.time;
		mvtoStop = mvtoStart + duracion;

		originalPosition = this.transform.position;
		updatePoi();
		isMoving = true;
	}


	void Update () {
		if( !isMoving )	return;

		if(isMoving && Time.time >= mvtoStop) { // Colocamos en la posicion final por posibles desviaciones
			this.transform.position = this.poi;
			isMoving = false;
			return;
		}

		// Para los estados que no hay que moverse
		if(isMoving && (transform.position - poi).magnitude <= eps)  
			return;


		float tiempoRestante = mvtoStop - Time.time;
		tiempoRestante /= (mvtoStop - mvtoStart);

		Vector3 obj = (1-tiempoRestante) * poi + tiempoRestante * originalPosition;
		this.transform.position = obj;
	}

	void updatePoi(){
		poi = this.originalPosition + estados[estadosIdx];

		estadosIdx = (estadosIdx + 1) % estados.Count;
	}
}
