
class Account {
    static login(targetUrl, email, password, token, callback) {
        let additionalHeaders = {};

        if (token) {
            additionalHeaders["RequestVerificationToken"] = token;
        }
        
        let model = {
            email: email,
            password: password
        };
        
        $.ajax({
            url: targetUrl,
            type: "POST",
            data: model, //JSON.stringify(model),
            headers: additionalHeaders,
            contentType: "application/json",
            success: function(data) {
                callback(data);
            }
        });
    }
}
