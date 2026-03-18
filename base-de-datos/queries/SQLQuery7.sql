USE [LeaderBoard]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[sp_crear_usuario]
		@nombre = N'Alejandro 4',
		@tipo_id = 1,
		@edad = 89,
		@correo = N'alejandro1234567@gmail.com',
		@numero_de_telefono = N'092823172315',
		@cedula = N'0123356782'

SELECT	'Return Value' = @return_value
GO
