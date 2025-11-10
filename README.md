# Zadatak 2 — Sustav za upravljanje parkiralištem

## 1. Analiza korisničkog zahtjeva

### Ključne funkcionalnosti
- Evidentiranje ulaska i izlaska vozila iz garaže.
- Naplata parkiranja prema vremenu zadržavanja.
- Pregled slobodnih parkirnih mjesta (ukupno i po katovima).
- Mogućnost mjesečnih ugovora za stalne korisnike.
- Automatizirano generiranje mjesečnih izvještaja o radu.
- „Kišna“ akcija (popust 50% za nenatkrivena mjesta u kišno vrijeme).
- Plaćanje isključivo unutar garaže (ne na izlazu).

### Ključni procesi
1. **Upravljanje parkiranjem**
   - Detekcija ulaska i dodjela parkirnog mjesta.
   - Evidencija trajanja parkiranja.
   - Naplata na terminalima ili putem ugovora.

2. **Proces naplate**
   - Računanje cijene temeljem trajanja i pravila cijena (PricingRules).
   - Primjena „kišne“ akcije.
   - Validacija plaćanja i dopuštenje izlaza (10-minutno pravilo).

3. **Praćenje vremenskih uvjeta**
   - Logiranje vremenskih podataka u `WeatherLogs`.
   - Automatska aktivacija popusta temeljem trajanja padalina.

4. **Izvještavanje**
   - Automatsko generiranje mjesečnih izvještaja (`MonthlyReports`).
   - Analiza popunjenosti, prihoda i utjecaja „kišne“ akcije.

5. **Upravljanje korisnicima**
   - Administracija korisnika i ugovora (`Users`, `Contracts`).
   - Različite razine pristupa (Admin, Contract, Regular).

### Potencijalni problemi / rizici
- **Nerazrađen mehanizam identifikacije vozila** (kartica, QR kod, tablice?).
- **Ovisnost o točnim vremenskim logovima** za popust po kiši.

---

## 2. Idejna arhitektura rješenja

Arhitektura je **više-slojna**, s jasnim razdvajanjem odgovornosti:
<img width="586" height="548" alt="image" src="https://github.com/user-attachments/assets/8cdeb27f-7cf1-4f20-a09f-60b3b264ff60" />

### Tehnološki stack
- **Backend:** ASP.NET Core  
- **Frontend / Admin:** Blazor Server ili MVC
- **Baza podataka:** SQL Server
- **Cache:** Redis
- **Message Queue:** RabbitMQ / Azure Service Bus
- **Autentifikacija:** ASP.NET Identity + JWT

---

## 3. Idejni nacrt baze podataka

Glavne tablice sustava:
- `Users` — korisnici sustava (Admin, Contract, Regular)
- `ParkingSpots` — parkirna mjesta po katovima
- `ParkingSessions` — evidencija svake sesije parkiranja
- `Payments` — naplate po sesiji
- `Contracts` — mjesečni pretplatnički ugovori
- `MonthlyReports` — rezultati poslovanja
- `AuditLog` — zapis svih važnih promjena sustava

Zamišljeni relacijski odnosi (ERa dijagram):

<img width="1184" height="1085" alt="image" src="https://github.com/user-attachments/assets/81e4eeb5-cc09-47f8-a21d-5c8309c04c4e" />


## 4. Pseudokod ključnih procesa

### Proces 1 — Ulazak vozila
### Proces 2 — Izračun i naplata parkiranja
### Proces 3 — Izlazak vozila
### Proces 4 — Generiranje mjesečnog izvještaja




