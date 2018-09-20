var AreChanges = false;

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Hide all checkboxes
    $('input.includeInCashFlow[type=checkbox]').hide();

    //Assign icon to appropiate table cells
    $('input.includeInCashFlow:checkbox:checked').siblings('span').addClass('greenCOLOR fa fa-check-circle-o');
    $('input.includeInCashFlow:checkbox:not(:checked)').siblings('span').addClass('redCOLOR fa fa-remove');
});

window.onbeforeunload = function (e) {
    if (AreChanges)
        return '';
};

$('input, select').change(function () {
    AreChanges = true;
});

$('input').keyup(function () {
    AreChanges = true;
});

//Change the cursor when the mouse goes over a table cell that can be changed
$('.includeInCashFlowSelect').hover(function () {
    $(this).css('cursor', 'pointer');
});

//When table cell is clicked
$('.includeInCashFlowSelect').click(function () {
    if (!$(this).children('input').is(':disabled')) {

        var checkbox = $(this).children('input');

        //Check/Uncheck checkbox
        if ($(checkbox).is(':checked')) {
            $(checkbox).prop('checked', false);
        }
        else {
            $(checkbox).prop('checked', true);
        }

        //Change icon
        if ($(checkbox).is(':checked')) {
            $(checkbox).siblings('span').removeClass('redCOLOR fa fa-remove').addClass('greenCOLOR fa fa-check-circle-o');
        }
        else {
            $(checkbox).siblings('span').removeClass('greenCOLOR fa fa-check-circle-o').addClass('redCOLOR fa fa-remove');
        }
    }
});

$('.ddl.form-control').change(function () {
    $('#refreshButton').trigger('click');
});

$("#sortableAccount").sortable({
    handle: ".handle",
    stop: function (event, ui) {
        $('input.accountSubId').each(function (i, obj) {
            $(obj).val(i + 1);
        })

        $('.accountRemoveButton').each(function (i, obj) {
            $(obj).val("Remove " + (i + 1));
        })
    }
});
