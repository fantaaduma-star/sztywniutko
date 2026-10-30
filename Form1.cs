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

    public Form1()
    {
        InitializeComponent();

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
        bool okA = RegisterHotKey(Handle, SKROT_A, MOD_CONTROL | MOD_ALT | MOD_NOREPEAT, (uint)Keys.A);
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
