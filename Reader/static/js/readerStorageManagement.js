const READERELEMIDENT = "#reader-frontpagesection";

function getUniqueTitle(title) {
    let i = 0;
    while (checkIfStateExists(i>0 ? title + i.toString() : title)) {
        i++;
    }
    return i>0 ? title + i.toString() : title;
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

function copyTitle(text) {
    textfield = document.querySelector("#reader-fontpagesection-titlefield")

    textfield.select();
    textfield.setSelectionRange(0, 99999);
    navigator.clipboard.writeText(textfield.value);
}