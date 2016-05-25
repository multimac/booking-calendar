FROM microsoft/dotnet:latest
COPY ./ /dotnet/

WORKDIR /dotnet/src/Booking.Api/
RUN dotnet restore --verbosity Warning

EXPOSE 4590
VOLUME /dotnet/

ENTRYPOINT ["dotnet"]
CMD ["run"]