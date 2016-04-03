using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Definicion de una casilla. */
public class TileDef {
	// Cosas publicas que definen una casilla. ASe accede directamente
	public TileType type;

	private int y;
	public int height {
		get{ return y; }
		set{
			y = value;
			if (value <= 0)
				type = TileType.none;
		}
	}

	public bool isOn;

	//Posicion actual de la casilla
	public int x;
	public int z;
	public bool isMoving = false;


	//Para movimiento de casillas normal
	public tipoMovimiento typeMvto = tipoMovimiento.none;
	public int nSteps = 0;



	//-------------------------------------------------------------------
	// Cosas privadas que definen el comportamiento lógico de la casilla
	private List<Vector3> estados;
	private int currentState;


	public void createStates(){
		if(nSteps==0 || typeMvto==tipoMovimiento.none)
			return;

		currentState = 0;
		estados = new List<Vector3>();

		//Este código se duplica, pero por el momento es la forma más cahpuza de hacerlo
		initStates();

		//Nos registramos como listeners de eventos de musica
		MusicManager.S.registerTileDefListener(this.startMvto);
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


	public void startMvto(float duracion){
		if(!isMoving){
			isMoving = true;
			return;
		}

		//isMoving
		isMoving = false;
		currentState = (currentState + 1) % estados.Count;

		x = (int)estados[currentState].x;
		y = (int)estados[currentState].y;
		z = (int)estados[currentState].z;
			
	}
}