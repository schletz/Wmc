<?php
require('router.php');
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Store Manager</title>
    <link rel="icon" type="image/x-icon" href="img/favicon.ico">
    <link rel="stylesheet" href="lib/bootstrap5/bootstrap.min.css" />
    <!-- Font Awesome. Aus dem ZIP muss auch der Ordner webfonts in lib kopiert werden. -->
    <link rel="stylesheet" href="lib/fontawesome5/all.min.css" />
    <link rel="stylesheet" href="css/site.css" />
    <?php
    // CSS Laden. Existiert ein File im Views Ordner mit dem Namen (viewname).php.css, so wird
    // darauf verwiesen. Da dies im Header sein muss, braucht es diese Logik.
    $cssFile = $filename = "views/{$viewName}.php.css";
    if (file_exists($cssFile)) {
        echo "<link rel=\"stylesheet\" href=\"{$cssFile}\" />";
    }
    ?>
    <script src="lib/fontawesome5/all.min.js"></script>
</head>

<body>
    <header>
        Store Manager
    </header>
    <main>
        <nav class="sidebar">
            <ul data-nav-group="Stores verwalten">
                <li><a href="?controller=Stores">Storeliste</a></li>
            </ul>
            <ul data-nav-group="Produkte verwalten">
                <li><a href="?controller=Products">Produktliste</a></li>
            </ul>
        </nav>
        <div class="main-content">
            <?php
            // Sucht im Ordner Views nach dem festgelegten Viewnamen. Er ist standardmäßig der
            // Controllername, außer eine Controllermethode überschreibt $this->viewName
            $filename = "views/{$viewName}.php";
            if (file_exists($filename)) {
                require($filename);
            } else {
                require('notFound.php');
            }
            ?>
        </div>
    </main>
    <footer>
        <img src="img/schullogo.png" style="height:32px;" class="me-3" />
        <strong class="pe-3">
            HTL Spengergasse - Abteilung Informatik
        </strong>
        &copy; Michael Schletz, 2022
    </footer>

    <script src="js/site.js"></script>
</body>

</html>