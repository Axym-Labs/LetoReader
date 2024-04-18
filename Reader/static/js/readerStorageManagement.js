
const TEXTSTATEIDENT = "TEXTSTATE:";
const READERELEMIDENT = "#reader-frontpagesection";

function saveConfiguration(configStr) {
    localStorage.setItem('readerConfig', configStr);
}

function loadConfigurationStrIfExists() {
    let configString = localStorage.getItem('readerConfig');
    if (configString && configString != "{}") {
        return configString;
    } else {
        return null;
    }
}

function updateState(title, stateStr) {
    localStorage.setItem(TEXTSTATEIDENT + title, stateStr);
}

function loadStateArraysStr(_) {
    let arrStr = "[";
    for (var key in localStorage) {
        if (key.startsWith(TEXTSTATEIDENT)) {
            let val = localStorage.getItem(key);
            if (val != null && val != "" && val != "{}")
            arrStr += val + ", ";
        }
    }
    if (arrStr[arrStr.length-1] == ",")
        arrStr = arrStr.substring(0, arrStr.length-1)
    arrStr += "]"
    return arrStr;
}

function deleteTextState(title) {
    localStorage.removeItem(TEXTSTATEIDENT + title)
}

function renameState(title, newTitle) {
    let state = localStorage.getItem(TEXTSTATEIDENT + title);
    localStorage.removeItem(TEXTSTATEIDENT + title);
    let stateObj = JSON.parse(state);
    stateObj["Title"] = newTitle;
    localStorage.setItem(TEXTSTATEIDENT + newTitle, JSON.stringify(stateObj));
}

function getUniqueTitle(title) {
    let i = 0;
    while (checkIfStateExists(i>0 ? title + i.toString() : title)) {
        i++;
    }
    return i>0 ? title + i.toString() : title;
}

function checkIfStateExists(title) {
    return localStorage.getItem(TEXTSTATEIDENT + title) !== null;
}

function activateFocusMode() {
    location.href = READERELEMIDENT;
    document.querySelector(READERELEMIDENT).classList.add("min-h-screen");
}

function deactivateFocusMode() {
    document.querySelector(READERELEMIDENT).classList.remove("min-h-screen");
}

async function getClipboardContent() {

    var isiOSDevice = navigator.userAgent.match(/ipad|iphone/i);

    if (isiOSDevice) {
        return await navigator.clipboard.readText();
    } else {
        return await navigator.clipboard.readText();
    }
}

function copyText() {
    textfield = document.querySelector("#reader-fontpagesection-textfield")

    textfield.select();
    textfield.setSelectionRange(0, 99999);
    navigator.clipboard.writeText(textfield.value);
}