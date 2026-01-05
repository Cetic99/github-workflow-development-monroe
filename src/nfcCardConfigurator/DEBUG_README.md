# NFC Card Tool - Debug Feature

## Debug Console (Side Panel)

Debug panel omogućava praćenje svih operacija aplikacije u realnom vremenu, integrisano direktno u glavni prozor sa istom color šemom kao i ostatak aplikacije.

### Kako otvoriti Debug Console:

1. Klikni na **🐛 Debug** dugme (zeleno) u gornjem desnom uglu glavnog prozora
2. Debug panel će se otvoriti sa desne strane aplikacije (400px širine)
3. Klikni ponovo na **🐛 Debug** ili na **✕** u panelu da zatvoriš panel

### Funkcionalnosti:

- **Real-time logging** - Sve debug poruke se prikazuju odmah
- **Side panel design** - Ne otvara novi prozor, već panel unutar aplikacije
- **Auto-scroll** - Automatsko pomeranje na najnovije poruke (može se isključiti)
- **Clear button** (zeleno) - Brisanje svih poruka
- **Message counter** - Prikazuje broj poruka u konzoli
- **Consistent theme** - Iste boje kao i ostatak aplikacije (teal/cyan paleta)
- **Collapsible** - Panel se može sakriti i prikazati po potrebi

### Dizajn:

- **Panel Width**: 400px
- **Background**: Svetlo cyan (#E8F8F5) - kao ostatak aplikacije
- **Log Area**: Bela pozadina sa card shadow efektom
- **Font**: Consolas 11px (monospace za logove)
- **Header**: Bela pozadina (#FFFFFF) sa card shadow
- **Footer**: Svetlo cyan-green (#D1F2ED)
- **Text Colors**: 
  - Primary: Dark Teal-Green (#0F3A34)
  - Secondary: Medium Teal (#2C6B62)
- **Debug Button**: Zelena boja (ModernButton style)
- **Clear Button**: Zelena boja (ModernButton style, #0D8472)
- **Border**: Light Teal Border (#A5D8CF)

### Šta se loguje:

#### NFC Service:
- ✅ Inicijalizacija servisa
- ✅ Pronalaženje NFC čitača
- ✅ Povezivanje sa čitačem
- ✅ Detekcija kartice
- ✅ Čitanje UID-a
- ✅ Čitanje podataka sa kartice
- ✅ Pisanje podataka na karticu
- ✅ Card monitoring events

#### UI Events:
- ✅ Window loading
- ✅ Button clicks
- ✅ Connection status changes
- ✅ Errors and warnings

### Tips:
- Ostavi debug panel otvoren dok testiraš aplikaciju
- Koristi Clear dugme da očistiš log pre testiranja nove funkcionalnosti
- Isključi Auto-scroll ako želiš da čitaš stare poruke dok novi logovi dolaze
- Panel se automatski prilagođava visini prozora
- Svetla tema omogućava lakše čitanje tokom dužeg rada

### Prednosti Side Panel dizajna:

✅ Sve na jednom mestu - nema potrebe za prebacivanjem između prozora
✅ Lakše praćenje - vidiš debug logove i aplikaciju istovremeno
✅ Moderniji izgled - integrisani UI umesto floating prozora
✅ Brže testiranje - toggle on/off sa jednim klikom
✅ **Konzistentna tema** - Iste boje kao i ostatak aplikacije (teal/cyan)
✅ Profesionalan izgled - Card shadows i zaobljeni uglovi

## Debug Poruke u Kodu

Sve ključne operacije loguju debug poruke:

```csharp
Debug.WriteLine("Your debug message here");
```

Ove poruke se automatski prikazuju u Debug panelu kada je otvoren.

## Layout

```
+--------------------------------+------------------+
|    |  |
|    Main Application (800px)    | Debug Panel  |
|    [Logo] [Title] [Debug ✕]    | (400px)          |
|    |             |
|    [NFC Reader Selection]      | 🐛 Debug Console |
|    [Card Status]               | ✕ |
|    [Read] [Write]          |        |
|  | ╔══════════════╗ |
|       | ║ Log messages ║ |
|       | ║ (White bg)   ║ |
|      | ╚══════════════╝ |
|     |       |
|   | [Auto] [Count]   |
|          | [Clear]          |
+--------------------------------+------------------+
```

### Color Palette Match:

Debug panel koristi **istu color paletu** kao i glavna aplikacija:

- **Background**: `#E8F8F5` (Very Light Cyan)
- **Card Background**: `#FFFFFF` (White)
- **Section Background**: `#D1F2ED` (Light Cyan-Green)
- **Text Primary**: `#0F3A34` (Dark Teal-Green)
- **Text Secondary**: `#2C6B62` (Medium Teal)
- **Border Color**: `#A5D8CF` (Light Teal Border)
- **Primary Color**: `#0D8472` (Dark Teal - za dugmiće)

Total Window Width: 1200px (kada je debug panel otvoren)
