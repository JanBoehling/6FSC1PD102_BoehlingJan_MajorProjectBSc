<?php
$servername = 'database-5017942646.webspace-host.com';
$database = 'dbs14278174';
$username = 'dbu3329914';
$password = '';

$sqlIN = $_GET["sql"];

try {
    $conn = new mysqli($servername, $username, $password, $database);
  }
  
catch(Exception $e) {
    echo "Connection failed!<br/>";
    echo $e . "<br/>";
    die();
}

$result = $conn->query($sqlIN);

if ($result->num_rows > 0){
    while ($row = $result->fetch_assoc()){
        foreach ($row as $item) {
            echo $item . "\n";
        }
    }
}

$conn->close();

?>