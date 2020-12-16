using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

/// <summary>
/// @author Juuso Jokinen
/// @author Oskari Neuvonen
/// Version 15.12.2020
/// Kahden pelaajan matopeli, jossa tarkoituksena kilpailla toisen pelaajan kanssa
/// </summary>
public class Matopeli1 : PhysicsGame
{
    /// <summary>
    /// Suunnitelmana olisi kirjoittaa matopeli, jossa teemme kaksi matoa joita ohjataan yhdellä näppäimistöllä käyttäen nuolinäppäimiä sekä WASD:ia
    /// Aloitetaan tekemällä pelikartalle rajat ja jakamalla se neliöihin
    /// "const int" käsitteiden tarkoituksena määrittää pelikartta, kartan koko sekä madon pituus ja aloituspaikka pelin alussa. 
    /// </summary>

    private const int Ruudut = 20;
    private const int Leveys = 31;
    private const int Korkeus = 31;


    /// <summary>
    /// Määritetään aloituksen suunta
    /// </summary>
    private Direction suunta1;
    private Direction suunta2;
    private Direction tulevaSuunta1;
    private Direction tulevaSuunta2;


    /// <summary>
    /// Lisätään pelille uusi objekti "omena", jota madolla kerätään kasvattaaksemme matoa
    /// </summary>
    private GameObject omena;


    /// <summary>
    /// lisätään käsite madon palasille
    /// </summary>
    List<GameObject> matopalat1 = new List<GameObject>();
    List<GameObject> matopalat2 = new List<GameObject>();


    /// <summary>
    /// Annetaan pelille uudet käsitteet "laskurit" ja nimetään ne 
    /// </summary>
    private IntMeter pelaajan1Pisteet;
    private IntMeter pelaajan2Pisteet;

    public override void Begin()
    {
        LuoKentta();
        LisaaLaskurit();
        PelinAlku();
        AsetaOhjaimet();
    }


    /// <summary>
    /// Käsitellään pelin aloitustilanne
    /// </summary>
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


