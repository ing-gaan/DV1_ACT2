# Solución Actividad 2: Un Juego De Plataformas

## Introducción

El juego que se presenta en este repositorio se llama ***The white knight rises*** y trata de la lucha de un caballero para conseguir llegar a la meta que esta representada por una bandera. En su camino tendrá que usar su espada para pelear contra zombis y seres con cabeza de calabaza; además, tendrá la oportunidad de recoger algunas monedas y la obligación de recolectar algunas llaves para abrir las puertas que le impiden su avance.

## Mecánicas

El videojuego es de plataformas que cuenta con unas mecánicas básicas que consisten en:

- El jugador puede caminar (Flechas a los lados), correr (Flechas a los lados con shift izquierdo), saltar (Flecha arriba) y atacar (Barra espaciadora); además, tiene una cantidad de vidas limitadas.
- El jugador pierde una vida cada vez que cae en una zona que no es tierra firme o plataforma; también, cuando un enemigo lo toca.
- El jugador gana un punto cada vez que recolecta una moneda. Algunas monedas están detrás de cajas, las cuales se pueden destruir con cuatro ataques.
- El jugador tiene que recoger las llaves para poder seguir avanzando; de manera que, cada puerta consume una llave. 
- Los enemigos caminan horizontalmente en zonas restringidas, si el jugador esta cerca aumentan su velocidad excepto si se aproxima por la espalda.
- Los enemigos zombis mueren saltando y atacando para propinarles un golpe en la cabeza.
- Los enemigos con cabeza de calabaza mueren atacándolos de cualquier manera.



## Diseño, desarrollo e implementación

### Carga de objetos

El juego se divide en tres escenas, *Menu*, *Level_1* y *GameOver*. Desde menú se lanza el juego y desde allí se accede a la escena *GameOver* cuando el jugador pierde las vidas disponibles o cuando llega a la meta. Por último, desde *GameOver* se puede volver a juagar o salir del juego. En *Level_1* se desarrolla todo el juego que al iniciar muestra al personaje principal y unas instrucciones básicas de las mecánicas; además se cargan:

- *Background* que se compone de varias imágenes que se van moviendo en relación al personaje con la intención de provocar un efecto *parallax*.
- *TileMaps* que consisten en varios tipos que a su vez están dentro de un grupo, de modo que hay cuatro *TileMap* ***Ground*** que componen el suelo, dos ***Gaps*** que componen la zona donde el jugador muere si cae, dos ***Platforms*** donde se incluyen los suelos elevados y dos ***Clouds*** que conforman las plataformas más elevadas y que son resbaladizas.
- Los textos con las vidas del jugador, las monedas y llaves recolectadas.
- Un *GameObject* (*Player*) donde se enlazan los distintos componentes que dan vida al personaje que controla el jugador.

Adicionalmente, a medida que avanza el jugador se van cargando los distintos elementos y enemigos, para ello se usó un archivo de texto que contiene el nombre del elemento junto con sus coordenadas; entonces, al iniciar el juego se cargan y se almacenan dichas coordenadas en listas de tipo *Vector3* y cada vez que el jugador alcanza cierta posición en x, se instancian clones de los *prefabs* de los elementos. Además, cada vez que se recoge o destruye un elemento o enemigo se destruye el GameObject creado.

### Comportamiento de los Objetos.

Vale la pena mencionar que el personaje (***Player***), los enemigos (***Pumpkin***, ***ZombieFemale*** y ***ZombieMale***) y los objetos (***Box***, ***Coin***, ***Door*** y ***Key***) son tratados como prefabs.

#### Jugador

El personaje principal cuenta con los siguientes componentes:

- ***SpriteRenderer***.
- ***Animator*** que controla siete animaciones distintas.
- ***Rigidbody2D*** dinámico para simular las físicas del jugador mediante la manipulación de la velocidad (desplazamientos) y de la fuerza de impulso (saltos).
- ***BoxCollider2D*** para detectar las colisiones con los objetos, enemigos y TileMaps.
- ***AudioSource*** para reproducir algunos sonidos como los golpes, la recolección de objetos y la muerte.

Aparte de los anteriores, también se enlaza un *script* (*Player*) donde se incluyen los métodos para detección de colisiones, el control de movimiento, el control de las vidas, el control de los golpes, entre otros. Además, le incorpora un Raycast al personaje para determinar cuando está cerca de las cajas que se pueden romper y de las puertas.

#### Enemigos

Los enemigos cuentan con los siguientes componentes:

- ***SpriteRenderer***.
- ***Animator*** que controla seis animaciones distintas en el caso de *Pumpkin* y cuatro en el caso de los zombis.
- ***Rigidbody2D*** dinámico para simular los moviminetos del enemigo mediante la manipulación de la velocidad (desplazamientos).
- ***BoxCollider2D*** para detectar las colisiones con el jugador, los objetos y los *TileMaps*.
- ***AudioSource*** para reproducir el sonido de muerte.

