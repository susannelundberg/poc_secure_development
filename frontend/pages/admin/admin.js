import {Navbar} from "../../utilities/menu.js";
import { api } from "../../utilities/api.js";
import { IMAGE_BASE } from "../../config/settings.js";

const productList = document.getElementById("product-list");
const addForm = document.getElementById("add-form");
const updateForm = document.getElementById("update-form");
let selectedId = null;

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

        const button = document.createElement("button");
        button.textContent = "Ändra produkt";
        button.addEventListener("click", () => {
            updateForm.hidden = false;
            selectedId = product.id;
        });

        card.append(image, name, price, button);
        productList.append(card);
    });
};

updateForm.addEventListener("submit", async (event) =>
{
    event.preventDefault();

    const body = {
        name: document.getElementById("update-name").value,
        price: Number(document.getElementById("update-price").value)
    };

    const data = await api(`/products/${selectedId}`, "PUT", body);

    loadProducts();
    updateForm.hidden = true;
});

const loadProducts = async (search = "") =>
{
    const path = search ? `/products?search=${encodeURIComponent(search)}` : "/products";
    const products = await api(path, "GET");
    showProducts(products);
};

addForm.addEventListener("submit", async (event) =>
{
    event.preventDefault();

    const body = {
        name: document.getElementById("add-name").value,
        price: Number(document.getElementById("add-price").value)
    };

    const data = await api("/products", "POST", body);

    if (data.id)
    {
        loadProducts();
    }
    else
    {
        alert(data.message);
    }
});

loadProducts();

const initApp = async () => {
  new Navbar();
};

initApp();