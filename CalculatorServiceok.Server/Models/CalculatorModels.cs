using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
    // Model for Addition
    public class AddRequest
    {
        [Required]
        [MinLength(2, ErrorMessage = "Se requieren al menos dos números para sumar.")]
        public List<double> Addends { get; set; } = new();
    }

    // Model for Subtraction
    public class SubRequest
    {
        [Required]
        public double Minuend { get; set; }
        [Required]
        public double Subtrahend { get; set; }
    }

    // Model for Multiplication
    public class MultRequest
    {
        [Required]
        [MinLength(2, ErrorMessage = "Se requieren al menos dos factores para multiplicar.")]
        public List<double> Factors { get; set; } = new();
    }

    // Model for the Division
    public class DivRequest
    {
        [Required]
        public double Dividend { get; set; }

        [Required]
        // Note: The divisor != 0 validation is done in the Controller using business logic
        public double Divisor { get; set; }
    }

    // Model for the Square Root
    public class SqrtRequest
    {
        [Required]
        public double Number { get; set; }
    }

    // --- THE HISTORICAL MODEL ---
    public class JournalEntry
    {
        public string Operation { get; set; } = string.Empty;

        public string Calculation { get; set; } = string.Empty;

        // The current date is automatically assigned when creating the entry
        public DateTime Date { get; set; } = DateTime.Now;
    }
}

/*☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～
NOTAS PARA MI:) 

El Controlador es el "Cerebro", estos Modelos son los Formularios.

Imagina que vas a un banco a pedir un préstamo. El banco tiene un formulario específico para eso. No puedes escribir los datos en una servilleta.
Estos modelos le dicen a .NET: "Para una suma, el paquete debe tener este aspecto".
Si el cliente (tu calculadora de consola) envía algo que no encaja con estos modelos, el servidor ni siquiera intenta hacer el cálculo; 
responde directamente: "Oye, el formulario está mal rellenado".

¿Cómo se conectan con el resto?
Cuando en tu controlador pusiste [FromBody] AddRequest request, le estabas diciendo al servidor: 
"Usa la plantilla AddRequest para entender los datos que vienen en el cuerpo del mensaje".
  
*/