using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Estructura auxiliar usada para la reproduccion de los comandos
 */
struct PairInt {
	public PairInt(int a, int b) {
		x = a;
		y = b;
	}

	public int x, y;
}


/**
 * Director de orquesta. Es el encargado de llevar el control de todo el juego.
 * Maneja los paneles y controla las acciones pulsadas.
 * Almacena la lista de todas acciones a ejecutar.
 * La reproduccion la reliza mediante un sistema de pilas.
 * Avisa al layout para colorear/descolorear las celdas.
 * Controla la victoria del juego
 */
public class Director : MonoBehaviour {
	public static Director S; //Singleton

	public int totalGoals; //Numero de objetivos. Usado para condicion de victoria
	public GameObject texto; //Texto a mostrar cuando se supera un nivel

	public bool ______________________________;

	public List< List<ActionType> > acciones; //Lista de lista de acciones. Por cada panel existe una lista de acciones.
	public bool recording; //Bool indicando si se esta reproduciendo o grabando acciones
	public int panelActiveId; // Int indicando el panel actual activo


	private Stack<PairInt> llamadas; //Pila utilizada para realizar la reproduccion de los comandos



	public void Awake(){
		S = this;
	}


	public void Start(){
		this.recording = true;
				
		this.acciones = new List<List<ActionType>> ();
		this.acciones.Add(new List<ActionType>());
		this.acciones.Add(new List<ActionType>());
		this.acciones.Add(new List<ActionType>());
	}


	// Añade la accion pasada por parametro al panel actual.
	// Guarda la accion en la lista de acciones local.
	public void addAction(ActionType at){
		if (!this.recording)
			return;

		this.acciones[this.panelActiveId].Add (at);
		Actions_Layout.S.insertInPanel (this.panelActiveId, at);
	}


	// Ejecuta los comandos que estaban guardados
	// Para ello, apila en una pila parejas <int,int>, que representan 
	// <panelId, ActionId>. Simula una pila de llamadas de funciones.
	public void play(){
		Actions_Layout.S.changeButton();

		if (this.recording) {
			this.recording = false;
			this.llamadas = new Stack<PairInt>();
			this.llamadas.Push(new PairInt(0,0));
			Actions_Layout.S.switchOffAllLights();

			Next ();
		}else{
			this.restartLevel();
		}
	}


	// Ejecuta la siguiente accion.
	// Utiliza el sistema de pila para simular las llamadas a funcion.
	// De esta maenra se consiguen llamadas recursivas, y coloreado de la
	// interfaz.
	public void Next(){
		if (this.recording)
			return;
		
		if (this.llamadas.Count > 0) {
			PairInt pi = this.llamadas.Pop ();

			if(pi.y < 0){ //si acabamos una rutina, apagamos el ultimo elemento
				Actions_Layout.S.lightCube (pi.x, this.acciones[pi.x].Count-1, false);
				Invoke("Next", 1);
			}else
				this.ejecutaAccion (pi.x, pi.y);
		}
	}


	//Ejecuta la accion i sobre la lista a, y apila la siguiente en la pila de llamadas
	public void ejecutaAccion(int a, int i){

		//apagamos la anterior si podemos
		if (i > 0)
			Actions_Layout.S.lightCube (a, i - 1, false);
		//enciende la actual
		Actions_Layout.S.lightCube (a, i, true);

		//apilamos la sigiente ejecucion, si existe
		if (i + 1 < this.acciones [a].Count)
			this.llamadas.Push (new PairInt (a, i + 1));
		else
			this.llamadas.Push (new PairInt (a, -1));


		switch (this.acciones[a][i]) {
		case ActionType.up:
			Robot.S.moveUP ();
			break;
		case ActionType.jump:
			Robot.S.jump();
			break;
		case ActionType.right:
			Robot.S.rotateRight();
			break;
		case ActionType.left:
			Robot.S.rotateLeft();
			break;
		case ActionType.light:
			int ret = GUI_Layout.S.SwitchLight(Robot.S.x,Robot.S.z);
			if (ret > 0) this.totalGoals --; 
			if (ret < 0) this.totalGoals++;

			testGoal();
			Invoke("Next",1);
			break;
		case ActionType.p1:
			this.llamadas.Push(new PairInt(1, 0));
			Invoke("Next", 0);
			break;
		case ActionType.p2:
			this.llamadas.Push (new PairInt(2,0));
			Invoke("Next", 0);
			break;
		default:
			break;
		}
	}


	// Comprueba si se ha alcanzado la condicion de victoria
	public void testGoal(){
		if (this.totalGoals == 0) {
			this.recording = true;
			this.texto.SetActive(true);

			levelLoad.levelPassed[levelLoad.level-1] = true;
			Invoke("LoadMenu", 2);
		}
	}
	
	public void LoadMenu(){
		Application.LoadLevel("_Scene_menu");
	}


	// Es llamado cuando se desea borrar todas las acciones.
	// Vacia la lista local y manda la orden a la interfaz
	public void trash(){
		if (!this.recording)
			return;

		for(int i=0; i<this.acciones.Count; i++)
			this.acciones[i].Clear ();

		Actions_Layout.S.clearPanels ();
	}


	public void restartLevel(){
		this.recording = true;

		GUI_Layout.S.restartLayout();
		Actions_Layout.S.switchOffAllLights();

		this.panelActiveId = 0;
		Actions_Layout.S.paneles [0].activatePanel ();
	}


	// Almacena el panel activo y ordena que se pinte al nuevo.
	// Apaga el panel activo viejo
	public void panelActive(int i){
		if(i != this.panelActiveId)
			Actions_Layout.S.paneles[this.panelActiveId].deactivatePanel();

		this.panelActiveId = i;


	}
}
