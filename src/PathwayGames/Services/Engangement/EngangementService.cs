using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;

namespace PathwayGames.Services.Engangement
{
    public class EngangementService : IEngangementService
    {
        public ConfusionMatrix CalculateConfusionMatrix(List<Slide> slides)
        {
            ConfusionMatrix confusionMatrix = new ConfusionMatrix(slides);
            
            return confusionMatrix;
        }
        public void Calculate(float h, float fa)
        {
            // print('ZH=', z_value(h)[0])
            // print('ZF=', z_value(fa)[0])
            // print("d'=", z_value(h)[0] - z_value(fa)[0])
            // print('beta=', z_value(h)[1] / z_value(fa)[1])
            // d’, C and A’ and B’’D values
            // B’’D = [(1 - H)(l - FA) - HFA]/[(1 - H)(l - FA) + HFA]
        }

        public (double z, double y) CalculateZValue(double prob)
        {
            double k;
            double r;
            double z;
            double y;

            if (prob > 0.5)
            {
                prob = 1 - prob;
                k = 1;
            }
            else
            {
                k = -1;
            }

            if (prob < 0.00001) {
                z = 4.3F;
            }
            else {
                r = Math.Sqrt(-Math.Log(prob));
                z = (((2.321213 * r + 4.850141) * r - 2.297965) * r - 2.787189) / ((1.637068 * r + 3.543889) * r + 1);
            }
            y = 1 / Math.Sqrt(2 * Math.PI) * Math.Exp(-z * z / 2);
            z = z * k;

            return (z, y);
        }
    }
}
