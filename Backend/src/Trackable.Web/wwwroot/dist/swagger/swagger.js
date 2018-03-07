// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

﻿var xhttp = new XMLHttpRequest();
xhttp.withCredentials = true;
xhttp.onreadystatechange = function () {
    if (this.readyState == 4 && this.status != 200) {
        window.location.href = "../api/users/login?redirectUri=" + window.location.href;
    }
    if (document.getElementsByClassName('auth-wrapper')) {
        document.getElementsByClassName('auth-wrapper')[0].parentNode.parentNode.style.display = 'none';
    }
};
xhttp.open("GET", "../api/users/me", true);
xhttp.send();
