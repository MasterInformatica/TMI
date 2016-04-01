using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MusicManager : MonoBehaviour {
	public static MusicManager S; //Singleton


	public GameObject casillaRef;
	public Metodo2 m;


	public List<double> rhythm;
	private int currentIdx = 0;

	/** Marca de tiempo para realizar los cálculos a partir de ella (tiempo 0).
	 * Invocar la funcion StartTime() antes de comenzar la ejecución del tiempo
	 */
	public float relativeTime=0;

	void Awake () {
		S = this;
		currentIdx=0;

		initMusic();
		//Chapuza para no tener que calcular cuando nos salimos
		rhythm.Add(1e9);


		m = casillaRef.GetComponent<Metodo2>();

	}
		
	private void initMusic(){
		rhythm = new List<double>();
		rhythm.Add(0.0);
		rhythm.Add(0.5);
		rhythm.Add(0.8);
		rhythm.Add(1.2);
		rhythm.Add(1.6);
		rhythm.Add(2.4);
		rhythm.Add(2.8);
		rhythm.Add(3.2);
		rhythm.Add(3.6);
		rhythm.Add(4.0);
		rhythm.Add(4.4);
		rhythm.Add(5.0);
		rhythm.Add(5.2);
		rhythm.Add(7.0);


/*		rhythm.Add(0.0);
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
*/

	}


	double getCurrentRithmPulse(){
		//Sin la chapuza del final sería algo así
		//return (currentIdx >= rhythm.Count) ? 1e9 :	rhythm[currentIdx];

		return rhythm[currentIdx];
	}

	double getNextRithmPulse(){
		//Sin la chapuza del final sería algo así
		//return (currentIdx >= rhythm.Count-1) ? 1e9 : rhythm[currentIdx+1];

		return rhythm[currentIdx+1];
	}
			
	public void StartTime(){
		relativeTime = Time.time;
		currentIdx = 0;

		m.startMvto((float)(rhythm[1] - rhythm[0]));
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
			m.startMvto((float)(rhythm[currentIdx+1] - rhythm[currentIdx]));
		}
	}


}
