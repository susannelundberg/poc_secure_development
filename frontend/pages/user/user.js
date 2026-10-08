import {Navbar} from "../../utilities/menu.js";
import { Cart } from "../../utilities/cart.js";
import { IMAGE_BASE } from "../../config/settings.js";
import { api } from "../../utilities/api.js";

const cart = new Cart("cart");
const cartList = document.getElementById("cart-list");
const orderList = document.getElementById("order-list");
const orderButton = document.getElementById("order-button");

const showCart = (cartItems) => {

    cartList.innerHTML = "";

    if (cartItems.length === 0) {
        const message = document.createElement("p");
        message.textContent = "Din varukorg är tom";
        cartList.append(message);
        return;
    }

        cartItems.forEach(cartItems =>
    {
        const card = document.createElement("div");

        const image = document.createElement("img");
        image.src = IMAGE_BASE + cartItems.imageUrl;
        image.alt = cartItems.name;

        const name = document.createElement("h3");
        name.textContent = cartItems.name;

        const price = document.createElement("p");
        price.textContent = `${cartItems.price} kr`;

        const quantity = document.createElement("p");
        quantity.textContent = cartItems.quantity;

        const button = document.createElement("button");
        button.textContent = "Ta bort från varukorg";
        button.addEventListener("click", () => {
            cart.removeItem(cartItems.id);
        })

        card.append(image, name, price, quantity, button);
        cartList.append(card);
        
    });
}

const loadCart = async () => {
    const cartItems = await cart.getCartItems();
    showCart(cartItems);
}

const order = async () => {
    const button = document.createElement("button");
    button.textContent = "Skicka beställning";
    button.addEventListener("click", async () => {
        const body = {
            orderItems: cart.getCartItems().map((item) => ({
                productId: item.id,
                quantity: item.quantity
        })
    )
}
    const newOrder = await api("/orders", "POST", body);
        if (newOrder.success) {
            cart.clearCart();
        };
    });
    orderButton.append(button);
}

const showOrder = (orderItems) => {

    orderList.innerHTML = "";

    if (orderItems.length === 0) {
        const message = document.createElement("p");
        message.textContent = "Du har inga ordrar";
        orderList.append(message);
        return;
    }

    orderItems.forEach(orderItem =>
    {
        const card = document.createElement("div");

        const orderNumber = document.createElement("p");
        orderNumber.textContent = `Ordernummer: ${orderItem.orderNumber}`;

        const totalPrice = document.createElement("p");
        totalPrice.textContent = `Total kostnad: ${orderItem.totalPrice} kr`;

        card.append(orderNumber, totalPrice);
        orderList.append(card);
    });
}

const loadOrder = async () => {
    const orderItems = await api("/orders", "GET")
    showOrder(orderItems.data);
}

loadCart();
order();
loadOrder();

const initApp = async () => {
  new Navbar();
};

initApp();