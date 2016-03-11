using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * La idea loca de esta clase es que pinta barritas a alturas distintas según el ritmo.
 * Las marcs de tiempo las lee del MusicManager, por lo que la función principal estára en el Start (intentando evitar
 * condiciones de carrera entre los métodos Awake de las dos clases).
 */
public class Barritas : MonoBehaviour {
	public GameObject barrita;
	public GameObject guia_prefab;
	public static float verticalDistance = 1;
	public static float widthScreen = 50;

	public bool ________________________________;

	public GameObject anchor;
	public GameObject guia;

	private float widthPerSecond;

	void Start(){
		if(this.anchor==null){
			anchor = new GameObject();
			anchor.transform.position = new Vector3(0, 100, 0);
			anchor.name = "Anchor de las barritas";
		}

		iniciaBarras();
		iniciaGuia();

		Invoke("IniciaEspectaculo", 2);
	}	

	void IniciaEspectaculo(){
		print("Que comiencen los juegos del hambre!!");
		MusicManager.S.StartTime();
	}


	void iniciaGuia(){
		guia = Instantiate(guia_prefab);
		guia.transform.parent = anchor.transform;
		guia.transform.localPosition = new Vector3(0,0,0);

	}



	void iniciaBarras(){
		List<double> rhythm = MusicManager.S.rhythm;

		//Calculamos a cuanto espacio de la pantalla corresponde un segundo de la melodia
		widthPerSecond = widthScreen / (float)(rhythm[rhythm.Count-2]);


		//pintar las barritas por cada golpe de ritmo
		short level = -1;
		float start=0, end;

		for(int i=0; i<rhythm.Count-1; i++){
			end = (float)rhythm[i];

			//barrita a altura level, de tamaño (end-start)*widthPerSecond
			GameObject go = Instantiate(barrita);
			go.transform.parent = anchor.transform;

			float width = (end-start)*widthPerSecond;
			go.transform.localPosition = new Vector3((start*widthPerSecond) + (width/2), level*verticalDistance, 0);
			go.transform.localScale = new Vector3(width,1,1);

			//preparar la siguiente vuelta
			level *= -1;
			start = end;
		}
	}


	void Update(){
		if(MusicManager.S.relativeTime == 0)
			return;

		float elapsedtime = Time.time - MusicManager.S.relativeTime;

		//Aunque es muy burro, como se supone que el metodo Update se va a llamar muchas veces, nos movemos directamente
		//en vez de hacer la interpolacion y movernos como se debería.
		guia.transform.position = new Vector3(elapsedtime*widthPerSecond, 100, 0);
	}
}
