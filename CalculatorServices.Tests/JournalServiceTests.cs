using CalculatorServiceok.Server.Services;

namespace CalculatorServiceok.Server.Tests
{
    [TestFixture]
    public class JournalServiceTests
    {
        private JournalService _journalService;

        [SetUp]
        public void Setup()
        {
			// We initialize the service before each test
			_journalService = new JournalService();
        }

        [Test]
        public void AddEntry_DeberiaGuardarYRecuperarCorrectamente()
        {
			// 1. Arrange (Prepare)
			string idUsuario = "pau";
            string operacion = "Suma";
            string calculo = "5 + 5 = 10";

			// 2. Act (To act)
			_journalService.AddEntry(idUsuario, operacion, calculo);
            var historial = _journalService.GetEntries(idUsuario);

			// 3. Assert (Verify)
			Assert.That(historial.Count, Is.EqualTo(1));
            Assert.That(historial[0].Operation, Is.EqualTo("Suma"));
            Assert.That(historial[0].Calculation, Is.EqualTo("5 + 5 = 10"));
        }

        [Test] 
        public void GetEntries_DeberiaRetornarListaVacia_SiElIdNoExiste()
        {
            // Act
            var resultado = _journalService.GetEntries("usuario_inexistente");

            // Assert
            Assert.That(resultado, Is.Empty);

        }
    }
}