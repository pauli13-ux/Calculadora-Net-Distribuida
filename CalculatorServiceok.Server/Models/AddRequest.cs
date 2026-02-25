using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
	public class AddRequest
	{
		[Required]
		[MinLength(2, ErrorMessage = "Se requieren al menos dos números para sumar.")]
		public List<double> Addends { get; set; } = new();
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

Aqui tuve que arreglar algo que se llama REFACTORIZACION, sacar cada clase (AddRequest, SubRequest...) a su propio archivo individual dentro de la carpeta Models. 
Esto sirve para que el archivo no sea demasiado grande y dificil de leer porque todas las clases están amontonadas, es mejor que los archivos esten separados para que cada uno tenga una responsabilidad distinta

  
*/