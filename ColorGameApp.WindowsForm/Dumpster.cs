using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ColorGameApp.WindowsForm
{
    public static class Dumpster
    {
        public static Random rand = new Random();

        public static int[] losujNumeryKolorow(int k, int n)
        {
            int[] tab = new int[k];
            bool bad;
            for (int i = 0; i < k; i++)
            {
                do
                {
                    bad = false;
                    tab[i] = rand.Next(n + 1);
                    for (int j = 0; j < i; j++)
                    {
                        if (tab[i] == tab[j])
                        {
                            bad = true;
                            break;
                        }
                    }
                } while (bad);
            }
            return tab;
        }
    }
}
