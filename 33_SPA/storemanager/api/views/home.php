<?php
    // Notice Fehler abschalten, da nicht alle Felder in viewData enthalten sind.
    error_reporting(E_ERROR);
?>
<h1>Home</h1>
<?php
echo $viewData['message']
?>