var EmployeeSearch = {
    value: '',
    url: '',
    field: 'EmailAddress',
    modalId: '#employee-list-container',
    show: function (url) {
        EmployeeSearch.url = url;
        var prom = $.Deferred();
        $(EmployeeSearch.modalId).modal('show');

        $(EmployeeSearch.modalId).modal();
        $(EmployeeSearch.modalId).on('hidden.bs.modal', function (e) {
            prom.resolve(EmployeeSearch.value);
        });

        return prom;
    },
    hide: function () { $(EmployeeSearch.modalId).modal('hide'); },

    onClose: null,

    initial: function () {
        var searchData = [];

        $('#employee-list-container').on('shown.bs.modal', function (e) {
            $("#employee-search-textbox").focus();
        });

        $("#employee-search-textbox").keydown(function (e) {
            var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
            if (key != 13) return;
            e.preventDefault();
            $("#employee-search-button").click();
        });

        $("#employee-search-button").click(function () {
            var id = $("#employee-search-textbox").val();
            var searchBy = '';
            if ($.isNumeric(id)) {
                $("#employee-search-by").val('badge');
            }
            searchBy = $("#employee-search-by").val();
            var url = EmployeeSearch.url;
            $.ajax(url, {
                type: "GET",
                data: {
                    badge: (searchBy == "badge" ? id : null),
                    lastName: (searchBy == "lastName" ? id : null),
                    firstName: (searchBy == "firstName" ? id : null)
                }
            }).done(function (data) {
                var result = '';
                searchData = data;
                $.each(data, function (i, item) {
                    result += '<tr id="employee-list-' + i + '">' +
                        '<td>' + item.LastName + '</td>' +
                        '<td>' + item.FirstName + '</td>' +
                        '<td>' + item.Department + '</td>' +
                        '<td>' + ((item.EmailAddress) ? item.EmailAddress : "") + '</td>' +
                        '<td>' + item.Badge + '</td>' +
                        '</tr>';
                });
                $("#employee-list tbody").html(result);
                $("#employee-list tbody tr").click(function () {
                    var idx = $(this).attr("id").replace("employee-list-", "");
                    var value = data[idx][EmployeeSearch.field] || "";
                    postFilterUpdate(value);
                });
            });

            function postFilterUpdate(value) {
                EmployeeSearch.value = value;
                $('#employee-list-container').modal('hide');
            }

        });

        $(EmployeeSearch.modalId).on('hidden.bs.modal', function (e) {
            if (EmployeeSearch.onClose != null)
                EmployeeSearch.onClose(EmployeeSearch.value);
        });
    }

}
$(function () {
    EmployeeSearch.initial();
});

