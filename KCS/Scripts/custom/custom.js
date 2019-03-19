// Hide/show header alert message
$('#successMessage, #errorMessage').bind("DOMSubtreeModified", function () {
    if ($(this).html() != "")
        $(this).delay(5000).css("width", '100%').slideToggle('slow', function () {
            $(this).html("").show();
        });
});