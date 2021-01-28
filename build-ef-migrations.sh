#/bin/sh

NAME=$1

echo "ready build ef core migration for: ${NAME}"

# sqlite
export DB_TYPE="sqlite"
export DB_CONN="Data Source=./data/rc.db;"
dotnet ef migrations add $NAME -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.Sqlite/Shashlik.RC.Data.Sqlite.csproj -s ./Shashlik.RC/Shashlik.RC.csproj

# mysql
export DB_TYPE="mysql"
export DB_CONN="server=192.168.50.178;user id=testuser;password=123123;database=rc;"
dotnet ef migrations add $NAME -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.MySql/Shashlik.RC.Data.MySql.csproj -s ./Shashlik.RC/Shashlik.RC.csproj

# npgsql
export DB_TYPE="npgsql"
export DB_CONN="server=192.168.50.178;user id=testuser;password=123123;database=rc;"
dotnet ef migrations add $NAME -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.PostgreSql/Shashlik.RC.Data.PostgreSql.csproj -s ./Shashlik.RC/Shashlik.RC.csproj

# sqlserver
export DB_TYPE="sqlserver"
export DB_CONN="server=192.168.50.178;user id=sa;password=Shashlik123123;database=rc;"
dotnet ef migrations add $NAME -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.SqlServer/Shashlik.RC.Data.SqlServer.csproj -s ./Shashlik.RC/Shashlik.RC.csproj