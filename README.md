# Booking Calendar

## Initial Setup

1. Create .env file in root to store Docker Compose development variables for the database (db)
2. If you want to debug the API locally (i.e. not inside docker)...
   1. Go to ./dotnet/src/Booking.Api/ and use ```dotnet user-secrets``` to set up development variables for the API (api)

### Possible variables:
```
BOOKING_API_CONNECTIONSTRING (api/db) - The connection string from the 'api' container to the 'db' container, excluding password
BOOKING_API_PASSWORD (api/db) - Password for the API to use when connecting to the database

BOOKING_API_ADMIN_EMAIL (api) - Email address for the admin account in the API (exclude to not create admin account)
BOOKING_API_ADMIN_PASSWORD (api) - Password for the admin account in the API (exclude to prevent admin account from logging in)

POSTGRES_PASSWORD (db) - Password for the 'postgres' account in the database
``` 