Aparte de los anteriores, también se les enlaza un *script* (*Enemy*) donde se incluyen los métodos para detección de colisiones, el control de movimiento, entre otros. Además, le incorpora un Raycast al enemigo para detectar vacíos y prevenir las caídas.

#### Objetos

Dependiendo del tipo, los objetos cuentan con algunos de los siguientes componentes:

- ***SpriteRenderer***.
- ***Animator*** que controla las distintas animaciones de los objetos, excepto en el caso de Key.
- ***Rigidbody2D*** dinámico para simular la gravedad de las cajas (Box).
- ***BoxCollider2D*** (CircleCollider2D en el caso de Coin) para detectar las colisiones o la cercanía con el jugador.
- ***AudioSource*** para reproducir el sonido de golpe y explosión (Box) o el sonido de apertura (Door).

Aparte de los anteriores, también se enlaza un *script* a algunos objetos como en el caso de *Box* (*script Box*) donde se incluyen métodos para el control de los golpes recibidos y su destrucción, y en el caso de *Door* (*script Door*) donde se incluye el método para el control de apertura.

### Clases y estructuras

A parte de las clases mencionas anteriormente que dan comportamiento a los distintos *GameObject*, también se crearon unas clases y enumeraciones de apoyo, entre las que se encuentran:

- ***MoveController*** que es la clase donde se apoyan *Player* y *Enemy* para controlar los movimientos de los personajes; para lo cual hace uso de otra clase donde se almacenan distintas constantes (*Constants*) y una enumeración (*CharacterType*) para el tipo de personaje.
- ***ButtonsActions*** que contiene las funcionalidades de los distintos botones.
- ***CameraFollow*** que se encarga del movimiento de la cámara basándose en el movimiento del jugador; para ello, entre otras cosas, se hace uso de la función Lerp para hacer los desplazamientos más suaves. Además, se controla el zoom de acuerdo a la velocidad del jugador o si se oprime la tecla Z.
- ***Parallax*** para controlar el efecto *parallax* de cada imagen del *Background* en relación al movimiento de la cámara.
- ***GameManager*** que controla el desarrollo del juego y que cuenta con métodos para cargar las posiciones de los objetos desde un archivo de texto, instanciar los objetos en base a las posiciones cargadas y la posición del jugador, entre otros. Asimismo, se incluyen tres diccionarios que almacenan las instancias de los enemigos, cajas y puertas; donde sus llaves corresponden a sus *GameObject* y sus valores a las clases/*scripts* que les dan comportamiento. 

## Referencias

#### Referencias de los objetos del juego

***Tilesets:***
- Free Graveyard Platformer Tileset. Game Art 2D. https://www.gameart2d.com/free-graveyard-platformer-tileset.html

***Sprites:***

- The knight - free sprites. Game Art 2D. https://www.gameart2d.com/the-knight-free-sprites.html
- Jack o' lantern. Game Art 2D. https://www.gameart2d.com/jack-o-lantern-free-sprites.html
- the zombies. Game Art 2D. https://www.gameart2d.com/the-zombies-free-sprites.html
- Broken boxes. Open Gameart. https://opengameart.org/content/broken-boxes
- LPC animated doors. Open Gameart. https://opengameart.org/content/lpc-animated-doors
- Bomb explosion animation. Open Gameart. https://opengameart.org/content/bomb-explosion-animation
- Coin animation. Open Gameart. https://opengameart.org/content/coin-animation
Flag animation sheet. Open Gameart. https://opengameart.org/content/flag-animation-sheet

***Backgrounds:***
- Halloween 2D game backgrounds. Craftpix. https://craftpix.net/freebies/free-halloween-2d-game-backgrounds/
- Vía láctea estrellas cielo nocturno. Pixabay. https://pixabay.com/es/photos/v%C3%ADa-l%C3%A1ctea-estrellas-cielo-nocturno-2695569/


***Sonidos:***

- 512 sound effects 8 bit style. Open Gameart. https://opengameart.org/content/512-sound-effects-8-bit-style
- Zombies sound pack. Open Gameart. https://opengameart.org/content/zombies-sound-pack

***Musica***
- 5 chiptunes action. Open Gameart. https://opengameart.org/content/5-chiptunes-action

#### Referencias con informacion
- Unity user manual. Unity3D. https://docs.unity3d.com/Manual/index.html
- Unity Scripting Reference. Unity3D. https://docs.unity3d.com/ScriptReference/index.html
- Smooth Camera Follow in Unity - Tutorial. (2017, 28 junio). [Vídeo]. YouTube. https://www.youtube.com/watch?v=MFQhpwc6cKE
