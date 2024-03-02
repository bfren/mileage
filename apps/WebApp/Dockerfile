FROM bfren/aspnet:net8

COPY ./publish /app/live

ENV BF_ASPNET_ASSEMBLY="Mileage.WebApp.dll" \
	ASPNETCORE_ENVIRONMENT="Production"
