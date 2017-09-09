using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGeneration : MonoBehaviour
{
		//Largo tablero
		public int largoTablero;

		//Punto de partida del tablero
		public Vector2 puntoPartida;

		//Objeto que representa el suelo
		public GameObject suelo;

		//Objeto que representa una pared
		public GameObject pared;

		//Representa al jugador
		public GameObject jugador;

		public GameObject startPoint;

		public GameObject endPoint;
			
		//Representa el tiempo para que ocurra cada acción en el algoritmo.
		public float tiempoTransicion = 1f;

		//Matriz de números - Representación interna del tablero (Celdas y paredes)
		private int[,] tablero;

		//Matriz de puntos para marcar las casillas visitadas.
		private List<Vector2> celdasVisitadas = new List<Vector2> ();

		//Matriz de objetos - Tablero
		private GameObject[,] celdas;
		
		void Awake ()
		{
				//Random.seed = 0;
				
				//Para poder hacer las paredes de las celdas es necesario multiplicar el largo por 2.
				largoTablero *= 2;

				largoTablero -= 1;

				//Se crean datos a usar
				tablero = new int[largoTablero, largoTablero];	
				celdas = new GameObject[largoTablero, largoTablero];

				//Por cada una de las posiciones del tablero.
				for (int i = 0; i < largoTablero; i++) {
						for (int j = 0; j<largoTablero; j++) {	
								//Se calcula la posición en la que debe ubicarse la celda nueva.
								Vector2 nuevaPos = new Vector2 (puntoPartida.x + i, puntoPartida.y - j);
								GameObject go;
						
								//Solo si tanto la fila como la columna corresponden a una celda normal
								//se marca 1, si es pared se marca 2.
								if (j % 2 == 0 && i % 2 == 0) {
										tablero [i, j] = 1;
										go = Instantiate (suelo, nuevaPos, Quaternion.identity) as GameObject;
								} else {
										tablero [i, j] = 2;
										go = Instantiate (pared, nuevaPos, Quaternion.identity) as GameObject;
								}
								//Se almacena en la matriz de celdas (para almacenar la referencia real).
								celdas [i, j] = go;
						}
				}

				int x = (int)puntoPartida.x - 1;
				int y = (int)puntoPartida.y + 1;
				//Poniendo marco
				for (int i = 0; i < largoTablero; i++) {
						GameObject go;
						go = Instantiate (pared, new Vector2 (puntoPartida.x + i, y), Quaternion.identity) as GameObject;
						go = Instantiate (pared, new Vector2 (puntoPartida.x + i, y - largoTablero - 1), Quaternion.identity) as GameObject;
						go = Instantiate (pared, new Vector2 (x, puntoPartida.y - i), Quaternion.identity) as GameObject;
						go = Instantiate (pared, new Vector2 (x + largoTablero + 1, puntoPartida.y - i), Quaternion.identity) as GameObject;
				}
				ConstruirLaberintos ();
		        //Comentar el método 'ConstruirLaberintos' antes de usar el método 'Construir Maze'.
		        //StartCoroutine ("ConstruirMaze"); -> Utilizar si se quiere ver todo de manera progresiva.
	}

		void ConstruirLaberintos ()
		{
				//Declaracion de variables.
				Stack celdasProbadas = new Stack ();
				//Punto aleatorio de partida.
				int x = Random.Range (0, largoTablero);
				int y = Random.Range (0, largoTablero);
				//Si el punto aleatorio hace referencia a un muro, se prueba otro.
				while (tablero[x,y]==2) {
						//Punto aleatorio.
						x = Random.Range (0, largoTablero);
						y = Random.Range (0, largoTablero);
				}
		
				//Total de celdas a probar. ((length/2)²)
				largoTablero += 1;
				int totalCeldas = largoTablero / 2 * largoTablero / 2;
				largoTablero -= 1;

				//Vector que almacena la posicion de la celda que esta siendo probada.
				Vector2 celdaActual;

				//Se instancia al jugador para poder ver como es resuelto el tablero.
				Vector2 posPlayer = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
				GameObject player = Instantiate (jugador, posPlayer, Quaternion.identity) as GameObject;
				GameObject start = Instantiate (startPoint, posPlayer, Quaternion.identity) as GameObject;

				Vector2 posEnd = Vector2.zero;
				
				//Se añade el primer punto visitado a las celdas visitadas.
				celdasVisitadas.Add (new Vector2 (x, y)); 
		
				//Cantidad inicial de celdas visitadas.
				int cantCeldasVis = 1;

				//Representa la sucesion de direcciones tomadas por el algoritmo.
				Stack direcciones = new Stack ();

				//Mientras la cantidad de celdas visitadas no exceda la cantidad todal de celdas del tablero...
				while (cantCeldasVis < totalCeldas) {
						//Stack para las paredes.
						Stack paredes = new Stack ();
			
						//Si es izquierda == 0
						if (x > 1) {
								if (tablero [x - 1, y] == 2 && tablero [x - 2, y] == 1 && !celdasVisitadas.Contains (new Vector2 (x - 2, y)))
										paredes.Push (0);
						}
			
						//Si es arriba == 1
						if (y < largoTablero - 2) {
								if (tablero [x, y + 1] == 2 && tablero [x, y + 2] == 1 && ! celdasVisitadas.Contains (new Vector2 (x, y + 2)))
										paredes.Push (1);
						}
			
						//Si es derecha == 2
						if (x < largoTablero - 2) {
								if (tablero [x + 1, y] == 2 && tablero [x + 2, y] == 1 && !celdasVisitadas.Contains (new Vector2 (x + 2, y)))
										paredes.Push (2);
						}
			
						//Si es abajo == 3
						if (y > 1) {
								if (tablero [x, y - 1] == 2 && tablero [x, y - 2] == 1 && !celdasVisitadas.Contains (new Vector2 (x, y - 2)))
										paredes.Push (3);
						}

						//Si la celda tiene paredes alrededor...
						if (paredes.Count > 0) {
								//Se escoge una al azar.
								int aux = Random.Range (0, paredes.Count);
								int pared = 0;
								for (int k = 0; k<=aux; k++) {
										pared = (int)paredes.Pop ();
								}
				
								GameObject go;

								//Si la pared escogida es la izquierda
								if (pared == 0) {
										x -= 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;
					
										x -= 1;
										direcciones.Push (0);
								}

								//Si la pared escogida es la arriba
								if (pared == 1) {
										y += 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;

										y += 1;
										direcciones.Push (1);
								}

								//Si la pared escogida es la derecha
								if (pared == 2) {
										x += 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;
										
										x += 1;
										direcciones.Push (2);
								}

								//Si la pared escogida es la abajo
								if (pared == 3) {
										y -= 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;
										
										y -= 1;
										direcciones.Push (3);
								}
				
								//Meter celda al stack cola
								celdasProbadas.Push (new Vector2 (x, y));

								//Mover al jugador a la celda
								posPlayer = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);	
								jugador.transform.position = posPlayer;
								
								posEnd = posPlayer;
				
								//Asignar la celda actual
								celdaActual = (Vector2)celdasProbadas.Peek ();

								//Si el punto que se quiere visitar no esta en las celdas visitadas.
								if (!celdasVisitadas.Contains (new Vector2 (x, y))) {
										celdasVisitadas.Add (new Vector2 (x, y));
										cantCeldasVis++;
								}
				
						} else {//Si no hay caminos posibles para la celda actual entonces...
								//Se actualiza la celda actual a la última celda visitada antes de la actual 
								celdaActual = (Vector2)celdasProbadas.Pop ();
								//Se almacena la última dirección antes tomada antes de la celda actual
								int aux = (int)direcciones.Pop ();
								int auxX = 0;
								int auxY = 0;
				
								x = (int)celdaActual.x;				
								y = (int)celdaActual.y;

								//Si fue izquierda
								if (aux == 0) {
										auxX = x + 1;
										auxY = y;
								}

								//Si fue arriba
								if (aux == 1) {
										auxY = y - 1;
										auxX = x;
								}

								//Si fue derecha
								if (aux == 2) {
										auxX = x - 1;
										auxY = y;
								}

								//Si fue abajo
								if (aux == 3) {
										auxY = y + 1;
										auxX = x;
								}
				
								//Se calcula la posición en la que debe ubicarse la celda nueva para devolverse.
								Vector2 newPos = new Vector2 (puntoPartida.x + celdaActual.x, puntoPartida.y - celdaActual.y);
								Vector2 newAuxPos = new Vector2 (puntoPartida.x + auxX, puntoPartida.y - auxY);
				
								GameObject go;
				
								//Solo si tanto la fila como la columna corresponden a una celda normal
								//se marca 1, si es pared se marca 2.
								if (x % 2 == 0 && y % 2 == 0) {
										tablero [x, y] = 1;
								}
				
								jugador.transform.position = newPos;
								if (auxX % 2 == 0 && auxY % 2 == 0) {
										tablero [auxX, auxY] = 1;
								}
				
								jugador.transform.position = newAuxPos;
						}
				}
				GameObject end = Instantiate (endPoint, posEnd, Quaternion.identity) as GameObject;
				Debug.Log ("Nivel terminado. Time=" + Time.timeSinceLevelLoad);
		}

		IEnumerator ConstruirMaze ()
		{
				//Declaracion de variables.
				Stack celdasProbadas = new Stack ();
				//Punto aleatorio de partida.
				int x = Random.Range (0, largoTablero);
				int y = Random.Range (0, largoTablero);
				//Si el punto aleatorio hace referencia a un muro, se prueba otro.
				while (tablero[x,y]==2) {
						//Punto aleatorio.
						x = Random.Range (0, largoTablero);
						y = Random.Range (0, largoTablero);
				}
		
				//Total de celdas a probar. ((length/2)²)
				largoTablero += 1;
				int totalCeldas = largoTablero / 2 * largoTablero / 2;
				largoTablero -= 1;
		
				//Vector que almacena la posicion de la celda que esta siendo probada.
				Vector2 celdaActual;
		
				//Se instancia al jugador para poder ver como es resuelto el tablero.
				Vector2 posPlayer = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
				GameObject player = Instantiate (jugador, posPlayer, Quaternion.identity) as GameObject;
				GameObject start = Instantiate (startPoint, posPlayer, Quaternion.identity) as GameObject;
		
				Vector2 posEnd = Vector2.zero;

				yield return new WaitForSeconds (tiempoTransicion);
		
				//Se añade el primer punto visitado a las celdas visitadas.
				celdasVisitadas.Add (new Vector2 (x, y)); 
		
				//Cantidad inicial de celdas visitadas.
				int cantCeldasVis = 1;
		
				//Representa la sucesion de direcciones tomadas por el algoritmo.
				Stack direcciones = new Stack ();
		
				//Mientras la cantidad de celdas visitadas no exceda la cantidad todal de celdas del tablero...
				while (cantCeldasVis < totalCeldas) {
						//Stack para las paredes.
						Stack paredes = new Stack ();
			
						//Si es izquierda == 0
						if (x > 1) {
								if (tablero [x - 1, y] == 2 && tablero [x - 2, y] == 1 && !celdasVisitadas.Contains (new Vector2 (x - 2, y)))
										paredes.Push (0);
						}
			
						//Si es arriba == 1
						if (y < largoTablero - 2) {
								if (tablero [x, y + 1] == 2 && tablero [x, y + 2] == 1 && ! celdasVisitadas.Contains (new Vector2 (x, y + 2)))
										paredes.Push (1);
						}
			
						//Si es derecha == 2
						if (x < largoTablero - 2) {
								if (tablero [x + 1, y] == 2 && tablero [x + 2, y] == 1 && !celdasVisitadas.Contains (new Vector2 (x + 2, y)))
										paredes.Push (2);
						}
			
						//Si es abajo == 3
						if (y > 1) {
								if (tablero [x, y - 1] == 2 && tablero [x, y - 2] == 1 && !celdasVisitadas.Contains (new Vector2 (x, y - 2)))
										paredes.Push (3);
						}
			
						//Si la celda tiene paredes alrededor...
						if (paredes.Count > 0) {
								//Se escoge una al azar.
								int aux = Random.Range (0, paredes.Count);
								int pared = 0;
								for (int k = 0; k<=aux; k++) {
										pared = (int)paredes.Pop ();
								}
				
								GameObject go;
				
								//Si la pared escogida es la izquierda
								if (pared == 0) {
										x -= 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;
					
										x -= 1;
										direcciones.Push (0);
								}
				
								//Si la pared escogida es la arriba
								if (pared == 1) {
										y += 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;
					
										y += 1;
										direcciones.Push (1);
								}
				
								//Si la pared escogida es la derecha
								if (pared == 2) {
										x += 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;
					
										x += 1;
										direcciones.Push (2);
								}
				
								//Si la pared escogida es la abajo
								if (pared == 3) {
										y -= 1;
										tablero [x, y] = 1;
										DestroyImmediate (celdas [x, y]);
										Vector2 newPos = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);
										jugador.transform.position = newPos;
										go = Instantiate (suelo, newPos, Quaternion.identity) as GameObject;
										celdas [x, y] = go;
					
										y -= 1;
										direcciones.Push (3);
								}
				
								//Meter celda al stack cola
								celdasProbadas.Push (new Vector2 (x, y));
				
								//Mover al jugador a la celda
								posPlayer = new Vector2 (puntoPartida.x + x, puntoPartida.y - y);	
								jugador.transform.position = posPlayer;
				
								posEnd = posPlayer;

								yield return new WaitForSeconds (tiempoTransicion);
				
								//Asignar la celda actual
								celdaActual = (Vector2)celdasProbadas.Peek ();
				
								//Si el punto que se quiere visitar no esta en las celdas visitadas.
								if (!celdasVisitadas.Contains (new Vector2 (x, y))) {
										celdasVisitadas.Add (new Vector2 (x, y));
										cantCeldasVis++;
								}
				
						} else {//Si no hay caminos posibles para la celda actual entonces...
								//Se actualiza la celda actual a la última celda visitada antes de la actual 
								celdaActual = (Vector2)celdasProbadas.Pop ();
								//Se almacena la última dirección antes tomada antes de la celda actual
								int aux = (int)direcciones.Pop ();
								int auxX = 0;
								int auxY = 0;
				
								x = (int)celdaActual.x;				
								y = (int)celdaActual.y;
				
								//Si fue izquierda
								if (aux == 0) {
										auxX = x + 1;
										auxY = y;
								}
				
								//Si fue arriba
								if (aux == 1) {
										auxY = y - 1;
										auxX = x;
								}
				
								//Si fue derecha
								if (aux == 2) {
										auxX = x - 1;
										auxY = y;
								}
				
								//Si fue abajo
								if (aux == 3) {
										auxY = y + 1;
										auxX = x;
								}
				
								//Se calcula la posición en la que debe ubicarse la celda nueva para devolverse.
								Vector2 newPos = new Vector2 (puntoPartida.x + celdaActual.x, puntoPartida.y - celdaActual.y);
								Vector2 newAuxPos = new Vector2 (puntoPartida.x + auxX, puntoPartida.y - auxY);
				
								GameObject go;
				
								//Solo si tanto la fila como la columna corresponden a una celda normal
								//se marca 1, si es pared se marca 2.
								if (x % 2 == 0 && y % 2 == 0) {
										tablero [x, y] = 1;
								}
				
								jugador.transform.position = newPos;
								if (auxX % 2 == 0 && auxY % 2 == 0) {
										tablero [auxX, auxY] = 1;
								}
				
								jugador.transform.position = newAuxPos;

								yield return new WaitForSeconds (tiempoTransicion);
						}
				}
				GameObject end = Instantiate (endPoint, posEnd, Quaternion.identity) as GameObject;
				Debug.Log ("Nivel terminado. Time=" + Time.timeSinceLevelLoad);
		}
}
