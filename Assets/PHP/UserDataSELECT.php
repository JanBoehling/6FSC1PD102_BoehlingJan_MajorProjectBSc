<?php
$servername = 'database-5017942646.webspace-host.com';
$database = 'dbs14278174';
$username = 'dbu3329914';
$password = '';

$selectIN = $_GET["select"];
$fromIN = $_GET["from"];
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

$sql = "SELECT $selectIN FROM $fromIN WHERE $whereIN = '$predicateIN';";

if ($result->num_rows > 0){
    while ($row = $result->fetch_assoc()){
        echo $row;
    }
}
else{
    echo "Could not fetch data";
}

$conn->close();

?>