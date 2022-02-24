 <?php
    // Notice Fehler abschalten, da nicht alle Felder in viewData enthalten sind.
    error_reporting(E_ALL & ~E_NOTICE);
    $closedate = !empty($viewData['store']->closedate)
        ? (new DateTime($viewData['store']->closedate))->format("Y-m-d")
        : "";
    ?>
 <h1>Store editieren</h1>
 <div class="card">
     <div class="card-body">
         <h5 class="card-title">Details zum Store <?= $viewData['store']->name ?></h5>
         <form method="post" action="?controller=stores&action=store">
             <input type="hidden" name="guid" value=" <?= $viewData['store']->guid ?>" />
             <div class="d-flex flex-wrap align-items-start" style="gap:1rem">
                 <div>
                     <label for="name" class="flex-shrink-0">Name</label>
                     <input required style="width:20em" minlength="2" maxlength="255" class="form-control" type="text" id="name" name="name" value=" <?= $viewData['store']->name ?>" />
                     <div class="text-danger"> <?= $viewData['validation']->name ?></div>
                 </div>
                 <div>
                     <label for="url" class="flex-shrink-0">URL</label>
                     <input style="width:20em" minlength="2" maxlength="255" class="form-control" type="url" id="url" name="url" value=" <?= $viewData['store']->url ?>" />
                     <div class="text-danger"> <?= $viewData['validation']->url ?></div>
                 </div>
                 <div>
                     <label for="closeDate" class="flex-shrink-0">Close Date</label>
                     <div class="d-flex align-items-center">
                         <input class="form-control" type="date" name="closedate" id="closeDate" value="<?= $closedate ?>" />
                         <div style="cursor:pointer" onclick="document.querySelector('input[name=closedate]').value = ''">&#10006;</div>
                     </div>
                     <div class="text-danger"> <?= $viewData['validation']->closedate ?></div>
                 </div>
                 <div>
                     <label for="managerid" class="flex-shrink-0">Manageraccount</label>
                     <div class="d-flex align-items-center">
                         <select style="width:10em" class="form-select" name="managerid" id="managerid">
                             <option value=""></option>
                             <?php foreach ($viewData['users'] as $user) { ?>
                                 <option
                                 <?=  $viewData['store']->managerid == $user->id ? "selected" : "" ?>
                                 value="<?= $user->id ?>"><?= $user->username ?></option>
                             <?php } ?>
                         </select>
                         <div style="cursor:pointer" onclick="document.querySelector('select[name=managerid]').value = ''">&#10006;</div>
                     </div>
                 </div>

             </div>
             <button class="mt-3 btn btn-primary" type="submit">Senden</button>
             <button class="mt-3 btn btn-info" type="button" onclick="window.location.href='?controller=stores'">Zurück</button>
         </form>
     </div>
 </div>
 <?php
    if (!empty($viewData['error'])) {
        echo "<div class=\"border border-danger border-3 mt-3 p-1\">{$viewData['error']}</div>";
    }
    ?>