<?php
$closedate = !empty($viewData['store']->closedate) 
    ? (new DateTime($viewData['store']->closedate))->format("Y-m-d")
    : "";

echo <<<HTML
<h1>Store editieren</h1>
<div class="card">
    <div class="card-body">
        <h5 class="card-title">Details zum Store {$viewData['store']->name}</h5>
        <form method="post" action="?controller=stores&action=store">
            <input type="hidden" name="guid" value="{$viewData['store']->guid}" />
            <div class="d-flex flex-wrap " style="gap:1rem">
                <div class="d-flex align-items-center" style="gap:1rem"> 
                    <label for="name" class="flex-shrink-0">Name</label>
                    <input required minlength="2" maxlength="255" class="form-control" type="text" name="name" value="{$viewData['store']->name}" />
                </div>
                <div class="d-flex align-items-center" style="gap:1rem">
                    <label for="closeDate" class="flex-shrink-0">Close Date</label>
                    <input class="form-control" type="date" name="closedate" value="$closedate" />
                    <span 
                        style="margin-left: -4.5rem; cursor:pointer; font-size:16px; font-weight:bold; color:grey"
                        onclick="document.querySelector('input[name=closedate]').value = ''"><i class="fas fa-trash-alt"></i></span>
                </div>
                <div>
            </div>
            </div>
            <button class="mt-3 btn btn-primary" type="submit">Senden</button>
            <button 
                class="mt-3 btn btn-info"  type="button"
                onclick="window.location.href='?controller=stores'">Zurück</button>
        </form>
    </div>
</div>
HTML;

if (!empty($viewData['error'])) {
    echo "<div class=\"border border-danger border-3 mt-3 p-1\">{$viewData['error']}</div>";
}
?>