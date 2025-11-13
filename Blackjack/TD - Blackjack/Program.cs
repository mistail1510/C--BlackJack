using System.Runtime.CompilerServices;

namespace TD___BlackJack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Quel est votre nom ? ");
            string nom = Console.ReadLine();

            if (nom == string.Empty) { nom = "Humain"; }

            switch (Game(nom))
            {
                //BlackJack joueur !
                case 0:
                    Console.WriteLine($"\nBravo! {nom} a fait un BlackJack!");
                    Console.ReadLine();
                    break;

                //BlackJack Ordinateur !
                case 1:
                    Console.WriteLine("\nAïe! L'Ordinateur a fait un BlackJack!");
                    Console.ReadLine();
                    break;

                //Double BlackJack ?!
                case 2:
                    Console.WriteLine($"\nÉgalité! {nom} a fait un BlackJack et l'Ordinateur aussi!");
                    Console.ReadLine();
                    break;

                //Bust Joueur
                case 3:
                    Console.WriteLine($"\n{nom} a bust ! {nom} a perdu !");
                    Console.ReadLine();
                    break;

                //Bust Ordinateur
                case 4:
                    Console.WriteLine($"\nL'Ordinateur a bust ! {nom} a gagné !");
                    Console.ReadLine();
                    break;

                //Score Humain supérieur
                case 5:
                    Console.WriteLine($"\nVotre score est supérieur à celui de l'Ordinateur, {nom} a gagné.");
                    Console.ReadLine();
                    break;

                //Score Ordinateur supérieur
                case 6:
                    Console.WriteLine($"\nLe score de l'Ordinateur est supérieur à celui de {nom}, {nom} a perdu.");
                    Console.ReadLine();
                    break;

                //Égalité scores
                case 7:
                    Console.WriteLine("\nLes scores des deux joueurs sont égaux, égalité.");
                    Console.ReadLine();
                    break;
            }
        }


        public static int Game(string nom)
        {
            //Création cartes et valeurs cartes
            Dictionary<string, int> valeurCartes = new Dictionary<string, int>();
            valeurCartes.Add("A", 11);
            valeurCartes.Add("2", 2);
            valeurCartes.Add("3", 3);
            valeurCartes.Add("4", 4);
            valeurCartes.Add("5", 5);
            valeurCartes.Add("6", 6);
            valeurCartes.Add("7", 7);
            valeurCartes.Add("8", 8);
            valeurCartes.Add("9", 9);
            valeurCartes.Add("10", 10);
            valeurCartes.Add("J", 10);
            valeurCartes.Add("Q", 10);
            valeurCartes.Add("K", 10);

            //Création main joueur et ordi
            List<string> joueurH = new List<string>();
            List<string> joueurO = new List<string>();

            //Création paquet
            List<string> paquet = new List<string>();
            int nbPaquet;

            Console.Write("\nAvec combien de paquets de cartes souhaitez jouer (minimum 2) ? ");
            while (int.TryParse(Console.ReadLine(), out nbPaquet) == false && nbPaquet < 2)
            {
                Console.Write("\nAvec combien de paquets de cartes souhaitez jouer (minimum 2) ? ");
            }

            for (int i = 0; i < nbPaquet; i++)
                for (int j = 0; j < 4; j++)
                    foreach (KeyValuePair<string, int> carte in valeurCartes) { paquet.Add(carte.Key); }

            //Mélange du paquet
            paquet = paquet.OrderBy(x => Guid.NewGuid()).ToList();

            //Initialisation variable stop pioche
            bool stopPioche = false;

            //Première distribution des cartes
            for (int i = 0; i < 2; i++)
            {
                joueurH.Add(paquet[0]);
                paquet.RemoveAt(0);


                joueurO.Add(paquet[0]);
                paquet.RemoveAt(0);
            }

            //Initialisation variables compteur As
            int compteurAsH = CheckAs(joueurH);
            int compteurAsO = CheckAs(joueurO);

            //Initialisation scores
            int scoreH = Score(valeurCartes, joueurH, compteurAsH);
            int scoreO = Score(valeurCartes, joueurO, compteurAsO);

            //Check blackjacks
            if (scoreH == 21 && scoreO == 21)
            {
                stopPioche = true;
                Display(scoreH, scoreO, joueurH, joueurO, stopPioche, nom);
                return 2;
            }
            else if (scoreH == 21)
            {
                Display(scoreH, scoreO, joueurH, joueurO, stopPioche, nom);
                return 0;
            }
            else Display(scoreH, scoreO, joueurH, joueurO, stopPioche, nom);

            //Boucle pour demander au joueur s'il veut piocher nouvelle(s) carte(s)
            while (stopPioche == false)
                {
                    Console.WriteLine("Voulez vous piocher une nouvelle carte ?\no - OUI\nn - NON");
                    string choixJoueur = Console.ReadLine();

                    switch (choixJoueur)
                    {
                        case "o":
                        case "O":
                            Console.WriteLine($"\n{nom} : Je pioche.");

                            //Pioche
                            joueurH.Add(paquet[0]);
                            paquet.RemoveAt(0);

                            //MàJ du score + display
                            scoreH = Score(valeurCartes, joueurH, compteurAsH);
                            Display(scoreH, scoreO, joueurH, joueurO, stopPioche, nom);

                            //Check bust
                            if (scoreH > 21)
                                return 3;

                            break;

                        case "n":
                        case "N":
                            Console.WriteLine($"\n{nom} : Je m'arrête là.");

                            //Stop boucle
                            stopPioche = true;

                            //Display Cartes après avoir fini de piocher
                            Display(scoreH, scoreO, joueurH, joueurO, stopPioche, nom);

                            Console.ReadLine();

                            break;

                        default: continue;
                    }

                }

            //Revelation post pioche joueur blackjack ordinateur
            if (scoreO == 21)
            {
                Display(scoreH, scoreO, joueurH, joueurO, stopPioche, nom);
                return 1;
            }

            Console.WriteLine("Ordinateur : À mon tour.");

            //Boucle pour faire piocher l'ordinateur jusqu'à ce qu'il arrive à >=17
            while (scoreO < 17)
            {
                joueurO.Add(paquet[0]);
                paquet.RemoveAt(0);

                scoreO = Score(valeurCartes, joueurO, compteurAsO);

                Display(scoreH, scoreO, joueurH, joueurO, stopPioche, nom);

                Console.ReadLine();
            }

            if (scoreO > 21)
                return 4;
            else if (scoreH > scoreO)
                return 5;
            else if (scoreH < scoreO)
                return 6;
            else
                return 7;
        }


        //Fonction comptage de score
        public static int Score(Dictionary<string, int> valeurCartes, List<string> joueur, int compteurAs)
        {
            int score = 0;
            foreach (string s in joueur) { score += valeurCartes.GetValueOrDefault(s); }

            if (score > 21 && compteurAs > 0)
            {
                score -= 10;
                compteurAs--;
            }

            return score;
        }

        //Fonction pour check le nombre d'As dans une certaine main
        public static int CheckAs(List<string> joueur)
        {
            int check = 0;
            foreach (string s in joueur)
            {
                if (s == "A") check++;
            }

            return check;
        }

        //Fonction Display pour gagner temps et lisibilité
        public static void Display(int scoreH, int scoreO, List<string> joueurH, List<string> joueurO, bool stopPioche, string nom)
        {
            Console.Write("({0} points) {1} : ", scoreH, nom);
            foreach (string s in joueurH) { Console.Write(s + " "); }

            Console.WriteLine();

            //Montrer la première carte du croupier uniquement après que le joueur ait fini de piocher
            if (stopPioche == false) Console.WriteLine("(? points) Ordinateur : ? {0}\n", joueurO[1]);
            else
            {
                Console.Write("({0} points) Ordinateur : ", scoreO);
                foreach (string s in joueurO) { Console.Write(s + " "); }
                Console.WriteLine();
            }
        }
    }
}