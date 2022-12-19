# Ein Vue.js Client für unser Backend

Im ersten Teil haben wir mit *npm init vue@3* eine leere Vue.js Applikation erstellt. Diese
muss nun an das Backend angepasst werden.

## Installation von Paketen

Öffne in VS Code den Ordner der Vue.js Applikation. Die Datei *package.json* muss im Hauptordner
erscheinen. Öffne danach die Konsole in VS Code (CTRL + ö) und installiere 3 Pakete:

```
npm install axios
npm install vuex
npm install bootstrap@5.2.3
```

Danach lösche zur Sicherheit den *node_modules* Ordner und installiere alle Depenencies neu.
Das geht in der Konsole von VS Code mit dem folgenden Befehl:

```
rd /S /Q node_modules && npm install
```

Öffne danach die Datei *vite.config.js* und passe den Ausgabepfad an. Er wird verwendet, wenn du
mit *npm build* die Applikation erstellst. Der Pfad soll auf das *wwwroot* Verzeichnis in der
Webapi verweisen:

**vite.config.js**
```javascript
import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  build: {
    outDir: '../WerAuchDenOrdnerKopiertDenKannIchNichtMehrHelfen.Webapi/wwwroot'
  }
})
```

Nun setze in der Datei *package.json* den Parameter *--emptyOutDir* für das build Skript.
Er löscht alte Dateien im Ordner *wwwroot*.

**package.json**
```javascript
"scripts": {
"dev": "vite",
"build": "vite build --emptyOutDir",
"preview": "vite preview --port 4173",
"lint": "eslint . --ext .vue,.js,.jsx,.cjs,.mjs --fix --ignore-path .gitignore"
},
```

## Erstellen eines Stores im State Manager

Ein *State Manager* ist ein Speicherbereich, auf den alle Komponenten zugreifen können. Außerdem
können die Komponenten auch auf Aktualisierungen reagieren. Wir brauchen den State Manager, um
nach dem Login die Userdaten zu setzen. Dadurch kann der Router bei bestimmten Routen prüfen,
ob der User angemeldet ist und nur in diesem Fall dorthin routen.

Erstelle dafür die Datei *src/store.js* mit dem [Inhalt aus dem Musterprojekt](Spengernews.Client/src/store.js).

Wie du erkennen kannst, hat der state ein Property *user* mit *name* (username), *guid* (GUID des
angemeldeten Users) und ein Flag *isLoggedIn*. Die Methode *authenticate* setzt die entsprechenden
Daten. Sie wird in der Login Komponente verwendet.

Weitere Informationen zum State Manager Vuex sind auf https://vuex.vuejs.org abrufbar.

## Clientseitige Routen

In der Datei [src/router/index.js](router/../Spengernews.Client/src/router/index.js) wird
das Routing definiert. Es gibt mit dem Property *meta* auch die Möglichkeit, zusätzliche Informationen
anzugeben. Wir setzen einfach ein Property *authorize*, wenn eine Route einen angemeldeten User
erfordert.

Damit am Client kein User, der nicht angemeldet ist, diese Route aufrufen kann, können wir mit
*router.beforeEach* jeden Seitenwechsel noch genauer prüfen. Ist ein User nicht angemeldet (das
sagt uns der State Manager), aber die Route ist geschützt, leiten wir auf die Startseite weiter.

```javascript
router.beforeEach((to, from, next) => {
  const authenticated = store.state.user.isLoggedIn;
  if (to.meta.authorize && !authenticated) {
    next("/");
    return;
  }
  next();
  return;
});
```

Weitere Informationen zum Vue Router sind auf https://router.vuejs.org abrufbar.

## Konfiguration von Axios

In der Datei [src/main.js](Spengernews.Client/src/main.js) wird nun alles zusammengelegt.
Vue.js wird mit dem State Manager und dem Router konfiguriert und die App.vue Komponente wird
in das div mit der ID *#app* geladen. Wichtig für axios ist die Zeile

```javascript
axios.defaults.baseURL = process.env.NODE_ENV == 'production' ? "/api" : "https://localhost:5001/api";
```

Sie sagt folgendes aus: Läuft der Server in production (die SPA wird also von der WebAPI ausgeliefert),
ist die URL der API einfach */api*, da sie ja am selben Server läuft. Im *development mode* (der
Devserver liefert die Applikation aus) brauchen wir eine absolute URL.

Nun können wir einfach in den Komponenten mit *await axios.get("news")* einen Request auf */api/news*
setzen, ohne dass wir den vollen Pfad immer angeben müssen. Das erleichtert eine nachträgliche
Änderung.

