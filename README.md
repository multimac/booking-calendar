# Booking Calendar

## Initial Setup

1. Create .env file in root to store environment variables for running app inside Docker
2. If you want to debug the API locally (i.e. not inside docker)...
   1. Go to ./dotnet/src/Booking.Api/ and use ```dotnet user-secrets``` to set up development variables for the API

### Possible variables:
```
BOOKING_CONNECTIONSTRING (api/db) - The connection string from the 'api' container to the 'db' container, excluding password
BOOKING_PASSWORD (api/db) - Password for the API to use when connecting to the database

BOOKING_ADMIN_EMAIL (api) - Email address for the admin account in the API (exclude to not create admin account)
BOOKING_ADMIN_PASSWORD (api) - Password for the admin account in the API (exclude to prevent admin account from logging in)

BOOKING_OAUTH_SECRET (api) - Secret for the API to use when authenticating to the OAuth server

POSTGRES_PASSWORD (db) - Password for the 'postgres' account in the database
``` 
