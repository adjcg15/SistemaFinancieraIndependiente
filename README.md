# SistemaFinancieraIndependiente

## Estructura del proyecto
### SFIClient
Proyecto que contiene toda la lógica relacionada al cliente. La estructura de este proyecto es la siguiente:

* Assets: carpete que contiene los recursos del proyecto, principalmente imágenes e iconos.
* Views: Contiene todas las ventanas que se presentan a los usuarios: _nombreVentana**GUI**_. Cada ventana cuenta con un archivo controlador asociado (`.xaml.cs`), nombrado de la misma forma, sin embargo, el nombre de la clase dentro del archivo sigue el nombrado _nombreVentana**Controller**_.


En cuanto a los archivos importantes dentro del proyecto, se destacan:

* `App.xaml`: contiene los estilos globales.
* `MainWindow.xaml`: contiene el punto de partida de la aplicación. 
