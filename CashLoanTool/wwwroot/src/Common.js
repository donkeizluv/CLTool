export default {
    getCookie: function(cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    },
    langCode: 'vn',
    errorMessages: {
        'en': {
            '404': 'Not Found'
        },
        'vn': {
            '404': 'Không tìm thấy'
        }
    },
    errorCodeTranslater: function(code) {
        return this.errorMessages[this.langCode][code];
    }
}
