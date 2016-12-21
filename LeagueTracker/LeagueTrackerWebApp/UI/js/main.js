/**
 * Created by austinm on 11/25/16.
 */

function slider(a) {
    $(#dropdown).click(function(e){
        if(!$(event.target).hasClass('slideBack')) {
            $(a).removeClass('slideOut');
        }
    });
}