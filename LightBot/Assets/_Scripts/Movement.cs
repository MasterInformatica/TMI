using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * Clase encargada de realizar los movimientos por el tablero.
 * Realiza los movimientos de avanzar, girarDcha, girarIzq, saltar.
 * 
 * Esta clase no comprueba si el movimiento se puede realizar o no. Tiene
 * que ser complementada con otro componente que se encargue de la logica
 * antes de llamar a los metodos de este componente
 **/
public class Movement : MonoBehaviour {

	//Vectores que indican hacia donde se encuentra mirando el gameObject actualemnte,
	// asi como la rotacion actual. Sirve para poder calcular la direccion y giro de destino.
	public Vector3 dirAct = new Vector3 (0, 0, 1f);
	public Vector3 rotAct = new Vector3 (0, 0, 0);

	//Para mover hacia delante, fijamos el vector de desplazamiento, y multiplicamos por
	// la direccion actual para eliminar las coordenadas que no nos interesan.
	public Vector3 dirFrente = new Vector3(2.1f,0,2.1f);
	public Vector3 dirArriba = new Vector3 (0, 1f, 0);

	public Vector3 rotLeft = new Vector3 (0, -90f, 0);
	public Vector3 rotRight = new Vector3 (0, 90f, 0);


	public float MVTO_DURATION = 5f;

	public bool _____________________________;

	public float timeStart; //Timestamp en el que comienza el movimiento.
	public float timeDuration; //Tiempo que queda para que acabe el mvto.

	public bool isMoving; //Inidica que se mueve de frente.
	public bool isRotating;
	public bool isJumping;

	public Vector3 poi; //Posicion destino
	public Vector3 orgPosition; //posicion original
	
	public Vector3 rotPoi; //Rotacion final
	public Vector3 orgRotation; //Rotacion original

	public List<Vector3>      bezierPts;

	private CharacterAnimator animationController;

	public void Start(){
		GameObject character = this.transform.Find ("Character").gameObject;
		this.animationController = character.GetComponent<CharacterAnimator> ();
	}


	public void pauseAction(){
		this.isMoving = true;
		this.isRotating = false;
		this.isJumping = false;
		this.animationController.setIdle();
		
		this.timeStart = Time.time;
		this.timeDuration = this.MVTO_DURATION;
		
		
		this.orgPosition = this.transform.position;
		this.poi = this.transform.position;
	}


	/** 
	 * Comienza el movimiento hacia delante de la entidad.
	 * No comprueba si se encucentra ya en movimiento
	 **/
	public void Avanza(){
		this.isMoving = true;
		this.isRotating = false;
		this.isJumping = false;
		this.animationController.setMoving ();

		this.timeStart = Time.time;
		this.timeDuration = this.MVTO_DURATION;


		this.orgPosition = this.transform.position;
		this.poi = this.GetAvanzaPoi ();
	}


	/**
	 * Rotacion hacia la izquierda
	 **/
	public void RotateLeft(){
		this.isMoving = false;
		this.isRotating = true;
		this.isJumping = false;
		this.animationController.setIdle ();

		this.timeStart = Time.time;
		this.timeDuration = this.MVTO_DURATION;

		this.orgRotation = this.rotAct;
		this.rotPoi = this.rotAct + this.rotLeft;

		this.ActualizaDireccionTrasRotacion (true);
	}

	/**
	 * Rotacion hacia la derecha
	 **/
	public void RotateRight(){
		this.isMoving = false;
		this.isRotating = true;
		this.isJumping = false;
		this.animationController.setIdle ();

		this.timeStart = Time.time;
		this.timeDuration = this.MVTO_DURATION;
		
		this.orgRotation = this.rotAct;
		this.rotPoi = this.rotAct + this.rotRight;

		this.ActualizaDireccionTrasRotacion (false);
	}


	/**
	 * Wrapper para la accion de saltar
	 **/
	public void JumpUp(){
		this.Jump (true);
	}

	/**
	 * Wrapper para la accion de saltar
	 **/
	public void JumpDown(){
		this.Jump (false);
	}


	/**
	 * Realiza la accion de salto.
	 * Segun el parametro lo realiza hacia arriba o hacia abajo.
	 * Simula una curva de Bezier de 3 puntos para realizar un salto algo mas "natural"
	 */
	public void Jump(bool up = true){
		this.isJumping = true;
		this.isRotating = false;
		this.isMoving = false;
		this.animationController.setJumping ();

		//iniciamos los puntos de bezier con el primero y el ultimo
		bezierPts = new List<Vector3>();
		bezierPts.Add ( this.transform.position );  // Current position

		Vector3 _poi = GetJumpPoi (up);
		bezierPts.Add (this.transform.position+ ((_poi - this.transform.position) / 2 + 4*this.dirArriba));
		bezierPts.Add ( _poi );                   

		this.timeStart = Time.time;
		this.timeDuration = this.MVTO_DURATION;

	}


