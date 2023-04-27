import { createStore } from 'vuex'   // npm install vuex

export default createStore({
    state() {
        return {
            user: null
        }
    },
    mutations: {
        authenticate(state, userdata) {
            state.user = !userdata ? null : userdata;
        }
    }
});
