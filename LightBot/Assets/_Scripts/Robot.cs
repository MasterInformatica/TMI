using UnityEngine;
using System.Collections;

/**
 * Clase encargada de la gestion del robot.
 * Antes de realizar una accion comprueba si es posible.
 * Los movimientos los realiza a traves del componente Movement asociado al robot.
 */
public class Robot : MonoBehaviour {
	public static Robot S; //Singleton
	public Movement mvt; //Referencia al componente de movimiento.



	public int x, y, z; //Posicion actual del robot. Sirve para comrpobar si el siguiente movimiento es posible
	public int _x, _z; //Indica hacia donde esta mirando para poder saber hacia donde moverse

	public int x_coord, z_coord; //Cordenadas reales de la celda destino



	public void Start(){
		S = this;
		this.mvt = (Movement)this.GetComponent<Movement>();
	}



	public void init(int posx, int posy, int posz, int rotx, int rotz){
		x = posx; y = posy; z = posz;
		_x = rotx; _z = rotz;


		x_coord = posx;
		z_coord = posz;
		this.updatePosition();
	}

	public void pauseAction(){
		this.mvt.pauseAction ();
	}

	// Avanzar el robot hacia delante.
	public void moveUP(){
		int x = GUI_Layout.S.board_def[x_coord, z_coord].x;
		int y = GUI_Layout.S.board_def[x_coord, z_coord].height;
		int z = GUI_Layout.S.board_def[x_coord, z_coord].z;

		//1.- Comprobar que se puede mover a la nueva posicion
		int newpos_x = x + _x;
		int newpos_z = z + _z;

		//1.1- esta dentro del tablero
		if(newpos_x <0 || newpos_z <0 ||
		   newpos_x >= GUI_Layout.S.MAX_SIZE || newpos_z > GUI_Layout.S.MAX_SIZE)
			return;


		//Si hay alguna casilla que llegue al destino, la cogemos
		PairInt pp = getCasillaPosibleDestino(newpos_x, newpos_z);

		if(pp==null)
			return;

		//1.3 Está a la misma altura
		if(GUI_Layout.S.board_def[pp.x, pp.y].getNextPosition().y != this.y)
			return;


		//2.- Mover
		this.mvt.Avanza ();
		this.x = newpos_x;
		this.z = newpos_z;
		this.x_coord = pp.x;
		this.z_coord = pp.y;

	}


	// Saltar hacia delante
	public void jump(){
		int x = GUI_Layout.S.board_def[x_coord, z_coord].x;
		int y = GUI_Layout.S.board_def[x_coord, z_coord].height;
		int z = GUI_Layout.S.board_def[x_coord, z_coord].z;


		//1.- Comprobar que se puede mover a la nueva posicion
		int newpos_x = x + _x;
		int newpos_z = z + _z;

		//1.1- esta dentro del tablero
		if(newpos_x <0 || newpos_z <0 ||
		   newpos_x >= GUI_Layout.S.MAX_SIZE || newpos_z > GUI_Layout.S.MAX_SIZE)
			return;


		Debug.Log("B");
		//Si hay alguna casilla que llegue al destino, la cogemos
		PairInt pp = getCasillaPosibleDestino(newpos_x, newpos_z);
		if(pp==null)
			return;

		Debug.Log("C " + pp.x + " , " + pp.y + " - " + GUI_Layout.S.board_def[pp.x, pp.y].height);
		Debug.Log("C" + " YO: " + x_coord + z_coord);// + " , " + pp.y + " - " + GUI_Layout.S.board_def[pp.x, pp.y].height);
		//1.3 Está a la misma altura
		if(GUI_Layout.S.board_def[pp.x, pp.y].getNextPosition().y == this.y)
			return;


		Debug.Log("D" + pp.x + " , " + pp.y + " - " + GUI_Layout.S.board_def[pp.x, pp.y].height);
		//1.3- esta a una altura de diferencia
		if (!(y == GUI_Layout.S.board_def [pp.x, pp.y].height+1 ||
			y == GUI_Layout.S.board_def [pp.x , pp.y].height-1 ))
			return;

		Debug.Log("E");
		//2.- Mover
		if (y > GUI_Layout.S.board_def [pp.x, pp.y].height)
			this.mvt.JumpDown ();
		else
			this.mvt.JumpUp ();

		this.x = newpos_x;
		this.z = newpos_z;
		this.y = GUI_Layout.S.board_def [pp.x, pp.y].height;
		this.x_coord = pp.x;
		this.z_coord = pp.y;
	}


