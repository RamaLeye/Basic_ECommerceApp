# ECommerceApp

Cette application gère un e-Commerce implementé à l'aide de RabbitMQ et composé de 3 services ( stock, user, bill) et d'une application principale. Chaque service possède son manager et son SDK.
Les clients RPC se trouvent dans la librairie de classes "RPC". 

Execution depuis le PowerShell : 

- cd UserManagement 
	puis dotnet run

- cd StockManagement
	puis dotnet run

- cd MyCommerceApp
	puis dotnet run

	
Nous retrouvons dans l'application principale MyCommerceApp l'ensemble des étapes suivantes : 

	Authentification à partir de l'input d'un username qui sera verifié par un le service d'utilisateurs depuis un fichier Json.
	Affichage des produits à partir du service de stock depuis un fichier JSON
	Shopping avec affichage constant d'un menu interactif
	Affichage panier et facture sur demande
	Quitter avec fermeture des différents clients RPC et du programme
	
*A des fins de test, le username "Kanlam" présent dans le fichier Json du dossier users peut être utilisé.
	
