
//$(window).on('resize', function() {
//    var height = $(".navbar").height();
//    $("body").css('padding-top', height);
//});
//$(window).resize();


//<Custom JS>
$('ul.dropdown-menu [data-toggle=dropdown]').on('click', function (event) {
    // Avoid following the href location when clicking
    event.preventDefault();
    // Avoid having the menu to close when clicking
    event.stopPropagation();
    // If a menu is already open we close it
    //$('ul.dropdown-menu [data-toggle=dropdown]').parent().removeClass('open');
    // opening the one you clicked on
    $(this).parent().addClass('open');

    var menu = $(this).parent().find("ul");
    var menupos = menu.offset();

    if ((menupos.left + menu.width()) + 30 > $(window).width()) {
        var newpos = -menu.width();
    } else {
        var newpos = $(this).parent().width();
    }
    menu.css({ left: newpos });

});

toJavaScriptDate = function (value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    return new Date(parseFloat(results[1]));
}

toDateTimeString = function (date) {
    if (!date) return "";
    if (typeof date === "string")
        date = date[0] == "/" ? toJavaScriptDate(date) : new Date(date);
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    year = year <= 9 ? '000' + year : year;
    var hours = date.getHours() == 0 ? "12" : date.getHours() > 12 ? date.getHours() - 12 : date.getHours();
    var minutes = (date.getMinutes() < 10 ? "0" : "") + date.getMinutes();
    var seconds = date.getSeconds();
    seconds = seconds <= 9 ? '0' + seconds : seconds;
    var ampm = date.getHours() < 12 ? "AM" : "PM";
    return '' + month + '/' + day + '/' + year + ' ' + hours + ':' + minutes + ':' + seconds + '&nbsp;' + ampm;
}

toDateString = function (date) {
    if (!date) return "";
    if (typeof date === "string")
        date = date[0] == "/" ? toJavaScriptDate(date) : new Date(date);
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    year = year <= 9 ? '000' + year : year;
    return '' + month + '/' + day + '/' + year;
}

toNullString = function(value) {
    return value != null ? value : "";
}

nullString = function (value) {
    return ((value) ? value.toString() : "");
}

toCommentsString = function (value, n) {
    if (typeof value !== "string") return;
    value = value.replace(/"/g, "'");
    var toLong = value.length > n;
    var s = toLong ? value.substr(0, n - 1) : value;
    //s = htmlDecode(s);
    s = toLong ? s.substr(0, s.lastIndexOf(' ')) : s;
    return toLong ? s + '&hellip;' : s;
}

function htmlEncode(value) {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

function SetupHints() {
    $('[data-toggle="popover"]').popover({
        html: true,
        trigger: 'hover',
        placement: 'auto',
        template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>'
    });
}

