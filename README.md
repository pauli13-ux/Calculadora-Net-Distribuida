# 🧮 MI PRIMERA CALCULADORA

Aplicación cliente-servidor para realizar cálculos matemáticos, que implementa una capa de persistencia en memoria y un sistema de logging con Serilog para el seguimiento de la actividad".

## CARACTERÍSTICAS

* **Operaciones**: Suma, Resta, Multiplicación, División y Raíz Cuadrada.
* **Journal**: Registro de operaciones en memoria mediante un servicio Singleton.
* **Logging**: Implementación de **Serilog** para registrar eventos en consola y archivos de texto diarios.
* **Documentación**: Interfaz de **Swagger** integrada para probar los endpoints.

## Tecnologías utilizadas
* .NET Core Web API (Servidor)
* Aplicación de Consola (Cliente)
* **NUnit**: Para las pruebas unitarias.
* **Serilog**: Para el manejo de logs.

## Instalación y Configuración
1. Clonar el repositorio:
   ```bash
   git clone [https://github.com/tu-usuario/Calculadora-Net-Distribuida.git](https://github.com/tu-usuario/Calculadora-Net-Distribuida.git)

## Estado del Proyecto
El proyecto cuenta con una cobertura de pruebas unitarias completa para el motor de cálculo:
- ✅ **Suma**: Verificada con listas de números.
- ✅ **División**: Verificada con cocientes y restos exactos.
- ✅ **Raíz Cuadrada**: Verificada mediante múltiples casos de prueba (`TestCase`).

