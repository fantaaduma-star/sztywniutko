using System.Runtime.InteropServices;

namespace sztywniutko;

public partial class Form1 : Form
{
    // --- funkcje z Windows (user32.dll), których .NET nie ma "w pudełku" ---
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int WM_HOTKEY = 0x0312;      // komunikat, który Windows wysyła po wciśnięciu skrótu
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_NOREPEAT = 0x4000;  // przytrzymanie klawisza nie powtarza zdarzenia

    private const int SKROT_A = 1;             // nasze własne numery skrótów
    private const int SKROT_STOP = 2;

    private bool nasluchuje = false;

    private readonly List<string> zrzuty = new();   // ścieżki do zrobionych zrzutów – do późniejszego użytku

    private const string PROMPT = "Opisz krótko po polsku, co widać na tym zrzucie ekranu.";

    private string ostatniaOdpowiedz = "";          // ostatnia odpowiedź Claude'a – do dalszego użytku w programie

    private readonly TextBox poleOdpowiedzi;        // tu wyświetlamy odpowiedź

    private Keys skrotZrzutu = Skrot.Wczytaj();     // skrót do zrzutu – zapisany na dysku albo domyślny Ctrl+Alt+A
    private const Keys SKROT_STOP_KLAWISZE = Keys.Control | Keys.Alt | Keys.S;
    private bool nagrywanie = false;                // czy właśnie czekamy, aż użytkownik wciśnie nowy skrót
    private readonly Button przyciskSkrotu;

