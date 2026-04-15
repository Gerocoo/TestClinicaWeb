# Test Tecnico - Gestione Esami Clinici

Questa repository contiene la soluzione al test tecnico per la gestione, ricerca e prenotazione di esami clinici. 
Il progetto è suddiviso in due interfacce distinte che condividono la stessa logica di dominio, ma adattate ai rispettivi contesti tecnologici e di utilizzo:

1. **App Desktop:** C# Windows Forms (.NET Framework / .NET Core)
2. **App Web:** ASP.NET Core MVC & Vanilla JS

## Istruzioni per l'avvio

* **Database:** Nella root del progetto è presente lo script `DBTest.sql`. È sufficiente eseguirlo in SQL Server per generare il database `TestClinica` con la struttura e i dati fittizi necessari.
* **Stringa di connessione:** Entrambi i progetti puntano di default all'istanza `localhost\SQLEXPRESS` con `Integrated Security=True`. In caso di server differente, aggiornare la stringa di connessione presente in:
  * *Desktop:* `Form1.cs`
  * *Web:* `HomeController.cs`

---

# Scelte Architetturali e Progettuali

Durante lo sviluppo si è evitato di replicare in modo identico la logica tra i due ambienti, preferendo adattare le funzionalità ai rispettivi casi d'uso:

# Gestione dei duplicati:
* **Desktop:** L'applicazione Desktop è pensata per un operatore che compila una singola prescrizione, dove un esame inserito due volte è verosimilmente un errore. Pertanto, l'inserimento di duplicati nella griglia di riepilogo è stato disabilitato.
* **Web:** L'applicazione Web, concepita come portale di prenotazione, deve consentire all'utente di prenotare lo stesso esame più volte (es. per date o ambulatori differenti, logiche non implementate in quanto non richieste dal test).

# Ordinamento della lista: Drag & Drop vs Bottoni
* **Web:** È stato implementato un sistema di ordinamento tramite **Drag & Drop nativo HTML5**, per offrire un'esperienza utente fluida e moderna.
* **Desktop:** Sono stati mantenuti i classici bottoni "Su/Giù". Forzare il drag & drop nativo sulle righe di una `DataGridView` in WinForms richiede codice di basso livello basato sugli eventi del mouse che risulterebbe inutilmente complesso e fragile in un contesto gestionale, dove l'uso di bottoni espliciti rappresenta uno standard molto più solido.

### 3. Gestione delle Configurazioni
Per il caricamento dei filtri di ricerca predefiniti:
* **Desktop:** Legge il file `config.ini` tramite Reflection (come da requisiti). Questo permette di mappare i parametri dinamicamente, rendendo il codice scalabile senza l'uso di if/else rigidi.
* **Web:** Utilizza il file `appsettings.json` iniettato direttamente nel Controller, in perfetta linea con gli standard odierni di ASP.NET Core.

---

## Accorgimenti e Funzionalità Aggiuntive

Oltre ai requisiti minimi di base, sono state integrate alcune funzionalità per garantire la robustezza del software:

* **Gestione Graceful degli errori DB:** Se il database non è raggiungibile all'avvio, le applicazioni non vanno in crash (YSOD o eccezioni non gestite), ma catturano l'eccezione mostrando un messaggio di errore all'utente chiaramente formattato.
* **Esportazione Dati:** Implementazione del download della lista degli esami selezionati in formato `.csv` (gestito tramite Blob Javascript nel Web e `SaveFileDialog` nel Desktop).
* **UI Asincrona (Web):** Le liste (select) dipendenti/a cascata vengono popolate tramite chiamate AJAX (`fetch()`) ad endpoint JSON dedicati, evitando qualsiasi ricaricamento intero della pagina.