using CalculatorServiceok.Server.Services;
using NUnit.Framework;
using System.Collections.Generic;

namespace CalculatorServiceok.Server.Tests
{
    [TestFixture]
    public class JournalServiceTests
    {
        private JournalService _journalService;

        [SetUp]
        public void Setup()
        {
            // Inicializamos el servicio antes de cada prueba
            _journalService = new JournalService();
        }

        [Test]
        public void AddEntry_DeberiaGuardarYRecuperarCorrectamente()
        {
            // 1. Arrange (Preparar)
            string idUsuario = "pau";
            string operacion = "Suma";
            string calculo = "5 + 5 = 10";

            // 2. Act (Actuar)
            _journalService.AddEntry(idUsuario, operacion, calculo);
            var historial = _journalService.GetEntries(idUsuario);

            // 3. Assert (Verificar)
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