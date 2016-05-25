using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Tipos para una casilla */
public enum TileType{
	normal,
	goal,
	none
}


/*
 * Script encargado de toda la interfaz de la parte izquierda.
 * Crea el tablero a partir de un fichero xml, y coloca al robot en la posicion adecuada.
 * Almacena el tablero de forma logica para poder comprobar si los movimientos son posibles.
 * La representacion fisica de la representacion logica se encuentran almacenadas de manera independiente.
 */
public class GUI_Layout : MonoBehaviour {
	public static GUI_Layout S;//Singleton para acceder


	// Prfebs para crear una casilla
	public GameObject tileMvto_prefab; // Prefab vacio para las casillas con movimientos.
	public GameObject tileBlock_prefab;
	public GameObject specialCover_prefab;
	public GameObject normalCover_prefab;

	// Materiales para el cover de una casilla
	public Material offCover_material;
	public Material onCover_material;

	//Prefab del robot
	public GameObject robot_prefab;

	
	// Lectura del xml
	public PT_XMLReader      xmlr;  // PT_XMLReader
	public PT_XMLHashtable   xml;


	// Lista de xml para cada nivel
	public TextAsset[] niveles;
	public AudioClip[] tracks;


	// tamanyo maximo del tablero
	public int MAX_SIZE=20;

	//Texto para mostrar el numero de nivel
	public GameObject LevelTxt;


	public bool _____________________ ;
	public TextAsset texto;

	// Datos para pintar el mapa. Leidos del xml
	private float blockSize = 1f; 
	private float blockSeparation = 0.1f;
	private float coverSize = 0.1f; 
	private float scale = 2f;

	//Datos para pintar el robot. Leidos del xml
	private Vector3 robotPosition;
	private Vector3 robotRotation;
	private float robotHeight;

	//tamanyo del tablero
	private int size_x;
	private int size_z;
	private int default_height;


	// Definiciones e instancias de las celdas
	//-----------------------------------------------------------------------------------------------------------------
	public TileDef[,]    board_def; //Representación _LÓGICA_ del tablero
	public GameObject[,] board;  //Representación _FÍSICA_ del tablero
	public PairInt[,]    current_board; //Estado actual del mapa, contempola los movimientos de las casillas
	//-----------------------------------------------------------------------------------------------------------------


	//Anchor para el board
	public GameObject boardAnchor;

	//Gameobject del robot para poder destruirlo
	public GameObject robot;

	public AudioSource audio;

	public void Awake(){
		S = this;
		this.texto = this.niveles[levelLoad.level-1];

		GUIText gt = this.LevelTxt.GetComponent<GUIText>();
		gt.text = "Nivel: " + levelLoad.level;
	}
	

	public void Start(){
		this.resetLayout ();

		this.readLayout (texto.text);

		this.drawlayout ();
		this.drawRobot();
		
		//Cargamos la cancion en cuestion
		audio.clip = tracks [levelLoad.music-1];
		audio.Play ();

		//Iniciamos el music manager
		MusicManager.S.StartTime();
	}


	// -------------------------------------
	// -------------- METODOS --------------
	// -------------------------------------

