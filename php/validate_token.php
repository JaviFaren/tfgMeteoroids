<?php
header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json");

require_once 'db_config.php';

$token = $_POST['session_token'];

$stmt = $conn->prepare("SELECT id, name, email FROM users WHERE session_token = ?");
$stmt->bind_param("s", $token);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows > 0) {
    $stmt->bind_result($id, $name, $email);
    $stmt->fetch();

    echo json_encode([
        "success" => true,
        "user" => [
            "id" => $id,
            "name" => $name,
            "email" => $email,
            "session_token" => $token,
        ]
    ]);
} else {
    echo json_encode(["success" => false, "message" => "Token invalido", "code" => "session_token_invalid"]);
}
