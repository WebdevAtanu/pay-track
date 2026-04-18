function toggleForm() {
    const loginForm = document.getElementById("loginForm");
    const signupForm = document.getElementById("signupForm");
    const formTitle = document.getElementById("formTitle");
    const toggleText = document.getElementById("toggleText");

    loginForm.classList.toggle("active");
    signupForm.classList.toggle("active");

    if (loginForm.classList.contains("active")) {
        formTitle.innerText = "Login";
        toggleText.innerHTML = `Don't have an account?
            <span class="toggle-link" onclick="toggleForm()">Sign Up</span>`;
    } else {
        formTitle.innerText = "Sign Up";
        toggleText.innerHTML = `Already have an account?
            <span class="toggle-link" onclick="toggleForm()">Login</span>`;
    }
}