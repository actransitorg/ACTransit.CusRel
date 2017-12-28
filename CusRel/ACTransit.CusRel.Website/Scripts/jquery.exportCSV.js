// http://jsfiddle.net/terryyounghk/KPEGU/
//Downloding CSV File....


function exportTableToCSV($table, filename) {  
    var csvData = '';
    var showAlert = false;
    $rows = $table.find('tr');
    for (var i = 0; i < $rows.length; i++) {
        var $cells = $($rows[i]).children('th,td'); //header or content cells

        for (var y = 0; y < $cells.length; y++) {
            var txt = ($($cells[y]).text()).toString().trim().replace(/"/g, '""').replace(/[\n\r]+/g, "  ");
            if (txt.search(/("|,|\n)/g) >= 0)
                txt = '"' + txt + '"';
            if (y > 0) {
                csvData += ",";
            }
            csvData += txt;
        }
        csvData += '\n';
    };    
    if (window.navigator.msSaveOrOpenBlob) {
       //For IE 10 +....
        var fileData = ['\ufeff' + csvData];
        blobObject = new Blob(fileData);
        window.navigator.msSaveOrOpenBlob(blobObject, filename);      
    } else {
        //For Other Broswer...
        var csvDataURI = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csvData);
        $(this)
            .attr({
                'download': filename,
                'href': csvDataURI,
            });       
    }
}

//http://stackoverflow.com/questions/9770042/print-html-through-javascript-jquery
//For Printing....
function print(tableName, titleName, assignValue) {
    var disp_setting = "toolbar=no,location=no,directories=no,menubar=no,";
    disp_setting += "scrollbars=yes,width=780, height=780, left=100, top=25";
    var content_vlue = document.getElementById(tableName).innerHTML;
    var print = window.open("", "_blank", disp_setting);
    var oTable = document.getElementById(tableName);
    print.document.open();
    print.document.write('<!DOCTYPE html>');
    print.document.write('<html><head><title></title>');
    print.document.write('<style type="text/css"> body { background-color: white !important }  @media print { a:link:after, a:visited:after { content: ""; } }</style>');
    print.document.write('</head><body><center><span style="display:block;"><img src="../Content/Image/logo-small.png"/></span><h1 style=" text-align: center; font-size:14px;font-weight: bold;   font-family:" Arial", sans-serif;  ">Report : ' + titleName + assignValue + '</h1>');
    print.document.write(oTable.parentNode.innerHTML);
    print.document.write('</center></body></html>');
    print.document.close();
    print.print();
    print.close();
}

$(function () {
    $("[rel='tooltip']").tooltip();
});

