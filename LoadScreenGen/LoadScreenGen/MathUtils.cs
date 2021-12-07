using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadScreenGen {
    class MathUtils {
        public static double ApproximateProbability(List<double> probList) {
            double approx = 1.0;
            foreach(double d in probList) {
                approx *= d;
            }
            return approx;
        }

        public static double ProbabilityLoss(double probability, List<double> probList) {
            return Math.Abs(ApproximateProbability(probList) / probability - 1.0);
        }

        public static List<double> CreateRandomProbability(double probability, int num_approx) {
            double dividedProb = Math.Floor(100.0 * Math.Pow(probability, 1.0 / num_approx)) / 100.0;

            double bestLoss = -1.0;
            List<double>? bestAttempt = null;
            List<double>? prevAttempt = null;

            List<double> currentAttempt = new List<double>();
            for(int i = 0; i < num_approx; i += 1) {
                currentAttempt.Add(dividedProb);
            }
            double currentLoss = ProbabilityLoss(probability, currentAttempt);
            if((currentLoss < bestLoss) || (bestLoss < -0.5)) {
                bestLoss = currentLoss;
                bestAttempt = currentAttempt;
            }

            for(int i = 0; i < num_approx; i += 1) {
                prevAttempt = currentAttempt;
                currentAttempt = new List<double>(prevAttempt);
                currentAttempt[i] = currentAttempt[i] + 0.01;

                currentLoss = ProbabilityLoss(probability, currentAttempt);
                if(currentLoss < bestLoss) {
                    bestLoss = currentLoss;
                    bestAttempt = currentAttempt;
                }
            }

            return bestAttempt!;
        }

        public static int GCD(int a, int b) {
            if(b == 0) {
                return a;
            } else {
                return GCD(b, a % b);
            }
        }
    }
}
