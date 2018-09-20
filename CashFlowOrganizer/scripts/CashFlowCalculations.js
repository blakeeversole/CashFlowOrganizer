
var AreChanges = false;

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    SetIncome(false);
    SetExpenses(false);
    SetCashFlow();    
});

//function ReLoad(){
//    SetIncome(false);
//    SetExpenses(false);
//    SetCashFlow();
//}   

window.onbeforeunload = function (e) {
    if(AreChanges)
        return '';
};

$('.ddl.form-control').change(function () {
    $('#refreshButton').trigger('click');
});

$('input, select').change(function () {
    AreChanges = true;
});

$('input').keyup(function () {
    AreChanges = true;
});

$("#sortableIncome").sortable({
    handle: ".handle",
    stop: function (event, ui) {
        $('input.incomeSubId').each(function (i, obj) {
            $(obj).val(i + 1);
        })

        $('.incomeRemoveButton').each(function (i, obj) {
            $(obj).val("Remove " + (i + 1));
        })
    }
});

$("#sortableExpense").sortable({
    handle: ".handle",
    stop: function (event, ui) {
        $('input.expenseSubId').each(function (i, obj) {
            $(obj).val(i + 1);
        })

        $('.expenseRemoveButton').each(function (i, obj) {
            $(obj).val("Remove " + (i + 1));
        })
    }
});

$('input.incomeAmount').keyup(function () {
    SetIncome(true);
})

$('input.expenseAmount').keyup(function () {
    SetExpenses(true);
})

function SetCashFlow() {

    income = $('#incomeTotal').html();
    expense = $('#expenseTotal').html();

    if (income == null)
        income = "0";

    if (expense == null)
        expense = "0";

    income = income.replace('$', '').replace(/,/g, '');
    expense = expense.replace('$', '').replace(/,/g, '');

    cashflow = parseFloat(income) - parseFloat(expense);
    cashflow = cashflow.toFixed(2);

    if (cashflow >= 0) {
        cashflow = AddCommas(cashflow);
        $('#totalCashFlow').html(cashflow).prepend('+').removeClass('redCOLOR').addClass('greenCOLOR');
    }        
    else {
        cashflow = AddCommas(cashflow);
        $('#totalCashFlow').html(cashflow).prepend('-').removeClass('greenCOLOR').addClass('redCOLOR');
    }
        
}

function SetIncome(setCashFlow) {
    
    total = MonthlyIncomeOrExpenseTotals('income');

    total = round(total, 2).toFixed(2);

    total = AddCommas(total);

    $('#incomeTotal').html(total);

    if(setCashFlow)
        SetCashFlow();    
}

function SetExpenses(setCashFlow) {
    
    total = MonthlyIncomeOrExpenseTotals('expense');

    total = round(total, 2).toFixed(2);

    total = AddCommas(total);

    $('#expenseTotal').html(total);

    if (setCashFlow)
        SetCashFlow();
}

function MonthlyIncomeOrExpenseTotals(selector) {
    var total = parseFloat(0);
    $('.' + selector + 'Item').each(function (i, obj) {
        var type = $(obj).find('.' + selector + 'Type').val();
        var val = $(obj).find('.' + selector + 'Amount').val();

        if (val > 0) {
            if (type == 'MON') {
                total = total + parseFloat(val);
            }
            else if (type == 'SEM') {
                total = total + parseFloat(val * 2);
            }
            else if (type == 'BWE') {
                total = total + parseFloat((val) * (26 / 12));
            }
            else if (type == 'WEE') {
                total = total + parseFloat((val) * (52 / 12));
            }
            else if (type == 'DAY') {
                total = total + parseFloat((val) * (365 / 12));
            }
        }        
    })

    return total;
}

function AddCommas(number) {

    if (number < 0)
        number = (number * -1).toFixed(2);

    number = number.toString();
    var numberString;

    if (number.length < 7) {
        numberString = '$' + number;
    }
    else {
        for (var i = 6; i < number.length; i += 3) {
            if (i == 6)
                numberString = ',' + number.substring(number.length - 6, number.length);
            else
                numberString = ',' + number.substring(number.length - (i), number.length - i + 3) + numberString;
        }

        diff = number.substring(0, number.length - i + 3);
        numberString = '$' + diff + numberString;
    }   

    return numberString;
}

function round(value, exp) {
    if (typeof exp === 'undefined' || +exp === 0)
        return Math.round(value);

    value = +value;
    exp = +exp;

    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
        return NaN;

    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

    // Shift back
    value = value.toString().split('e');
    return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
}
