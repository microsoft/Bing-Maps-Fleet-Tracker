// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

$.ajax({
    url: "../api/users/me",
    xhrFields: {
        withCredentials: true
    },
    success: function () {
    },
    error: function () {
        window.location.href = "../api/users/login?redirectUri=" + window.location.href;
    }
});