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
        'vn': {
            '404': '404 Không tìm thấy',
            '401': '401 Không được phép',
            '403': '403 Không được phép',
            '500': '500 Lỗi hệ thống',
            '504': '504 Hết thời gian chờ'
        }
    },
    errorCodeTranslater: function(code) {
        return this.errorMessages[this.langCode][code];
    }
}
