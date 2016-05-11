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

		original_x = posx;
		original_z = posz;

		this.updatePosition();
	}

	public void pauseAction(){
		this.mvt.pauseAction ();
	}

	// Avanzar el robot hacia delante.
	public void moveUP(){
		int x = GUI_Layout.S.board_def[original_x,original_z].x;
		int y = GUI_Layout.S.board_def[original_x,original_z].height;
		int z = GUI_Layout.S.board_def[original_x,original_z].z;

		//1.- Comprobar que se puede mover a la nueva posicion
		int newpos_x = x + _x;
		int newpos_z = z + _z;

		//1.1- esta dentro del tablero
		if(newpos_x <0 || newpos_z <0 ||
		   newpos_x >= GUI_Layout.S.MAX_SIZE || newpos_z > GUI_Layout.S.MAX_SIZE)
			return;

		PairInt destino = GUI_Layout.S.current_board[newpos_x, newpos_z];

		//1.2- existe la celda destino
		if (destino==null ||  GUI_Layout.S.board_def[destino.x, destino.y].type == TileType.none)
			return;


		/* 1.3- El robot puede avanzar al frente en los siguientes casos:
		 *   a) La celda actual se mueve:
		 *      a.1) La celda destino tmb se mueve: La posicion original y la final es la misma (pueden estar quietos o en mvto).
		 *      a.2) La celda destino no se mueve: Imposible moverse allí
		 *   b) La celda actual está quieta:
		 *      b.1) Destino se mueve: Imposible moverse allí
		 *      b.2) Destino quieta: Comprobación normal.
		 */

	/*	//1.3.a Actual se mueve
		if( GUI_Layout.S.board_def[this.original_x, this.original_z].isMoving){
			//1.3.a.2 La celda destino no se mueve
			if(! GUI_Layout.S.board_def[destino.x, destino.y].isMoving)
				return;

			//1.3.a.1 La celda destino se mueve, y además casi al mismo sitio
			Vector3 destinoCeldaDestino = GUI_Layout.S.board_def[destino.x, destino.y].getNextPosition();
			//TODO
		

			if(destino!=null && GUI_Layout.S.board_def[destino.x, destino.y].isMoving){ //1.2.a.1 Las dos se mueven
				int real_newpos_x = (int) GUI_Layout.S.board_def[this.original_x, this.original_z].getNextPosition().x + _x;
				int real_newpos_z = (int) GUI_Layout.S.board_def[this.original_x, this.original_z].getNextPosition().z + _z;

				if(GUI_Layout.S.board_def[destino.x, destino.y].getNextPosition().x != real_newpos_x ||
					GUI_Layout.S.board_def[destino.x, destino.y].getNextPosition().z != real_newpos_z) //Se mueven, pero no al mismo sitio
					return;

				if(GUI_Layout.S.board_def[destino.x, destino.y].getNextPosition().y != y ) //se mueven al mismo sitio, pero no a la misma altura
					return;
			}
		}*/




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

		this.transform.localPosition = new Vector3(0, this.transform.position.y, 0);
	}


	public void updatePosition(int newx, int newz, int origx, int origz){
		this.original_x = origx;
		this.original_z = origz;

		this.transform.parent = GUI_Layout.S.board[this.original_x, this.original_z].transform;
		this.transform.localPosition = new Vector3(0, this.transform.position.y, 0);
	}
}
