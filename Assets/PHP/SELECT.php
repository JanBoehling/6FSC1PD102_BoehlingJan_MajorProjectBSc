<?php
$servername = 'database-5017942646.webspace-host.com';
$database = 'dbs14278174';
$username = 'dbu3329914';
$password = '';

$selectIN = $_GET["select"];
$fromIN = $_GET["from"];

try {
    $conn = new mysqli($servername, $username, $password, $database);
  }
  
catch(Exception $e) {
    echo "Connection failed!<br/>";
    echo $e . "<br/>";
    die();
}

$sql = "SELECT $selectIN FROM $fromIN;";

$result = $conn->query($sql);

if ($result->num_rows > 0){
    while ($row = $result->fetch_assoc()){
        foreach ($row as $item) {
            echo $item . ";";
        }
    }
}
else{
    echo "Could not fetch data";
}

$conn->close();

?>