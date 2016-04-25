using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Delegado: interfaz para el metodo de listener

public delegate void startMvto_t(float duracion);


public class MusicManager : MonoBehaviour, AudioProcessor.AudioCallbacks {
	public static MusicManager S; //Singleton
	public AudioSource audio;
	public float delta = 1.3f;
	public bool ___________________________________________________;

	private float last;
	// Por elo momento solo poseemos un nivel de delegados, pero se podría considerear tener varios niveles
	// de listeners (tiles, logicos, muñeco, ...) y en función de la eficiencia no llamar siempre a todos
	private List<startMvto_t> tile_listeners;
	private List<startMvto_t> tileDef_listeners;

	/** Marca de tiempo para realizar los cálculos a partir de ella (tiempo 0).
	 * Invocar la funcion StartTime() antes de comenzar la ejecución del tiempo
	 */
	public float relativeTime=0;


	void Awake () {
		S = this;

		this.tile_listeners = new List<startMvto_t>();
		this.tileDef_listeners = new List<startMvto_t>();
	}


	void Start(){
		AudioProcessor.S.loadSource(audio);
		AudioProcessor.S.addAudioCallback(this);
		last = 0.0f;
	}



	public void registerTileListener(startMvto_t d){
		this.tile_listeners.Add(d);
	}

	public void registerTileDefListener(startMvto_t d){
		this.tileDef_listeners.Add(d);
	}

	public void StartTime(){
		relativeTime = Time.time;
	}

	void Update(){
	}


	//==================================================================================================================
	//==================================================================================================================
	void callListeners(float duracion){
		//Toda la magia se hace aquí

		//casillas
		foreach(startMvto_t d in this.tile_listeners)
			d(duracion);

		//definicion logica de las casillas
		foreach(startMvto_t d in this.tileDef_listeners)
			d(duracion);
	}
	//==================================================================================================================
	//==================================================================================================================



	public void onOnbeatDetected()
	{
		float recent = Time.time;
		if (recent - last >= delta)
			last = recent - delta;
		callListeners ((float)recent - last);
		last = recent;
	}
	
	//This event will be called every frame while music is playing
	public void onSpectrum(float[] spectrum)
	{
		//The spectrum is logarithmically averaged
		//to 12 bands
		
		for (int i = 0; i < spectrum.Length; ++i)
		{
			Vector3 start = new Vector3(i, 0, 0);
			Vector3 end = new Vector3(i, spectrum[i], 0);
			Debug.DrawLine(start, end);
		}
	}


}
