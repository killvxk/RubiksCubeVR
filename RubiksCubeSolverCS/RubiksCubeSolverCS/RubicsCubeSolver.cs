﻿using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace RubiksCubeSolverCS
{
    public enum Color { White = 0, Yellow = 1, Orange = 4, Green = 3, Blue = 5, Red = 2 };

    public enum Move { L, R, U, D, F, B, LR, RR, UR, DR, FR, BR };

    static class MoveMethods
    {
        public static Move CharToMove(char c)
        {
            switch (c)
            {
                case 'R':
                    return Move.R;
                case 'L':
                    return Move.L;
                case 'U':
                    return Move.U;
                case 'D':
                    return Move.D;
                case 'F':
                    return Move.F;
                case 'B':
                    return Move.B;
                default:
                    Debug.Assert(false);
                    break;
            }

            return Move.LR;
        }

        public static List<Move> StringToMovesList(string movesStr)
        {
            List<Move> moves = new List<Move>();

            foreach (char c in movesStr)
            {
                moves.Add(CharToMove(c));
            }

            return moves;
        }

        public static Move Reverse(this Move m)
        {
            switch (m)
            {
                case Move.R:
                    return Move.RR;
                case Move.L:
                    return Move.LR;
                case Move.U:
                    return Move.UR;
                case Move.D:
                    return Move.DR;
                case Move.F:
                    return Move.FR;
                case Move.B:
                    return Move.BR;
                case Move.RR:
                    return Move.R;
                case Move.LR:
                    return Move.L;
                case Move.UR:
                    return Move.U;
                case Move.DR:
                    return Move.D;
                case Move.FR:
                    return Move.F;
                case Move.BR:
                    return Move.B;
                default:
                    Debug.Assert(false);
                    return Move.B;
            }
        }
    }

    class RubicsCubeSolver
    {
        public int[,] cube = new int[9, 6];

        public RubicsCubeSolver()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    cube[i, j] = j;
                }
            }
        }

        public bool IsSolved()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (cube[i, j] != j)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void DoMove(Move m, uint times)
        {
            times = times % 4;
            for (uint i = 0; i < times; i++)
            {
                DoMove(m);
            }
        }

        public void DoMoves(string moves)
        {
            foreach (char c in moves)
            {
                DoMove(c);
            }
        }

        public bool CrossIsSolved()
        {
            if (cube[1, 0] == 0 && cube[3, 0] == 0 && cube[5, 0] == 0 && cube[7, 0] == 0 && cube[7, 2] == 2 && cube[7, 3] == 3 && cube[7, 4] == 4 && cube[7, 5] == 5)
            {
                return true;
            }

            return false;
        }

        public void ShuffleCube(uint movesCount)
        {
            string movesVariants = "LRUDFB";
            string moves = "";

            Random rnd = new Random();

            for (uint i = 0; i < movesCount; i++)
            {
                moves = moves + movesVariants[rnd.Next(0, movesVariants.Length)].ToString();
            }

            DoMoves(moves);
        }

        /*public List<Move> SolveCross()
        {
            List<Move> moves = new List<Move>();
            while (!CrossIsSolved())
            {
                moves.AddRange(SolveEdgeMoves());
            }

            return moves;
        }*/

        List<Move> OptimizeMoves(List<Move> moves)
        {
            List<Move> tmp = new List<Move>();
            List<Move> optimized = new List<Move>();

            foreach (Move m in moves)
            {
                if (tmp.Count == 0)
                {
                    tmp.Add(m);
                }
                else
                {
                    if (tmp[0] == m)
                    {
                        if (tmp.Count == 2)
                        {
                            optimized.Add(m.Reverse());
                            tmp.Clear();
                        }
                        else
                        {
                            tmp.Add(m);
                        }
                    }
                    else
                    {
                        optimized.AddRange(tmp);
                        tmp.Clear();
                        tmp.Add(m);
                    }
                }
            }

            optimized.AddRange(tmp);

            return optimized;
        }

        void AppendAndDoMoves(ref List<Move> moves, string str)
        {
            moves.AddRange(MoveMethods.StringToMovesList(str));
            DoMoves(str);
        }

        public List<Move> SolveEdgeMoves()
        {
            List<Move> moves = new List<Move>();
            int[,] copyCube = CopyCube();

            //Check edges on all sides --- don't forget, white can be in wrong position on the bottom
            for (int side = 0; side < 6; ++side)
            {
                for (int edge = 1; edge < 9; edge += 2)
                {
                    if (cube[edge, side] == 0)
                    {
                        //Solve edge

                        switch (side)
                        {
                            case 1:
                                {//White edge on top
                                    if (edge == 1)
                                    { //orange side
                                        switch (cube[1, 4])
                                        {
                                            case 2://red
                                                AppendAndDoMoves(ref moves, "UUFF");
                                                break;
                                            case 3://green
                                                AppendAndDoMoves(ref moves, "URR");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "BB");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "UUULL");
                                                break;
                                        }
                                    }
                                    else if (edge == 3)
                                    {//Blue side
                                        switch (cube[1, 5])
                                        {
                                            case 2:
                                                AppendAndDoMoves(ref moves, "UUUFF");
                                                break;
                                            case 3://green
                                                AppendAndDoMoves(ref moves, "UURR");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "UBB");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "LL");
                                                break;
                                        }
                                    }
                                    else if (edge == 5)
                                    {//green side
                                        switch (cube[1, 3])
                                        {
                                            case 2://red
                                                AppendAndDoMoves(ref moves, "UFF");
                                                break;
                                            case 3:
                                                AppendAndDoMoves(ref moves, "RR");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "UUUBB");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "UULL");
                                                break;
                                        }
                                    }
                                    else if (edge == 7)
                                    {//red side
                                        switch (cube[1, 2])
                                        {
                                            case 2:
                                                AppendAndDoMoves(ref moves, "FF");
                                                break;
                                            case 3:
                                                AppendAndDoMoves(ref moves, "UUURR");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "UUBB");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "ULL");
                                                break;
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {//White edge on red side

                                    if (edge == 1)
                                    {//Yellow side
                                        switch (cube[7, 1])
                                        {
                                            case 2:
                                                AppendAndDoMoves(ref moves, "UUURRRFR");
                                                break;
                                            case 3://green on top, white on red side
                                                AppendAndDoMoves(ref moves, "FRRRFFF");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "UUURBBBRRR");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "FFFLF");
                                                break;
                                        }
                                    }
                                    else if (edge == 3)
                                    { //Colored edge on blue side
                                        switch (cube[5, 5])
                                        {
                                            case 2:
                                                AppendAndDoMoves(ref moves, "LDLLL");
                                                break;
                                            case 3:
                                                AppendAndDoMoves(ref moves, "UUFUU");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "LLLULBB");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "L");
                                                break;

                                        }
                                    }
                                    else if (edge == 5)
                                    { //check color on green side
                                        switch (cube[3, 3])
                                        {
                                            case 2:
                                                AppendAndDoMoves(ref moves, "RRRDDDR");
                                                break;
                                            case 3:
                                                AppendAndDoMoves(ref moves, "RRR");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "RUUURRRBB");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "RUURRRLL");
                                                break;
                                        }
                                    }
                                    else if (edge == 7)
                                    { //check white side
                                        switch (cube[1, 0])
                                        {
                                            case 2:
                                                AppendAndDoMoves(ref moves, "FFFDRRRDDD");
                                                break;
                                            case 3:
                                                AppendAndDoMoves(ref moves, "FFFRRR");
                                                break;
                                            case 4:
                                                AppendAndDoMoves(ref moves, "FFUUURBBBRRR");
                                                break;
                                            case 5:
                                                AppendAndDoMoves(ref moves, "FL");
                                                break;
                                        }
                                    }

                                    break;
                                }
                            case 3: //White edge on green side
                                if (edge == 1)
                                {//Yellow side
                                    switch (cube[5, 1])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "RRRFR");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "UFRRRFFF");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "RBBBRRR");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "UFFFLF");
                                            break;
                                    }
                                }
                                else if (edge == 3)
                                { //Colored edge on red side
                                    switch (cube[5, 2])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "F");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "FFFUUUFRR");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "RRBBBRR");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "FFFUFLL");
                                            break;

                                    }
                                }
                                else if (edge == 5)
                                { //check color on orange side
                                    switch (cube[3, 4])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "RRFRR");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "BUBBBRR");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "BBB");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "RRRUFFFLF");
                                            break;
                                    }
                                }
                                else if (edge == 7)
                                { //check white side
                                    switch (cube[5, 0])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "RFRRR");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "RRUFRRRFFF");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "RRRBBBR");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "RRUFFFLF");
                                            break;
                                    }
                                }
                                break;
                            case 4: //White edge on orange side
                                if (edge == 1)
                                {//Yellow side
                                    switch (cube[1, 1])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "URRRFR");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "BBBRB");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "URBBBRRR");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "BLLLBBB");
                                            break;
                                    }
                                }
                                else if (edge == 3)
                                { //Colored edge on green side
                                    switch (cube[5, 3])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "RRRURFF");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "R");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "RRRUUURBB");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "RRRUURLL");
                                            break;

                                    }
                                }
                                else if (edge == 5)
                                { //check color on blue side
                                    switch (cube[3, 5])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "LUUULLLFF");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "LUULLLRR");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "LULLLBB");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "LLL");
                                            break;
                                    }
                                }
                                else if (edge == 7)
                                { //check white side
                                    switch (cube[7, 0])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "BBURRRFR");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "BRBBB");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "BBURBBBRRR");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "BBBLLLB");
                                            break;
                                    }
                                }
                                break;
                            case 5: //White edge on blue side

                                if (edge == 1)
                                {//Yellow side
                                    switch (cube[3, 1])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "LFLLL");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "UUUFRRRFFF");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "LLLBL");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "UUUFFFLF");
                                            break;
                                    }
                                }
                                else if (edge == 3)
                                { //Colored edge on orange side
                                    switch (cube[5, 4])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "LLFFFLL");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "LUUUFRRRFFF");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "B");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "LUUUFFFLF");
                                            break;

                                    }
                                }
                                else if (edge == 5)
                                { //check color on red side
                                    switch (cube[3, 2])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "FFF");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "FUUUFFFRR");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "FUUFFFBB");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "FUFFFLL");
                                            break;
                                    }
                                }
                                else if (edge == 7)
                                { //check white side
                                    switch (cube[3, 0])
                                    {
                                        case 2:
                                            AppendAndDoMoves(ref moves, "LLLFFFL");
                                            break;
                                        case 3:
                                            AppendAndDoMoves(ref moves, "LLUFRRRFFF");
                                            break;
                                        case 4:
                                            AppendAndDoMoves(ref moves, "LBLLL");
                                            break;
                                        case 5:
                                            AppendAndDoMoves(ref moves, "LLUFFFLF");
                                            break;
                                    }
                                }

                                break;

                            case 0:
                                if (edge == 1)
                                {
                                    if (cube[7, 2] != 2)
                                    {
                                        while (cube[1, 2] == 0 || cube[7, 1] == 0)
                                        {
                                            AppendAndDoMoves(ref moves, "U");
                                        }
                                        AppendAndDoMoves(ref moves, "FF");
                                    }
                                }
                                else if (edge == 5)
                                {
                                    if (cube[7, 3] != 3)
                                    {
                                        while (cube[1, 3] == 0 || cube[5, 1] == 0)
                                        {
                                            AppendAndDoMoves(ref moves, "U");
                                        }
                                        AppendAndDoMoves(ref moves, "RR");
                                    }
                                }
                                else if (edge == 7)
                                {
                                    if (cube[7, 4] != 4)
                                    {
                                        while (cube[1, 3] == 0 || cube[1, 1] == 0)
                                        {
                                            AppendAndDoMoves(ref moves, "U");
                                        }
                                        AppendAndDoMoves(ref moves, "BB");
                                    }
                                }
                                else if (edge == 3)
                                {
                                    if (cube[7, 5] != 5)
                                    {
                                        while (cube[1, 5] == 0 || cube[3, 1] == 0)
                                        {
                                            AppendAndDoMoves(ref moves, "U");
                                        }
                                        AppendAndDoMoves(ref moves, "LL");
                                    }
                                }

                                break;
                        }
                    }
                }
            }

            cube = copyCube;

            return OptimizeMoves(moves);
        }

        public void DoMove(Move m)
        {
            switch (m)
            {
                case Move.R:
                    R();
                    break;
                case Move.L:
                    L();
                    break;
                case Move.U:
                    U();
                    break;
                case Move.D:
                    D();
                    break;
                case Move.F:
                    F();
                    break;
                case Move.B:
                    B();
                    break;
                case Move.LR:
                    DoMove(Move.L, 3);
                    break;
                case Move.RR:
                    DoMove(Move.R, 3);
                    break;
                case Move.UR:
                    DoMove(Move.U, 3);
                    break;
                case Move.DR:
                    DoMove(Move.D, 3);
                    break;
                case Move.FR:
                    DoMove(Move.F, 3);
                    break;
                case Move.BR:
                    DoMove(Move.B, 3);
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }
        }

        void DoMove(char c)
        {
            DoMove(MoveMethods.CharToMove(c));
        }

        int[,] CopyCube()
        {
            int[,] cubeCopy = new int[9, 6];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    cubeCopy[i, j] = cube[i, j];
                }
            }

            return cubeCopy;
        }

        void R()
        {
            int[,] copyCube = CopyCube();

            //Orange->White
            copyCube[2, 0] = cube[6, 4];
            copyCube[5, 0] = cube[3, 4];
            copyCube[8, 0] = cube[0, 4];

            //White->Red
            copyCube[2, 2] = cube[2, 0];
            copyCube[5, 2] = cube[5, 0];
            copyCube[8, 2] = cube[8, 0];

            //Red->Yellow
            copyCube[2, 1] = cube[2, 2];
            copyCube[5, 1] = cube[5, 2];
            copyCube[8, 1] = cube[8, 2];

            //Yellow->Orange
            copyCube[6, 4] = cube[2, 1];
            copyCube[3, 4] = cube[5, 1];
            copyCube[0, 4] = cube[8, 1];

            //Rotate Green
            copyCube[0, 3] = cube[6, 3];
            copyCube[1, 3] = cube[3, 3];
            copyCube[2, 3] = cube[0, 3];
            copyCube[3, 3] = cube[7, 3];
            copyCube[5, 3] = cube[1, 3];
            copyCube[6, 3] = cube[8, 3];
            copyCube[7, 3] = cube[5, 3];
            copyCube[8, 3] = cube[2, 3];

            cube = copyCube;
        }

        void L()
        {
            int[,] copyCube = CopyCube();

            //White->Orange
            copyCube[2, 4] = cube[6, 0];
            copyCube[5, 4] = cube[3, 0];
            copyCube[8, 4] = cube[0, 0];

            //Orange->Yellow
            copyCube[6, 1] = cube[2, 4];
            copyCube[3, 1] = cube[5, 4];
            copyCube[0, 1] = cube[8, 4];

            //Yellow->Red
            copyCube[0, 2] = cube[0, 1];
            copyCube[3, 2] = cube[3, 1];
            copyCube[6, 2] = cube[6, 1];

            //Red->White
            copyCube[0, 0] = cube[0, 2];
            copyCube[3, 0] = cube[3, 2];
            copyCube[6, 0] = cube[6, 2];

            //Rotate Blue
            copyCube[0, 5] = cube[6, 5];
            copyCube[1, 5] = cube[3, 5];
            copyCube[2, 5] = cube[0, 5];
            copyCube[3, 5] = cube[7, 5];
            copyCube[5, 5] = cube[1, 5];
            copyCube[6, 5] = cube[8, 5];
            copyCube[7, 5] = cube[5, 5];
            copyCube[8, 5] = cube[2, 5];

            cube = copyCube;
        }

        void U()
        {
            int[,] copyCube = CopyCube();

            //Orange->Green
            copyCube[0, 3] = cube[0, 4];
            copyCube[1, 3] = cube[1, 4];
            copyCube[2, 3] = cube[2, 4];

            //Green->Red
            copyCube[0, 2] = cube[0, 3];
            copyCube[1, 2] = cube[1, 3];
            copyCube[2, 2] = cube[2, 3];

            //Red->Blue
            copyCube[0, 5] = cube[0, 2];
            copyCube[1, 5] = cube[1, 2];
            copyCube[2, 5] = cube[2, 2];

            //Blue->Orange
            copyCube[0, 4] = cube[0, 5];
            copyCube[1, 4] = cube[1, 5];
            copyCube[2, 4] = cube[2, 5];

            //Rotate Yellow
            copyCube[0, 1] = cube[6, 1];
            copyCube[1, 1] = cube[3, 1];
            copyCube[2, 1] = cube[0, 1];
            copyCube[3, 1] = cube[7, 1];
            copyCube[5, 1] = cube[1, 1];
            copyCube[6, 1] = cube[8, 1];
            copyCube[7, 1] = cube[5, 1];
            copyCube[8, 1] = cube[2, 1];

            cube = copyCube;
        }


        void D()
        {
            int[,] copyCube = CopyCube();

            //Orange->Blue
            copyCube[6, 5] = cube[6, 4];
            copyCube[7, 5] = cube[7, 4];
            copyCube[8, 5] = cube[8, 4];

            //Blue->Red
            copyCube[6, 2] = cube[6, 5];
            copyCube[7, 2] = cube[7, 5];
            copyCube[8, 2] = cube[8, 5];

            //Red->Green
            copyCube[6, 3] = cube[6, 2];
            copyCube[7, 3] = cube[7, 2];
            copyCube[8, 3] = cube[8, 2];

            //Green->Orange
            copyCube[6, 4] = cube[6, 3];
            copyCube[7, 4] = cube[7, 3];
            copyCube[8, 4] = cube[8, 3];

            //Rotate White
            copyCube[0, 0] = cube[6, 0];
            copyCube[1, 0] = cube[3, 0];
            copyCube[2, 0] = cube[0, 0];
            copyCube[3, 0] = cube[7, 0];
            copyCube[5, 0] = cube[1, 0];
            copyCube[6, 0] = cube[8, 0];
            copyCube[7, 0] = cube[5, 0];
            copyCube[8, 0] = cube[2, 0];

            cube = copyCube;
        }

        void F()
        {
            int[,] copyCube = CopyCube();

            //Blue->Yellow
            copyCube[8, 1] = cube[2, 5];
            copyCube[7, 1] = cube[5, 5];
            copyCube[6, 1] = cube[8, 5];

            //Yellow->Green
            copyCube[0, 3] = cube[6, 1];
            copyCube[3, 3] = cube[7, 1];
            copyCube[6, 3] = cube[8, 1];

            //Green->White
            copyCube[2, 0] = cube[0, 3];
            copyCube[1, 0] = cube[3, 3];
            copyCube[0, 0] = cube[6, 3];

            //White->Blue
            copyCube[2, 5] = cube[0, 0];
            copyCube[5, 5] = cube[1, 0];
            copyCube[8, 5] = cube[2, 0];

            //Rotate Red
            copyCube[0, 2] = cube[6, 2];
            copyCube[1, 2] = cube[3, 2];
            copyCube[2, 2] = cube[0, 2];
            copyCube[3, 2] = cube[7, 2];
            copyCube[5, 2] = cube[1, 2];
            copyCube[6, 2] = cube[8, 2];
            copyCube[7, 2] = cube[5, 2];
            copyCube[8, 2] = cube[2, 2];

            cube = copyCube;
        }

        void B()
        {
            int[,] copyCube = CopyCube();

            //Yellow->Blue
            copyCube[6, 5] = cube[0, 1];
            copyCube[3, 5] = cube[1, 1];
            copyCube[0, 5] = cube[2, 1];

            //Blue->White
            copyCube[6, 0] = cube[0, 5];
            copyCube[7, 0] = cube[3, 5];
            copyCube[8, 0] = cube[6, 5];

            //White->Green
            copyCube[8, 3] = cube[6, 0];
            copyCube[5, 3] = cube[7, 0];
            copyCube[2, 3] = cube[8, 0];

            //Green->Yellow
            copyCube[0, 1] = cube[2, 3];
            copyCube[1, 1] = cube[5, 3];
            copyCube[2, 1] = cube[8, 3];

            //Rotate Orange
            copyCube[0, 4] = cube[6, 4];
            copyCube[1, 4] = cube[3, 4];
            copyCube[2, 4] = cube[0, 4];
            copyCube[3, 4] = cube[7, 4];
            copyCube[5, 4] = cube[1, 4];
            copyCube[6, 4] = cube[8, 4];
            copyCube[7, 4] = cube[5, 4];
            copyCube[8, 4] = cube[2, 4];

            cube = copyCube;
        }



        public List<Move> WhiteCornersToTopMoves()
        {
            List<Move> moves = new List<Move>();
            int[,] copyCube = CopyCube();

            //get a white corner into the top layer

            if (cube[0, 0] == 0 || cube[6, 2] == 0 || cube[8, 5] == 0)
            {
                while (cube[6, 1] == 0 || cube[0, 2] == 0 || cube[2, 5] == 0)
                {
                    AppendAndDoMoves(ref moves, "U");
                }
                AppendAndDoMoves(ref moves, "FUFFF");

            }
            if (cube[2, 0] == 0 || cube[8, 2] == 0 || cube[6, 3] == 0)
            {
                while (cube[8, 1] == 0 || cube[2, 2] == 0 || cube[0, 3] == 0)
                {
                    AppendAndDoMoves(ref moves, "U");
                }
                AppendAndDoMoves(ref moves, "RURRR");

            }
            if (cube[6, 0] == 0 || cube[8, 4] == 0 || cube[6, 5] == 0)
            {
                while (cube[0, 1] == 0 || cube[2, 4] == 0 || cube[0, 5] == 0)
                {
                    AppendAndDoMoves(ref moves, "U");
                }
                AppendAndDoMoves(ref moves, "LULLL");

            }
            if (cube[8, 0] == 0 || cube[8, 3] == 0 || cube[6, 4] == 0)
            {
                while (cube[2, 1] == 0 || cube[2, 3] == 0 || cube[0, 4] == 0)
                {
                    AppendAndDoMoves(ref moves, "U");
                }
                AppendAndDoMoves(ref moves, "BUBBB");

            }

            cube = copyCube;

            return OptimizeMoves(moves);
        }

        public List<Move> SolveCornersMoves()
        {
            List<Move> moves = new List<Move>();
            int[,] copyCube = CopyCube();

            //then perform the alg for the case: RUURRRUUURURRR, RURRR, FFFUUUF

            if (cube[8, 1] == 0)
            {
                if (cube[2, 2] == 2)
                { //red and blue
                    AppendAndDoMoves(ref moves, "UFUUFFFUUUFUFFF");
                }
                else if (cube[2, 2] == 3)
                { //green and red
                    AppendAndDoMoves(ref moves, "RUURRRUUURURRR");
                }
                else if (cube[2, 2] == 4)
                { //orange and green
                    AppendAndDoMoves(ref moves, "UUUBUUBBBUUUBUBBB");
                }
                else if (cube[2, 2] == 5)
                { //orange and blue
                    AppendAndDoMoves(ref moves, "UULUUULLLUULULLL");
                }


            }

            else if (cube[0, 3] == 0)
            {
                if (cube[2, 2] == 2)
                { //red green
                    AppendAndDoMoves(ref moves, "RURRR");
                }
                else if (cube[2, 2] == 3)
                { //green orange
                    AppendAndDoMoves(ref moves, "UUUBUBBB");
                }
                else if (cube[2, 2] == 4)
                { //orange blue
                    AppendAndDoMoves(ref moves, "UULULLL");
                }
                else if (cube[2, 2] == 5)
                { //red blue
                    AppendAndDoMoves(ref moves, "UFUFFF");
                }
            }

            else if (cube[2, 2] == 0)
            {
                if (cube[0, 3] == 2)
                { //blue red
                    AppendAndDoMoves(ref moves, "ULLLUUUL");
                }
                else if (cube[0, 3] == 3)
                { //red green
                    AppendAndDoMoves(ref moves, "FFFUUUF");
                }
                else if (cube[0, 3] == 4)
                { //orange green
                    AppendAndDoMoves(ref moves, "URRRUUUR");
                }
                else if (cube[0, 3] == 5)
                { //orange blue
                    AppendAndDoMoves(ref moves, "UUBBBUUUB");
                }

            }
            else
            {
                AppendAndDoMoves(ref moves, "U");
            }

            cube = copyCube;

            return OptimizeMoves(moves);
        }

        public bool WhiteCornersOnTop()
        {
            if (cube[0, 0] != 0 && cube[2, 0] != 0 && cube[6, 0] != 0 && cube[8, 0] != 0
              && cube[6, 2] != 0 && cube[8, 2] != 0 && cube[6, 3] != 0 && cube[8, 3] != 0
              && cube[6, 4] != 0 && cube[8, 4] != 0 && cube[6, 5] != 0 && cube[8, 5] != 0)
            {

                return true;
            }

            if (cube[0, 0] == 0 && cube[2, 0] == 0 && cube[6, 0] == 0 && cube[8, 0] == 0)
            {
                return true;
            }

            return false;
        }

        public bool CornersAreSolved()
        {
            if (cube[0, 0] == 0 && cube[2, 0] == 0 && cube[6, 0] == 0 && cube[8, 0] == 0)
            {
                return true;
            }

            return false;
        }

        public List<Move> SolveMidLayerMoves()
        {
            List<Move> moves = new List<Move>();
            int[,] copyCube = CopyCube();

            if (cube[1, 1] != 1 && cube[1, 4] != 1)
            {
                if (cube[1, 1] == 2)
                {
                    if (cube[1, 4] == 3)
                    {
                        AppendAndDoMoves(ref moves, "URRRUUURRRUUURRRURUR");
                    }
                    else if (cube[1, 4] == 5)
                    {
                        AppendAndDoMoves(ref moves, "BUBUBUUUBBBUUUBBB");
                    }
                }
                else if (cube[1, 1] == 3)
                {
                    if (cube[1, 4] == 4)
                    {
                        AppendAndDoMoves(ref moves, "BBBUUUBBBUUUBBBUBUB");
                    }
                    else if (cube[1, 4] == 2)
                    {
                        AppendAndDoMoves(ref moves, "UUFUFUFUUUFFFUUUFFF");
                    }
                }
                else if (cube[1, 1] == 4)
                {
                    if (cube[1, 4] == 5)
                    {
                        AppendAndDoMoves(ref moves, "UUULLLUUULLLUUULLLULUL");
                    }
                    else if (cube[1, 4] == 3)
                    {
                        AppendAndDoMoves(ref moves, "BBBUUUBBBUUUBBBUBUB");
                    }
                }
                else if (cube[1, 1] == 5)
                {
                    if (cube[1, 4] == 2)
                    {
                        AppendAndDoMoves(ref moves, "UUFFFUUUFFFUUUFFFUFUF");
                    }
                    else if (cube[1, 4] == 4)
                    {
                        AppendAndDoMoves(ref moves, "LLLUUULLLUUULLLULUL");
                    }
                }
            }

            else if (cube[5, 1] != 1 && cube[1, 3] != 1)
            {
                if (cube[5, 1] == 2)
                {
                    if (cube[1, 3] == 3)
                    {
                        AppendAndDoMoves(ref moves, "RRRUUURRRUUURRRURUR");
                    }
                    else if (cube[1, 3] == 5)
                    {
                        AppendAndDoMoves(ref moves, "UULULULUUULLLUUULLL");
                    }
                }
                else if (cube[5, 1] == 3)
                {
                    if (cube[1, 3] == 4)
                    {
                        AppendAndDoMoves(ref moves, "UUUBBBUUUBBBUUUBBBUBUB");
                    }
                    else if (cube[1, 3] == 2)
                    {
                        AppendAndDoMoves(ref moves, "UFUFUFUUUFFFUUUFFF");
                    }
                }
                else if (cube[5, 1] == 4)
                {
                    if (cube[1, 3] == 5)
                    {
                        AppendAndDoMoves(ref moves, "UULLLUUULLLUUULLLULUL");
                    }
                    else if (cube[1, 3] == 3)
                    {
                        AppendAndDoMoves(ref moves, "RURURUUURRRUUURRR");
                    }
                }
                else if (cube[5, 1] == 5)
                {
                    if (cube[1, 3] == 2)
                    {
                        AppendAndDoMoves(ref moves, "UFFFUUUFFFUUUFFFUFUF");
                    }
                    else if (cube[1, 3] == 4)
                    {
                        AppendAndDoMoves(ref moves, "UUUBUBUBUUUBBBUUUBBB");
                    }
                }
            }
            else if (cube[7, 1] != 1 && cube[1, 2] != 1)
            {
                if (cube[7, 1] == 2)
                {
                    if (cube[1, 2] == 3)
                    {
                        AppendAndDoMoves(ref moves, "UUURRRUUURRRUUURRRURUR");
                    }
                    else if (cube[1, 2] == 5)
                    {
                        AppendAndDoMoves(ref moves, "ULULULUUULLLUUULLL");
                    }
                }
                else if (cube[7, 1] == 3)
                {
                    if (cube[1, 2] == 4)
                    {
                        AppendAndDoMoves(ref moves, "UUBBBUUUBBBUUUBBBUBUB");
                    }
                    else if (cube[1, 2] == 2)
                    {
                        AppendAndDoMoves(ref moves, "FUFUFUUUFFFUUUFFF");
                    }
                }
                else if (cube[7, 1] == 4)
                {
                    if (cube[1, 2] == 5)
                    {
                        AppendAndDoMoves(ref moves, "ULLLUUULLLUUULLLULUL");
                    }
                    else if (cube[1, 2] == 3)
                    {
                        AppendAndDoMoves(ref moves, "UUURURURUUURRRUUURRR");
                    }
                }
                else if (cube[7, 1] == 5)
                {
                    if (cube[1, 2] == 2)
                    {
                        AppendAndDoMoves(ref moves, "FFFUUUFFFUUUFFFUFUF");
                    }
                    else if (cube[1, 2] == 4)
                    {
                        AppendAndDoMoves(ref moves, "UUBUBUBUUUBBBUUUBBB");
                    }
                }
            }
            else if (cube[3, 1] != 1 && cube[1, 5] != 1)
            {
                if (cube[3, 1] == 2)
                {
                    if (cube[1, 5] == 3)
                    {
                        AppendAndDoMoves(ref moves, "UURRRUUURRRUUURRRURUR");
                    }
                    else if (cube[1, 5] == 5)
                    {
                        AppendAndDoMoves(ref moves, "LULULUUULLLUUULLL");
                    }
                }
                else if (cube[3, 1] == 3)
                {
                    if (cube[1, 5] == 4)
                    {
                        AppendAndDoMoves(ref moves, "UBBBUUUBBBUUUBBBUBUB");
                    }
                    else if (cube[1, 5] == 2)
                    {
                        AppendAndDoMoves(ref moves, "UUUFUFUFUUUFFFUUUFFF");
                    }
                }
                else if (cube[3, 1] == 4)
                {
                    if (cube[1, 5] == 5)
                    {
                        AppendAndDoMoves(ref moves, "LLLUUULLLUUULLLULUL");
                    }
                    else if (cube[1, 5] == 3)
                    {
                        AppendAndDoMoves(ref moves, "UURURURUUURRRUUURRR");
                    }
                }
                else if (cube[3, 1] == 5)
                {
                    if (cube[1, 5] == 2)
                    {
                        AppendAndDoMoves(ref moves, "UUUFFFUUUFFFUUUFFFUFUF");
                    }
                    else if (cube[1, 5] == 4)
                    {
                        AppendAndDoMoves(ref moves, "UBUBUBUUUBBBUUUBBB");
                    }
                }
            }
            else if (cube[3, 2] != 2 || cube[5, 5] != 5)
            {
                AppendAndDoMoves(ref moves, "LULULUUULLLUUULLL");
            }
            else if (cube[5, 2] != 2 || cube[3, 3] != 3)
            {
                AppendAndDoMoves(ref moves, "FUFUFUUUFFFUUUFFF");
            }
            else if (cube[5, 3] != 3 || cube[3, 4] != 4)
            {
                AppendAndDoMoves(ref moves, "RURURUUURRRUUURRR");
            }
            else if (cube[5, 4] != 4 || cube[3, 5] != 5)
            {
                AppendAndDoMoves(ref moves, "BUBUBUUUBBBUUUBBB");
            }

            cube = copyCube;

            return OptimizeMoves(moves);
        }

        public bool MidLayerIsSolved()
        {
            if (cube[3, 2] != 2 || cube[5, 2] != 2)
                return true;
            else if (cube[3, 3] != 3 || cube[5, 3] != 3)
                return true;
            else if (cube[3, 4] != 4 || cube[5, 4] != 4)
                return true;
            else if (cube[3, 5] != 5 || cube[5, 5] != 5)
                return true;
            else
                return false;
        }
    }
}
