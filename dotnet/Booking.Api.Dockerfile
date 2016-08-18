FROM microsoft/dotnet:1.0.0-preview2-sdk
COPY ./ /dotnet/

WORKDIR /dotnet/src/Booking.Api/
RUN dotnet restore --verbosity Warning

EXPOSE 4590
VOLUME /dotnet/

ENTRYPOINT ["dotnet"]
CMD ["run"]