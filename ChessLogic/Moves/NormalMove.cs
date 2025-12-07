using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class NormalMove : Move
    {
        public override MoveType Type => MoveType.Normal;
        public override Position FromPos { get; }
        public override Position ToPos { get; }
        public NormalMove(Position from, Position to)
        {
            FromPos = from;
            ToPos = to;
        }
        

        //50 rule move
        public override bool Execute(Board board)
        {
            Pieces piece = board[FromPos];
            bool capture = !board.IsEmty(ToPos);

            board[ToPos] = piece;
            board[FromPos] = null;
            piece.HasMoved = true;

            return capture || piece.Type == PieceType.Pawn;
        }
    }    
}   
