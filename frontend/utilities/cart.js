export class Cart {

    constructor(key) {
        this.key = key;
    } 

    getCartItems() {
        const items = JSON.parse(sessionStorage.getItem(this.key));
        if (!items) return [];
        return items;
    }

    setCartItems(data) {
        sessionStorage.setItem(this.key, JSON.stringify(data));
    }

    removeItem(id) {
        const items = this.getCartItems().filter((i) => i.id !== id);
        this.setCartItems(items);
    }

    updateCart(item) {
        let items = this.getCartItems();
        let found = items.find((i) => i.id === item.id);

        if (found) {
            item.quantity = found.quantity + 1;
            items = items.filter((i) => i.id !== item.id);
        }
        else {
            item.quantity = 1;
        }

        items.push(item);

        this.setCartItems(items);
    }

    clearCart(){
        sessionStorage.removeItem(this.key);
    }


}