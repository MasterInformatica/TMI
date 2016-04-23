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



	//Posiciones originales del robot. Representan la posición original de la celda actual antes de empezar su movimiento.
	public int original_x, original_z;



	public void Start(){
		S = this;
		this.mvt = (Movement)this.GetComponent<Movement>();
	}



	public void init(int posx, int posy, int posz, int rotx, int rotz){
		x = posx; y = posy; z = posz;
		_x = rotx; _z = rotz;

		original_x = x;
		original_z = z;

		this.updatePosition();
	}



	// Avanzar el robot hacia delante.
	public void moveUP(){
		//1.- Comprobar que se puede mover a la nueva posicion
		int newpos_x = x + _x;
		int newpos_z = z + _z;

		//1.1- esta dentro del tablero
		if(newpos_x <0 || newpos_z <0 ||
		   newpos_x >= GUI_Layout.S.MAX_SIZE || newpos_z > GUI_Layout.S.MAX_SIZE)
			return;

		//1.2- existe la celda
		if (GUI_Layout.S.board_def[newpos_x, newpos_z].type == TileType.none)
			return;


		//1.3- esta a la misma altura
		if (y != GUI_Layout.S.board_def [newpos_x, newpos_z].height)
			return;

		//2.- Mover
		this.mvt.Avanza ();
		this.x = newpos_x;
		this.z = newpos_z;

	}


	// Saltar hacia delante
	public void jump(){
		//1.- Comprobar que se puede mover a la nueva posicion
		int newpos_x = x + _x;
		int newpos_z = z + _z;
		
		//1.1- esta dentro del tablero
		if(newpos_x <0 || newpos_z <0 ||
		   newpos_x >= GUI_Layout.S.MAX_SIZE || newpos_z > GUI_Layout.S.MAX_SIZE)
			return;
		
		//1.2- existe la celda
		if (GUI_Layout.S.board_def[newpos_x, newpos_z].type == TileType.none)
			return;
		

		//1.3- esta a una altura de diferencia
		if (!(y == GUI_Layout.S.board_def [newpos_x, newpos_z].height+1 ||
			  y == GUI_Layout.S.board_def [newpos_x, newpos_z].height-1 ))
			return;
		
		//2.- Mover
		if (y > GUI_Layout.S.board_def [newpos_x, newpos_z].height)
			this.mvt.JumpDown ();
		else
			this.mvt.JumpUp ();

		this.x = newpos_x;
		this.z = newpos_z;
		this.y = GUI_Layout.S.board_def [newpos_x, newpos_z].height;
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
		this.original_x = x;
		this.original_z = z;

		this.transform.parent = GUI_Layout.S.board[this.original_x, this.original_z].transform;

		//TODO: Al colocarnos en la nueva casilla, sería interesante que nos colocaramos
		//      centrados para que quede todo más chuli.

		//Al hacer el atach, nos recolocamos por si hemos perdido algo de  precisión
		//Vector3 newPos = GUI_Layout.S.board[this.original_x, this.original_z].transform.position;

		//this.transform.position = new Vector3(newPos.x, this.transform.position.y, newPos.z);


	}
}
