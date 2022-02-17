<h1>Stores</h1>
<h3>Server gerenderte Tabelle</h3>
<p>
    Hier wird die Tabelle zeilenweise mit PHP aus der Variable <em>$viewData['stores']</em> gelesen
    und dargestellt.
</p>
<table class="table table-sm">
    <colgroup>
        <col style="width:1em" />
        <col style="width:5em" />
    </colgroup>
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
        </tr>
    </thead>
    <tbody>
        <?php
        foreach ($viewData['stores'] as $store) {
            echo "<tr>";
            echo "<td>{$store['id']}</td>";
            echo "<td>{$store['name']}</td>";
            echo "</tr>";
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
    </colgroup>
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
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
                container.appendChild(tr);
            }
        })
        .catch(err => alert(err.message));
</script>
