<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SQL Server Test</title>
    <style>
        html {
            font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
        }
        table {
            border:1px solid gray;
            border-spacing: 5px;
        }
        th {
            text-align: left;
        }
    </style>
</head>
<body>
<?php
    $serverName = "10.0.1.2";   // IP des Servers
    $databaseName = "AssetsDb"; // DB Name
    $uid = "sa";                // Username
    $pwd = "SqlServer2019";     // Passwort
    
    $pdo = new PDO("sqlsrv:Server=$serverName;Database=$databaseName;TrustServerCertificate=1", $uid, $pwd);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->setAttribute(PDO::ATTR_CASE, PDO::CASE_LOWER);
    $results = $pdo->query("SELECT * FROM Locations")->fetchAll(PDO::FETCH_CLASS);
?>    

<p>Teste den Zugriff auf die Datenbank <em><?= $databaseName ?></em> auf <?= $serverName ?> (Username: <?= $uid ?>, Passwort: <?= $pwd ?>)</p>
<table>
    <thead>
        <tr>
            <th>ID</th>
            <th>GUID</th>
            <th>Name</th>
            <th>Adresse</th>
        </tr>
    </thead>
    <tbody>
        <?php
            foreach($results as $row) {
                echo "<tr>";
                echo "<td>{$row->id}</td>";
                echo "<td>{$row->guid}</td>";
                echo "<td>{$row->name}</td>";
                echo "<td>{$row->street}, {$row->zip} {$row->city}</td>";
                echo "</tr>";
            }
        ?>
    </tbody>
</table>
</body>
</html>

