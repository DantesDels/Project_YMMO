# YMMO — Plateforme Immobilière

Bienvenue sur le dépôt du projet **YMMO**. Ce projet se compose d'une API backend (.NET 10) et d'un frontend (Vue 3 / Vite).

## 🚀 Guide de Démarrage

### 1. Lancer le Backend
Pour que l'API fonctionne correctement (et pour activer l'interface Swagger), le projet doit être lancé en environnement de **développement**.

Ouvre un terminal dans le dossier `YMMO_BackEnd` et exécute :

**Sur PowerShell (Windows) :**
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"; dotnet run
```

*Note : En mode Development, l'API est configurée pour utiliser appsettings.Development.json et activer Swagger.*

### 2. Accéder à l'interface Swagger
Une fois le backend lancé, indiqué par Now listening on:  
http://localhost:5000  
tu peux visualiser et tester tes endpoints API directement dans ton navigateur :  
**http://localhost:5000/swagger/index.html**


### 3. Lancer le Frontend
Dans un nouveau terminal, place-toi dans le dossier YMMO_FrontEnd et lance le serveur de développement :

```
cd ../YMMO_FrontEnd
npm install
npm run dev
```  
Ton interface est désormais accessible sur :  
http://localhost:5173/

--- 

💡 Architecture de communication
Le backend et le frontend communiquent via Axios.

L'API communique sur le port 5000.
Le Frontend communique sur le port 5173.

Assure-toi que les variables d'environnement (.env) dans le frontend pointent bien vers http://localhost:5000/api.