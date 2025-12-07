using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result Result { get; private set; } = null;

        //50 move rule
        private int noCaptureOrPawnMoves = 0;

        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if(Board.IsEmty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Pieces piece = Board [pos];
            IEnumerable<Move> moveCandidates = piece.GetMoves(pos, Board);
            return moveCandidates.Where(move => move.IsLegal(Board));
            

        }

        public IEnumerable<Move> MovesForPiece(Position pos)
        {
            if (Board.IsEmty(pos) || Board[pos].Color != CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Pieces piece = Board[pos];
            // Chỉ lấy các nước đi, KHÔNG kiểm tra IsLegal()
            return piece.GetMoves(pos, Board);
        }
        public void MakeMove(Move move)
        {   //Battotquaduong
            Board.SetPawnSkipPosition(CurrentPlayer, null);

            //50 move rule
            bool captureOrPawn =move.Execute(Board);    
            if(captureOrPawn)
            {
                noCaptureOrPawnMoves = 0;
            }
            else
            {
                noCaptureOrPawnMoves++;
            }
                CurrentPlayer = CurrentPlayer.Opponent();

            CheckForGameOver(); 
        }

        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
            {
                Pieces piece = Board[pos];
                return piece.GetMoves(pos, Board);
            });
            return moveCandidates.Where(move => move.IsLegal(Board));
        }

        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())
            {
                // Nếu không có nước đi hợp lệ, hãy kiểm tra xem Vua có đang bị chiếu không
                if (Board.IsIncheck(CurrentPlayer))
                {
                    // Bị chiếu và không thể di chuyển -> Checkmate
                    Result = Result.Win(CurrentPlayer.Opponent());
                }
                else
                {
                    // Không bị chiếu và không thể di chuyển -> Stalemate
                    Result = Result.Draw(EndReason.Stalemate);
                }
            }
            //insufficient material
            else if(Board.InsufficientMaterial())
            {
                Result = Result.Draw(EndReason.InsufficientMaterial);
            }
            //50 move rule
            else if(FiftyMoveRule())
            {
                Result = Result.Draw(EndReason.FiftyMoveRule);
            }
        }
        public bool IsGameOver()
        {
            return Result != null;
        }

        //50 move rule
        private bool FiftyMoveRule()
        {
            int fullMoves = noCaptureOrPawnMoves / 2;
            return fullMoves == 50;
        }
    }
}