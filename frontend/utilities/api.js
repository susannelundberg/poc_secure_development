import { readFromSessionStorage } from "./auth.js";
import { API_URL } from "../config/settings.js";

export const api = async (path, method, body) =>
{
    const url = API_URL + path;
    const token = readFromSessionStorage();
    const respons = await fetch(url, {method: method, body: JSON.stringify(body), headers: {Authorization: `Bearer ${token}`, "Content-Type": "application/json"}
    });
    if (respons.status === 204) return null;
    return await respons.json();
}



