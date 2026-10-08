import {Navbar} from "../../utilities/menu.js";
import { api } from "../../utilities/api.js";
import { IMAGE_BASE } from "../../config/settings.js";
import { Cart } from "../../utilities/cart.js";
import { isLoggedIn } from "../../utilities/auth.js";

const cart = new Cart("cart");
const productList = document.getElementById("product-list");
const searchForm = document.getElementById("search-form");
const searchInput = document.getElementById("search-product");

const showProducts = (products) =>
{
    productList.innerHTML = "";

    products.forEach((product) =>
    {
        const card = document.createElement("div");

        const image = document.createElement("img");
        image.src = IMAGE_BASE + product.imageUrl;
        image.alt = product.name;

        const name = document.createElement("h3");
        name.textContent = product.name;

        const price = document.createElement("p");
        price.textContent = `${product.price} kr`;

        if (isLoggedIn()) {
            const button = document.createElement("button");
            button.textContent = "Lägg till i varukorg";
            button.addEventListener("click", () => {
                cart.updateCart(product)
            });
            card.append(button);
        };

        card.append(image, name, price);
        productList.append(card);
    });
};

const loadProducts = async (search = "") =>
{
    const path = search ? `/products?search=${encodeURIComponent(search)}` : "/products";
    const products = await api(path, "GET");
    showProducts(products);
};

searchForm.addEventListener("submit", (event) =>
{
    event.preventDefault();
    loadProducts(searchInput.value);
});

loadProducts();

const initApp = async () => {
  new Navbar();
};

initApp();