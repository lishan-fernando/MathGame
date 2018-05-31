#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Math2.Enums;
using Math2.Interfaces;
using Math2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#endregion

namespace Math2.BusinessLogics
{
    public class GameBusinessLogics : IGameBusinessLogics
    {
        #region private variables

        private const string AdditionOperation = "+";
        private const string SubtractionOperation = "-";
        private const string MultiplicationOperation = "*";
        private const string DivisionOperation = "/";

        private readonly List<string> _operations;
        private Timer _timer;
        private int _time = 0;

        #endregion

        #region public property

        public Game Game { get; set; }

        #endregion

        #region constructor

        public GameBusinessLogics()
        {
            Game = new Game();
            _timer = new Timer();
            _operations = new List<string>
            {
                AdditionOperation,
                SubtractionOperation,
                MultiplicationOperation,
                DivisionOperation
            };
            _timer.Interval = 1000;

            _timer.Elapsed -= timer_Elapsed;
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        #endregion

        #region public methods

        public void AddPlayer(string player, WebSocket webSocket)
        {
            try
            {
                if (!Game.Players.Any(p => p.Name.Equals(player)))
                {
                    Game.Players.Add(new Player
                    {
                        Name = player,
                        playerSoket = webSocket,
                        Points = 0
                    });
                }
            }
            catch (Exception e)
            {
                // To do
            }
        }

        public async Task NewGame()
        {
            if (Game.Question.Status == EnumQuestionStatus.Answerd || Game.Question.Status == 0)
            {
                Game.Status = EnumGameStatus.Go;
                Game.Question = PopulateQuestion();
            }

            await Broadcast();
        }

        public async Task EvaluateAnswer(Answer answer)
        {
            if (Game.Question.Id.Equals(answer.QuestionId))
            {
                var pl = Game.Players.SingleOrDefault(p => p.Name.Equals(answer.Player));

                if (pl != null)
                {
                    if (!pl.Answers.Any(a => a.QuestionId.Equals(answer.QuestionId)))
                    {
                        pl.Answers.Add(answer);
                        if (answer.AnswerText.Equals(Game.Question.IsCorrect))
                        {
                            _timer.Stop();
                            pl.Points += 1;
                            Game.Status = EnumGameStatus.End;
                            Game.Question.Status = EnumQuestionStatus.Answerd;
                            Game.Question.QuestionText =
                                string.Format("Congratulations {0} provided the correct answer !", pl.Name);
                            Game.StatusText = string.Format("Congratulations {0} provided the correct answer !", pl.Name);
                            await Broadcast().ContinueWith(p =>
                            {
                                _time = 0;
                                _timer.Start();
                            });
                        }
                        else
                        {
                            _timer.Stop();
                            pl.Points -= 1;
                            await Broadcast().ContinueWith(p =>
                            {
                                _time = 0;
                                _timer.Start();
                            });
                        }
                    }
                }
            }
        }

        public async Task Broadcast()
        {
            if (Game.Players != null && Game.Players.Any())
            {
                foreach (var s in Game.Players)
                {
                    var webSocket = s.playerSoket;
                    await Broadcast(webSocket);
                }
            }
        }

        #endregion

        #region events

        private async void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _time += 1;

            if (Game.Status.Equals(EnumGameStatus.End) && _time <= 5)
            {
                Game.Question.QuestionText = string.Format("New game starts {0} in seconds", _time);
                Game.StatusText = string.Format("New game starts {0} in seconds", _time);
                await Broadcast();
            }

            if (Game.Status.Equals(EnumGameStatus.End) && _time >= 5)
            {
                await NewGame();
            }
        }

        #endregion

        #region private methods

        private async Task Broadcast(WebSocket webSocket)
        {
            try
            {
                var json = JsonConvert.SerializeObject(Game,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                UTF8Encoding encoder = new UTF8Encoding();
                byte[] buffer = encoder.GetBytes(json);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    System.Threading.CancellationToken.None);
            }
            catch (Exception e)
            {
                // To do
            }
        }

        private Question PopulateQuestion()
        {
            var random = new Random();
            var r = random.Next(_operations.Count);
            var operation = _operations[r];

            var firstNumber = random.Next(1, 10);
            var secondNumber = random.Next(1, 10);
            var thirdNumber = random.Next(-9, 81);

            return new Question
            {
                Id = Guid.NewGuid().ToString(),
                QuestionText = string.Format("{0} {1} {2} = {3}", firstNumber, operation, secondNumber, thirdNumber),
                IsCorrect = IsCorrectAnswer(firstNumber, secondNumber, thirdNumber, operation),
                Status = EnumQuestionStatus.Questioned
            };
        }

        private string IsCorrectAnswer(int firstNumber, int secondNumber, int answer, string operation)
        {
            switch (operation)
            {
                case AdditionOperation:
                    return firstNumber + secondNumber == answer ? "y" : "n";
                case SubtractionOperation:
                    return firstNumber - secondNumber == answer ? "y" : "n";
                case MultiplicationOperation:
                    return firstNumber * secondNumber == answer ? "y" : "n";
                case DivisionOperation:
                    return firstNumber / secondNumber == answer ? "y" : "n";
            }

            return string.Empty;
        }

        #endregion
    }
}