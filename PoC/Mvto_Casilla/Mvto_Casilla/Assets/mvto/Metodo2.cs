using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * MÉTODO 2 de movimiento:
 * Este método permite realizar movimientos a partir de llamadas startMvto(duracion).
 *  Si se realiza una llamada startMvto pero no ha finalizado el mvto anterior (duracion), ignora la llamada.
 *  Si finaliza un movimiento, espera a una llamada startMvto para comenzar el siguiente.
 * 
 **/
enum tipoMovimiento{
	horizontal, 
	vertical,
	frente
}


public class Metodo2 : MonoBehaviour {
	public float TAM = 1; //Tamaño a mover


	float mvtoStart;
	float mvtoStop;

	bool esperaEntreMovimientos = true; //Determina si hay que esperar entre un movimiento o no.

	Vector3 poi;
	Vector3 originalPosition;

	public bool ______________________________;
	public float eps = 0.1f;
	public float eps2 = 0.1f;
	bool isMoving;

	public List<Vector3> estados;
	int estadosIdx;

	void Start () {
		estadosIdx = 0;

		estados = new List<Vector3>();
		//PRUEBAS
		initMvto(tipoMovimiento.frente, 1);
		originalPosition = transform.position;
	}


	/** Inicializa el script para moverse según tm en el numero de pasos indicado */
	void initMvto(tipoMovimiento tm, int pasos){
		switch(tm){
		case tipoMovimiento.horizontal:
			creaHorizontal(pasos);
			break;
		case tipoMovimiento.vertical:
			creaVertical(pasos);
			break;
		case tipoMovimiento.frente:
			creaFrente(pasos);
			break;
		}
	}


	void creaFrente(int pasos){
		//movimiento dcha, pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(new Vector3(0, 0 , TAM));
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0, 0, 0));
		}

		//mvto izq, pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(new Vector3(0, 0 , -1*TAM));
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0, 0, 0));
		}
	}


	void creaHorizontal(int pasos){
		//movimiento dcha, pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(new Vector3(TAM, 0 , 0));
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0, 0, 0));
		}

		//mvto izq, pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(new Vector3(-1*TAM, 0 , 0));
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0, 0, 0));
		}
	}

	void creaVertical(int pasos){
		//movimiento dcha, pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(new Vector3(0, TAM , 0));
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0, 0, 0));
		}

		//mvto izq, pausa
		for(int i=0; i<pasos; i++){
			this.estados.Add(new Vector3(0, -1*TAM, 0));
			if(this.esperaEntreMovimientos) this.estados.Add(new Vector3(0, 0, 0));
		}
	}


	public void startMvto(float duracion){

		if(isMoving && Mathf.Abs(Time.time - mvtoStop) >= Time.deltaTime) {
//			print("No puedes pasar!!! Tiempo act: " + Time.time + "Stop: " + mvtoStop);
			return;
		}
			
		//print("Muevete " + estados[estadosIdx]);

		mvtoStart = Time.time;
		mvtoStop = mvtoStart + duracion;

		originalPosition = this.transform.position;
		updatePoi();
		isMoving = true;
	}


	void Update () {
		if( !isMoving )	return;

		if(isMoving && Time.time >= mvtoStop) { // Colocamos en la posicion final por posibles desviaciones
			//this.transform.position = this.poi;
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
