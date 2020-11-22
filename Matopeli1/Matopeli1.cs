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
    private int omenanSyonti = 0;

    // Määritetään aloitukseen suunta
    Direction suunta;
    Direction tulevaSuunta;

    GameObject omena;

    // Lisätään pelille uusi objekti "omena", jota madolla kerätään kasvattaaksemme matoa 

    List<GameObject> matopalat = new List<GameObject>();


    // lisätään "mato" objekti

    private PhysicsObject mato1;

    // lisätään kentän reunat

    private PhysicsObject vasenReuna;
    private PhysicsObject oikeaReuna;
    private PhysicsObject alaReuna;
    private PhysicsObject ylaReuna;


    public override void Begin()
    {
        AsetaOhjaimet();
        LuoKentta();
        PelinAlku();
    }


    private void PelinAlku()
    {
        suunta = Direction.Up; tulevaSuunta = Direction.Up;
        foreach(GameObject madonPala in matopalat) {
            Remove(madonPala);
        }
        matopalat.Clear();

        // Luodaan mato-kavereita xd
        LuoMatopala(6 * Ruudut, 3 * Ruudut);
        LuoMatopala(5 * Ruudut, 3 * Ruudut);
        LuoMatopala(4 * Ruudut, 3 * Ruudut);
        LuoMatopala(3 * Ruudut, 3 * Ruudut);
    }


    private void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Right, ButtonState.Pressed, MuutaSuunta, null, Direction.Right);
        Keyboard.Listen(Key.Left, ButtonState.Pressed, MuutaSuunta, null, Direction.Left);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, MuutaSuunta, null, Direction.Up);
        Keyboard.Listen(Key.Down, ButtonState.Pressed, MuutaSuunta, null, Direction.Down);
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    private void LuoKentta()
    {
        Level.Width = Ruudut * Leveys;
        Level.Height = Ruudut * Korkeus;
        Level.CreateBorders();


        // Luodaan punainen omena.
        omena = new GameObject(Ruudut, Ruudut);
        omena.Color = Color.Red;
        Add(omena);


        // Luodaan ajastin, joka kutsuu PaivitaMatoa aliohjelmaa 0.1 sekunnin välein.
        Timer paivitysAjastin = new Timer();
        paivitysAjastin.Interval = 0.1;
        paivitysAjastin.Timeout += PaivitaMatoa;
        paivitysAjastin.Start();

        AsetaOhjaimet();
    }


    private void LuoReuna(Vector sijainti, double leveys, double korkeus)
    {
        PhysicsObject reuna = PhysicsObject.CreateStaticObject(leveys, korkeus);
        reuna.Color = Color.AshGray;
        reuna.Position = sijainti;

        Add(reuna);
    }


    private void LuoMato(Vector sijainti, double leveys, double korkeus)
    {
        PhysicsObject mato = new PhysicsObject(leveys, korkeus);
        mato.Position = sijainti;
        mato.Color = Color.White;
        Add(mato);

    }


     void MuutaSuunta(Direction uusiSuunta)
    {
        if (suunta == Direction.Right && uusiSuunta != Direction.Left)
            tulevaSuunta = uusiSuunta;

        if (suunta == Direction.Up && uusiSuunta != Direction.Down)
            tulevaSuunta = uusiSuunta;

        if (suunta == Direction.Left && uusiSuunta != Direction.Right)
            tulevaSuunta = uusiSuunta;

        if (suunta == Direction.Down && uusiSuunta != Direction.Up)
            tulevaSuunta = uusiSuunta;
    }


    private void MadonNopeus()
    {

    }


    private void Omenat()
    {
        omena = new GameObject(Ruudut, Ruudut);
        omena.Color = Color.Red;
        Add(omena);
    }


    private void LuoLaskuri()
    {

    }


    private void KasitteleTormays()
    {

    }


    void PaivitaMatoa()
    {
        suunta = tulevaSuunta;
        GameObject paa = matopalat[0]; // Uudeksi pääksi tulee nykyinen häntä.
        GameObject vanhaPaa = matopalat[matopalat.Count - 1]; // Edeltävä pää on listan lopussa.

        // Liikutetaan uusi pää kohtaan, joka on vanhasta päästä RuudunKoon verran siihen
        // suuntaan, johon ollaan menossa.
        paa.Position = vanhaPaa.Position + suunta.GetVector() * Ruudut;

        matopalat.RemoveAt(0); // Poistetaan listan ensimmäinen alkio, eli viimeinen pala häntää.
        matopalat.Add(paa); // Liikutetaan se listan loppuun.

        // Peli päättyy jos pää törmää muihin paloihin.
        for (int i = 0; i < matopalat.Count - 1; i++)
        {
            if (matopalat[i].IsInside(paa.Position))
            {
                PelinAlku(); // Aloittaa pelin alusta.
                return; // PaivitaMatoa aliohjelma päättyy tähän.
            }
        }

        // Jos pää menee kentän ulkopuolelle, niin peli alkaa alusta.
        if (!Level.BoundingRect.IsInside(paa.Position))
        {
            PelinAlku();
            return;
        }

        // Tarkistetaan törmäsikö pää omenaan.
        if (omena.IsInside(paa.Position))
        {
            // Siirretään omena satunnaiseen paikkaan.
            double satunnainenX = RandomGen.NextInt(-Leveys / 2, Leveys / 2);
            double satunnainenY = RandomGen.NextInt(-Korkeus / 2, Korkeus / 2);
            omena.Position = new Vector(satunnainenX, satunnainenY) * Ruudut;

            // Luodaan uusi madon pala.
            LuoMatopala(matopalat[0].Position.X, matopalat[1].Position.Y);
        }
    }


        void LuoMatopala(double x, double y)
        {
            GameObject pala = new GameObject(Ruudut, Ruudut);
            pala.X = x;
            pala.Y = y;
            matopalat.Insert(0, pala); // Lisää palan matopalat listan alkuun.
            Add(pala); // Lisää palan myös pelikentälle.
        }
    


}