	/*
	 * carga el tablero definido en el xml pasado por paramtero
	 */
	public void readLayout(string xmlText){
		//Antes de hacer nada, reseteamos el tablero
		this.resetLayout ();


		xmlr = new PT_XMLReader();
		xmlr.Parse(xmlText); 
		xml = xmlr.xml["xml"][0];


		//1.- Definiciones de las dimensiones
		PT_XMLHashtable dimensions = xml["dimensions"][0];
		//1.1 - tablero
		this.size_x = int.Parse (dimensions ["board"][0].att ("length_x"));
		this.size_z = int.Parse (dimensions ["board"][0].att ("length_z"));
		this.scale = float.Parse (dimensions ["board"][0].att ("scale"));

		//1.2 - bloque
		this.blockSize = float.Parse (dimensions ["block"][0].att ("size"));
		this.blockSeparation = float.Parse (dimensions ["block"][0].att ("separation"));

		//1.3 - cover
		this.coverSize = float.Parse (dimensions ["cover"][0].att ("size"));

		//1.4 - robot
		this.robotHeight = float.Parse (dimensions ["robot"][0].att("height"));


		//2.- Celdas por defecto
		this.default_height = int.Parse (xml ["board"] [0].att ("default_height"));
		this.initBoard (this.default_height, this.size_x, this.size_z);
		this.emptyCurrentBoard();


		//3.- Definimos de las celdas
		PT_XMLHashList tileX = xml["board"][0]["tile"];
		for (int i=0; i<tileX.Count; i++) {
			int x, z;
			x = int.Parse (tileX[i].att("x"));
			z = int.Parse (tileX[i].att("z"));


			this.board_def[x, z].height = int.Parse(tileX[i].att ("height"));

			string attr = (tileX[i].HasAtt("attr")) ? tileX[i].att ("attr") : "none";
			switch (attr){
			case "goal":
				this.board_def[x,z].type=TileType.goal;
				Director.S.totalGoals ++;
				break;
			default:
				this.board_def[x,z].type = TileType.normal;
				break;
			}
			this.board_def[x,z].isOn = false;

			//leemos movimientos.
			string mvto_t = (tileX[i].HasAtt("mvto")) ? tileX[i].att("mvto") : "none";
			switch( mvto_t){
			case "h":
				this.board_def[x,z].typeMvto = tipoMovimiento.horizontal;
				this.board_def[x,z].nSteps = int.Parse(tileX[i].att("nsteps"));
				this.board_def[x,z].createStates();
				break;
			case "v":
				this.board_def[x,z].typeMvto = tipoMovimiento.vertical;
				this.board_def[x,z].nSteps = int.Parse(tileX[i].att("nsteps"));
				this.board_def[x,z].createStates();
				break;
			case "f":
				this.board_def[x,z].typeMvto = tipoMovimiento.frente;
				this.board_def[x,z].nSteps = int.Parse(tileX[i].att("nsteps"));
				this.board_def[x,z].createStates();
				break;
			case "none":
				this.board_def[x,z].typeMvto = tipoMovimiento.none;
				break;
			}


			//Cada celda ocupa su posicion original al comenzar el juego
			this.current_board[x,z] = new PairInt(x, z);
		}

		//4.- Posicion inicial del robot.
		PT_XMLHashtable robot = xml["robot"][0]["position"][0];
		this.robotPosition = new Vector3(float.Parse(robot.att("x")),
		                                 float.Parse(robot.att("y")),
		                                 float.Parse(robot.att("z")));
		robot = xml["robot"][0]["rotation"][0];
		this.robotRotation = new Vector3(float.Parse(robot.att("x")),
		                                 float.Parse(robot.att("y")),
		                                 float.Parse(robot.att("z")));

		//Transformamos la rotacion en el intervalo [0,360)
		while(this.robotRotation.y < 0){
			this.robotRotation.y += 360;
		}
		while(this.robotRotation.y >=360){
			this.robotRotation.y -= 360;
		}
	}


	/*
	 * Dibuja los bloques del tablero segun la definicion guardada en this.board_def.
	 * Agrupa todos los elementos en un layout. Se supone que se ha invocado
	 * con anterioridad a resetLayout() que borra todos los hijos del anchor
	 * (es decir, borra todos los bloque creados)
	 */
	private void drawlayout(){
		GameObject tile;
		for (int i=0; i<this.size_x; i++) {
			for (int j=0; j<this.size_z; j++) {
				if(this.board_def[i,j].type == TileType.none)
					continue;

				//Creamos el tile
				tile = createTile(this.board_def[i,j].height, this.board_def[i,j].type == TileType.goal, i, j);
				tile.name = "" + i + "x" + j;
				//posicion segun la i, j
				tile.transform.position = new Vector3(i*(2*this.blockSize + this.blockSeparation), 0,
				                                      j*(2*this.blockSize + this.blockSeparation));
				//hijo del anchor
				tile.transform.parent = this.boardAnchor.transform;

				//lo guardamos en el mapa
				this.board[i,j] = tile;
			}
		}
	}

	/** 
	 * Dibuja al robot en la posicion original leida del xml.
	 * El robot viene definido por una posicion inicial, y una direccion
	 * a la que se encuentra mirando.
	 */
	private void drawRobot(){
		GameObject robot = Instantiate(this.robot_prefab) as GameObject;

		//Colocar en la posicion y la rotacion
		robot.transform.position = new Vector3(this.robotPosition.x*(2*this.blockSize + this.blockSeparation),
		                                       (this.robotPosition.y-1)*(this.blockSize + this.blockSeparation) + 
		                                       		this.robotHeight + this.blockSize/2 + this.coverSize,
		                                       this.robotPosition.z*(2*this.blockSize + this.blockSeparation));

		robot.transform.rotation = Quaternion.Euler(this.robotRotation);

		//Calculamos la direccion a la que mira
		Vector3 dir;
		if(this.robotRotation.y < 90)
			dir = new Vector3(1,0,0);
		else if (this.robotRotation.y < 180)
			dir = new Vector3(0,0,-1);
		else if (this.robotRotation.y < 270)
			dir = new Vector3(-1,0,0);
		else 
			dir = new Vector3(0,0,1);

		((Movement)robot.GetComponent<Movement>()).dirAct = dir;
		((Movement)robot.GetComponent<Movement>()).rotAct = this.robotRotation;


		((Robot)robot.GetComponent<Robot> ()).init ((int)(this.robotPosition.x), (int)(this.robotPosition.y), 
		                                            (int)(this.robotPosition.z),
	                                            	(int)dir.x, (int)dir.z);

		this.robot = robot;
	}


