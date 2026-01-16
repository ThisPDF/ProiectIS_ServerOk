# OKServer - Aplicatie Web API cu .NET 8

Aceasta este o aplicatie completa Web API construita cu .NET 8, care include o interfata web moderna pentru gestionarea dispozitivelor si a semnalelor lor de viata (heartbeats).

## Ce face aplicatia

Aplicatia permite:
- Gestionarea dispozitivelor (adaugare, modificare, stergere)
- Inregistrarea si vizualizarea semnalelor de viata de la dispozitive
- O interfata web usoara de utilizat
- Un API REST pentru integrare cu alte sisteme

## Structura aplicatiei

Aplicatia foloseste:
- Baza de date PostgreSQL pentru stocarea datelor
- Entity Framework Core pentru lucrul cu baza de date
- AutoMapper pentru conversia datelor
- ASP.NET Core pentru API si interfata web

## Entitatile din baza de date

### Dispozitiv (Device)
- Id: identificator unic
- Name: numele dispozitivului
- OwnerEmail: email-ul proprietarului
- CreatedAt: data crearii

### Semnal de viata (Heartbeat)
- Id: identificator unic
- DeviceId: id-ul dispozitivului care trimite semnalul
- Status: starea (OK, WARN, ERROR)
- Message: mesaj descriptiv
- Timestamp: data si ora semnalului

## API Endpoints

### DeviceController
- GET /api/devices - obtine toate dispozitivele
- GET /api/devices/{id} - obtine un dispozitiv dupa ID
- POST /api/devices - creeaza un dispozitiv nou
- PUT /api/devices/{id} - modifica un dispozitiv
- DELETE /api/devices/{id} - sterge un dispozitiv

### HeartbeatController
- GET /api/devices/{id}/heartbeats - obtine toate semnalele pentru un dispozitiv
- POST /api/devices/{id}/heartbeats - creeaza un semnal nou pentru un dispozitiv
- GET /api/heartbeats/{id} - obtine un semnal dupa ID
- DELETE /api/heartbeats/{id} - sterge un semnal

## Instalare si rulare

### Varianta 1: Cu Docker (recomandat)

1. Asigura-te ca ai Docker si Docker Compose instalate pe computer.

2. Deschide terminalul in folderul proiectului.

3. Ruleaza comanda:
   ```
   docker-compose up --build
   ```

4. Asteapta ca containerele sa se porneasca. Aplicatia va fi disponibila la adresa:
   - Interfata web: http://localhost:8080
   - Baza de date PostgreSQL: localhost:5433 (daca ai nevoie sa te conectezi direct)

### Varianta 2: Fara Docker (dezvoltare locala)

1. Instaleaza PostgreSQL pe computerul tau si creeaza o baza de date numita `OKServerDb`.

2. Modifica fisierul `appsettings.json` pentru a seta conexiunea la baza de date locala:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=OKServerDb;Username=your_username;Password=your_password"
   }
   ```

3. Instaleaza .NET 8 SDK daca nu il ai.

4. Deschide terminalul in folderul proiectului.

5. Ruleaza migratiile pentru baza de date:
   ```
   dotnet ef database update
   ```

6. Porneste aplicatia:
   ```
   dotnet run
   ```

7. Deschide browser-ul si mergi la:
   - http://localhost:5000 (sau https://localhost:5001 pentru HTTPS)

## Cum se foloseste interfata web

Interfata web are doua sectiuni principale:

### Sectiunea Dispozitive
- Vezi toate dispozitivele adaugate
- Adauga dispozitive noi (completeaza numele si email-ul)
- Modifica dispozitive existente
- Sterge dispozitive
- Vezi semnalele de viata pentru fiecare dispozitiv

### Sectiunea Toate Semnalele
- Vezi toate semnalele de viata din toate dispozitivele
- Semnalele sunt colorate dupa stare:
  - Verde: OK
  - Galben: WARN
  - Rosu: ERROR
- Sterge semnale individuale

### Modal pentru Semnalele unui Dispozitiv
- Cand dai click pe "Vezi Heartbeats" la un dispozitiv
- Vezi toate semnalele acelui dispozitiv
- Adauga semnale noi direct din modal

## Structura fisierelor proiectului

```
OKServer/
├── Controllers/          # Controlerele API
├── Data/                 # Contextul bazei de date
├── DTOs/                 # Obiecte de transfer date
├── Mappings/             # Configurari AutoMapper
├── Models/               # Modelele de date
├── Repositories/         # Clase pentru accesul la date
├── Services/             # Logica de business
├── wwwroot/              # Fisiere statice pentru interfata web
│   ├── index.html
│   ├── css/
│   └── js/
├── Dockerfile            # Configuratie pentru containerul aplicatiei
├── docker-compose.yml    # Configuratie pentru rularea cu Docker
├── appsettings.json      # Setari aplicatie
├── Program.cs            # Punctul de intrare al aplicatiei
└── OKServer.csproj       # Fisierul proiect .NET
```

## Tehnologii folosite

- .NET 8 (ASP.NET Core)
- PostgreSQL (baza de date)
- Entity Framework Core (ORM)
- AutoMapper (mapping obiecte)
- HTML/CSS/JavaScript (interfata web)
- Docker (containerizare)