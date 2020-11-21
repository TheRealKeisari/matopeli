using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class MatopeliHTjuumatjo : PhysicsGame
{
    // Suunnitelmana olisi kirjoittaa matopeli, jossa teemme kaksi matoa joita ohjataan yhdellä näppäimistöllä käyttäen nuolinäppäimiä sekä WASD:ia
    // Aloitetaan tekemällä pelikartalle rajat ja jakamalla se neliöihin
    // "const int" käsitteiden tarkoituksena määrittää pelikartta, kartan koko sekä madon pituus pelin alussa

    const int Ruudut = 64;
    const int Leveys = 40;
    const int Korkeus = 40;
    const int MadonPituus = 3;

    // Lisätään pelille uusi objekti "omena", jota madolla kerätään kasvattaaksemme matoa 

    List<GameObject>  omena = new List<GameObject>();
    GameObject PeliOmena;

    // lisätään "mato" objekti

    PhysicsObject Mato;


    public override void Begin()
    {
        LuoKentta();
        AsetaOhjaimet();

        PelinAlku();
    }

    void PelinAlku()
    {

    }


    void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    void LuoKentta()
    {

    }


    void LuoMato()
    {

    }


    void MadonNopeus()
    {

    }


    void Omenat()
    {

    }


    void LuoLaskuri()
    {

    }


    void KasitteleTormays()
    {

    }



}

