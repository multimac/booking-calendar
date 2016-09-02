#!/bin/bash -e

if [ -z ${BOOKING_API_PASSWORD} ]; then
    echo "BOOKING_API_PASSWORD is not set, cannot continue..."
    exit 1
fi

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE USER booking
        ENCRYPTED PASSWORD '${BOOKING_API_PASSWORD}';
EOSQL