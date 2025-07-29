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

$usernameIN = $_GET["username"];
$passwordIN = $_GET["password"];
$streakIN = (int)$_GET["streak"];
$xpIN = (int)$_GET["XP"];
$profilePictureIndexIN = (int)$_GET["profilePictureIndex"];

try {
    $conn = new mysqli($servername, $username, $password, $database);
  }
  
catch(Exception $e) {
    echo "Connection failed!<br/>";
    echo $e . "<br/>";
    die();
}

$sql = "INSERT INTO UserData (username, password, streak, XP, profilePictureIndex) VALUES ('$usernameIN', '$passwordIN', '$streakIN', '$xpIN', '$profilePictureIndexIN')";

if ($conn->query($sql) === TRUE) {
    echo "200"; // HTTP-Statuscode 'OK'
}
else{
    echo $conn->connect_errno;
}

$conn->close();

?>