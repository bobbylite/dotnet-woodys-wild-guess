/**
 * Collapses the navbar when a link is clicked.
 */
export function collapseNavbar() {
    $(document).ready(function() {
        $('.navbar-collapse').collapse('hide');
    });
  };

/**
 * Builds OnEnterKeyPressed event for the chatroom.
 * @param {*} dotnetHelper 
 */
export function OnEnterKeyPressed(dotnetHelper) {
    $(document).ready(function() {
        $(document).keypress(async function(e) {
            if (e.key === "Enter") {
                $("input").blur();
                await dotnetHelper.invokeMethodAsync('Send');
            }
        });
    });
}