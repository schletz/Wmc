# BFF (Backend for frontend) Demo App

## Server: ASP.NET Core WebAPI

Die API wird in der Datei [Program.cs](BffDemo.Webapi/Program.cs) konfiguriert:
- Konfiguration des Cookies mit *Secure=true* und 2 verschiedenen SameSite Richtlinien:
  - None im Development Mode, sodass der Devserver von Vue.js das Cookie übermitteln kann.
  - Strict im Production Mode.
- CORS Policy im Development Mode, sodass Preflight Requests vom Devserver akzeptiert werden.

## Client: Vue.js

Da wir in JavaScript beim Start der App nicht wissen, ob ein Cookie existiert, wurde in ASP eine Route angelegt, die die Userdaten ausgibt.
Diese Route ist nur mit Cookie zugreifbar.
Wenn wir also HTTP 200 mit Daten vom Server bekommen, können wir unseren State mit den Userdaten setzen.

**[BffDemo.Client/src/main.js](BffDemo.Client/src/main.js)**
```javascript
axios.defaults.baseURL =
  process.env.NODE_ENV == "production" ? "/api" : "https://localhost:5001/api";
axios.defaults.withCredentials = true;

axios
  .get("user/userinfo")
  .then((response) => {
    store.commit("authenticate", response.data);
  })
  .finally(() => {
    const app = createApp(App);
    app.use(router)
    app.use(store);
    app.mount('#app');
  })
```


