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

$linkIN = $_GET["link"];
$isCompletedIN = (int)$_GET["isCompleted"];
$userIDIN = (int)$_GET["userID"];

try {
    $conn = new mysqli($servername, $username, $password, $database);
  }
  
catch(Exception $e) {
    echo "Connection failed!<br/>";
    echo $e . "<br/>";
    die();
}

$sql = "INSERT INTO UnitProgress (unitLink, isCompleted, userID) VALUES ('$linkIN', '$isCompletedIN', '$userIDIN')";

if ($conn->query($sql) === TRUE) {
    echo "200"; // HTTP-Statuscode 'OK'
}
else{
    echo $conn->connect_errno;
}

$conn->close();

?>