
#r "nuget: Microsoft.Data.SqlClient"

open Microsoft.Data
open Microsoft.Data.SqlClient

let conn =
    SqlConnection(
        $"""
            Server=(localdb)\MSSQLLocalDB;
            Database=Demo;
            User Id=DebuggingTp;
            Password=DebuggingTp;
        """)
let cmd =
    let c = conn.CreateCommand()
    do c.CommandText <- "select count(*) from DemoTable"
    c

conn.Open()
printfn $"Rows: {cmd.ExecuteScalar()}"
conn.Close()
