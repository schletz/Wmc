#!/bin/bash
# Date: November 2022
# Author: Michael Schletz
# Description: Startpunkt für den Laravel Container.
# Legt ein leeres laravel Projekt in /var/www/html an, wenn das Verzeichnis leer ist.

if [ -z "$(ls -A /var/www/html)" ]; then
    composer create-project laravel/laravel /var/www/html
    cp /sql_server_test.php /var/www/html/public
    chmod -R 777 /var/www/html/*
fi
apachectl -D FOREGROUND
