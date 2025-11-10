# Zadatak 1 — Upravljanje partnerima osiguravajućeg društva

## Opis

Jednostavna web aplikacija razvijena u **ASP.NET Core** s **Dapper ORM**‑om, koja omogućuje upravljanje partnerima i policama osiguranja.

Aplikacija sadrži:
- **Početnu stranicu** s listom svih partnera (Full Name, tip, datum, oznaka * ako partner ima > 5 polica ili ukupno > 5000 kn),
- **Formu za unos novog partnera** s validacijom svih polja,
- **Dijalog za unos polica** povezanih s partnerom,
- **Seeder** za automatsko punjenje baze prilikom prvog pokretanja.

---

## Tehnologije

- **Backend:** C# / ASP.NET Core 8 MVC  
- **Frontend:** HTML, JavaScript, Bootstrap 
- **Baza:** SQL Server / LocalDB  
- **ORM:** Dapper Micro ORM  

---

## Pokretanje projekta

1. Klonirati repozitorij:

2. Provjeriti da SQL Server (LocalDB) radi i ažurirati konekcijski string u `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=Wiener;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Pokrenuti inicijalnu SQL skriptu:
   ```bash
   Database/Scripts/create-database.sql
   ```
   Skripta kreira tablice:
   - `PartnerTypes`
   - `Partners`
   - `Policies`

4. Pokrenuti aplikaciju
5. 
   Aplikacija automatski poziva `DatabaseSeeder` iz `Data/Seed/DatabaseSeeder.cs`  
   i puni bazu inicijalnim testnim podacima.
 
---

## Struktura baze

<img width="1055" height="636" alt="image" src="https://github.com/user-attachments/assets/c8fcab6f-6106-42ec-93d0-1a2e3e220b79" />


- **PartnerTypes** — Personal / Legal  
- **Partners** — osnovni podaci o partneru  
- **Policies** — police osiguranja povezane s partnerom  

---

## Inicijalni podaci

- 5 partnera se seeda automatski (Personal / Legal)  
- Partner **Marko Marković** ima 6 polica  → prikazan sa *  
- Partner **Ana Anić** ima ukupnu vrijednost polica > 5000 kn  → označen *  

---

## Napomene

- Aplikacija je ogledni primjer rješenja i ispunjava osnovne zahtjeve zadatka.  
- Nema implementiranu autentifikaciju / autorizaciju (samo demo).  
- Inicijalni seed omogućuje testiranje bez ručnog unosa.

---

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

Prilikom ulaska vozila, sustav pronalazi slobodno mjesto preko Redis-a (da bi izbjegao čitanje iz baze pri svakom ulasku).  
Nakon toga upisuje sesiju u SQL bazu i ažurira cache.

```csharp
public async Task HandleVehicleEntryAsync(string vehicleId)
{
    int freeSpotId = await _redis.GetAsync<int>("garage:freespots:next");

    if (freeSpotId == 0)
    {
        throw new InvalidOperationException("No free parking spots.");
    }

    using var transaction = await _db.BeginTransactionAsync();

    try
    {
        // kreiraj parking
        var session = new ParkingSession
        {
            SpotId = freeSpotId,
            VehicleIdentifier = vehicleId,
            EntryTime = DateTime.UtcNow,
            Status = "Active"
        };

        await _db.ParkingSessions.AddAsync(session);
        await _db.SaveChangesAsync();

        // označi mjesto kao zauzeto 
        await _db.ExecuteSqlAsync(
            "UPDATE ParkingSpots SET IsOccupied = 1 WHERE SpotId = @id", new { id = freeSpotId });

        await _redis.DecrementAsync("garage:freespots:count");
        await _redis.PublishAsync("events:spot:occupied", freeSpotId.ToString());

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### Proces 2 — Izračun i naplata parkiranja

Ovdje koristimo `PricingService` i `WeatherService` za dohvat tarifa i vremenskih podataka.  
Popust se primjenjuje ako je vozilo bilo barem 33% vremena izloženo kiši.

```csharp
public async Task ProcessPaymentAsync(long sessionId)
{
    var session = await _db.ParkingSessions.FindAsync(sessionId);

    if (session == null || session.ExitTime == null)
        throw new InvalidOperationException("Invalid session.");

    var duration = (session.ExitTime.Value - session.EntryTime).TotalHours;
    var baseRate = await _pricingService.GetActiveRateAsync("Hourly");

    decimal baseAmount = baseRate * (decimal)Math.Ceiling(duration);

    // dohvati logove o vremenu
    bool hasRainDiscount = await _weatherService.WasRainingForMoreThanAsync(
        session.EntryTime, session.ExitTime.Value, thresholdPercent: 33);

    decimal discount = 0;
    string? discountReason = null;

    if (!session.Spot.IsCovered && hasRainDiscount)
    {
        discount = baseAmount * 0.5m;
        discountReason = "Rain Discount";
    }

    decimal finalAmount = baseAmount - discount;

    var payment = new Payment
    {
        SessionId = sessionId,
        BaseAmount = baseAmount,
        DiscountAmount = discount,
        Amount = finalAmount,
        DiscountReason = discountReason,
        PaymentMethod = "Card",
        Status = "Completed",
        PaymentTime = DateTime.UtcNow
    };

    await _db.Payments.AddAsync(payment);
    await _db.SaveChangesAsync();

    await _redis.PublishAsync("events:payment:completed", sessionId.ToString());

    session.Status = "Paid";
    await _db.SaveChangesAsync();
}
```

### Proces 3 — Izlazak vozila

Sustav provjerava status plaćanja i vrijeme otkako je plaćeno.Ako je prošlo više od 10 minuta, naplaćuje dodatni sat.

```csharp
public async Task HandleVehicleExitAsync(string ticketNumber)
{
    var session = await _db.ParkingSessions
        .Include(s => s.Payment)
        .Include(s => s.Spot)
        .FirstOrDefaultAsync(s => s.TicketNumber == ticketNumber);

    if (session == null)
        throw new InvalidOperationException("Ticket not found.");

    if (session.Status != "Paid")
        throw new Exception("Payment required before exit.");

    var minutesSincePayment = (DateTime.UtcNow - session.Payment.PaymentTime).TotalMinutes;

    if (minutesSincePayment > 10)
    {
        await _paymentService.ChargeExtraHourAsync(session.SessionId);
    }

    session.ExitTime = DateTime.UtcNow;
    session.Status = "Completed";
    session.Spot.IsOccupied = false;

    await _db.SaveChangesAsync();

    await _redis.IncrementAsync("garage:freespots:count");
    await _redis.PublishAsync("events:spot:free", session.Spot.SpotId.ToString());
}
```

### Proces 4 — Generiranje mjesečnog izvještaja

```csharp
public async Task GenerateMonthlyReportAsync(int year, int month)
{
    var sessions = await _db.ParkingSessions
        .Where(s => s.EntryTime.Year == year && s.EntryTime.Month == month)
        .ToListAsync();

    var totalRevenue = sessions.Sum(s => s.Payment?.Amount ?? 0);
    var totalDiscount = sessions.Sum(s => s.Payment?.DiscountAmount ?? 0);
    var rainDiscountAmt = sessions
        .Where(s => s.Payment?.DiscountReason == "Rain Discount")
        .Sum(s => s.Payment.DiscountAmount);

    var occupancyRate = await _redis.GetAsync<decimal>("garage:avg:occupancy:" + month);

    var report = new MonthlyReport
    {
        Year = year,
        Month = month,
        TotalSessions = sessions.Count,
        TotalRevenue = totalRevenue,
        TotalDiscount = totalDiscount,
        RainDiscountAmount = rainDiscountAmt,
        AverageOccupancyRate = occupancyRate,
        GeneratedAt = DateTime.UtcNow
    };

    await _db.MonthlyReports.AddAsync(report);
    await _db.SaveChangesAsync();

    await _redis.SetAsync($"reports:{year}:{month}", report, TimeSpan.FromDays(30));
}
```


