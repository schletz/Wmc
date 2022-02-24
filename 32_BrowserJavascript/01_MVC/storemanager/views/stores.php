<h1>Stores</h1>
<h3>Server gerenderte Tabelle</h3>
<p>
    Hier wird die Tabelle zeilenweise mit PHP aus der Variable <em>$viewData['stores']</em> gelesen
    und dargestellt.
</p>
<p><a href="?controller=Stores&action=newStore">Neuen Store anlegen</a></p>
<table class="table table-sm">
    <colgroup>
        <col style="width:1em" />
        <col style="width:5em" />
        <col style="width:2em" />
        <col style="width:2em" />
    </colgroup>
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Close Date</th>
            <th>Aktionen</th>
        </tr>
    </thead>
    <tbody>
        <?php
        foreach ($viewData['stores'] as $store) {
            $date = !empty($store->closedate) ? (new DateTime($store->closedate))->format('d.m.Y') : "";
            echo <<<HTML
            <tr>
                <td>{$store->id}</td>
                <td>{$store->name}</td>
                <td>{$date}</td>
                <td><a href="?controller=stores&action=store&guid={$store->guid}" class="btn btn-primary btn-sm"><i class="fas fa-info"></i></a>
                <a href="?controller=stores&action=deletestore&guid={$store->guid}" class="btn btn-primary btn-sm"><i class="fas fa-trash"></i></a></td>
            </tr>
            HTML;
        }
        ?>
    </tbody>
</table>

<h3>Über AJAX geladene Daten</h3>
<p>
    Hier wird die URL <em>?controller=Stores&action=AllStores</em> über fetch aufgerufen. Dann
    wird aus dem JSON Ergebnis die Tabelle generiert.
</p>
<table class="table table-sm">
    <colgroup>
        <col style="width:1em" />
        <col style="width:5em" />
        <col style="width:2em" />
    </colgroup>
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Close Date</th>
        </tr>
    </thead>
    <tbody id="storeContainer">
    </tbody>
</table>

<script>
    fetch("?controller=Stores&action=AllStores")
        .then(res => res.json())
        .then(stores => {
            const container = document.getElementById("storeContainer");
            for (const store of stores) {
                const tr = document.createElement("tr");
                let td = document.createElement("td");
                td.innerHTML = store.id;
                tr.appendChild(td);
                td = document.createElement("td");
                td.innerHTML = store.name;
                tr.appendChild(td);
                td = document.createElement("td");
                td.innerHTML = store.closedate ? new Date(store.closedate).toLocaleDateString() : "";
                tr.appendChild(td);                
                container.appendChild(tr);
            }
        })
        .catch(err => alert(err.message));
</script>