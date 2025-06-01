<?php
$servername = 'database-5017942646.webspace-host.com';
$database = 'dbs14278174';
$username = 'dbu3329914';
$password = '';

try {
    $conn = new mysqli($servername, $username, $password, $database);
  }
  
catch(Exception $e) {
    echo "Connection failed!<br/>";
    echo $e . "<br/>";
    die();
}

$sql = "SELECT * FROM UserData";

$result = $conn->query($sql);

if ($result->num_rows > 0){
    while ($row = $result->fetch_assoc()){
        echo "ID: " . $row["userID"] . "<br>Username: " . $row["username"] . "<br>Password: " . $row["password"] . "<br>Streak: " . $row["streak"] . "<br>XP: " . $row["XP"] . "<br><br>";
    }
}
else{
    echo "Could not fetch user data";
}

$conn->close();

?>