$(document).ready(function () {
    $("#emailSelected").prop("disabled", false).on("click", sendToCheckedEmails);
    $("#selectAll").prop("disabled", false).on("click", function () {
            setEmailCB(this.checked);
        });
});
var emailClass = /\bemailTo\b/;
function getCheckedEmails() {
    var emailList = [];
    $("input").each(function(){ //using pure ecma within function for speed
        if (emailClass.test(this.className) && this.checked) {
            emailList.push(this.value);
        }
    });
    //datatables actually removes(rather than hides) element
    return emailList;
}
function sendToCheckedEmails() {
    var emailList = getCheckedEmails();
    if (emailList.length)
    {
       window.location.href = "mailTo:" + emailList.join(";");
    }
    else
    {
        alert("Please select at least one email recipient");
    }
}
function setEmailCB(val) {
    var checked = !!val;
    $("input").each(function () { //using pure ecma within function for speed
        if (emailClass.test(this.className)) {
            this.checked = checked;
        }
    });
    //datatables actually removes(rather than hides) element
}
