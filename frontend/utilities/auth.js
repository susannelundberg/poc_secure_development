export const saveToSessionStorage = (jwtToken) => 
{ 
    sessionStorage.setItem( "jwt", jwtToken );
}

export const readFromSessionStorage = () => 
{ 
    const token = sessionStorage.getItem("jwt");
    return token;
}

export const removeFromSessionStorage = () =>  
{ 
    sessionStorage.removeItem("jwt");
}

export const isLoggedIn = () =>
{
    return readFromSessionStorage() !== null;
}