﻿function togglePasswordVisibility(passwordInputId, iconId) {
    var passwordInput = document.getElementById(passwordInputId);
    var passwordIcon = document.getElementById(iconId);

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        passwordIcon.classList.remove("fa-eye");
        passwordIcon.classList.add("fa-eye-slash");
    } else {
        passwordInput.type = "password";
        passwordIcon.classList.remove("fa-eye-slash");
        passwordIcon.classList.add("fa-eye");
    }
}