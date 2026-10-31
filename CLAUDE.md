# sztywniutko

Projekt do nauki: prosta aplikacja Windows Forms (.NET 10, `net10.0-windows`) pisana w Riderze.
Użytkownik dopiero uczy się C# i WinForms — tłumacz po polsku, co i dlaczego robi kod, krok po kroku.

## Co robi aplikacja

Okno „Szponcik” z jednym przyciskiem `StartStopButton`:
- kliknięcie START → rejestruje globalne skróty klawiszowe (działają też, gdy okno jest w tle / zminimalizowane),
- `Ctrl+Alt+A` → zrzut ekranu (wszystkie monitory) zapisany jako PNG, potem wysłany do Claude'a z promptem;
  odpowiedź trafia do pola tekstowego `poleOdpowiedzi` i stringa `ostatniaOdpowiedz`,
- `Ctrl+Alt+S` albo ponowne kliknięcie → wyłącza nasłuchiwanie.

## Struktura

- `Program.cs` — punkt wejścia (`Main`), uruchamia `Form1`. Raczej nie ruszać.
- `Form1.cs` — logika okna: P/Invoke `RegisterHotKey`/`UnregisterHotKey` z `user32.dll`,
  odbiór `WM_HOTKEY` w nadpisanym `WndProc`, lista `zrzuty` ze ścieżkami zrobionych zrzutów.
- `Form1.Designer.cs` — generowany przez designer Ridera (kontrolki, ich właściwości, podpięcia zdarzeń).
  Nie edytować ręcznie — zmiany w wyglądzie robi użytkownik w designerze.
- `ZrzutEkranu.cs` — statyczna klasa: `ZrobIZapisz()` robi zrzut i zwraca ścieżkę pliku.
  Zrzuty lądują w `%LOCALAPPDATA%\sztywniutko\zrzuty\zrzut_yyyy-MM-dd_HH-mm-ss-fff.png`.
- `KlientClaude.cs` — statyczna klasa: `ZapytajOZrzut(sciezka, prompt)` → `Task<string>`. **Bez klucza API** —
  uruchamia zainstalowany Claude Code CLI (`claude.exe`, z WinGet) jako proces:
  `claude -p --output-format text --allowedTools Read`, katalog roboczy = folder zrzutów, prompt przez stdin
  (UTF-8 bez BOM), odpowiedź ze stdout. Limit czasu ustawia użytkownik (`LimitCzasu`).
  Opcjonalny parametr `skill` → prompt zaczyna się od `/nazwa-skilla` (sprawdzone: działa w `claude -p`).
  Bo katalog roboczy to folder zrzutów, widać tylko skille osobiste (`~/.claude/skills`) i z pluginów. Użytkownik nie chce płacić za klucz API —
  nie wracać do pakietu NuGet `Anthropic`.
- `CapsLock.cs` — statyczna klasa: `FlashCaps(powtorzenia, czasFleszaMs, pauzaMs)` → `Task`. Mruga diodą,
  symulując wciśnięcia Caps Locka (`keybd_event`), zawsze parzystą liczbę razy — stan wraca do początkowego,
  bez odczytywania go (odczyt stanu w aplikacji w tle bywa nieaktualny). Na razie nigdzie nie wywoływana.
- `Skrot.cs` — statyczna klasa pomocnicza dla skrótu zrzutu (wartość `Keys` z flagami, np. `Control | Alt | A`):
  `Opis`, `Modyfikatory`/`KodKlawisza` (dla `RegisterHotKey`), `Wczytaj`/`Zapisz` do `%LOCALAPPDATA%\sztywniutko\skrot.txt`.
- `poleOdpowiedzi` (TextBox) i `przyciskSkrotu` (Button) są tworzone kodem w konstruktorze `Form1`, nie w designerze.
- Nagrywanie skrótu: klik `przyciskSkrotu` → `nagrywanie = true` → nadpisany `ProcessCmdKey` łapie kombinację
  (wymagany Ctrl lub Alt, Esc anuluje, STOP = Ctrl+Alt+S zarezerwowany) → `ZakonczNagrywanie` rejestruje i zapisuje.
- Polski układ klawiatury: Ctrl+Alt = AltGr, więc Ctrl+Alt+A/S blokują globalnie wpisywanie „ą”/„ś”, gdy nasłuch jest włączony.

- `sztywniutko.Tests/` — projekt testów xUnit (`net10.0-windows`, referencja do głównego projektu, dodany do `.sln`).
  Główny `.csproj` wyklucza ten folder (`Compile Remove`). `CapsLockTests` naprawdę mruga diodą.
  Uruchamianie: `dotnet test sztywniutko.Tests\sztywniutko.Tests.csproj`.

## Konwencje

- Nazwy zmiennych, metod i komentarze po polsku (`nasluchuje`, `Uruchom`, `Zatrzymaj`, `ZrobZrzut`).
- Komentarze krótkie, wyjaśniające „dlaczego” — to projekt do nauki.
- Handler przycisku nazywa się `button1_Click` (historyczna nazwa z designera; podpięty w `Form1.Designer.cs`).

## Uwagi praktyczne

- Użytkownik ma pliki otwarte w Riderze. Rider potrafi nadpisać zmiany z dysku swoją wersją z edytora —
  po edycji przypomnij, żeby przeładował plik, a nie zapisywał starej wersji.
- Budowanie: `dotnet build sztywniutko.csproj` (gdy aplikacja jest uruchomiona, `.exe` jest zablokowany —
  do sprawdzenia kompilacji użyj `-o %TEMP%\sztywniutko-check`).
- Aplikacja GUI — uruchamia ją użytkownik (`Shift+F10` w Riderze).
- Globalny skrót zarejestrowany przez `RegisterHotKey` jest zabierany innym programom — używać kombinacji z modyfikatorami.
- `CopyFromScreen` rzuca wyjątek na ekranie blokady / przy oknie UAC — dlatego `ZrobZrzut` ma `try/catch`.
