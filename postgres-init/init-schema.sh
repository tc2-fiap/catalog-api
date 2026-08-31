#!/bin/bash
# Standalone-compose equivalent of the schema/role slice this service owns
# in orchestration's cluster init script — same shape, one service only.
set -euo pipefail

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
  CREATE SCHEMA IF NOT EXISTS catalog;
  CREATE ROLE catalog_role LOGIN PASSWORD '$CATALOG_DB_PASSWORD';
  ALTER ROLE catalog_role SET search_path TO catalog;
  GRANT USAGE, CREATE ON SCHEMA catalog TO catalog_role;
  GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA catalog TO catalog_role;
  ALTER DEFAULT PRIVILEGES IN SCHEMA catalog GRANT ALL PRIVILEGES ON TABLES TO catalog_role;
  ALTER DEFAULT PRIVILEGES IN SCHEMA catalog GRANT ALL PRIVILEGES ON SEQUENCES TO catalog_role;
EOSQL
