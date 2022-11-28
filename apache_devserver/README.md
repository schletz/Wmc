# Erstellen des Devservers

Gehe in der Konsole in das Verzeichnis *apache_devserver* und führe die folgenden Befehle aus
(mit Enter bestätigen). Der Pfad *C:\Temp\assetmanager* ist anzupassen und soll auf das Verzeichnis
auf deinem Rechner zeigen, wo die Applikation ist.

```
docker rm -f assetmanager
docker volume prune -f
docker image prune -f
docker build -t assetmanager . 

docker create -p 80:80  -v C:\Temp\assetmanager:/var/www/html --name assetmanager assetmanager
```

Die Befehle richten erstmalig das Netzwerk ein. Danach muss in Docker Desktop nur mehr der
Container *assetmanager* gestartet werden.

> Hinweis: Der Container *assetmanager* erstellt ein leeres Laravel Projekt, wenn der Ordner,
> auf den */var/www/html* zeigt, leer ist. Das dauert einige Zeit, d. h. beim ersten Start reagiert
> der Webserver erst nach ca. 1 Minute. Du kannst die Ausgaben des Containers nachsehen, indem
> du in Docker Desktop auf den Namen des (laufenden) Containers klickst:

![](docker_log_1858.png)

Unter Linux können mit *docker logs -f assetmanager* die Ausgaben angesehen werden.

Es wird auch ein Testskript für den SQL Server in den *public* Ordner kopiert. Mit
*http://localhost/sql_server_test.php* kannst du es im Browser aufrufen.
