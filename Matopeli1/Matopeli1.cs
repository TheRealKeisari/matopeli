using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Matopeli1 : PhysicsGame
{
    // Suunnitelmana olisi kirjoittaa matopeli, jossa teemme kaksi matoa joita ohjataan yhdellä näppäimistöllä käyttäen nuolinäppäimiä sekä WASD:ia
    // Aloitetaan tekemällä pelikartalle rajat ja jakamalla se neliöihin
    // "const int" käsitteiden tarkoituksena määrittää pelikartta, kartan koko sekä madon pituus ja aloituspaikka pelin alussa

    private const int Ruudut = 20;
    private const int Leveys = 31;
    private const int Korkeus = 31;
    private const int MadonPituus = 3;
    private const int r = 7;

    // Määritetään aloitukseen suunta
    Direction suunta1;
    Direction suunta2;
    Direction tulevaSuunta1;
    Direction tulevaSuunta2;

    GameObject omena;

    // Lisätään pelille uusi objekti "omena", jota madolla kerätään kasvattaaksemme matoa 

    List<GameObject> matopalat1 = new List<GameObject>();
    List<GameObject> matopalat2 = new List<GameObject>();

    // Annetaan pelille uudet käsitteet "laskurit" ja nimetään ne 

    private IntMeter pelaajan1Pisteet;
    private IntMeter pelaajan2Pisteet;

    public override void Begin()
    {
        AsetaOhjaimet();
        LuoKentta();
        PelinAlku();
    }


    // Käsitellään pelin aloitustilanne
    private void PelinAlku()
    {
        suunta1 = Direction.Up; 
        tulevaSuunta1 = Direction.Up;
        suunta2 = Direction.Down;
        tulevaSuunta2 = Direction.Down;
        pelaajan1Pisteet.Value = 0;
        pelaajan2Pisteet.Value = 0;
        foreach (GameObject madonPala in matopalat1)
        {
            Remove(madonPala);
        }
        matopalat1.Clear();
        foreach (GameObject madonPala in matopalat2)
        {
            Remove(madonPala);
        }
        matopalat2.Clear();

        // Luodaan mato-kavereita
        LuoMatopala1(6 * Ruudut, 3 * Ruudut);
        LuoMatopala1(5 * Ruudut, 3 * Ruudut);
        LuoMatopala1(4 * Ruudut, 3 * Ruudut);
        LuoMatopala1(3 * Ruudut, 3 * Ruudut);

        // Luodaan mato-kavereita
        LuoMatopala2(6 * Ruudut, 1 * Ruudut);
        LuoMatopala2(5 * Ruudut, 1 * Ruudut);
        LuoMatopala2(4 * Ruudut, 1 * Ruudut);
        LuoMatopala2(3 * Ruudut, 1 * Ruudut);
    }


    // Asetetaan ohjaimet pelaajille
    private void AsetaOhjaimet()
    {

        /// Pelaajan 1 ohjaimet
        Keyboard.Listen(Key.Right, ButtonState.Pressed, MuutaSuunta, null, Direction.Right);
        Keyboard.Listen(Key.Left, ButtonState.Pressed, MuutaSuunta, null, Direction.Left);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, MuutaSuunta, null, Direction.Up);
        Keyboard.Listen(Key.Down, ButtonState.Pressed, MuutaSuunta, null, Direction.Down);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        /// Pelaajan 2 ohjaimet
        Keyboard.Listen(Key.D, ButtonState.Pressed, MuutaSuunta2, null, Direction.Right);
        Keyboard.Listen(Key.A, ButtonState.Pressed, MuutaSuunta2, null, Direction.Left);
        Keyboard.Listen(Key.W, ButtonState.Pressed, MuutaSuunta2, null, Direction.Up);
        Keyboard.Listen(Key.S, ButtonState.Pressed, MuutaSuunta2, null, Direction.Down);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    // Luodaan pelikenttä
    private void LuoKentta()
    {
        Level.Width = Ruudut * Leveys;
        Level.Height = Ruudut * Korkeus;
        Level.CreateBorders();


        // Luodaan punainen neliö-omena.
        omena = new GameObject(Ruudut, Ruudut);
        omena.Color = Color.Red;
        Add(omena);


        // Luodaan ajastin 0.1s
        Timer paivitysAjastin = new Timer();
        paivitysAjastin.Interval = 0.1;
        paivitysAjastin.Timeout += PaivitaMatoa;
        paivitysAjastin.Start();

        AsetaOhjaimet();
        LisaaLaskurit();
    }


    // Käsitellään matojen suunnanmuutokset
     void MuutaSuunta(Direction uusiSuunta)
    {
        if (suunta1 == Direction.Right && uusiSuunta != Direction.Left)
            tulevaSuunta1 = uusiSuunta;

        if (suunta1 == Direction.Up && uusiSuunta != Direction.Down)
            tulevaSuunta1 = uusiSuunta;

        if (suunta1 == Direction.Left && uusiSuunta != Direction.Right)
            tulevaSuunta1 = uusiSuunta;

        if (suunta1 == Direction.Down && uusiSuunta != Direction.Up)
            tulevaSuunta1 = uusiSuunta;
    }


    void MuutaSuunta2(Direction uusiSuunta2)
    {
        if (suunta2 == Direction.Right && uusiSuunta2 != Direction.Left)
            tulevaSuunta2 = uusiSuunta2;

        if (suunta2 == Direction.Up && uusiSuunta2 != Direction.Down)
            tulevaSuunta2 = uusiSuunta2;

        if (suunta2 == Direction.Left && uusiSuunta2 != Direction.Right)
            tulevaSuunta2 = uusiSuunta2;

        if (suunta2 == Direction.Down && uusiSuunta2 != Direction.Up)
            tulevaSuunta2 = uusiSuunta2;
    }


    // Lisätään luomamme laskurit peliin
    private void LisaaLaskurit()
    {
        pelaajan1Pisteet = LuoLaskuri(Screen.Left + 100.0, Screen.Top - 100.0);
        pelaajan2Pisteet = LuoLaskuri(Screen.Right - 100.0, Screen.Top - 100.0);
    }


    // Luodaan pistelaskurit, jolloin tiedämme kumpi mato on kerännyt useamman omenan
    private IntMeter LuoLaskuri(double x, double y)
    {
        IntMeter laskuri = new IntMeter(0);

        Label naytto = new Label();
        naytto.BindTo(laskuri);
        naytto.X = x;
        naytto.Y = y;
        naytto.TextColor = Color.Black;
        naytto.BorderColor = Level.Background.Color;
        naytto.Color = Level.Background.Color;
        Add(naytto);

        return laskuri;
    }


    // Käsitellään pisteiden lisäys laskureihin
    private void PisteenLisays1()
    {
        pelaajan1Pisteet.Value += 1;
    }


    private void PisteenLisays2()
    {
        pelaajan2Pisteet.Value += 1;
    }


    // Tämän aliohjelman sisällä käsittelemme sitä, kuinka mato käyttäytyy pelissä
    void PaivitaMatoa()
    {
        suunta1 = tulevaSuunta1;
        GameObject paa1 = matopalat1[0]; // Uusi pää on nykyinen häntä
        GameObject vanhaPaa1 = matopalat1[matopalat1.Count - 1]; // Edeltävä pää on listan loppuun

        suunta2 = tulevaSuunta2;
        GameObject paa2 = matopalat2[0]; // Uusi pää on nykyinen häntä
        GameObject vanhaPaa2 = matopalat2[matopalat2.Count - 1]; // Edeltävä pää on listan loppuun

        // Liikutetaan uusi pää kohtaan, joka on vanhasta päästä RuudunKoon verran siihen
        // suuntaan, johon ollaan menossa.
        paa1.Position = vanhaPaa1.Position + suunta1.GetVector() * Ruudut;
        paa2.Position = vanhaPaa2.Position + suunta2.GetVector() * Ruudut;

        matopalat1.RemoveAt(0); // Poistetaan listan ensimmäinen alkio
        matopalat1.Add(paa1); // Alkio listan loppuun
        matopalat2.RemoveAt(0); // Poistetaan listan ensimmäinen alkio
        matopalat2.Add(paa2); // Alkio listan loppuun


        // Peli päättyy jos osutaan omiin paloihin
        for (int i = 0; i < matopalat1.Count - 1; i++)
        {
            if (matopalat1[i].IsInside(paa1.Position))
            {
                PelinAlku(); // Restart
                return; // PaivitaMatoa aliohjelma päättyy
            }
            if (matopalat1[i].IsInside(paa2.Position))
            {
                PelinAlku(); // Restart
                return; // PaivitaMatoa aliohjelma päättyy
            }
        }

        
        // Peli päättyy jos osutaan omiin paloihin
        for (int i = 0; i < matopalat2.Count - 1; i++)
        {
            if (matopalat2[i].IsInside(paa2.Position))
            {
                PelinAlku(); // Restart
                return; // PaivitaMatoa aliohjelma päättyy
            }
            if (matopalat2[i].IsInside(paa1.Position))
            {
                PelinAlku(); // Restart
                return; // PaivitaMatoa aliohjelma päättyy
            }
        }


        // Jos menee ulos kartasta, niin peli päättyy
        if (!Level.BoundingRect.IsInside(paa1.Position))
        {
            PelinAlku();
            return;
        }
        if (!Level.BoundingRect.IsInside(paa2.Position))
        {
            PelinAlku();
            return;
        }


            // törmäsikö pää omenaan
        if (omena.IsInside(paa1.Position))
        {
            // Siirretään omena satunnaiseen paikkaan.
            double satunnainenX = RandomGen.NextInt(-Leveys / 2, Leveys / 2);
            double satunnainenY = RandomGen.NextInt(-Korkeus / 2, Korkeus / 2);
            omena.Position = new Vector(satunnainenX, satunnainenY) * Ruudut;

            // Luodaan uusi madon pala.
            PisteenLisays1();
            LuoMatopala1(matopalat1[0].Position.X, matopalat1[1].Position.Y);
        }
        if (omena.IsInside(paa2.Position))
        {
            // Siirretään omena satunnaiseen paikkaan.
            double satunnainenX = RandomGen.NextInt(-Leveys / 2, Leveys / 2);
            double satunnainenY = RandomGen.NextInt(-Korkeus / 2, Korkeus / 2);
            omena.Position = new Vector(satunnainenX, satunnainenY) * Ruudut;

            // Luodaan uusi madon pala.
            PisteenLisays2();
            LuoMatopala2(matopalat2[0].Position.X, matopalat2[1].Position.Y);

        }

    }


    // Käsitellään uuden palan lisäys madon jatkeeksi
    void LuoMatopala1(double x, double y)
    {
        GameObject pala = new GameObject(Ruudut, Ruudut);
        pala.X = x;
        pala.Y = y;
        matopalat1.Insert(0, pala); // Lisää palan matopalat listan alkuun.
        Add(pala); // Lisää pala pelikentälle.
    }


    void LuoMatopala2(double x, double y)
    {
        GameObject pala = new GameObject(Ruudut, Ruudut);
        pala.X = x;
        pala.Y = y;
        matopalat2.Insert(0, pala); // Lisää palan matopalat listan alkuun.
        Add(pala); // Lisää pala pelikentälle.
    }




}