        /// Luodaan mato-kavereita
        LuoMatopala(matopalat1, 6 * Ruudut, 3 * Ruudut);
        LuoMatopala(matopalat1, 5 * Ruudut, 3 * Ruudut);
        LuoMatopala(matopalat1, 4 * Ruudut, 3 * Ruudut);
        LuoMatopala(matopalat1, 3 * Ruudut, 3 * Ruudut);

        
        LuoMatopala(matopalat2, 6 * Ruudut, 1 * Ruudut);
        LuoMatopala(matopalat2, 5 * Ruudut, 1 * Ruudut);
        LuoMatopala(matopalat2, 4 * Ruudut, 1 * Ruudut);
        LuoMatopala(matopalat2, 3 * Ruudut, 1 * Ruudut);
    }


    /// <summary>
    /// Asetetaan ohjaimet pelaajille
    /// </summary>
    private void AsetaOhjaimet()
    {

        /// Pelaajan 1 ohjaimet
        Keyboard.Listen(Key.Right, ButtonState.Pressed, MuutaSuunta, null, Direction.Right, '1');
        Keyboard.Listen(Key.Left, ButtonState.Pressed, MuutaSuunta, null, Direction.Left, '1');
        Keyboard.Listen(Key.Up, ButtonState.Pressed, MuutaSuunta, null, Direction.Up, '1');
        Keyboard.Listen(Key.Down, ButtonState.Pressed, MuutaSuunta, null, Direction.Down, '1');
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        /// Pelaajan 2 ohjaimet
        Keyboard.Listen(Key.D, ButtonState.Pressed, MuutaSuunta, null, Direction.Right, '0');
        Keyboard.Listen(Key.A, ButtonState.Pressed, MuutaSuunta, null, Direction.Left, '0');
        Keyboard.Listen(Key.W, ButtonState.Pressed, MuutaSuunta, null, Direction.Up, '0');
        Keyboard.Listen(Key.S, ButtonState.Pressed, MuutaSuunta, null, Direction.Down, '0');
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Käsitellään matojen suunnanmuutokset 
    /// </summary>
    private void MuutaSuunta(Direction uusiSuunta, char suunta)
    {
        TarkistaSuunta(uusiSuunta, Direction.Right, Direction.Left, suunta);

        TarkistaSuunta(uusiSuunta, Direction.Left, Direction.Right, suunta);

        TarkistaSuunta(uusiSuunta, Direction.Up, Direction.Down, suunta);

        TarkistaSuunta(uusiSuunta, Direction.Down, Direction.Up, suunta);
    }


    void TarkistaSuunta(Direction uusiSuunta, Direction x, Direction y, char suunta)
    {
        if (suunta == '1')
        {
            if (suunta1 == x && uusiSuunta != y) tulevaSuunta1 = uusiSuunta;
        }
        else if (suunta2 == x && uusiSuunta != y) tulevaSuunta2 = uusiSuunta;
    }


    /// <summary>
    /// Luodaan pelikenttä
    /// </summary>
    private void LuoKentta()
    {
        Level.Width = Ruudut * Leveys;
        Level.Height = Ruudut * Korkeus;
        Level.CreateBorders();


        /// Luodaan punainen neliö-omena.
        omena = new GameObject(Ruudut, Ruudut);
        omena.Color = Color.Red;
        Add(omena);


        /// Luodaan ajastin 0.1s
        Timer paivitysAjastin = new Timer();
        paivitysAjastin.Interval = 0.1;
        paivitysAjastin.Timeout += PaivitaMatoa;
        paivitysAjastin.Start();


    }


    /// <summary>
    /// Lisätään luomamme laskurit peliin ja sijoitetaan ne sopiviin kohtiin pelikarttaa
    /// </summary>
    private void LisaaLaskurit()
    {
        pelaajan1Pisteet = LuoLaskuri(Screen.Left + 100.0, Screen.Top - 100.0);
        pelaajan2Pisteet = LuoLaskuri(Screen.Right - 100.0, Screen.Top - 100.0);
    }


    /// <summary>
    /// Luodaan pistelaskurit, jolloin tiedämme kumpi mato on kerännyt useamman omenan
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns laskuri></returns>
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



    /// <summary>
    /// Tämän aliohjelman sisällä käsittelemme sitä, kuinka mato käyttäytyy pelissä
    /// </summary>
    void PaivitaMatoa()
    {
        suunta1 = tulevaSuunta1;
        GameObject paa1 = matopalat1[0]; /// Uusi pää on nykyinen häntä
        GameObject vanhaPaa1 = matopalat1[matopalat1.Count - 1]; /// Edeltävä pää on listan loppuun

        suunta2 = tulevaSuunta2;
        GameObject paa2 = matopalat2[0]; /// Uusi pää on nykyinen häntä
        GameObject vanhaPaa2 = matopalat2[matopalat2.Count - 1]; /// Edeltävä pää on listan loppuun

        /// Liikutetaan uusi pää kohtaan, joka on vanhasta päästä RuudunKoon verran siihen
        /// suuntaan, johon ollaan menossa.
        paa1.Position = vanhaPaa1.Position + suunta1.GetVector() * Ruudut;
        paa2.Position = vanhaPaa2.Position + suunta2.GetVector() * Ruudut;

        matopalat1.RemoveAt(0); /// Poistetaan listan ensimmäinen alkio
        matopalat1.Add(paa1); /// Alkio listan loppuun

        matopalat2.RemoveAt(0); /// Poistetaan listan ensimmäinen alkio
        matopalat2.Add(paa2); /// Alkio listan loppuun


        /// Jos menee ulos kartasta, niin peli päättyy
        if (!Level.BoundingRect.IsInside(paa1.Position) || (!Level.BoundingRect.IsInside(paa2.Position)) || (collision(matopalat1, matopalat2)))
        {
            PelinAlku();
            return;
        }
        SyotiinkoOmena(matopalat1, paa1, pelaajan1Pisteet);
        SyotiinkoOmena(matopalat2, paa2, pelaajan2Pisteet);
    }

    private void SyotiinkoOmena(List<GameObject> matopalat, GameObject paa, IntMeter laskuri)
    {
        if (omena.IsInside(paa.Position))
        {
            /// Siirretään omena satunnaiseen paikkaan.
            double satunnainenX = RandomGen.NextInt(-Leveys / 2, Leveys / 2);
            double satunnainenY = RandomGen.NextInt(-Korkeus / 2, Korkeus / 2);
            omena.Position = new Vector(satunnainenX, satunnainenY) * Ruudut;

            /// Luodaan uusi madon pala.
            laskuri.Value += 1;
            LuoMatopala(matopalat, matopalat[0].Position.X, matopalat[1].Position.Y);
        }
    }

    /// <summary>
    /// Peli päättyy jos osutaan omiin paloihin (mato 1 ensimmäinen silmukka, mato2 toisessa)
    /// </summary>
    private bool collision(List<GameObject> matopalat1, List<GameObject> matopalat2)
    {
        for (int i = 0; i < matopalat1.Count - 1; i++)
        {
            if (matopalat1[i].IsInside(matopalat1[matopalat1.Count - 1].Position) || (matopalat1[i].IsInside(matopalat2[matopalat2.Count - 1].Position)))
            {
                return true; /// PaivitaMatoa aliohjelma päättyy
            }
        }


        for (int i = 0; i < matopalat2.Count - 1; i++)
        {
            if (matopalat2[i].IsInside(matopalat2[matopalat2.Count - 1].Position) || (matopalat2[i].IsInside(matopalat1[matopalat1.Count - 1].Position)))
            {
                return true; /// PaivitaMatoa aliohjelma päättyy
            }
        }
        return false;
    }


    /// <summary>
    /// Käsitellään uuden palan lisäys madon jatkeeksi
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void LuoMatopala(List<GameObject> matopala, double x, double y)
    {
        GameObject pala = new GameObject(Ruudut, Ruudut);
        pala.X = x;
        pala.Y = y;
        matopala.Insert(0, pala); /// Lisää palan matopalat listan alkuun.
        Add(pala); /// Lisää pala pelikentälle.
    }
}