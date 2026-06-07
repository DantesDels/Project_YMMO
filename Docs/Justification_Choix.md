Les types float ont été remplacé par le type décimal 
car float introduit des erreurs d'arrondi sur les montants financiers.

Dans property : yearBuilt passe de string a int pour pouvoir réaliser des comparaisons numériques,
d'ordonner ou de valider une plage. Et surface passe en decimal pour réaliser des calculs tel que
prix du m2 par exemple. Ces changements de valeurs servent donc pour la gestion de la BDD ensuite,
dans le but du Big Data.

-> le type d'une propriété doit refléter sa nature sémantique, pas juste "ça compile".