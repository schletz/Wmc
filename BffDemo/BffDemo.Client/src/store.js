import { createStore } from "vuex";

export default createStore({
  state() {
    return {
      userdata: null,
    };
  },
  mutations: {
    authenticate(state, userdata) {
      if (!userdata) {
        state.userdata = null;
        return;
      }
      state.userdata = userdata;
    },
  },
});
