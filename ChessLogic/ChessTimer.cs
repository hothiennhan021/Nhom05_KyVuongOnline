using System;
using System.Timers;

namespace ChessLogic
{
    public class ChessTimer
    {
        private readonly System.Timers.Timer _timer;

        public int WhiteRemaining { get; private set; }
        public int BlackRemaining { get; private set; }

        // --- MỚI THÊM: Tổng số phút đã chơi ---
        public int TotalPlayedMinutes { get; private set; } = 0;
        private int _elapsedSeconds = 0;

        public Player ActivePlayer { get; private set; }

        public event Action<Player>? TimeExpired;
        public event Action<int, int>? Tick;

        public ChessTimer(int minutes)
        {
            WhiteRemaining = minutes * 60;
            BlackRemaining = minutes * 60;

            _timer = new System.Timers.Timer(1000);
            _timer.AutoReset = true;
            _timer.Elapsed += OnTimedEvent;
        }

        public void Start(Player player)
        {
            ActivePlayer = player;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void SwitchTurn()
        {
            ActivePlayer = (ActivePlayer == Player.White) ? Player.Black : Player.White;
        }

        public void Sync(int white, int black)
        {
            WhiteRemaining = white;
            BlackRemaining = black;
            Tick?.Invoke(WhiteRemaining, BlackRemaining);
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            // --- Đếm thời gian thực tế đã chơi ---
            _elapsedSeconds++;
            if (_elapsedSeconds % 60 == 0)
                TotalPlayedMinutes++;

            if (ActivePlayer == Player.White)
            {
                WhiteRemaining--;
                if (WhiteRemaining <= 0)
                {
                    _timer.Stop();
                    TimeExpired?.Invoke(Player.White);
                }
            }
            else
            {
                BlackRemaining--;
                if (BlackRemaining <= 0)
                {
                    _timer.Stop();
                    TimeExpired?.Invoke(Player.Black);
                }
            }

            Tick?.Invoke(WhiteRemaining, BlackRemaining);
        }
    }
}
