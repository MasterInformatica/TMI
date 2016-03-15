using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * MÉTODO 1 de movimiento:
 * Este método es similar a como funcionan las barritas del helper. 
 * La clase solicita el array de tiempos de ritmos en la constructora, y se mueve
 * de acuerdo a este array, sin realiar ninguna comunicación extra con alguna otra clase
 * 
 * El tiempo de inicio (relativeTime), podría acceder a través de la clase MusicManager, o 
 * realizar una copia (cuando != 0). La gracia está en que al usar un tiempo relativo al frame
 * actual, todos los métodos time llamados en el mismo frame tienen el mismo valor,
 * independientemente de si un método Update realiza mucho trabajo o poco.
 **/

public class Metodo1 : MonoBehaviour {
    // Relativo a la sincronizacion
    private List<double> rhythm;
    private float relativeTime=0;

    //Relativo al movimiento
    public Vector3 poi; //objetivo
    public int idx = 0;

    //chapucero
    int mvto = 4;


	void Start () {
        this.rhythm = MusicManager.S.rhythm;
        poi = new Vector3(0.0f, 0.0f, 0.0f);
	}
	
	void Update () {
        if (relativeTime == 0){
            relativeTime = MusicManager.S.relativeTime;
            if(relativeTime == 0) return;
        }


        float currentTime = Time.time-relativeTime;
       //comprobamos si estamos en el proximo pulso
        if (rhythm[idx + 1] <= currentTime)
            actualizaObjetivo();

        //interpolacion lineal. Calcular cuanto nos tenemos que mover.
        float ttime = (float)rhythm[idx + 1] - currentTime;
        float nTicks = ttime / Time.deltaTime; //numero ticks hasta objetivo

        float dist = poi.x - this.transform.position.x; //distancia hasta objetivo
        float avance = dist / nTicks; //lo que hay que avanzar en este tick

        this.transform.position = new Vector3(transform.position.x + avance, 0, 0);
	}


    void actualizaObjetivo() {
        this.mvto *= -1;
        this.poi += new Vector3(2 * this.mvto, 0, 0);
    }
}
