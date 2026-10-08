import { isLoggedIn, removeFromSessionStorage } from "./auth.js";
import { api } from "./api.js";

export class Navbar {

    constructor() {
        const navbar = this.#createNavbar();
        document.querySelector('header').insertAdjacentHTML('afterbegin', navbar);
        const button = document.getElementById("logout");

        if (button) {
            button.addEventListener("click", async () =>
            {
                // koden som ska köras vid klick
                await api("/auth/logout", "POST");
                removeFromSessionStorage();
                window.location.href = "../start/start.html";
            });
        }
    }

    #createNavbar(){
        if (isLoggedIn()){
            return /*HTML*/`
            <nav>
                <ul>
                    <li class="menu-item"><a href="../../pages/start/start.html">Start</a></li>
                    <li class="menu-item"><a href="../../pages/user/user.html">Kundkorg</a></li>
                    <li class="menu-item"><button id="logout">Logga out</button></li>
                </ul>
            </nav>
            `
        }
        else{
            return /*HTML*/`
            <nav>
                <ul>
                    <li class="menu-item"><a href="../../pages/start/start.html">Start</a></li>
                    <li class="menu-item"><a href="../../pages/login/login.html">Logga in</a></li>
                    <li class="menu-item"><a href="../../pages/login/login.html">Registrera dig</a></li>
                </ul>
            </nav>
            `
        };
    }
}

