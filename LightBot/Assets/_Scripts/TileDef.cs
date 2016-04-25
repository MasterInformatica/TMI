using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Definicion de una casilla. */
public class TileDef {
	/**** DEBUG ****/
	public GameObject textoDBG;
	/**** DEBUG ****/




	// Cosas publicas que definen una casilla. Se accede directamente
	public TileType type; //normal, goal, none


	public bool isOn; //Light status

	//Posicion actual de la casilla
	public int x;
	public int z;
	private int y;
	public int height {  //Unicamente un wrapper para el set de y
		get{ return y; }
		set{
			y = value;
			if (value <= 0)
				type = TileType.none;
		}
	}


	//Posicion original de la casilla. Aunque la casilla se mueva, estos valore representan *realmente* a la casilla, es 
	//decir, una casilla se identifica de manera única por la posición que ocupa antes de comenzar a moverse.
	public int original_x;
	public int original_z;


	//Para movimiento de casillas normal
	public tipoMovimiento typeMvto = tipoMovimiento.none; //horizontal, vertical, frente
	public int nSteps = 0;


	//--------------------------------------------------------------------
	// La diferencia entr estos estados, y los de la definición física de las celdas (TileMvto) es que aquí no se contempla
	// el estado de no movimiento, sino que se utiliza el bool isMoving para representar la pausa, y tener un control mayor
	// sobre los estados.
	// De esta maenra se intenta informar al robot de una manera más precisa y evitar movimientos raros o a destiempo de este
	private List<Vector3> estados;
	private int currentState;

	public bool isMoving = false;

	public float endTime = -1;
	//--------------------------------------------------------------------


	public Robot robot; //Para acceder más sencillo a la instancia/singleton del robot


	/*
	 * Inicializa los estados de la definición lógica. No contempla estados de pausa. Se asumen
	 */
	public void createStates(){
		if(nSteps==0 || typeMvto==tipoMovimiento.none)
			return;

		currentState = 0;
		estados = new List<Vector3>();

		initStates();

		//Nos registramos como listeners de eventos de musica
		MusicManager.S.registerTileDefListener(this.startMvto);

		//Almacenamos la posición original, es decir, aquella que representa realmente a la casilla, independientemente de
		//donde se encuentre a causa del movimiento
		this.original_x = x;
		this.original_z = z;

		isMoving = false;
	}



	private void initStates(){
		int xx=0, yy=0, zz=0;
		Vector3 OriginalPos = new Vector3(x,y,z);

		switch(this.typeMvto){
		case tipoMovimiento.horizontal:
			xx=1;
			break;
		case tipoMovimiento.vertical:
			yy=1;
			break;
		case tipoMovimiento.frente:
			zz=1;
			break;
		}

		for(int i=0; i<nSteps; i++)
			this.estados.Add(OriginalPos + i * new Vector3(xx,yy,zz));

		for(int i=nSteps; i>0; i--){
			this.estados.Add(OriginalPos + i * new Vector3(xx,yy,zz));
		}
	}



	/* A la hora de mover una casilla (representación lógica), se presentań dos formas: Cambiar la posición al principio
	 * del movimiento, pero marcar con el flag isMoving que esa posición todavía no se ha alcanzado, o cambiar la posición 
	 * al final del movimiento (i.e.), cuando se alcance el tiempo marcado por duracion.
	 * 
	 * La forma elegida es la la segunda. El método startMvto solametne calcula el tiempo final, e indica a través del
	 * flag isMoving si este pulso toca moverse o no. Es el método Update el encargado de actualizar la posición cuando
	 * se acaba el pulso
	 */
	public void startMvto(float duracion){
		if(this.robot==null)
			this.robot = Robot.S;

		this.Update(); //Como no heredamos de MonoBehaviour, llamamos nosotros a mano.

		if( endTime > 0 && Mathf.Abs(Time.time - endTime) >= Time.deltaTime) {
			//me mandan moverme, pero estoy actualmente moviendome y me queda más de un frame para acabar el movimiento.
			//Aunque el tiempo de los frames no tiene por qué ser siempre el mismo, nos fiamos del tiempo del último
			//frame para suponer que el tiempo del frame actual va a ser el mismo.
			return;
		}


		//Si isMoving está a falso, significa que en la última ronda no nos hemos movido y nos toca ahora hacerlo
		if(!isMoving)
			isMoving = true;
		else
			isMoving = false;

		endTime = Time.time + duracion;
	}


	private void Update(){
		/*@ DBG @*/
		if(robot.x == this.original_x &&
			robot.z == this.original_z){
		
			GUIText gt = this.textoDBG.GetComponent<GUIText>();
			gt.text = "DBG: " + ((isMoving) ? "T" : "F") +  ": (" + this.x + " , " + this.y + " , " + this.z + ")\n";
//				"       (" + GUI_Layout.S.current_board[this.original_x, this.original_z].x + " , "+
//				GUI_Layout.S.current_board[this.original_x, this.original_z].y +")";

		}
		/*@ DBG @*/

		if(Time.time < endTime || endTime < 0) return;

		//Aqui significa que hemos superado el tiempo final, y además no hemos actualizado la pos (endTime > 0)
		if(isMoving){


				
			//Como nos vamos a mover, nos borramos del current board si nadie lo ha hecho por nosotros
			if(GUI_Layout.S.current_board[this.x, this.z].x == this.original_x &&
				GUI_Layout.S.current_board[this.x, this.z].y == this.original_z){
				GUI_Layout.S.current_board[this.x, this.z] = null;
			}


			currentState = (currentState + 1) % estados.Count;

			x = (int)estados[currentState].x;
			y = (int)estados[currentState].y;
			z = (int)estados[currentState].z;



			//Y además, como nos hemos movido, nos anotamos en el tablero general.
			GUI_Layout.S.current_board[x,z] = new PairInt(this.original_x, this.original_z);



		}
		endTime = -1;
	}


	/** Estos métodos sirven para identificar si una casilla se está moviendo, parada, cual es su próximo estado,
	 *  etc. Básicamente se utiliza para comprobar si dos casillas son adyacentes o no
	 */
	public Vector3 getCurrentPosition(){
		return estados[currentState];
	}

	public Vector3 getNextPosition(){
		if(isMoving)
			return estados[(currentState + 1) % estados.Count];
		else
			return estados[currentState];
	}


	/* *** DEBUG ***/
	public TileDef(){
		this.textoDBG = GameObject.FindWithTag("DBG");
	}
}