(function ($) {
    $.fn.modalBox = function (options) {
        "use strict";
        var me = this;
        options = options || {};
        me.show = function (promis) {
            $(me).removeClass('modalBoxInactive').addClass('modalBoxActive');

            if (promis != null) {
                promis.done(function () { me.hide(); });
            }
        };
        me.hide = function () {
            $(me).removeClass('modalBoxActive').addClass('modalBoxInactive');
        };
        function initial() {
            $(me).removeClass('modalBox modalBoxActive modalBoxInactive');
            $(me).addClass("modalBox modalBoxInactive");
        }
        initial();
        return this;
    };

}(jQuery));