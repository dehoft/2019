using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektikos_Projektas
{
    class Player
    {
        public string name;
        public double GP, MIN, PTS, FGM, FGA, FGP, THRPM, THRPA, THRPP, FTM, FTA, FTP, OREB, DREB, REB, AST, STL, BLCK, TOV;
        public bool TARGET_Yrs;

        public Player() { }

        public Player(string name, double gp, double min, double pts, double fgm, double fga, double fgp, double thrpm, double thrpa, double thrpp,
            double ftm, double fta, double ftp, double oreb, double dreb, double reb, double ast, double stl, double blck, double tov, bool target)
        {
            this.name = name;
            GP = gp;
            MIN = min;
            PTS = pts;
            FGM = fgm;
            FGA = fga;
            FGP = fgp;
            THRPM = thrpm;
            THRPA = thrpa;
            THRPP = thrpp;
            FTM = ftm;
            FTA = fta;
            FTP = ftp;
            OREB = oreb;
            DREB = dreb;
            REB = reb;
            AST = ast;
            STL = stl;
            BLCK = blck;
            TOV = tov;
            TARGET_Yrs = target;
        }
    }
}
