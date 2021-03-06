<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Store Manager</title>
    <link rel="icon" type="image/x-icon" href="img/favicon.ico">
    <!-- URL rewrite fügt bei css, img, js und lib den pfad public automatisch hinzu -->
    <link rel="stylesheet" href="lib/bootstrap5/bootstrap.min.css" />
    <!-- Font Awesome. Aus dem ZIP muss auch der Ordner webfonts in lib kopiert werden, da
    das CSS auf ../webfonts verweist. Die definierten CSS Klassen schreiben nur das Zeichen als
    Content before in der Webfonts Schriftart -->
    <link rel="stylesheet" href="lib/fontawesome6/all.min.css" />
    <link rel="stylesheet" href="css/site.css" />
    <?php
    writeCss();
    ?>
    <script src="js/site.js" defer></script>
</head>

<body>
    <header>
        Store Manager
    </header>
    <main>
        <nav class="sidebar">
            <ul data-nav-group="Stores verwalten">
                <li><i class="fa-solid fa-link"></i> <a href="?controller=Stores">Storeliste</a></li>
            </ul>
            <ul data-nav-group="Produkte verwalten">
                <li><i class="fa-solid fa-link"></i> <a href="?controller=Products">Produktliste</a></li>
            </ul>
        </nav>
        <div class="main-content">
            <?php
            renderBody();
            ?>
        </div>
    </main>
    <footer>
        <img src="img/schullogo.png" alt="Schullogo" style="height:32px;" class="me-3" />
        <strong class="pe-3">
            HTL Spengergasse - Abteilung Informatik
        </strong>
        &copy; Michael Schletz, 2022
    </footer>
</body>

</html>