    public Form1()
    {
        InitializeComponent();

        przyciskSkrotu = new Button
        {
            Location = new Point(150, 42),
            Size = new Size(240, 36),
        };
        przyciskSkrotu.Click += PrzyciskSkrotu_Click;   // podpinamy zdarzenie, tak jak designer robi dla StartStopButton
        Controls.Add(przyciskSkrotu);
        OdswiezPrzyciskSkrotu();

        // pole tekstowe tworzone kodem (a nie w designerze): utwórz -> ustaw właściwości -> dodaj do Controls
        poleOdpowiedzi = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Location = new Point(12, 90),
            Size = new Size(ClientSize.Width - 24, ClientSize.Height - 102),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,   // rośnie razem z oknem
        };
        Controls.Add(poleOdpowiedzi);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (nasluchuje)
            Zatrzymaj();
        else
            Uruchom();
    }

    private void Uruchom()
    {
        bool okA = ZarejestrujSkrotZrzutu();
        bool okStop = RegisterHotKey(Handle, SKROT_STOP, MOD_CONTROL | MOD_ALT | MOD_NOREPEAT, (uint)Keys.S);

        if (!okA || !okStop)
        {
            MessageBox.Show("Nie udało się zarejestrować skrótu – może używa go inny program.");
            Zatrzymaj();
            return;
        }

        nasluchuje = true;
        StartStopButton.Text = "STOP";
    }

    private void Zatrzymaj()
    {
        UnregisterHotKey(Handle, SKROT_A);
        UnregisterHotKey(Handle, SKROT_STOP);
        nasluchuje = false;
        StartStopButton.Text = "START";
    }

    private bool ZarejestrujSkrotZrzutu()
    {
        UnregisterHotKey(Handle, SKROT_A);   // ten sam numer nie może być zarejestrowany dwa razy
        return RegisterHotKey(Handle, SKROT_A, Skrot.Modyfikatory(skrotZrzutu) | MOD_NOREPEAT, Skrot.KodKlawisza(skrotZrzutu));
    }

    // ---------- nagrywanie nowego skrótu ----------

    private void PrzyciskSkrotu_Click(object? sender, EventArgs e)
    {
        nagrywanie = true;
        if (nasluchuje)
            UnregisterHotKey(Handle, SKROT_A);   // inaczej stary skrót zamiast trafić do okna, zrobiłby zrzut
        przyciskSkrotu.Text = "Wciśnij nowy skrót... (Esc = anuluj)";
    }

    // ProcessCmdKey dostaje KAŻDY klawisz wciśnięty w oknie – razem z modyfikatorami w jednej wartości keyData,
    // także te, których KeyDown nie widzi (Tab, strzałki, Enter, kombinacje z Alt).
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (!nagrywanie)
            return base.ProcessCmdKey(ref msg, keyData);   // normalne działanie okna

        Keys klawisz = keyData & Keys.KeyCode;

        if (keyData == Keys.Escape)
            ZakonczNagrywanie(null);
        else if (Skrot.ToSamModyfikator(klawisz))
            { }                                             // sam Ctrl/Alt/Shift – czekamy na właściwy klawisz
        else if (!keyData.HasFlag(Keys.Control) && !keyData.HasFlag(Keys.Alt))
            przyciskSkrotu.Text = "Dodaj Ctrl albo Alt! (Esc = anuluj)";
        else if (keyData == SKROT_STOP_KLAWISZE)
            przyciskSkrotu.Text = "Ten skrót to STOP – wybierz inny";
        else
            ZakonczNagrywanie(keyData);

        return true;   // true = "obsłużyłem ten klawisz", nie przekazuj go dalej (np. do przycisku)
    }

    // nowy == null oznacza anulowanie
    private void ZakonczNagrywanie(Keys? nowy)
    {
        nagrywanie = false;
        Keys stary = skrotZrzutu;
        if (nowy != null)
            skrotZrzutu = nowy.Value;

        if (nasluchuje && !ZarejestrujSkrotZrzutu())
        {
            MessageBox.Show($"Skrótu {Skrot.Opis(skrotZrzutu)} używa już inny program. Zostaje {Skrot.Opis(stary)}.");
            skrotZrzutu = stary;
            ZarejestrujSkrotZrzutu();
        }
        else if (nowy != null)
        {
            Skrot.Zapisz(skrotZrzutu);           // zapamiętujemy na następne uruchomienie
        }

        OdswiezPrzyciskSkrotu();
    }

    private void OdswiezPrzyciskSkrotu() =>
        przyciskSkrotu.Text = $"Skrót zrzutu: {Skrot.Opis(skrotZrzutu)} (zmień)";

    // Każde okno ma metodę WndProc, przez którą przechodzą WSZYSTKIE komunikaty od Windows
    // (kliknięcia, ruchy myszy, zmiana rozmiaru...). Wyłapujemy z nich WM_HOTKEY.
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY)
        {
            int id = m.WParam.ToInt32();       // który skrót? (nasz numer z RegisterHotKey)

            if (id == SKROT_A)
            {
                ZrobZrzut();
            }
            else if (id == SKROT_STOP)
            {
                Zatrzymaj();
            }
        }

        base.WndProc(ref m);                   // resztę komunikatów obsługuje standardowo
    }

    // "async void" tylko dla metod wywoływanych jak zdarzenia – nikt nie czeka na ich wynik
    private async void ZrobZrzut()
    {
        string sciezka;
        try
        {
            sciezka = ZrzutEkranu.ZrobIZapisz();
            zrzuty.Add(sciezka);
            System.Media.SystemSounds.Beep.Play();          // słychać nawet, gdy okno jest zminimalizowane
            Text = $"Zrzut nr {zrzuty.Count}: {Path.GetFileName(sciezka)}";
        }
        catch (Exception ex)
        {
            // np. ekran blokady albo okno UAC – Windows nie pozwala ich skopiować
            Text = $"Nie udało się zrobić zrzutu: {ex.Message}";
            return;
        }

        poleOdpowiedzi.Text = "Czekam na odpowiedź Claude'a...";
        try
        {
            // await: okno działa dalej, a po odpowiedzi kod wraca tutaj (na wątek okna)
            ostatniaOdpowiedz = await KlientClaude.ZapytajOZrzut(sciezka, PROMPT);
            poleOdpowiedzi.Text = ostatniaOdpowiedz;
        }
        catch (Exception ex)
        {
            // brak claude.exe, brak internetu, przekroczony czas, wylogowane konto...
            poleOdpowiedzi.Text = $"Błąd przy zapytaniu do Claude'a: {ex.Message}";
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        Zatrzymaj();                           // sprzątamy: zwalniamy skróty przy zamykaniu
        base.OnFormClosed(e);
    }
}