	/*
	 * Resetea las variables necesarias para iniciar una nueva ejecucion.
	 */
	private void resetLayout(){
		this.board_def = new TileDef[MAX_SIZE, MAX_SIZE];
		this.current_board = new PairInt[MAX_SIZE, MAX_SIZE];

		if(this.board != null){
			for (int i=0; i<MAX_SIZE; i++)
				for (int j=0; j<MAX_SIZE; j++)
					Destroy(this.board[i,j]);
		}

		this.board = new GameObject[MAX_SIZE, MAX_SIZE];

		//Destruimos y volvemos a crear el boardAnchor
		if (this.boardAnchor != null) {
			Destroy (this.boardAnchor);
		}
		this.boardAnchor = new GameObject ();
		this.boardAnchor.name = "BoardAnchor";

		if(this.robot != null)
			Destroy(robot);

		Director.S.totalGoals = 0;
	}


	/*
	 * Inicia el tablero de tamanyo nxm con celdas de altura h.
	 * Si es una casilla fuera de n,m lo inicializa con una celda a none.
	 */
	private void initBoard (int h, int n, int m){
		TileDef td;
		for (int i=0; i<this.MAX_SIZE; i++) {
			for (int j=0; j<this.MAX_SIZE; j++) {
				td = new TileDef();
				td.x = i;
				td.z = j;
				td.height = h;


				if(i> n || j> m){ //fuera del tablero
					td.type = TileType.none;
					this.board_def[i,j] = td;
				} else { //Celda buena por defecto
					td.type = (h<=0) ? TileType.none : TileType.normal;
					this.board_def[i,j] = td;
				}
			}
		}
	}

	/*
	 * Inicia el tablero real/actual del juego. 
	 * En un principio se inicia a null ya que no hay ninguna casila colocada.
	 */
	private void emptyCurrentBoard(){
		for(int i=0; i<this.MAX_SIZE; i++){
			for(int j=0; j<this.MAX_SIZE; j++){
				this.current_board[i,j] = null;
			}
		}
	}

	
	/*
	 * Devuelve un objeto con una torre de bloques creada, de altura pasada por parametro.
	 * El atributo special indica si es una casilla objetivo o no
	 */
	private GameObject createTile(int height, bool special, int x, int z){

		GameObject tile = (this.board_def[x,z].typeMvto == tipoMovimiento.none) ? 
			new GameObject() : 
			Instantiate(this.tileMvto_prefab);


		//1.- Bloques: escala, hijos del tile, apilar encima.
		GameObject go;
		for (int i = 0; i<height; i++){
			go = Instantiate(this.tileBlock_prefab) as GameObject;
			go.transform.localScale = new Vector3(this.scale, this.blockSize, this.scale);
			go.transform.parent = tile.transform; //hijo del tile
			go.transform.position = new Vector3(0, i*(this.blockSize + this.blockSeparation), 0); //encima del anterior
			go.name = "block_" + i;
		}

		//2.- Cover
		go = special ? Instantiate (this.specialCover_prefab) : Instantiate (this.normalCover_prefab);
		go.transform.localScale = new Vector3 (this.scale, this.coverSize, this.scale);
		go.transform.parent = tile.transform;

		float h = (height - 1) * (this.blockSize + this.blockSeparation) + this.blockSize / 2f + this.coverSize / 2f; 
		go.transform.position = new Vector3(0, h, 0);
		go.name = "cover";


		//3.- Casillas de movimiento
		if(this.board_def[x,z].typeMvto != tipoMovimiento.none){
			if(this.board_def[x,z].typeMvto == tipoMovimiento.vertical)
				tile.GetComponent<TileMvto>().TAM = (this.blockSize) + this.blockSeparation;
			else
				tile.GetComponent<TileMvto>().TAM = (2*this.blockSize) + this.blockSeparation;


			tile.GetComponent<TileMvto>().initMvto(this.board_def[x,z].typeMvto, this.board_def[x,z].nSteps);
		}

		return tile;
	}

	            
	/*
	 * Ilumina la celda de la possicion (x,z) si esta es de tipo goal.
	 * Si estaba encendida, la apaga.
	 */
	public int SwitchLight(int x, int z){
		if (this.board_def [x, z].type != TileType.goal)
			return 0;

		TileDef td = this.board_def[x,z];
		GameObject go = this.board [x, z];

		Renderer rmat = null;
		foreach (Transform t in go.transform) {
			if (t.name == "cover"){
				rmat = t.gameObject.GetComponent<Renderer>();
			}
		}


		if (td.isOn == false) {
			rmat.material = this.onCover_material;
			td.isOn = true;

			return 1;
		}
		else{
			rmat.material = this.offCover_material;
			td.isOn = false;

			return -1;
		}
	}


	public void restartLayout(){
		this.resetLayout ();

		this.readLayout(texto.text);

		this.drawlayout ();
		this.drawRobot();
	}
}
