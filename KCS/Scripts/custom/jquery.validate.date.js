//$(function () {
//    $.validator.methods.date = function (value, element) {
//        if ($.browser.webkit) {
//            var d = new Date();
//            return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value)));
//        }
//        else {
//            return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
//        }
//    };
//});

//To Fix jQuery date format 'en-GB' validation problem in Chrome
$(function () {
    $.validator.addMethod(
        "date",
        function (value, element) {
            var bits = value.match(/([0-9]+)/gi), str;
            if (!bits)
                return this.optional(element) || false;
            str = bits[1] + '/' + bits[0] + '/' + bits[2];
            return this.optional(element) || !/Invalid|NaN/.test(new Date(str));
        },
        "Please enter date in valid format."
    );
});