Weitere Informationen zu Axios sind auf [axios-http.com](https://axios-http.com/docs/intro) abrufbar.

## Das zentrale CSS in main.css

In *src/assets/main.css* kannst du dein zentrales CSS definieren. Dort kommen nur Anweisungen
hinein, die den Grundaufbau der Seite betreffen. Der Rest wird als *style* Element in den
einzelnen Komponenten definiert.

## Absetzen eines Login Requests

In der Component [src/components/LoginForm.vue](Spengernews.Client/src/components/LoginForm.vue)
wird mit Hilfe von Axios ein POST Request an die URL */api/user/login* geschickt. Damit axios
bei zukünftigen Requests den Token immer mitsendet, schreiben wir ihn mit der Zeile

```javascript
axios.defaults.headers.common['Authorization'] = `Bearer ${userdata.token}`;
```

in den Request Header. Achte dabei, dass das Backend auch mit dem Property *token* antwortet. In
*userdata* steht die Antwort der Route *api/user/login* und kann sich bei deinen Projekten natürlich
unterscheiden.

## Vue Components und Views

Die Views und Components sind *vue* Dateien und haben eine bestimmte Syntax. Um sie besser bearbeiten
zu können, kannst du die Extension [Vetur](https://marketplace.visualstudio.com/items?itemName=octref.vetur)
in VS Code installieren. Der Dateinamen soll aus mindestens 2 Wörtern bestehen (*NewsContent*,
*LoginForm*, ...) um Verwechslungen mit HTML Elementen zu vermeiden.

So eine Datei besteht aus mehreren Teilen: Einem Template für die HTML Darstellung, einem Style
Block und einem Script Block. Du kannst die folgende Vorlage in eine neu erstellte *vue*
Datei kopiere, um schneller arbeiten zu können:

```html
<script setup>
    // Imports
</script>

<template>
    <!-- A template must have one (1) root element. Usually a div with the classname
    of your component -->
    <div class="componentName">
       <!-- Your html -->
    </div>
</template>

<style scoped>
    /* Your css */
</style>

<script>
export default {
    props: {
        // Your parameters from the caller. Example: id : Number
    },
    data() {
        return {
            // Your data properties
        };
    },
    mounted() {
        // Your initialization code. Maybe async ( async mounted() {...} )
    },
    methods: {
        // Your methods
    },
    computed: {
        // Your computed properties
    }
};
</script>
```

Weitere Informationen sind auf der Seite
[Template Lifecycle](https://vuejs.org/guide/essentials/lifecycle.html)
abrufbar.

### Der Template Bereich

Im Template Bereich gibt es spezielle Attribute für deine HTML Elemente, die von Vue gerendert
werden:

- **{{ variable }}** Gibt den Inhalt der Variablen, die in *data* oder *props* definiert wurde, aus.
- **v-if** Blendet das HTML nur ein, wenn die angegebene Bedingung wahr ist. Kann in Zusammenhang
  mit *v-else* definiert werden.
- **v-for** Rendert das HTML Element für jeden Eintrag im Array. Wird in Zusammenarbeit mit
  *v-bind:key* verwendet.
- **v-on** Eventhandler, die auf Methoden in *methods* verweisen. Beispiel: *v-on:click="method()"*.
- **v-bind** Attributswerte, die von *data* gelesen werden sollen. Beispiel: *v-bind:value*.
- **v-bind:class** Speziell für das bedingte Aktivieren von CSS Klassen. Siehe
[Binding HTML Classes](https://vuejs.org/guide/essentials/class-and-style.html).
- **v-model** Für Formularfelder. Werte werden in ein Property in *data* geschrieben und von
  dort gelesen. Beispiel: *v-model=model.myFormField*.

Weitere Informationen zu Vue Templates sind auf der Seite
[Vue Template Syntax](https://vuejs.org/guide/essentials/template-syntax.html#text-interpolation)
beschrieben.

## Formulare und Validierung

In [src/views/CreateArticleView.vue](Spengernews.Client/src/views/CreateArticleView.vue) können
Artikel erstellt werden. Die Validierung erfordert ein paar Tricks. ASP.NET Core sendet automatisch
bei Validierungsfehlern den Status HTTP 400 und ein JSON mit folgendem Aufbau:

```javascript
// ...
"errors": {
    "nameOfProperty1": ["Validation Message"]
    "nameOfProperty2": ["Validation Message"]
}
```

Die folgenden Zeilen arbeiten diese Antwort so um, dass die Properties klein geschrieben werden
und aus dem Array der Validierungsmeldungen nur der erste Eintrag genommen wird. Das Ergebnis wird
in die Variable *validation* im *data* Bereich der Komponente geschrieben.

```javascript
this.validation = Object.keys(e.response.data.errors).reduce((prev, key) => {
    const newKey = key.charAt(0).toLowerCase() + key.slice(1);
    prev[newKey] = e.response.data.errors[key][0];
    return prev;
}, {});
```

Dadurch können wir leicht im Template das Formularfeld binden und eventuelle Fehler anzeigen
lassen:

```html
<div class="control">
    <input type="text" v-model="model.imageUrl" />
    <div v-if="validation.imageUrl" class="error">{{ validation.imageUrl }}</div>
</div>
```

