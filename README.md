# Pagans
entire norse mitology but in a 2d top-down rpg.

## Gameplay
use controller for a better experience.
scritto per funzionare su controller nes-like, Pagans usa 4 tasti direzionali (WASD, frecce direzionali o dpad) per il movimento, 2 per il game e 2 per i menu.

## movement
i movimenti non sono bloccati nella griglia, è possibile spostarsi di 1/4 di tassello ma non ci sono i movimenti diagonali.

## gaming binds
attualmente il battle system è gravemente in discussione ma probabilmente sceglierò di combattere come zelda.
i tasti quindi sono:
Z(A), X(B) rispettivamente per confermare e annullare
ma avendo una mappatura gba facciamo gli override per state.

### Exploring:
abbiamo 4 tasti:

- start (Enter) -> open menu
- select (tab)  -> null
- A (Z)         -> Spada, (se possibile) Parla
- B (X)         -> Scudo
- [temp] RShoulder (E) -> Use

COME USO L'OGGETTO EQUIPAGGIATO? sul computer con E, sul controller con le shoulder? :/

### UI
- start (Enter) -> back to freeroam
- select (Tab)  -> cambia la pagina se sei su Bag
- A (Z)         -> scegli, conferma
- B (X)         -> indietro

# Gaming Functionalities
during exploration:

immaginiamo la scena, hai l'inventario vuoto e nessun'arma equipaggiata ma devi sconfiggere un nemico. cosa fai?
1. equipaggio un'arma (per forza perchè subito dopo l'avvio ottieni la spada di tuo padre.)
2. attacco
Fallisco?
1. Potenzio l'arma
2. incanto l'arma

come si potenzia?
- vai da un nano e paghi
- vai ad un'altare con un libro e una gemma

libri:
i libri si trovano nei dungeon o nelle librerie. sono gratis.

gemme:
le gemme sono pietre preziose che lasciano cadere i satiri quando muoiono, ma posso essere comprate da alcuni nani.

## ordinamento degli oggetti
ci sono 3 UI:
- inventario
- equipaggiamento
- quests

nell'inventario ci devono stare:
- i consumabili (cibo, pozioni...)
- gli oggetti (chiavi, frammenti)
- le cose da vendere (legna)

invece nell'equipment si hanno gli oggetti collegati ai tasti. noi usiamo Z, X, E (cioè arma-scudo-usa).
vuol dire che qui ci sono 3 categorie (+1 per gli anelli):
- Armi
- Scudi
- Strumenti
- Anelli

nel player controller quando viene premuto (ad esempio) Z (o A sui controller) si parla con qualcuno, ma se davanti non c'è nessuno invece di fare Interact() si fa Attack().
Attack() usa l'arma equipaggiata e fa .Use(), abbiamo Weapons che è la lista delle armi che possediamo e equipedWeapon CHE E' UN INTERO, corrisponde all'*indice* dell'arma equipaggiata. quindi in Attack() per trovare la nostra arma facciamo: equipment.Weapons[equipment.equipedWeapon] che corrisponde alla nostra arma ma come InvSlot quindi .item.Use().

## ordinamento delle UI
nel menu ci sono 3 opzioni:
- Codex
- Bag
- Exit

 ### Codex
 è un nome temporaneo visto che non è in realtà un bestiario. qui ci vanno tutti i fogli e le storie che troviamo/apprendiamo esplorando.
 
 usano la grafica di page-with-border
 
 ### Bag
 questo è l'inventario. da qui si apre L'INVENTARIO. per aprire L'EQUIPAGGIAMENTO bisogna sfogliare le pagine dell'inventario quindi premendo select(tab)
