# General Flow
1. Data is retrieved from https://api.data.gov.sg
2. A portion of the entire JSON is extracted, transformed and rewritten into a JSON with 4 fields: Area, Forecast, Start Time and End Time.
3. The saved JSON will be located in /data/pull and saved as dd-MM-yyyy.json, e.g. /data/pull/01-11.2022.json

# How the containers will fit together
There will be 4 containers:
1. Container weather-pull-data will produce a [JSON](https://github.com/vms3-demo-purpose/weather-program/files/9934735/01-11-2022.json.txt)
to be saved in a volume.
2. Container weather-push-data will retrieve the JSON from the volume and push it to a container running MSSQL.
3. Container weather-save-data will store data in the following [schema](https://github.com/vms3-demo-purpose/weather-program/files/9934736/CREATE_TABLE.sql.txt) based on data from the JSON.
4. Container weather-show-data will present data by reading what is stored in the database.

# Running the container
Clone the repository. Open PowerShell (preferably with administrator rights), navigate to the `/final` directory and run the following command:

`docker compose up --build --force-recreate -d`

# Verifying that each container is running properly:

`docker logs weather-push-data`

The above command should output the following: Pulled 1234 weather records for date: 01-01-1970.

`docker logs --follow weather-push-data`

The above command should output a loooong list of weather records, with ID, Area, Forecast, StartTime and EndTime.

`docker exec -ti weather-save-data bash`

`/opt/mssql-tools/bin/sqlcmd -U SA`

`<INPUT PASSWORD>`

`SELECT * FROM WeatherRecords;`

`GO`

The above string of commands should also output a loooong list of weather records (but not formatted nicely).

# Versions of Framework and Libraries used:
1. docker-compose: 3
2. SQL Server: 2019-CU18-ubuntu-20.04
3. .NET: 6.0.402