	/**
	 * Metodo invocado una vez por frame. Segun la accion actual, ejecuta el movimiento
	 * dependiendo del tiempo en el que estamos
	 **/
	public void Update(){
		if (this.isMoving) {
			float u = (1f/timeDuration) * (Time.time - timeStart) ;
		
			this.transform.position = (1 - u) * this.orgPosition + u * this.poi;

			if (u>=1){ //fin del movimiento
				this.transform.position = this.poi;
				isMoving = false; //fin del movimiento
				Director.S.Next();
				Robot.S.updatePosition();
				//this.animationController.setIdle ();
			}
		}

		if (this.isRotating) {
			float u = (Time.time - timeStart) / timeDuration;

			this.transform.rotation = Quaternion.Euler ((1-u)*this.orgRotation + u * this.rotPoi);

			if (u>=1){
				this.transform.rotation = Quaternion.Euler (this.rotPoi);
				this.rotAct = this.rotPoi;
				isRotating = false; //fin del giro
				Director.S.Next();
				//this.animationController.setIdle ();
			}
		}

		if (this.isJumping) {
			float u = (Time.time - timeStart)/timeDuration;
			
			// Use Easing class from Utils to curve the u value
			float uC = Easing.Ease (u, Easing.InOut);
			
			if (u>=1) { // If u>=1, we're finished moving
				uC = 1; // Set uC=1 so we don't overshoot

				// Move to the final position
				transform.position = bezierPts[bezierPts.Count-1];
				this.isJumping = false;
				Director.S.Next();
				Robot.S.updatePosition();
				//this.animationController.setIdle ();

			} else { // 0<=u<1, which means that this is interpolating now
				// Use Bezier curve to move this to the right point
				Vector3 pos = Utils.Bezier(uC, bezierPts);
				transform.position = pos;
			}
		}

	}


	//------------------------------------------------------------------------------------------------------------------

	/**
	 * Devuelve a partir de la posicion actual y hacia donde este mirando el destino 
	 *  de un movimiento en linea recta
	 **/
	private Vector3 GetAvanzaPoi(){
		Vector3 aux = new Vector3 (Mathf.RoundToInt(this.dirAct.x * this.dirFrente.x),
		                           Mathf.RoundToInt(this.dirAct.y * this.dirFrente.y),
		                           Mathf.RoundToInt(this.dirAct.z * this.dirFrente.z));

		return this.transform.position + aux;
	}

	/**
	 * Devuelve a partir de la posicion actual, de donde se este mirando, y del tipo de salto
	 * la posicion final del movimiento.
	 **/
	private Vector3 GetJumpPoi(bool arriba = true){

		Vector3 aux = (arriba) ? this.dirArriba : -1*this.dirArriba;

		return aux + this.GetAvanzaPoi ();
	}


	/**
	 * Dado un vector, devuelve otro vector con un +/- 1 en las coordenadas
	 *  distintas de 0 (dependiendo del signo del vector original, y un 0 en 
	 *  las coordenadas que hubiera un 0 originalmente.
	 **/
	private Vector3 GetVectorUno(Vector3 v){
		Vector3 aux;

		aux.x = (v.x == 0) ? 0 : v.x / v.x;
		aux.y = (v.y == 0) ? 0 : v.y / v.y;
		aux.z = (v.z == 0) ? 0 : v.z / v.z;

		return aux;
	}


	/**
	 * Actualiza el vector de la direccion actual (hacia donde apunta el gameObject),
	 *  tras un giro. 
	 * El parametro indica si ha girado hacia la derecha o hacia la izquierda.
	 **/
	private void ActualizaDireccionTrasRotacion(bool left = true){
		Vector3 final = this.dirAct;

		if (left) {
			if(final.x == 1 && final.z == 0)
				this.dirAct = new Vector3(0,0,1);
			if(final.x == -1 && final.z == 0)
				this.dirAct = new Vector3(0,0,-1);
			if(final.x == 0 && final.z == 1)
				this.dirAct = new Vector3(-1,0,0);
			if(final.x == 0 && final.z == -1)
				this.dirAct = new Vector3(1,0,0);
		}
		else {
			if(final.x == 1 && final.z == 0)
				this.dirAct = new Vector3(0,0,-1);
			if(final.x == -1 && final.z == 0)
				this.dirAct = new Vector3(0,0,1);
			if(final.x == 0 && final.z == 1)
				this.dirAct = new Vector3(1,0,0);
			if(final.x == 0 && final.z == -1)
				this.dirAct = new Vector3(-1,0,0);
		}

	}
}
