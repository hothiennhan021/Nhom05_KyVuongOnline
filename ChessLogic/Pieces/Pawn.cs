using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class Pawn : Pieces
    {
        public override PieceType Type => PieceType.Pawn;
        public override Player Color { get; }

        public readonly Directions forward;

        public Pawn(Player color)
        {
            Color = color;
            if (color == Player.White)
            {
                forward = Directions.North;
            }
            else if (color == Player.Black)
            {
                forward = Directions.South;
            }
        }
        public override Pieces Copy()
        {
            Pawn copy = new Pawn(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static bool CanMoveTo(Position pos, Board board)
        {
            return Board.IsInside(pos) && board.IsEmty(pos);
        }

        private bool CanCaptureAt(Position pos, Board board)
        {
            if (!Board.IsInside(pos) || board.IsEmty(pos))
            {
                return false;
            }

            return board[pos].Color != Color;
        }
        //Tao chức năng phong hậu
        private static IEnumerable<Move> PromotionMoves(Position from, Position to)
        {
            yield return new PawnPromotion(from, to, PieceType.Knight);
            yield return new PawnPromotion(from, to, PieceType.Bishop);
            yield return new PawnPromotion(from, to, PieceType.Rook);
            yield return new PawnPromotion(from, to, PieceType.Queen);
        }
        private IEnumerable<Move> ForwardMoves(Position from, Board board)
        {
            // 1. Tính vị trí 1 bước tiến
            Position oneMovePos = from + forward;

            // Kiểm tra ô 1 bước có trống không
            if (CanMoveTo(oneMovePos, board))
            {
                // A. Nếu đi 1 bước mà chạm đáy (Hàng 0 hoặc 7) -> Phong cấp
                if (oneMovePos.Row == 0 || oneMovePos.Row == 7)
                {
                    foreach (Move promMove in PromotionMoves(from, oneMovePos))
                    {
                        yield return promMove;
                    }
                }
                else
                {
                    // B. Đi 1 bước thường
                    yield return new NormalMove(from, oneMovePos);
                }

                // ================================================================
                // 2. XỬ LÝ ĐI 2 BƯỚC (PHẦN SỬA LỖI QUAN TRỌNG)
                // ================================================================

                // Thay vì tin vào biến !HasMoved (dễ bị lỗi), ta kiểm tra Hàng (Row).
                // - Tốt Đen luôn xuất phát ở Hàng 1 (Row 1).
                // - Tốt Trắng luôn xuất phát ở Hàng 6 (Row 6).

                bool isAtStart = (Color == Player.Black && from.Row == 1)
                              || (Color == Player.White && from.Row == 6);

                if (isAtStart)
                {
                    Position twoMovePos = oneMovePos + forward;

                    // Nếu ô thứ 2 cũng trống -> Được phép đi 2 bước
                    if (CanMoveTo(twoMovePos, board))
                    {
                        yield return new NormalMove(from, twoMovePos);
                    }
                }
            }
        }

        private IEnumerable<Move> DiagonalMoves(Position from , Board board)
        {
            foreach (Directions dir in new Directions[] { Directions.West, Directions.East })
            {
                Position to = from + forward + dir;

                if(CanCaptureAt(to, board))
                {
                    if (to.Row == 0 || to.Row == 7)
                    {
                        foreach (Move promMove in PromotionMoves(from, to))
                        {
                            yield return promMove;
                        }
                    }
                    else
                    {
                        yield return new NormalMove(from, to);

                    }
                }
            }
        }


        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return ForwardMoves(from, board).Concat(DiagonalMoves(from, board));
        }

        public override bool CanCaptureOpponentKing(Position from, Board board)
        {
            return DiagonalMoves(from, board).Any(move =>
            {
                Pieces piece = board[move.ToPos];
                return piece != null && piece.Type == PieceType.King;
            });
        }

    }
}