	// Rotar hacia la derecha. Siempre se puede hacer
	public void rotateRight(){
		if (_x == 1 && _z == 0) {
			_x = 0;
			_z = -1;
		} else if (_x == -1 && _z == 0) {
			_x = 0;
			_z = 1;
		} else if (_x == 0 && _z == 1) {
			_x = 1;
			_z = 0;
		} else if (_x == 0 && _z == -1) {
			_x = -1;
			_z = 0;
		}

		this.mvt.RotateRight ();
	}


	// Rotar hacia la izquierda. Siempre se puede hacer
	public void rotateLeft(){
		if (_x == 1 && _z == 0) {
			_x = 0;
			_z = 1;
		}else if (_x == -1 && _z == 0) {
			_x = 0;
			_z = -1;
		}else if (_x == 0 && _z == 1) {
			_x = -1;
			_z = 0;
		}else if (_x == 0 && _z == -1) {
			_x = 1;
			_z = 0;
		}

		this.mvt.RotateLeft ();
	}


	public void updatePosition(){
		this.transform.parent = GUI_Layout.S.board[this.x_coord, this.z_coord].transform;
		this.transform.localPosition = new Vector3(0, this.transform.position.y, 0);
	}

	/*
	public void updatePosition(int newx, int newz, int origx, int origz){
		this.original_x = origx;
		this.original_z = origz;

		this.transform.parent = GUI_Layout.S.board[this.original_x, this.original_z].transform;
		this.transform.localPosition = new Vector3(0, this.transform.position.y, 0);
	}
*/

	/* Devuelve las coordenadas de una casilla que en el siguiente movimiento va a estar en la posición
	* pasada por parámetro.
	*/
	private PairInt getCasillaPosibleDestino(int x1, int z1){
		float eps = 0.7f;
		float x = this.transform.position.x / 2.0f +_x;
		float z = this.transform.position.z / 2.0f +_z;
		Debug.Log ("--(" + x + ", " + z + ") vs " + this.transform.position);
		GUI_Layout S = GUI_Layout.S;
		PairInt aux = null;
		//centro
		aux = S.current_board[x1,z1];
		if(aux!=null && 
			Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().x - x) < eps &&
		   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().z -z)< eps)
			return aux;
		//norte
		if(x1>0){
			aux = S.current_board[x1-1,z1];
			if(aux!=null && 
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().x - x) < eps &&
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().z -z)< eps)
				return aux;
		}
		//sur
		if(x1+1<GUI_Layout.S.MAX_SIZE){
			aux = S.current_board[x1+1,z1];
			if(aux!=null && 
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().x - x) < eps &&
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().z -z)< eps)
				return aux;
		}
		//este
		if(z1>0){
			aux = S.current_board[x1,z1-1];
			if(aux!=null && 
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().x - x) < eps &&
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().z -z)< eps)
				return aux;
		}
		//oeste
		if(z1+1<GUI_Layout.S.MAX_SIZE){
			aux = S.current_board[x1,z1+1];
			if(aux!=null && 
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().x - x) < 0.4f &&
			   Mathf.Abs (S.board_def[aux.x, aux.y].getNextPosition().z -z)< 0.4f)
				return aux;
		}


		return null;
	}
}
