export default class DisableFullscreenGesture {
    enable() {
        // Remove all stage gestures (including swipe down to unmaximize)
        global.stage.get_actions().forEach(a => {
            global.stage.remove_action(a);
        });
    }

    disable() {
        // Cannot restore removed actions - requires GNOME Shell restart
    }
}
