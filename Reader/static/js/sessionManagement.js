window.sessionStorageInterop = {
    getItem: function (key) {
        return sessionStorage.getItem(key);
    },
    setItem: function (key, value) {
        sessionStorage.setItem(key, value);
    },
    removeItem: function (key) {
        sessionStorage.removeItem(key);
    },
    clear: function () {
        sessionStorage.clear();
    }
};