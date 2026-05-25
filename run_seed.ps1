$cs = "Server=(localdb)\MSSQLLocalDB;Database=ScheduleAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
$script = Get-Content "src\ScheduleApp.Infrastructure\Scripts\02_seed_data.sql" -Raw
$batches = $script -split '(?m)^\s*GO\s*$'
$conn = New-Object System.Data.SqlClient.SqlConnection($cs); $conn.Open()
$ok = 0
foreach ($batch in $batches) { $batch = $batch.Trim(); if ($batch -ne '') { $cmd = $conn.CreateCommand(); $cmd.CommandText = $batch; $cmd.ExecuteNonQuery() | Out-Null; $ok++ } }
$conn.Close()
Write-Host "Seed ejecutado: $ok bloques"
