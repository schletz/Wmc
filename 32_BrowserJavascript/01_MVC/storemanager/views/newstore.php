<?php
// Notice Fehler abschalten, da das Storeobjekt bei neuen Stores uninitialsiert ist. Im Fehlerfall
// wird aber die Benutzereingabe zurückgesendet.
error_reporting(E_ALL & ~E_NOTICE);
$closedate = !empty($viewData['store']->closedate) 
    ? (new DateTime($viewData['store']->closedate))->format("Y-m-d")
    : "";
    
echo <<<HTML
<h1>Neuen Store anlegen</h1>
<form method="post" action="?controller=stores&action=newStore">
<div class="d-flex flex-wrap " style="gap:1rem">
    <div class="d-flex align-items-center" style="gap:1rem"> 
        <label for="name" class="flex-shrink-0">Name</label>
        <input class="form-control" type="text" name="name" value="{$viewData['store']->name}" />
    </div>
    <div class="d-flex align-items-center" style="gap:1rem">
        <label for="closeDate" class="flex-shrink-0">Close Date</label>
        <input class="form-control" type="date" name="closedate" value="$closedate" />
    </div>
    <div>
</div>
</div>
<button class="mt-3 btn btn-primary" type="submit">Senden</button>
<button 
    class="mt-3 btn btn-info"  type="button"
    onclick="window.location.href='?controller=stores'">Zurück</button>
</form>
HTML;
if (!empty($viewData['error'])) {
    echo "<div class=\"border border-danger border-3 mt-3 p-1\">{$viewData['error']}</div>";
}
?>