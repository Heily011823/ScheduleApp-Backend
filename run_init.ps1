$cs = "Server=(localdb)\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
$script = Get-Content "src\ScheduleApp.Infrastructure\Scripts\01_initial_database.sql" -Raw
$batches = $script -split '(?m)^\s*GO\s*$'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs); $conn.Open()
$ok = 0
foreach ($batch in $batches) { $batch = $batch.Trim(); if ($batch -ne '') { $cmd = $conn.CreateCommand(); $cmd.CommandText = $batch; $cmd.ExecuteNonQuery() | Out-Null; $ok++ } }
$conn.Close()
Write-Host "Init ejecutado: $ok bloques"
