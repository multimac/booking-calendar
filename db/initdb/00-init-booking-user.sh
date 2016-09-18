#!/bin/bash -e

if [ -z ${BOOKING_PASSWORD} ]; then
    echo "BOOKING_PASSWORD is not set, cannot continue..."
    exit 1
fi

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    CREATE USER booking
        ENCRYPTED PASSWORD '${BOOKING_PASSWORD}';
EOSQL