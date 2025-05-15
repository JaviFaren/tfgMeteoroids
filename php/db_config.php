<?php
$servername = "fdb1029.awardspace.net";
$username = "4607931_meteoroids";
$password = "m3r3nGu3";
$dbname = "4607931_meteoroids";
$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die(json_encode(["success" => false, "message" => "Conexión fallida"]));
}
?>