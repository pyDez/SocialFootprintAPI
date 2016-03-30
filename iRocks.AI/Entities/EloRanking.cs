using System;

namespace iRocks.AI.Entities
{
    public class EloRanking : IRanking
    {

        //Step 1 : transformation du score en un entier compris entre 0 et +l'infini avec la formule suivante :
        //ArgTanH(X/lambda)/mu	
        //Step 2 : ajout ou retrait de 5 a cet entier
        //Step 3 : transformation de cet entier en score compris entre 0 et 100 avec la formule suivante :
        //TanH(X*mu)*lambda	

        //min : 0 ; max : lambda




        private double mu = 0.009;
        private double lambda = 100.0;

        public Tuple<double, double> getNewRankings(double scoreA, double scoreB, bool AWin)
        {
            double A = ArgTanH(scoreA / lambda) / mu;
            A = A < 0 ? 0 : A;

            double B = ArgTanH(scoreB / lambda) / mu;
            B = B < 0 ? 0 : B;

            if (AWin)
            {
                A += 5.0;
                B -= 5.0;
            }
            else
            {
                A -= 5.0;
                B += 5.0;
            }
            return new Tuple<double, double>(Math.Tanh(A * mu) * lambda, Math.Tanh(B * mu) * lambda);
        }

        private static double ArgTanH(double x)
        {
            if (Math.Abs(x) > 1)
                throw new ArgumentException("x");
            return Math.Log((1 + x) / (1 - x)) / 2;
        }
    }
}
