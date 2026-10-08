import { api } from "../../utilities/api.js";
import { saveToSessionStorage } from "../../utilities/auth.js";

const loginForm = document.getElementById("login-form");
const registerForm = document.getElementById("register-form");

loginForm.addEventListener("submit", async (event) =>
{
    event.preventDefault();

    const body = {
        email: document.getElementById("login-email").value,
        password: document.getElementById("login-password").value
    };

    const data = await api("/auth/login", "POST", body);

    if (data.token)
    {
        saveToSessionStorage(data.token);
        if (data.role === "Admin"){
            window.location.href = "../admin/admin.html";
        } else {
            
            window.location.href = "../start/start.html";
        }
    }
    else
    {
        alert(data.message);
    }
});

registerForm.addEventListener("submit", async (event) =>
{
    event.preventDefault();

    const body = {
        firstName: document.getElementById("firstname").value,
        lastName: document.getElementById("lastname").value,
        streetAddress: document.getElementById("streetAddress").value,
        postCode: Number(document.getElementById("postCode").value),
        city: document.getElementById("city").value,
        email: document.getElementById("register-email").value,
        password: document.getElementById("register-password").value
    };

    const data = await api("/auth/register", "POST", body);

    alert(data.message ?? "Registreringen misslyckades");
});