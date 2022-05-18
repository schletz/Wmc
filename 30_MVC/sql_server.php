<?php
    /* Demo für den Zugriff auf SQL Server
     * Windows: 
     *   1. Lade von https://docs.microsoft.com/en-us/sql/connect/php/download-drivers-php-sql-server?view=sql-server-ver15
     *      das ZIP Archiv mit dem Treiber (Download Microsoft Drivers for PHP for SQL Server (Windows))
     *   2. Kopiere die Datei php_pdo_sqlsrv_80_ts_x64.dll aus dem ZIP Archiv in den Extensions Ordner
     *      von PHP (z. B. C:\xampp\php\ext)
     *   3. Füge in der php.ini (C:\xampp\php) im Block der Extensions die Zeile
     *      extension=pdo_sqlsrv_80_ts_x64
     *      ein und starte den Server neu.
     * Hinweis:
     *   Achte auf die PHP Version (mit phpinfo() ausgeben). Für 8.0 wird php_pdo_sqlsrv_80_ts_x64.dll
     *   verwendet, für 8.1 die Datei php_pdo_sqlsrv_81_ts_x64.dll, ...
     * 
     * Linux:
     * https://docs.microsoft.com/en-us/sql/connect/php/installation-tutorial-linux-mac?view=sql-server-ver15
     * Helfe euch Gott!
     */


    $serverName = "127.0.0.1";  // IP des Servers
    $databaseName = "Aza";      // DB Name
    $uid = "sa";                // Username
    $pwd = "SqlServer2019";     // Passwort
    
    $pdo = new PDO("sqlsrv:server = $serverName; Database = $databaseName;", $uid, $pwd);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->setAttribute(PDO::ATTR_CASE, PDO::CASE_LOWER);
    $result = $pdo->query("SELECT @@Version AS SQL_VERSION")->fetchAll(PDO::FETCH_CLASS);
    print_r($result[0]);
?>