
class Account {
    static loginAjax(email, password, token, callback) {
        let additionalHeaders = {};

        if (token) {
            additionalHeaders["RequestVerificationToken"] = token;
        }
        
        $.ajax({
            url: $("form").attr("asp-action"),
            type: "POST",
            data: JSON.stringify(""),
            headers: additionalHeaders,
            contentType: "application/json",
            success: function(data) {
                callback(data);
            }
        });
    }
}
