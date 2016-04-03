using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Delegado: interfaz para el metodo de listener

public delegate void startMvto_t(float duracion);


public class MusicManager : MonoBehaviour {
	public static MusicManager S; //Singleton

	public bool ___________________________________________________;
	public List<double> rhythm;
	private int currentIdx = 0;

	// Por elo momento solo poseemos un nivel de delegados, pero se podría considerear tener varios niveles
	// de listeners (tiles, logicos, muñeco, ...) y en función de la eficiencia no llamar siempre a todos
	private List<startMvto_t> listeners;

	/** Marca de tiempo para realizar los cálculos a partir de ella (tiempo 0).
	 * Invocar la funcion StartTime() antes de comenzar la ejecución del tiempo
	 */
	public float relativeTime=0;


	void Awake () {
		S = this;
		currentIdx=0;

		this.listeners = new List<startMvto_t>();

		initMusic();
		//Chapuza para no tener que calcular cuando nos salimos
		rhythm.Add(1e9);
	}


	public void registerTileListener(startMvto_t d){
		this.listeners.Add(d);
	}


	private void initMusic(){
		rhythm = new List<double>();

		rhythm.Add(0.0);
		rhythm.Add(2.0);
		rhythm.Add(3.0);
		rhythm.Add(4.0);
		rhythm.Add(5.0);
		rhythm.Add(6.0);
		rhythm.Add(7.0);
		rhythm.Add(8.0);
		rhythm.Add(9.0);
		rhythm.Add(10.0);
		rhythm.Add(11.0);
		rhythm.Add(12.0);
		rhythm.Add(13.0);
		rhythm.Add(14.0);


	}


	public void StartTime(){
		relativeTime = Time.time;
		currentIdx = 0;

	}

	void Update(){
		/**
		 * TODO: Aquí habría que mirar si es mejor usar Time.time; Time.fixedtime; Time.realtime; .....
		 **/
		if(relativeTime == 0)
			return;

		float currentTime = Time.time - relativeTime;

		if(rhythm[currentIdx+1] <= currentTime){
			currentIdx ++;
			//TODO: Aqui avisar a todos los que necesiten se avisados.
			callListeners((float)(rhythm[currentIdx+1]-rhythm[currentIdx]));
		}
	}

	void callListeners(float duracion){
		foreach(startMvto_t d in this.listeners)
			d(duracion);
	}

}
