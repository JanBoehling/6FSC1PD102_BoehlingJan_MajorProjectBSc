<?php
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

if ($_SERVER['REQUEST_METHOD'] == 'OPTIONS') {
    http_response_code(200);
    exit();
}

$servername = 'database-5017942646.webspace-host.com';
$database = 'dbs14278174';
$username = 'dbu3329914';
$password = '';

$tableIN = $_GET["table"];
$setIN = $_GET["set"];
$whereIN = $_GET["where"];
$predicateIN = $_GET["predicate"];

try {
    $conn = new mysqli($servername, $username, $password, $database);
  }
  
catch(Exception $e) {
    echo "Connection failed!<br/>";
    echo $e . "<br/>";
    die();
}

$sql = "UPDATE $tableIN SET $setIN WHERE $whereIN = $predicateIN;";

if ($conn->query($sql) === TRUE) {
    echo "200"; // HTTP-Statuscode 'OK'
}
else{
    echo $conn->connect_errno;
}

$conn->close();

?>