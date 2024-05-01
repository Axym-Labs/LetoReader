
ANNOUNCEMENTSIDENT = "Announcements";

function getAnnouncementStates() {
    return localStorage.getItem(ANNOUNCEMENTSIDENT) || [];
}

function markAnnouncementRead(announcementId) {
    var state = loadAnnouncementStates(announcementId);
    state.push(announcementId)
    localStorage.setItem(ANNOUNCEMENTSIDENT, state);
}