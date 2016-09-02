FROM microsoft/dotnet:1.0.0-preview2-sdk
COPY ./ /dotnet/

EXPOSE 4590
VOLUME /dotnet/

RUN dotnet restore /dotnet/ --verbosity Warning
WORKDIR /dotnet/src/Booking.Api/

ENTRYPOINT ["dotnet"]
CMD ["run"]