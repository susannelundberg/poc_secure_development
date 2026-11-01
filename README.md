# JENSENOnline AB: säker webbshop (POC)

Skolprojekt från kursen Säker systemutveckling på Jensen YH. Uppgiften utgick från ett påhittat företag som ville bygga in säkerhet i sin nya webbshop från början.

Det här är en **Proof of Concept**, alltså inte en färdig webbshop. Syftet är att visa hur säkerhetsfunktioner kan implementeras och motiveras. 
Därför är bara de delar byggda som behövs för det: registrering och inloggning, produktkatalog, kundvagn och order. 
Projektet hör ihop med en säkerhetsrapport med hotanalys (CIA och STRIDE), säkerhetsstrategi, tester och ett attackscenario.

## Teknik
ASP.NET Core Web API (.NET 10) med clean architecture, ASP.NET Core Identity och JWT, SQL Server i Docker och ett frontend i Vanilla JavaScript.

## Säkerhetsfunktioner
- Roller (Admin och User) och behörighetskontroll på serversidan
- Lockout och rate limiting på inloggningen
- Utloggning där token spärras, samt CORS-policy
- Validering av indata, skydd mot SQL injection och XSS, och säkra felmeddelanden
- Hemligheter utanför koden (User Secrets)

## Avgränsningar
Betalning (Klarna), adminvyer för alla order och användarhantering, HTTPS samt loggning och övervakning ingår inte i POC:n. De behandlas i rapporten.

## Hur man kör projektet

## Förkunskaper

- .NET 10 SDK
- Docker Desktop (igång)
- VS Code med tillägget Live Server (eller annan enkel webbserver)

## Starta databasen

Välj ett eget lösenord (minst 8 tecken, med stor och liten bokstav, siffra och specialtecken):
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<DITT-DB-LÖSENORD>" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

Har containern redan skapats startar du den med:
docker start sqlserver

## Sätt hemligheter (User Secrets)

Hemligheter ligger inte i koden eller i git. Kör från rotmappen och byt ut värdena:
dotnet user-secrets init --project src/api
dotnet user-secrets set "TokenSettings:TokenKey" "<slumpmässig sträng på minst 64 tecken>" --project src/api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=JensenOnline;User Id=sa;Password=<DITT-DB-LÖSENORD>;TrustServerCertificate=True" --project src/api
dotnet user-secrets set "AdminSeed:Email" "<admin-epost>" --project src/api
dotnet user-secrets set "AdminSeed:Password" "<admin-lösenord>" --project src/api

> **OBSERVERA!**
>
> - `TokenKey` signerar JWT och måste vara minst 64 tecken.
> - Databaslösenordet i connection string ska vara samma som i docker-kommandot.
> - `AdminSeed:Password` måste uppfylla Identitys krav: stor och liten bokstav, siffra och specialtecken.

## Starta API:t

dotnet run --project src/api

API:t lyssnar på `http://localhost:5048`. Vid start körs migrationerna automatiskt (databasen `JensenOnline` skapas), rollerna `Admin` och `User` läggs in, 
adminanvändaren skapas från `AdminSeed` och testprodukter läses in från `src/infrastructure/Data/Seed/data.json`.

## Starta frontend

Öppna mappen `frontend` i VS Code och kör "Open with Live Server" på `frontend/pages/start/start.html`. 
Frontend ska köras på port 5500 (`http://127.0.0.1:5500` eller `http://localhost:5500`), eftersom CORS-policyn i API:t bara släpper in de adresserna.
