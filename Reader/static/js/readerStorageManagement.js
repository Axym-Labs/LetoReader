const CORE_READER_ELEM_SELECTOR = "#reader-frontpagesection";

function getUniqueTitle(title) {
    let i = 0;
    while (checkIfStateExists(i>0 ? title + i.toString() : title)) {
        i++;
    }
    return i>0 ? title + i.toString() : title;
}

function activateFocusMode() {
    if (!location.href.includes(CORE_READER_ELEM_SELECTOR))
        location.href = location.href + CORE_READER_ELEM_SELECTOR;

    //const element = document.querySelector(CORE_READER_ELEM_SELECTOR);
    //if (element instanceof HTMLElement) {
    //    element.scrollIntoView(true);
    //    console.log("Scrolled to reader element");
    //}

    document.querySelector(CORE_READER_ELEM_SELECTOR).classList.add("min-h-screen");
}

function deactivateFocusMode() {

    removeFocusModeIdFromLocation();

    document.querySelector(CORE_READER_ELEM_SELECTOR).classList.remove("min-h-screen");
}

function removeFocusModeIdFromLocation() {
    history.pushState("", document.title, window.location.pathname
        + window.location.search);
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


window.addEventListener('load', function () {
    removeFocusModeIdFromLocation();
});