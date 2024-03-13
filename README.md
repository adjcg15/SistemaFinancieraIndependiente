# SistemaFinancieraIndependiente

## Estructura del proyecto
### SFIClient
Proyecto que contiene toda la lógica relacionada al cliente. La estructura de este proyecto es la siguiente:

* Assets: contiene los recursos del proyecto, principalmente imágenes e iconos.
* Views: contiene todas las ventanas que se presentan a los usuarios: _nombreVentana**GUI**_. Cada ventana cuenta con un archivo controlador asociado (`.xaml.cs`), nombrado de la misma forma, sin embargo, el nombre de la clase dentro del archivo sigue el nombrado _nombreVentana**Controller**_.


En cuanto a los archivos importantes dentro del proyecto, se destacan:

* `App.xaml`: contiene los estilos globales.
* `MainWindow.xaml`: contiene el punto de partida de la aplicación. 

### SFIHost
Proyecto que se encarga de configurar y exponer los servicios. No cuenta con una estructura compleja, sin embargo, cuenta con dos archivos importantes:

* `App.config`: contiene la configuración para registrar y exponer los servicios.
* `Server.cs`: punto de partida para la ejecución del servidor (main).

### SFIServices
Proyecto que contiene la lógica de los servicios y sus contrados. La estructura que sigue:

* Contracts (espacio de nombres `SFIServices.Contracts`): contiene todas las interfaces que definen los contratos de servicios.
* Services (espacio de nombres `SFIServices`): contiene las clases encargadas de implementar los contratos.

### SFIDataAccess
Proyecto que contiene la lógica de acceso a datos. La estructura que sigue se define a continuación:

* Model (espacio de nombres `SFIDataAccess.Model`): incluye las entidades del negocio que trazan al modelo de dominio y que se serializan por los servicios a través de `DataContracts`.
* DataAccessObjects (espacio de nombres `SFIDataAccess.DataAccessObjects`): incluye las clases encargadas de la lógica de acceso a datos, nombradas de la forma _EntidadEnPlural**DAO**_
