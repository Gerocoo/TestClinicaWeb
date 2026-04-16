# Test Tecnico - Gestione Esami Clinici (App Web)

Questa repository contiene l'implementazione Web del test tecnico per la gestione, ricerca e prenotazione di esami clinici, sviluppata in ASP.NET Core MVC e Vanilla JavaScript.

## Istruzioni per l'avvio

* **Database:** Nella root del progetto è presente lo script `DBTest.sql`. È sufficiente eseguirlo in SQL Server per generare il database `TestClinica` con la struttura e i dati fittizi necessari.
* **Stringa di connessione:** Il progetto punta di default all'istanza `localhost\SQLEXPRESS` con `Integrated Security=True`. In caso di server differente, aggiornare la stringa di connessione presente all'interno del file `appsettings.json` o direttamente nel `HomeController.cs`.

---

## Scelte Architetturali e Progettuali

### 1. Gestione dei duplicati (Consentita)
L'applicazione Web, concepita come portale di prenotazione rivolto all'utente o al paziente, deve consentire di prenotare lo stesso esame più volte (ad esempio per monitoraggi a distanza di tempo, o in ambulatori e date differenti, logiche non implementate in quanto non richieste dal test). Pertanto, a differenza del client Desktop, la validazione anti-duplicato non è stata forzata.

### 2. Ordinamento della lista (Drag & Drop HTML5)
È stato implementato un sistema di ordinamento tramite Drag & Drop nativo HTML5 sulle righe della tabella. Questo approccio, combinato con Vanilla JS, offre un'esperienza utente fluida e moderna, perfettamente in linea con gli standard delle interfacce web attuali.

### 3. Gestione delle Configurazioni
A differenza della versione Desktop (che utilizza un parser custom per i file INI tramite Reflection come da requisiti specifici WinForms), per l'applicazione Web si è scelto di non replicare quell'architettura in quanto considerata un anti-pattern in questo ecosistema.
L'applicazione si appoggia al file `appsettings.json` iniettato nativamente tramite Dependency Injection, allineando il portale agli standard di configurazione di ASP.NET Core MVC.

---

## Accorgimenti e Funzionalità Aggiuntive

* **UI Asincrona:** Le liste dipendenti a cascata (Ambulatori -> Parti del Corpo -> Esami) vengono popolate tramite chiamate AJAX asincrone (`fetch()`) ad endpoint JSON dedicati nel Controller. Questo evita qualsiasi ricaricamento intero della pagina, azzerando il flickering.
* **Esportazione Dati:** È stata implementata la funzionalità di download degli esami scelti in formato `.csv`, generata dinamicamente via Javascript sfruttando le API dei Blob.
* **Gestione Graceful degli errori DB:** Qualora il database non fosse raggiungibile, il sistema intercetta l'eccezione evitando il classico crash (YSOD).

