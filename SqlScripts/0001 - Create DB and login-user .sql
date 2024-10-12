CREATE Database [KhumaloCraft];

-- Create a new login at the SQL Server level
CREATE LOGIN [khumalocraft-user] WITH PASSWORD = 'YourStrongPassword';

--You might have to manually switch to the DB
USE [KhumaloCraft];

-- Create a user in the specific database for the new login
CREATE USER [khumalocraft-user] FOR LOGIN [khumalocraft-user];

ALTER USER [khumalocraft-user] WITH DEFAULT_SCHEMA = dbo;

GRANT ALTER ON SCHEMA::dbo TO [khumalocraft-user];

GRANT REFERENCES TO [khumalocraft-user];
GRANT CREATE PROCEDURE TO [khumalocraft-user];
GRANT CREATE VIEW TO [khumalocraft-user];

-- Grant necessary permissions
-- Adjust the permissions as needed
ALTER ROLE db_datareader ADD MEMBER [khumalocraft-user]; -- Allows read access
ALTER ROLE db_datawriter ADD MEMBER [khumalocraft-user]; -- Allows write access
--ALTER ROLE db_owner ADD MEMBER [khumalocraft-user]; -- Grants full control (optional, use with caution)

-- Optionally, grant execute permissions on all stored procedures
GRANT EXECUTE TO [khumalocraft-user];

-- For the Migration to Work
GRANT CREATE TABLE TO [khumalocraft-user];

-- Optional: If you have specific schemas, grant permissions accordingly
-- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[YourSchemaName] TO [khumalocraft-user];
