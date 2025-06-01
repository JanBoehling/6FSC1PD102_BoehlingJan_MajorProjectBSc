<?php
echo "Testing connection...<br/>";

$servername = 'localhost';
$database = '';
$password = '';
$username = '';

$conn = new mysqli($servername, $username, $password, $database);

if ($conn->connection_error){
    die("Connection failed: " . $conn->connection_error);
}

echo "Connection successful";

?>