using Math2.BusinessLogics;
using Math2.Models;
using NUnit.Framework;

namespace Math2.Test.BusnessLogic.Test
{
    [TestFixture]
    public class GameBusinessLogicsTest
    {
        private readonly string _playerName = "Lishan";
        private GameBusinessLogics gameBusinessLogics = new GameBusinessLogics();

        [Test]
        public void NewGameTest()
        {
            gameBusinessLogics.NewGame();

            Assert.NotNull(gameBusinessLogics.Game);
            Assert.NotNull(gameBusinessLogics.Game.Question);
            Assert.NotNull(gameBusinessLogics.Game.Question.QuestionText);
        }

        [SetUp]
        public void Setup()
        {
            gameBusinessLogics.Game = new Game();
        }
    }
}
