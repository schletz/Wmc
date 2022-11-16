//import { resolve } from "core-js/fn/promise";
import conf from "../config.js"

function getUrl(url) {
    return `${conf.apiUrl}${url.startsWith("/") ? "" : "/"}${url}`;
}

function getHeader(token) {
    const header = {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
    }
    if (token !== undefined) {
        header['Authorization'] = `Bearer ${token}`;
    }
    return header;
}

class RestService {
    login(username, password) {
        return new Promise((resolve, reject) => {
            this.postJson("api/user/login", { username: username, password: password })
                .then(data => {
                    this.username = data.cn;
                    this.token = data.token;
                    this.role = data.role;
                    resolve(data);
                })
                .catch(err => reject(err));
        });
    }
    get isAuthenticated() {
        return this.token !== undefined;
    }
    getJson(url) {
        return this.sendRequest(url, 'GET');
    }
    postJson(url, data) {
        return this.sendRequest(url, 'POST', data);
    }
    sendRequest(url, method, data) {
        return new Promise((resolve, reject) => {
            const fetchParams = {
                method: method,
                headers: getHeader(this.token)
            }
            if (method != 'GET' && data !== undefined) { fetchParams.body = JSON.stringify(data); }
            fetch(getUrl(url), fetchParams)
                .then(response => {
                    if (response.ok) {
                        response.json()
                            .then(data => resolve(data))
                            .catch(() => resolve({}));    // HTTP 201 no content
                        return;
                    }
                    if (response.status == 400) {
                        response.json()
                            .then(data => { reject({ status: response.status, data: data }); })
                        return;
                    }
                    reject({ status: response.status });
                })
                .catch(() => reject({ status: 0 }));             // Server nicht erreichbar
        });
    }
}

export default new RestService();
